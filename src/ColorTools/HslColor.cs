using System.Windows.Media;

namespace ColorTools
{
    public readonly struct HslColor : IColor
    {
        public HslColor(double h, double s, double l)
        {
            while (h < 0)
            {
                h += 360;
            }
            while (h > 360)
            {
                h -= 360;
            }
            H = h;
            S = s switch
            {
                < 0 => 0,
                > 100 => 100,
                _ => s,
            };
            L = l switch
            {
                < 0 => 0,
                > 100 => 100,
                _ => l,
            };
        }

        /// <summary>
        /// Hue [0,360]
        /// </summary>
        public double H { get; }

        /// <summary>
        /// Saturation [0,100]
        /// </summary>
        public double S { get; }

        /// <summary>
        /// Lightness [0,100]
        /// </summary>
        public double L { get; }

        public HslColor Lighten(double amount)
        {
            return new HslColor(H, S, L * amount);
        }

        public Color ToColor()
        {
            return ToRgb().ToColor();
        }

        public Color ToColor(byte alpha)
        {
            return ToRgb().ToColor(alpha);
        }

        public RgbColor ToRgb()
        {
            double r, g, b;

            var h = H / 60;
            var s = S / 100;
            var l = L / 100;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                var q = l <= 0.5
                    ? l * (1 + s)
                    : l + s - l * s;
                var p = 2 * l - q;

                r = HueToRgb(p, q, h + 2);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 2);
            }

            return new RgbColor(r * 255, g * 255, b * 255);

            static double HueToRgb(double p, double q, double h)
            {
                switch (h)
                {
                    case < 0:
                        h += 6;
                        break;
                    case > 6:
                        h -= 6;
                        break;
                }

                return h switch
                {
                    < 1 => p + (q - p) * h,
                    < 3 => q,
                    < 4 => p + (q - p) * (4 - h),
                    _ => p,
                };
            }
        }

        public HsbColor ToHsb()
        {
            var ml = L < 50 ? L : 100 - L;
            var b = S * ml * 0.01 + L;
            var s = b > 0
                ? 200 - 200 * L / b
                : 0;
            return new HsbColor(H, s, b);
        }

        public HslColor ToHsl()
        {
            return this;
        }
    }
}
