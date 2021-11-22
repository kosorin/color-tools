using System.Windows.Media;

namespace Koda.ColorTools.Wpf
{
    public static class ColorExtensions
    {
        public static Color ToColor(this IColor color)
        {
            if (color is not RgbColor rgb)
            {
                rgb = color.ToRgb();
            }

            return Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }

        public static Color ToColor(this IColor color, double alpha)
        {
            if (color is not RgbColor rgb)
            {
                rgb = color.ToRgb();
            }

            return Color.FromArgb((byte)(alpha * 255), (byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }

        public static Color ToColor(this HexColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static HexColor ToHex(this Color color)
        {
            return new HexColor(color.A, color.R, color.G, color.B);
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
    }
}
