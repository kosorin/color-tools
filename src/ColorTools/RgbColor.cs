using System.Windows.Media;

namespace ColorTools
{
    public readonly struct RgbColor : IColor
    {
        public RgbColor(double r, double g, double b)
        {
            R = r switch
            {
                < 0 => 0,
                > 255 => 255,
                _ => r,
            };
            G = g switch
            {
                < 0 => 0,
                > 255 => 255,
                _ => g,
            };
            B = b switch
            {
                < 0 => 0,
                > 255 => 255,
                _ => b,
            };
        }

        public RgbColor(Color color) : this(color.R, color.G, color.B)
        {
        }

        /// <summary>
        /// Red [0,255]
        /// </summary>
        public double R { get; }

        /// <summary>
        /// Green [0,255]
        /// </summary>
        public double G { get; }

        /// <summary>
        /// Blue [0,255]
        /// </summary>
        public double B { get; }

        public Color ToColor()
        {
            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }

        public Color ToColor(byte alpha)
        {
            return Color.FromArgb(alpha, (byte)R, (byte)G, (byte)B);
        }

        public RgbColor ToRgb()
        {
            return this;
        }

        public HsbColor ToHsb()
        {
            var max = R;
            if (max < G)
            {
                max = G;
            }
            if (max < B)
            {
                max = B;
            }

            var min = R;
            if (min > G)
            {
                min = G;
            }
            if (min > B)
            {
                min = B;
            }

            var delta = max - min;

            var h = 0d;
            var s = 0d;
            var v = max / 255d;

            if (max != 0 && delta != 0)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (R == max)
                {
                    h = (G - B) / delta;
                }
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                else if (G == max)
                {
                    h = 2 + (B - R) / delta;
                }
                else
                {
                    h = 4 + (R - G) / delta;
                }

                if (h < 0)
                {
                    h += 6;
                }

                s = delta / max;
            }

            return new HsbColor(h * 60, s * 100, v * 100);
        }

        public HslColor ToHsl()
        {
            var max = R;
            if (max < G)
            {
                max = G;
            }
            if (max < B)
            {
                max = B;
            }

            var min = R;
            if (min > G)
            {
                min = G;
            }
            if (min > B)
            {
                min = B;
            }

            var delta = max - min;
            var sum = max + min;

            var h = 0d;
            var s = 0d;
            var l = sum / (2 * 255);

            if (max != 0 && delta != 0)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (R == max)
                {
                    h = (G - B) / delta;
                }
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                else if (G == max)
                {
                    h = 2 + (B - R) / delta;
                }
                else
                {
                    h = 4 + (R - G) / delta;
                }

                if (h < 0)
                {
                    h += 6;
                }
            }

            if (delta != 0)
            {
                if (l <= 0.5)
                {
                    s = delta / sum;
                }
                else
                {
                    s = delta / (2 * 255 - sum);
                }
            }

            return new HslColor(h * 60, s * 100, l * 100);
        }
    }
}
