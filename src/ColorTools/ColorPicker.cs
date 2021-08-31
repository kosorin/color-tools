using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ColorTools
{
    [TemplatePart(Name = nameof(AlphaSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbRedSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbGreenSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbBlueSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbHueSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbSaturationSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbBrightnessSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HslHueSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HslSaturationSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HslLightnessSlider), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbSaturationBrightnessCanvas), Type = typeof(ColorCanvas))]
    [TemplatePart(Name = nameof(ColorModelSelector), Type = typeof(Selector))]
    public class ColorPicker : Control
    {
        private static readonly Color DefaultColor = Colors.Red;
        private static readonly Color DefaultEmptyColor = Colors.White;

        // TODO: Add routed events: SelectedColorChanged, SelectedHexChanged

        public static readonly DependencyProperty ColorModelProperty =
            DependencyProperty.Register(nameof(ColorModel), typeof(ColorModel), typeof(ColorPicker), new PropertyMetadata(ColorModel.Hsl, OnColorModelPropertyChanged));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorPicker),
                new FrameworkPropertyMetadata(DefaultColor, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged));

        public static readonly DependencyProperty SelectedHexProperty =
            DependencyProperty.Register(nameof(SelectedHex), typeof(string), typeof(ColorPicker),
                new FrameworkPropertyMetadata(DefaultColor.ToHex(true), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedHexPropertyChanged));

        public static readonly DependencyProperty OriginalColorProperty =
            DependencyProperty.Register(nameof(OriginalColor), typeof(Color?), typeof(ColorPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty AllowEmptyProperty =
            DependencyProperty.Register(nameof(AllowEmpty), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnAllowEmptyPropertyChanged));

        public static readonly DependencyProperty AllowAlphaProperty =
            DependencyProperty.Register(nameof(AllowAlpha), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnAllowAlphaPropertyChanged));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorPicker), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty EmptyBrushProperty =
            DependencyProperty.Register(nameof(EmptyBrush), typeof(Brush), typeof(ColorPicker), new PropertyMetadata(Brushes.White));

        private ColorSlider? _alphaSlider;
        private ColorSlider? _rgbRedSlider;
        private ColorSlider? _rgbGreenSlider;
        private ColorSlider? _rgbBlueSlider;
        private ColorSlider? _hsbHueSlider;
        private ColorSlider? _hsbSaturationSlider;
        private ColorSlider? _hsbBrightnessSlider;
        private ColorSlider? _hslHueSlider;
        private ColorSlider? _hslSaturationSlider;
        private ColorSlider? _hslLightnessSlider;
        private ColorCanvas? _hsbSaturationBrightnessCanvas;
        private Selector? _colorModelSelector;

        private bool _isUpdating;

        private bool _isSet = true;
        private double _alpha = DefaultColor.A;
        private readonly ColorState _colorState = new ColorState(DefaultColor.ToRgb());

        public ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

            Loaded += OnLoaded;
        }

        public ColorModel ColorModel
        {
            get => (ColorModel)GetValue(ColorModelProperty);
            set => SetValue(ColorModelProperty, value);
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

        public Color? OriginalColor
        {
            get => (Color?)GetValue(OriginalColorProperty);
            set => SetValue(OriginalColorProperty, value);
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

        private ColorSlider? AlphaSlider
        {
            get => _alphaSlider;
            set
            {
                if (_alphaSlider != null)
                {
                    _alphaSlider.ValueChanged -= OnAlphaChanged;
                }

                _alphaSlider = value;

                if (_alphaSlider != null)
                {
                    _alphaSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => colorState.Rgb.ToColor((byte)(value * 255))));
                    _alphaSlider.Update(_alpha);
                    _alphaSlider.ValueChanged += OnAlphaChanged;
                }
            }
        }

        private ColorSlider? RgbRedSlider
        {
            get => _rgbRedSlider;
            set
            {
                if (_rgbRedSlider != null)
                {
                    _rgbRedSlider.ValueChanged -= OnRgbRedChanged;
                }

                _rgbRedSlider = value;

                if (_rgbRedSlider != null)
                {
                    _rgbRedSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => new RgbColor((byte)(value * 255), colorState.Rgb.G, colorState.Rgb.B).ToColor()));
                    _rgbRedSlider.Update(_colorState.Rgb.R);
                    _rgbRedSlider.ValueChanged += OnRgbRedChanged;
                }
            }
        }

        private ColorSlider? RgbGreenSlider
        {
            get => _rgbGreenSlider;
            set
            {
                if (_rgbGreenSlider != null)
                {
                    _rgbGreenSlider.ValueChanged -= OnRgbGreenChanged;
                }

                _rgbGreenSlider = value;

                if (_rgbGreenSlider != null)
                {
                    _rgbGreenSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => new RgbColor(colorState.Rgb.R, (byte)(value * 255), colorState.Rgb.B).ToColor()));
                    _rgbGreenSlider.Update(_colorState.Rgb.G);
                    _rgbGreenSlider.ValueChanged += OnRgbGreenChanged;
                }
            }
        }

        private ColorSlider? RgbBlueSlider
        {
            get => _rgbBlueSlider;
            set
            {
                if (_rgbBlueSlider != null)
                {
                    _rgbBlueSlider.ValueChanged -= OnRgbBlueChanged;
                }

                _rgbBlueSlider = value;

                if (_rgbBlueSlider != null)
                {
                    _rgbBlueSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => new RgbColor(colorState.Rgb.R, colorState.Rgb.G, (byte)(value * 255)).ToColor()));
                    _rgbBlueSlider.Update(_colorState.Rgb.B);
                    _rgbBlueSlider.ValueChanged += OnRgbBlueChanged;
                }
            }
        }

        private ColorSlider? HsbHueSlider
        {
            get => _hsbHueSlider;
            set
            {
                if (_hsbHueSlider != null)
                {
                    _hsbHueSlider.ValueChanged -= OnHsbHueChanged;
                }

                _hsbHueSlider = value;

                if (_hsbHueSlider != null)
                {
                    _hsbHueSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 7, static (value, colorState) => new HsbColor(value * 360, colorState.Hsb.S, colorState.Hsb.B).ToColor()));
                    _hsbHueSlider.Update(_colorState.Hsb.H);
                    _hsbHueSlider.ValueChanged += OnHsbHueChanged;
                }
            }
        }

        private ColorSlider? HsbSaturationSlider
        {
            get => _hsbSaturationSlider;
            set
            {
                if (_hsbSaturationSlider != null)
                {
                    _hsbSaturationSlider.ValueChanged -= OnHsbSaturationChanged;
                }

                _hsbSaturationSlider = value;

                if (_hsbSaturationSlider != null)
                {
                    _hsbSaturationSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => new HsbColor(colorState.Hsb.H, value * 100, colorState.Hsb.B).ToColor()));
                    _hsbSaturationSlider.Update(_colorState.Hsb.S);
                    _hsbSaturationSlider.ValueChanged += OnHsbSaturationChanged;
                }
            }
        }

        private ColorSlider? HsbBrightnessSlider
        {
            get => _hsbBrightnessSlider;
            set
            {
                if (_hsbBrightnessSlider != null)
                {
                    _hsbBrightnessSlider.ValueChanged -= OnHsbBrightnessChanged;
                }

                _hsbBrightnessSlider = value;

                if (_hsbBrightnessSlider != null)
                {
                    _hsbBrightnessSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => new HsbColor(colorState.Hsb.H, colorState.Hsb.S, value * 100).ToColor()));
                    _hsbBrightnessSlider.Update(_colorState.Hsb.B);
                    _hsbBrightnessSlider.ValueChanged += OnHsbBrightnessChanged;
                }
            }
        }

        private ColorSlider? HslHueSlider
        {
            get => _hslHueSlider;
            set
            {
                if (_hslHueSlider != null)
                {
                    _hslHueSlider.ValueChanged -= OnHslHueChanged;
                }

                _hslHueSlider = value;

                if (_hslHueSlider != null)
                {
                    _hslHueSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 7, static (value, colorState) => new HslColor(value * 360, colorState.Hsl.S, colorState.Hsl.L).ToColor()));
                    _hslHueSlider.Update(_colorState.Hsl.H);
                    _hslHueSlider.ValueChanged += OnHslHueChanged;
                }
            }
        }

        private ColorSlider? HslSaturationSlider
        {
            get => _hslSaturationSlider;
            set
            {
                if (_hslSaturationSlider != null)
                {
                    _hslSaturationSlider.ValueChanged -= OnHslSaturationChanged;
                }

                _hslSaturationSlider = value;

                if (_hslSaturationSlider != null)
                {
                    _hslSaturationSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 2, static (value, colorState) => new HslColor(colorState.Hsl.H, value * 100, colorState.Hsl.L).ToColor()));
                    _hslSaturationSlider.Update(_colorState.Hsl.S);
                    _hslSaturationSlider.ValueChanged += OnHslSaturationChanged;
                }
            }
        }

        private ColorSlider? HslLightnessSlider
        {
            get => _hslLightnessSlider;
            set
            {
                if (_hslLightnessSlider != null)
                {
                    _hslLightnessSlider.ValueChanged -= OnHslLightnessChanged;
                }

                _hslLightnessSlider = value;

                if (_hslLightnessSlider != null)
                {
                    _hslLightnessSlider.InitializeBrushSource(new SliderBrushSource(_colorState, 3, static (value, colorState) => new HslColor(colorState.Hsl.H, colorState.Hsl.S, value * 100).ToColor()));
                    _hslLightnessSlider.Update(_colorState.Hsl.L);
                    _hslLightnessSlider.ValueChanged += OnHslLightnessChanged;
                }
            }
        }

        private ColorCanvas? HsbSaturationBrightnessCanvas
        {
            get => _hsbSaturationBrightnessCanvas;
            set
            {
                if (_hsbSaturationBrightnessCanvas != null)
                {
                    _hsbSaturationBrightnessCanvas.ValueChanged -= OnHsbSaturationBrightnessChanged;
                }

                _hsbSaturationBrightnessCanvas = value;

                if (_hsbSaturationBrightnessCanvas != null)
                {
                    _hsbSaturationBrightnessCanvas.InitializeBrushSource(new HsbCanvasBrushSource(_colorState));
                    _hsbSaturationBrightnessCanvas.Update(new Point(_colorState.Hsb.S * 0.01, _colorState.Hsb.B * 0.01));
                    _hsbSaturationBrightnessCanvas.ValueChanged += OnHsbSaturationBrightnessChanged;
                }
            }
        }

        private Selector? ColorModelSelector
        {
            get => _colorModelSelector;
            set
            {
                _colorModelSelector = value;

                if (_colorModelSelector != null)
                {
                    _colorModelSelector.ItemsSource = Enum.GetValues<ColorModel>();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AlphaSlider = GetTemplateChild(nameof(AlphaSlider)) as ColorSlider;

            RgbRedSlider = GetTemplateChild(nameof(RgbRedSlider)) as ColorSlider;
            RgbGreenSlider = GetTemplateChild(nameof(RgbGreenSlider)) as ColorSlider;
            RgbBlueSlider = GetTemplateChild(nameof(RgbBlueSlider)) as ColorSlider;
            HsbHueSlider = GetTemplateChild(nameof(HsbHueSlider)) as ColorSlider;
            HsbSaturationSlider = GetTemplateChild(nameof(HsbSaturationSlider)) as ColorSlider;
            HsbBrightnessSlider = GetTemplateChild(nameof(HsbBrightnessSlider)) as ColorSlider;
            HslHueSlider = GetTemplateChild(nameof(HslHueSlider)) as ColorSlider;
            HslSaturationSlider = GetTemplateChild(nameof(HslSaturationSlider)) as ColorSlider;
            HslLightnessSlider = GetTemplateChild(nameof(HslLightnessSlider)) as ColorSlider;

            HsbSaturationBrightnessCanvas = GetTemplateChild(nameof(HsbSaturationBrightnessCanvas)) as ColorCanvas;

            ColorModelSelector = GetTemplateChild(nameof(ColorModelSelector)) as Selector;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            ReplaceSelectedColor(SelectedColor);
        }

        private static void OnColorModelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorPicker colorPicker)
            {
                colorPicker.OnColorModelChanged();
            }
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

        private void OnColorModelChanged()
        {
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

            if (!_isSet && !AllowEmpty)
            {
                _colorState.Update(DefaultEmptyColor.ToRgb());
            }

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnAllowAlphaChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            if (!_isSet)
            {
                return;
            }

            UpdateSelectedColor();
        }

        private void OnAlphaChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _alpha = args.NewValue;

            UpdateSelectedColor();
        }

        private void OnRgbRedChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new RgbColor(args.NewValue, _colorState.Rgb.G, _colorState.Rgb.B));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnRgbGreenChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new RgbColor(_colorState.Rgb.R, args.NewValue, _colorState.Rgb.B));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnRgbBlueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new RgbColor(_colorState.Rgb.R, _colorState.Rgb.G, args.NewValue));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHsbHueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HsbColor(args.NewValue, _colorState.Hsb.S, _colorState.Hsb.B));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHsbSaturationChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HsbColor(_colorState.Hsb.H, args.NewValue, _colorState.Hsb.B));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHsbBrightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HsbColor(_colorState.Hsb.H, _colorState.Hsb.S, args.NewValue));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHslHueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HslColor(args.NewValue, _colorState.Hsl.S, _colorState.Hsl.L));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHslSaturationChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HslColor(_colorState.Hsl.H, args.NewValue, _colorState.Hsl.L));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHslLightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HslColor(_colorState.Hsl.H, _colorState.Hsl.S, args.NewValue));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void OnHsbSaturationBrightnessChanged(object sender, RoutedPropertyChangedEventArgs<Point> args)
        {
            if (_isUpdating)
            {
                return;
            }

            _colorState.Update(new HsbColor(_colorState.Hsb.H, args.NewValue.X * 100, args.NewValue.Y * 100));

            UpdateSelectedColor();
            UpdateComponents();
        }

        private void ReplaceSelectedColor(Color? selectedColor)
        {
            _isUpdating = true;
            try
            {
                if (!selectedColor.HasValue && !AllowEmpty)
                {
                    selectedColor = DefaultEmptyColor;
                }

                if (selectedColor.HasValue)
                {
                    if (!AllowAlpha)
                    {
                        selectedColor = selectedColor.Value.WithoutAlpha();
                    }

                    _alpha = selectedColor.Value.A;
                    _colorState.Update(selectedColor.Value.ToRgb());

                    _isSet = true;
                    SetCurrentValue(SelectedColorProperty, selectedColor.Value);
                    SetCurrentValue(SelectedHexProperty, selectedColor.Value.ToHex(AllowAlpha));

                    UpdateComponents();
                }
                else
                {
                    _isSet = false;
                    SetCurrentValue(SelectedColorProperty, null);
                    SetCurrentValue(SelectedHexProperty, string.Empty);
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateSelectedColor()
        {
            _isUpdating = true;
            try
            {
                var selectedColor = _colorState.Rgb.ToColor(AllowAlpha ? (byte)_alpha : (byte)0xFF);

                _isSet = true;
                SetCurrentValue(SelectedColorProperty, selectedColor);
                SetCurrentValue(SelectedHexProperty, selectedColor.ToHex(AllowAlpha));
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateComponents()
        {
            _isUpdating = true;
            try
            {
                AlphaSlider?.Update(_alpha);

                RgbRedSlider?.Update(_colorState.Rgb.R);
                RgbGreenSlider?.Update(_colorState.Rgb.G);
                RgbBlueSlider?.Update(_colorState.Rgb.B);
                HsbHueSlider?.Update(_colorState.Hsb.H);
                HsbSaturationSlider?.Update(_colorState.Hsb.S);
                HsbBrightnessSlider?.Update(_colorState.Hsb.B);
                HslHueSlider?.Update(_colorState.Hsl.H);
                HslSaturationSlider?.Update(_colorState.Hsl.S);
                HslLightnessSlider?.Update(_colorState.Hsl.L);

                HsbSaturationBrightnessCanvas?.Update(new Point(_colorState.Hsb.S * 0.01, _colorState.Hsb.B * 0.01));
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
