using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorTools
{
    public class ColorPicker : Control, IColorPicker
    {
        private static readonly Color InitialColor = Colors.Red;

        // TODO: Add routed events: SelectedColorChanged, SelectedHexChanged

        public static readonly DependencyProperty OriginalColorProperty =
            DependencyProperty.Register(nameof(OriginalColor), typeof(Color?), typeof(ColorPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorPicker),
                new FrameworkPropertyMetadata(InitialColor, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged));

        public static readonly DependencyProperty SelectedHexProperty =
            DependencyProperty.Register(nameof(SelectedHex), typeof(string), typeof(ColorPicker),
                new FrameworkPropertyMetadata(InitialColor.ToHex(true), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedHexPropertyChanged, OnCoerceSelectedHex));

        public static readonly DependencyProperty AllowEmptyProperty =
            DependencyProperty.Register(nameof(AllowEmpty), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnAllowEmptyPropertyChanged));

        public static readonly DependencyProperty AllowAlphaProperty =
            DependencyProperty.Register(nameof(AllowAlpha), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnAllowAlphaPropertyChanged));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorPicker), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty EmptyBrushProperty =
            DependencyProperty.Register(nameof(EmptyBrush), typeof(Brush), typeof(ColorPicker), new PropertyMetadata(Brushes.White));

        private bool _isUpdating;

        private bool _isSet;
        private double _alpha;
        private IColor _color;

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public ColorPicker()
        {
            _isSet = true;
            _alpha = 1;
            _color = InitialColor.ToRgb();
            Rgb = _color.ToRgb();
            Hsb = _color.ToHsb();
            Hsl = _color.ToHsl();

            Loaded += OnLoaded;
        }

        public bool IsSet
        {
            get => _isSet;
            set
            {
                _isSet = value;

                OnIsSetChanged();
            }
        }

        public double Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value switch
                {
                    < 0 => 0,
                    > 1 => 1,
                    var a => a,
                };

                OnAlphaChanged();
            }
        }

        public IColor Color
        {
            get => _color;
            set
            {
                _color = value;
                Rgb = _color.ToRgb();
                Hsb = _color.ToHsb();
                Hsl = _color.ToHsl();

                OnColorChanged();
            }
        }

        public RgbColor Rgb { get; private set; }

        public HsbColor Hsb { get; private set; }

        public HslColor Hsl { get; private set; }

        public Color? OriginalColor
        {
            get => (Color?)GetValue(OriginalColorProperty);
            set => SetValue(OriginalColorProperty, value);
        }

        public Color? SelectedColor
        {
            get => (Color?)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public string SelectedHex
        {
            get => (string)GetValue(SelectedHexProperty);
            set => SetValue(SelectedHexProperty, value);
        }

        public bool AllowEmpty
        {
            get => (bool)GetValue(AllowEmptyProperty);
            set => SetValue(AllowEmptyProperty, value);
        }

        public bool AllowAlpha
        {
            get => (bool)GetValue(AllowAlphaProperty);
            set => SetValue(AllowAlphaProperty, value);
        }

        public Brush AlphaBrush
        {
            get => (Brush)GetValue(AlphaBrushProperty);
            set => SetValue(AlphaBrushProperty, value);
        }

        public Brush EmptyBrush
        {
            get => (Brush)GetValue(EmptyBrushProperty);
            set => SetValue(EmptyBrushProperty, value);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            ReplaceSelectedColor(SelectedColor);
        }

        private static void OnSelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorPicker colorPicker)
            {
                colorPicker.OnSelectedColorChanged();
            }
        }

        private static void OnSelectedHexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorPicker colorPicker)
            {
                colorPicker.OnSelectedHexChanged();
            }
        }

        private static object OnCoerceSelectedHex(DependencyObject d, object baseValue)
        {
            // Ensure SelectedHex is not null
            return baseValue switch
            {
                string hex => hex,
                _ => string.Empty,
            };
        }

        private static void OnAllowEmptyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorPicker colorPicker)
            {
                colorPicker.OnAllowEmptyChanged();
            }
        }

        private static void OnAllowAlphaPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorPicker colorPicker)
            {
                colorPicker.OnAllowAlphaChanged();
            }
        }

        private void OnIsSetChanged()
        {
            IsSetChanged?.Invoke(this, IsSet);

            if (_isUpdating)
            {
                return;
            }

            UpdateSelectedColor(false);
        }

        private void OnAlphaChanged()
        {
            AlphaChanged?.Invoke(this, Alpha);

            if (_isUpdating)
            {
                return;
            }

            UpdateSelectedColor();
        }

        private void OnColorChanged()
        {
            ColorChanged?.Invoke(this, Color);

            if (_isUpdating)
            {
                return;
            }

            UpdateSelectedColor();
        }

        private void OnSelectedColorChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            ReplaceSelectedColor(SelectedColor);
        }

        private void OnSelectedHexChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            ReplaceSelectedColor(SelectedHex.ParseHex());
        }

        private void OnAllowEmptyChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            UpdateSelectedColor();
        }

        private void OnAllowAlphaChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            if (!IsSet)
            {
                return;
            }

            UpdateSelectedColor();
        }

        private void ReplaceSelectedColor(Color? selectedColor)
        {
            _isUpdating = true;
            try
            {
                if (!selectedColor.HasValue && !AllowEmpty)
                {
                    selectedColor = Color.ToColor(AllowAlpha ? Alpha : 1);
                }

                if (selectedColor.HasValue)
                {
                    if (selectedColor.Value.A != 255 && !AllowAlpha)
                    {
                        selectedColor = selectedColor.Value.WithoutAlpha();
                    }

                    IsSet = true;
                    Alpha = selectedColor.Value.A / 255d;
                    Color = selectedColor.Value.ToRgb();

                    SetCurrentValue(SelectedColorProperty, selectedColor.Value);
                    SetCurrentValue(SelectedHexProperty, selectedColor.Value.ToHex(AllowAlpha));
                }
                else
                {
                    IsSet = false;
                    SetCurrentValue(SelectedColorProperty, null);
                    SetCurrentValue(SelectedHexProperty, string.Empty);
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateSelectedColor(bool reset = true)
        {
            _isUpdating = true;
            try
            {
                if (reset)
                {
                    IsSet = true;
                }

                if (IsSet)
                {
                    var selectedColor = Color.ToColor(AllowAlpha ? Alpha : 1);

                    SetCurrentValue(SelectedColorProperty, selectedColor);
                    SetCurrentValue(SelectedHexProperty, selectedColor.ToHex(AllowAlpha));
                }
                else
                {
                    SetCurrentValue(SelectedColorProperty, null);
                    SetCurrentValue(SelectedHexProperty, string.Empty);
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public event IsSetChanged? IsSetChanged;

        public event AlphaChanged? AlphaChanged;

        public event ColorChanged? ColorChanged;
    }
}
