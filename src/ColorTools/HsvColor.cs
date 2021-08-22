using System.Windows.Media;

namespace ColorTools
{
    public readonly struct HsvColor : IColor
    {
        public HsvColor(double h, double s, double v)
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
            V = v switch
            {
                < 0 => 0,
                > 100 => 100,
                _ => v,
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
        /// Brightness (value) [0,100]
        /// </summary>
        public double V { get; }

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
            var v = V / 100;

            if (s == 0)
            {
                r = g = b = v;
            }
            else
            {
                var i = (int)h;
                var f = h - i;

                var p = v * (1 - s);
                var q = v * (1 - s * f);
                var t = v * (1 - s * (1 - f));

                switch (i % 6)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                    default:
                        r = g = b = 0;
                        break;
                }
            }

            return new RgbColor(r * 255, g * 255, b * 255);
        }

        public HsvColor ToHsv()
        {
            return this;
        }

        public HslColor ToHsl()
        {
            var l = V - V * S / 200;
            var ml = l < 50 ? l : 100 - l;
            var s = ml > 0
                ? 100 * (V - l) / ml
                : 0;
            return new HslColor(H, s, l);
        }
    }
}
