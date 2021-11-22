using System;
using System.Globalization;

namespace Koda.ColorTools
{
    public readonly struct HexColor
    {
        public HexColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Alpha [0,255]
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// Red [0,255]
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// Green [0,255]
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// Blue [0,255]
        /// </summary>
        public byte B { get; }

        public void Deconstruct(out byte a, out byte r, out byte g, out byte b)
        {
            a = A;
            r = R;
            g = G;
            b = B;
        }

        public static HexColor Parse(string hex)
        {
            return TryParse(hex, out var color) ? color : throw new FormatException("Provided string is not valid hex color.");
        }

        public static bool TryParse(string hex, out HexColor color)
        {
            color = new HexColor();

            if (string.IsNullOrEmpty(hex))
            {
                return false;
            }

            byte a, r, g, b;

            var ns = NumberStyles.HexNumber;
            var ci = CultureInfo.InvariantCulture;

            var data = hex.AsSpan();

            // Allow 0..n '#'
            while (data[0] == '#')
            {
                data = data[1..];
            }

            switch (data.Length)
            {
                case 4:
                    Span<char> buffer = stackalloc char[2];
                    // A
                    buffer[0] = data[0];
                    buffer[1] = data[0];
                    if (!byte.TryParse(buffer, ns, ci, out a))
                    {
                        return false;
                    }
                    data = data[1..];
                    break;
                case 8:
                    // AA
                    if (!byte.TryParse(data[..2], ns, ci, out a))
                    {
                        return false;
                    }
                    data = data[2..];
                    break;
                default:
                    a = 255;
                    break;
            }

            switch (data.Length)
            {
                case 3:
                    Span<char> buffer = stackalloc char[2];
                    // R
                    buffer[0] = data[0];
                    buffer[1] = data[0];
                    if (!byte.TryParse(buffer, ns, ci, out r))
                    {
                        return false;
                    }
                    // G
                    buffer[0] = data[1];
                    buffer[1] = data[1];
                    if (!byte.TryParse(buffer, ns, ci, out g))
                    {
                        return false;
                    }
                    // B
                    buffer[0] = data[2];
                    buffer[1] = data[2];
                    if (!byte.TryParse(buffer, ns, ci, out b))
                    {
                        return false;
                    }
                    break;
                case 6:
                    // RR
                    if (!byte.TryParse(data[..2], ns, ci, out r))
                    {
                        return false;
                    }
                    // GG
                    if (!byte.TryParse(data[2..4], ns, ci, out g))
                    {
                        return false;
                    }
                    // BB
                    if (!byte.TryParse(data[4..6], ns, ci, out b))
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }

            color = new HexColor(a, r, g, b);
            return true;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool withAlpha)
        {
            var hex = "#";

            var value = (R << 16) | (G << 8) | B;

            if (withAlpha)
            {
                value |= A << 24;
                hex += value.ToString("X8", CultureInfo.InvariantCulture);
            }
            else
            {
                hex += value.ToString("X6", CultureInfo.InvariantCulture);
            }

            return hex;
        }

        public RgbColor ToRgb()
        {
            return new RgbColor(R, G, B);
        }
    }
}
