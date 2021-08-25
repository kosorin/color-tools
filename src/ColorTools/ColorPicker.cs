using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorTools
{
    [TemplatePart(Name = nameof(AlphaComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbRedComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbGreenComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbBlueComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbHueComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbSaturationComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsbBrightnessComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HslHueComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HslSaturationComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HslLightnessComponent), Type = typeof(ColorSlider))]
    public class ColorPicker : Control
    {
        private const byte MaxAlpha = 0xFF;

        // TODO: Add routed events: SelectedColorChanged, SelectedHexChanged

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged));

        public static readonly DependencyProperty SelectedHexProperty =
            DependencyProperty.Register(nameof(SelectedHex), typeof(string), typeof(ColorPicker),
                new FrameworkPropertyMetadata("#FFFF0000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedHexPropertyChanged));

        public static readonly DependencyProperty OriginalColorProperty =
            DependencyProperty.Register(nameof(OriginalColor), typeof(Color?), typeof(ColorPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty AllowEmptyProperty =
            DependencyProperty.Register(nameof(AllowEmpty), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnAllowEmptyPropertyChanged));

        public static readonly DependencyProperty AllowAlphaProperty =
            DependencyProperty.Register(nameof(AllowAlpha), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnAllowAlphaPropertyChanged));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorPicker), new PropertyMetadata(Brushes.Transparent));

        private bool _isUpdating;

        public ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

            AlphaComponent = new ColorComponent(OnAlphaChanged);
            RgbRedComponent = new ColorComponent(OnRgbChanged);
            RgbGreenComponent = new ColorComponent(OnRgbChanged);
            RgbBlueComponent = new ColorComponent(OnRgbChanged);
            HsbHueComponent = new ColorComponent(OnHsbChanged);
            HsbSaturationComponent = new ColorComponent(OnHsbChanged);
            HsbBrightnessComponent = new ColorComponent(OnHsbChanged);
            HslHueComponent = new ColorComponent(OnHslChanged);
            HslSaturationComponent = new ColorComponent(OnHslChanged);
            HslLightnessComponent = new ColorComponent(OnHslChanged);

            Loaded += OnLoaded;
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

        private ColorComponent AlphaComponent { get; }

        private ColorComponent RgbRedComponent { get; }

        private ColorComponent RgbGreenComponent { get; }

        private ColorComponent RgbBlueComponent { get; }

        private ColorComponent HsbHueComponent { get; }

        private ColorComponent HsbSaturationComponent { get; }

        private ColorComponent HsbBrightnessComponent { get; }

        private ColorComponent HslHueComponent { get; }

        private ColorComponent HslSaturationComponent { get; }

        private ColorComponent HslLightnessComponent { get; }

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _isUpdating = false;

            AlphaComponent.AttachSlider(GetTemplateChild(nameof(AlphaComponent)) as ColorSlider);
            RgbRedComponent.AttachSlider(GetTemplateChild(nameof(RgbRedComponent)) as ColorSlider);
            RgbGreenComponent.AttachSlider(GetTemplateChild(nameof(RgbGreenComponent)) as ColorSlider);
            RgbBlueComponent.AttachSlider(GetTemplateChild(nameof(RgbBlueComponent)) as ColorSlider);
            HsbHueComponent.AttachSlider(GetTemplateChild(nameof(HsbHueComponent)) as ColorSlider);
            HsbSaturationComponent.AttachSlider(GetTemplateChild(nameof(HsbSaturationComponent)) as ColorSlider);
            HsbBrightnessComponent.AttachSlider(GetTemplateChild(nameof(HsbBrightnessComponent)) as ColorSlider);
            HslHueComponent.AttachSlider(GetTemplateChild(nameof(HslHueComponent)) as ColorSlider);
            HslSaturationComponent.AttachSlider(GetTemplateChild(nameof(HslSaturationComponent)) as ColorSlider);
            HslLightnessComponent.AttachSlider(GetTemplateChild(nameof(HslLightnessComponent)) as ColorSlider);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            OnSelectedColorChanged();
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

            var color = SelectedColor switch
            {
                null when !AllowEmpty => new RgbColor(255, 255, 255),
                { } selectedColor => selectedColor.ToRgb(),
                _ => new RgbColor(RgbRedComponent.Value, RgbGreenComponent.Value, RgbBlueComponent.Value)
            };

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
            UpdateColorComponents(color);
        }

        private void OnAllowAlphaChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            if (SelectedColor is { } selectedColor)
            {
                var color = selectedColor.ToRgb();

                UpdateSelectedColor(color);
                UpdateAlphaComponent(AlphaComponent.Value, color);
            }
        }

        private void OnAlphaChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = SelectedColor?.ToRgb() ?? new RgbColor(RgbRedComponent.Value, RgbGreenComponent.Value, RgbBlueComponent.Value);

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
        }

        private void OnRgbChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = new RgbColor(RgbRedComponent.Value, RgbGreenComponent.Value, RgbBlueComponent.Value);

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
            UpdateColorComponents(color);
        }

        private void OnHsbChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = new HsbColor(HsbHueComponent.Value, HsbSaturationComponent.Value, HsbBrightnessComponent.Value);

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
            UpdateColorComponents(color);
        }

        private void OnHslChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = new HslColor(HslHueComponent.Value, HslSaturationComponent.Value, HslLightnessComponent.Value);

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
            UpdateColorComponents(color);
        }

        private void ReplaceSelectedColor(Color? selectedColor)
        {
            _isUpdating = true;
            try
            {
                if (!selectedColor.HasValue && !AllowEmpty)
                {
                    selectedColor = Colors.White;
                }

                if (selectedColor.HasValue)
                {
                    if (!AllowAlpha)
                    {
                        selectedColor = selectedColor.Value.WithoutAlpha();
                    }

                    SetCurrentValue(SelectedColorProperty, selectedColor.Value);
                    SetCurrentValue(SelectedHexProperty, selectedColor.Value.ToHex(AllowAlpha));

                    var color = selectedColor.Value.ToRgb();

                    UpdateAlphaComponent(selectedColor.Value.A, color);
                    UpdateColorComponents(color);
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

        private void UpdateSelectedColor(IColor color)
        {
            _isUpdating = true;
            try
            {
                var selectedColor = color.ToColor(AllowAlpha ? (byte)AlphaComponent.Value : MaxAlpha);

                SetCurrentValue(SelectedColorProperty, selectedColor);
                SetCurrentValue(SelectedHexProperty, selectedColor.ToHex(AllowAlpha));
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateAlphaComponent(double alpha, IColor color)
        {
            _isUpdating = true;
            try
            {
                AlphaComponent.SetValue(alpha, value => color.ToColor((byte)value));
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateColorComponents(IColor color)
        {
            _isUpdating = true;
            try
            {
                var rgb = color.ToRgb();
                RgbRedComponent.SetValue(rgb.R, value => new RgbColor(value, rgb.G, rgb.B).ToColor());
                RgbGreenComponent.SetValue(rgb.G, value => new RgbColor(rgb.R, value, rgb.B).ToColor());
                RgbBlueComponent.SetValue(rgb.B, value => new RgbColor(rgb.R, rgb.G, value).ToColor());

                var hsb = color.ToHsb();
                HsbHueComponent.SetValue(hsb.H, value => new HsbColor(value, hsb.S, hsb.B).ToColor());
                HsbSaturationComponent.SetValue(hsb.S, value => new HsbColor(hsb.H, value, hsb.B).ToColor());
                HsbBrightnessComponent.SetValue(hsb.B, value => new HsbColor(hsb.H, hsb.S, value).ToColor());

                var hsl = color.ToHsl();
                HslHueComponent.SetValue(hsl.H, value => new HslColor(value, hsl.S, hsl.L).ToColor());
                HslSaturationComponent.SetValue(hsl.S, value => new HslColor(hsl.H, value, hsl.L).ToColor());
                HslLightnessComponent.SetValue(hsl.L, value => new HslColor(hsl.H, hsl.S, value).ToColor());
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
