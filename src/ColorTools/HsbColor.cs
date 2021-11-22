namespace Koda.ColorTools
{
    public readonly struct HsbColor : IColor
    {
        public HsbColor(double h, double s, double b)
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
            B = b switch
            {
                < 0 => 0,
                > 100 => 100,
                _ => b,
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
        /// Brightness/Value [0,100]
        /// </summary>
        public double B { get; }

        public void Deconstruct(out double h, out double s, out double b)
        {
            h = H;
            s = S;
            b = B;
        }

        public RgbColor ToRgb()
        {
            double r, g, b;

            var h = H / 60;
            var s = S / 100;
            var v = B / 100;

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

        public HsbColor ToHsb()
        {
            return this;
        }

        public HslColor ToHsl()
        {
            var l = B - B * S / 200;
            var ml = l < 50 ? l : 100 - l;
            var s = ml > 0
                ? 100 * (B - l) / ml
                : 0;
            return new HslColor(H, s, l);
        }
    }
}
