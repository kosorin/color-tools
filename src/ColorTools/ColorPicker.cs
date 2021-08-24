using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorTools
{
    [TemplatePart(Name = nameof(AlphaComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbRedComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbGreenComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(RgbBlueComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsvHueComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsvSaturationComponent), Type = typeof(ColorSlider))]
    [TemplatePart(Name = nameof(HsvValueComponent), Type = typeof(ColorSlider))]
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

        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnShowAlphaPropertyChanged));

        private bool _isUpdating;

        public ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

            AlphaComponent = new ColorComponent(OnAlphaChanged);
            RgbRedComponent = new ColorComponent(OnRgbChanged);
            RgbGreenComponent = new ColorComponent(OnRgbChanged);
            RgbBlueComponent = new ColorComponent(OnRgbChanged);
            HsvHueComponent = new ColorComponent(OnHsvChanged);
            HsvSaturationComponent = new ColorComponent(OnHsvChanged);
            HsvValueComponent = new ColorComponent(OnHsvChanged);
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

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        private ColorComponent AlphaComponent { get; }

        private ColorComponent RgbRedComponent { get; }

        private ColorComponent RgbGreenComponent { get; }

        private ColorComponent RgbBlueComponent { get; }

        private ColorComponent HsvHueComponent { get; }

        private ColorComponent HsvSaturationComponent { get; }

        private ColorComponent HsvValueComponent { get; }

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

        private static void OnShowAlphaPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorPicker colorPicker)
            {
                colorPicker.OnShowAlphaChanged();
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
            HsvHueComponent.AttachSlider(GetTemplateChild(nameof(HsvHueComponent)) as ColorSlider);
            HsvSaturationComponent.AttachSlider(GetTemplateChild(nameof(HsvSaturationComponent)) as ColorSlider);
            HsvValueComponent.AttachSlider(GetTemplateChild(nameof(HsvValueComponent)) as ColorSlider);
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

            if (SelectedColor is { } selectedColor)
            {
                ReplaceSelectedColor(selectedColor);

                var color = new RgbColor(selectedColor);
                UpdateAlphaComponent(selectedColor.A, color);
                UpdateColorComponents(color);
            }
            else
            {
                ReplaceSelectedColor(null);
            }

            UpdateColorPreview();
        }

        private void OnSelectedHexChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            if (SelectedHex.TryParseHex(out var selectedColor))
            {
                ReplaceSelectedColor(selectedColor);

                var color = new RgbColor(selectedColor);
                UpdateAlphaComponent(selectedColor.A, color);
                UpdateColorComponents(color);
            }
            else
            {
                ReplaceSelectedColor(null);
            }

            UpdateColorPreview();
        }

        private void OnShowAlphaChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = SelectedColor is { } selectedColor
                ? new RgbColor(selectedColor)
                : new RgbColor(RgbRedComponent.Value, RgbGreenComponent.Value, RgbBlueComponent.Value);

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
            UpdateColorPreview();
        }

        private void OnAlphaChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = SelectedColor is { } selectedColor
                ? new RgbColor(selectedColor)
                : new RgbColor(RgbRedComponent.Value, RgbGreenComponent.Value, RgbBlueComponent.Value);

            UpdateSelectedColor(color);
            UpdateAlphaComponent(AlphaComponent.Value, color);
            UpdateColorPreview();
        }

        private void OnRgbChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = new RgbColor(RgbRedComponent.Value, RgbGreenComponent.Value, RgbBlueComponent.Value);

            UpdateSelectedColor(color);
            UpdateColorComponents(color);
            UpdateColorPreview();
        }

        private void OnHsvChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = new HsvColor(HsvHueComponent.Value, HsvSaturationComponent.Value, HsvValueComponent.Value);

            UpdateSelectedColor(color);
            UpdateColorComponents(color);
            UpdateColorPreview();
        }

        private void OnHslChanged()
        {
            if (_isUpdating)
            {
                return;
            }

            var color = new HslColor(HslHueComponent.Value, HslSaturationComponent.Value, HslLightnessComponent.Value);

            UpdateSelectedColor(color);
            UpdateColorComponents(color);
            UpdateColorPreview();
        }

        private void ReplaceSelectedColor(Color? color)
        {
            if (_isUpdating)
            {
                return;
            }
            _isUpdating = true;

            if (color.HasValue)
            {
                if (!ShowAlpha)
                {
                    color = color.Value.WithoutAlpha();
                }

                SetCurrentValue(SelectedColorProperty, color.Value);
                SetCurrentValue(SelectedHexProperty, color.Value.ToHex(ShowAlpha));
            }
            else
            {
                SetCurrentValue(SelectedColorProperty, null);
                SetCurrentValue(SelectedHexProperty, string.Empty);
            }

            _isUpdating = false;
        }

        private void UpdateSelectedColor(IColor color)
        {
            if (_isUpdating)
            {
                return;
            }
            _isUpdating = true;

            var selectedColor = color.ToColor(ShowAlpha ? (byte)AlphaComponent.Value : MaxAlpha);
            SetCurrentValue(SelectedColorProperty, selectedColor);
            SetCurrentValue(SelectedHexProperty, selectedColor.ToHex(ShowAlpha));

            _isUpdating = false;
        }

        private void UpdateAlphaComponent(double alpha, IColor color)
        {
            if (_isUpdating)
            {
                return;
            }
            _isUpdating = true;

            Debug.WriteLine("Update alpha components");

            AlphaComponent.SetValue(alpha, value => color.ToColor((byte)value));

            _isUpdating = false;
        }

        private void UpdateColorComponents(IColor color)
        {
            if (_isUpdating)
            {
                return;
            }
            _isUpdating = true;

            Debug.WriteLine("Update color components");

            var rgb = color.ToRgb();
            RgbRedComponent.SetValue(rgb.R, value => new RgbColor(value, rgb.G, rgb.B).ToColor());
            RgbGreenComponent.SetValue(rgb.G, value => new RgbColor(rgb.R, value, rgb.B).ToColor());
            RgbBlueComponent.SetValue(rgb.B, value => new RgbColor(rgb.R, rgb.G, value).ToColor());

            var hsv = color.ToHsv();
            HsvHueComponent.SetValue(hsv.H, value => new HsvColor(value, hsv.S, hsv.V).ToColor());
            HsvSaturationComponent.SetValue(hsv.S, value => new HsvColor(hsv.H, value, hsv.V).ToColor());
            HsvValueComponent.SetValue(hsv.V, value => new HsvColor(hsv.H, hsv.S, value).ToColor());

            var hsl = color.ToHsl();
            HslHueComponent.SetValue(hsl.H, value => new HslColor(value, hsl.S, hsl.L).ToColor());
            HslSaturationComponent.SetValue(hsl.S, value => new HslColor(hsl.H, value, hsl.L).ToColor());
            HslLightnessComponent.SetValue(hsl.L, value => new HslColor(hsl.H, hsl.S, value).ToColor());

            _isUpdating = false;
        }

        private void UpdateColorPreview()
        {
            // TODO
            // SelectedColorPreviewBrush.Color = SelectedColor ?? Colors.Transparent;
        }
    }
}
