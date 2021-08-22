using System;
using System.Globalization;
using System.Windows.Media;

namespace ColorTools
{
    public static class ColorExtensions
    {
        public static bool TryParseHex(this string hex, out Color color)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return false;
            }

            byte a, r, g, b;

            Span<char> buffer = stackalloc char[2];
            var ns = NumberStyles.HexNumber;
            var ci = CultureInfo.InvariantCulture;

            var data = hex.AsSpan();
            while (data[0] == '#')
            {
                data = data[1..];
            }

            // A
            switch (data.Length)
            {
                case 4:
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
                    // A
                    if (!byte.TryParse(data[..2], ns, ci, out a))
                    {
                        return false;
                    }
                    data = data[2..];
                    break;
                default:
                    a = 0xFF;
                    break;
            }

            // RGB
            switch (data.Length)
            {
                case 3:
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
                    // R
                    if (!byte.TryParse(data[..2], ns, ci, out r))
                    {
                        return false;
                    }
                    // G
                    if (!byte.TryParse(data[2..4], ns, ci, out g))
                    {
                        return false;
                    }
                    // B
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
                hex += value.ToString("X8");
            }
            else
            {
                hex += value.ToString("X6");
            }

            return hex;
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
