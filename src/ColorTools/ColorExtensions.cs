using System;
using System.Globalization;
using System.Windows.Media;

namespace Koda.ColorTools
{
    public static class ColorExtensions
    {
        public static Color? ParseHex(this string hex)
        {
            return TryParseHex(hex, out var color) ? color : null;
        }

        public static bool TryParseHex(this string hex, out Color color)
        {
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

            color = Color.FromArgb(a, r, g, b);
            return true;
        }

        public static string ToHex(this Color color, bool withAlpha)
        {
            var hex = "#";

            var value = (color.R << 16) | (color.G << 8) | color.B;

            if (withAlpha)
            {
                value |= color.A << 24;
                hex += value.ToString("X8", CultureInfo.InvariantCulture);
            }
            else
            {
                hex += value.ToString("X6", CultureInfo.InvariantCulture);
            }

            return hex;
        }

        public static RgbColor ToRgb(this Color color)
        {
            return new RgbColor(color.R, color.G, color.B);
        }

        public static HsbColor ToHsb(this Color color)
        {
            return color.ToRgb().ToHsb();
        }

        public static HslColor ToHsl(this Color color)
        {
            return color.ToRgb().ToHsl();
        }

        public static Color WithAlpha(this Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        public static Color WithoutAlpha(this Color color)
        {
            return Color.FromRgb(color.R, color.G, color.B);
        }
    }
}
