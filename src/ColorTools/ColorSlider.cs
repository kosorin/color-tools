using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorTools
{
    [TemplatePart(Name = nameof(SliderCanvas), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(Handle), Type = typeof(Border))]
    [TemplatePart(Name = nameof(HandleTranslateTransform), Type = typeof(TranslateTransform))]
    public class ColorSlider : Control
    {
        private static readonly Color DefaultGradientColor = Colors.Transparent;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ColorSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(ColorSlider), new PropertyMetadata(0d, OnMinimumPropertyChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(ColorSlider), new PropertyMetadata(100d, OnMaximumPropertyChanged));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorSlider), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ColorSlider), new PropertyMetadata(null));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorSlider), new PropertyMetadata(Brushes.Transparent));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(ColorSlider));

        private Canvas? _sliderCanvas;

        private bool _isValueUpdating;
        private bool _isDragging;
        private GradientContext? _gradientContext;

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public ColorSlider()
        {
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate?)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public Brush AlphaBrush
        {
            get => (Brush)GetValue(AlphaBrushProperty);
            set => SetValue(AlphaBrushProperty, value);
        }

        private Canvas? SliderCanvas
        {
            get => _sliderCanvas;
            set
            {
                if (_sliderCanvas != null)
                {
                    _sliderCanvas.MouseLeftButtonDown -= SliderCanvas_MouseLeftButtonDown;
                    _sliderCanvas.MouseLeftButtonUp -= SliderCanvas_MouseLeftButtonUp;
                    _sliderCanvas.MouseMove -= SliderCanvas_MouseMove;
                    _sliderCanvas.MouseWheel -= SliderCanvas_MouseWheel;
                    _sliderCanvas.SizeChanged -= SliderCanvas_SizeChanged;

                    _sliderCanvas.Background = null;
                }

                _sliderCanvas = value;

                if (_sliderCanvas != null)
                {
                    _sliderCanvas.MouseLeftButtonDown += SliderCanvas_MouseLeftButtonDown;
                    _sliderCanvas.MouseLeftButtonUp += SliderCanvas_MouseLeftButtonUp;
                    _sliderCanvas.MouseMove += SliderCanvas_MouseMove;
                    _sliderCanvas.MouseWheel += SliderCanvas_MouseWheel;
                    _sliderCanvas.SizeChanged += SliderCanvas_SizeChanged;

                    _sliderCanvas.Background = _gradientContext?.Update(Minimum, Maximum);
                }
            }
        }

        private Border? Handle { get; set; }

        private TranslateTransform? HandleTranslateTransform { get; set; }

        public void Update(double value)
        {
            Value = value;

            UpdateGradient();
        }

        public void InitializeGradient(IColorState colorState, int stopCount, ColorFactory<double> stopColorFactory)
        {
            _gradientContext = new GradientContext(colorState, stopCount, stopColorFactory);
        }

        private void UpdateGradient()
        {
            if (IsVisible)
            {
                _gradientContext?.Update(Minimum, Maximum);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SliderCanvas = GetTemplateChild(nameof(SliderCanvas)) as Canvas;
            Handle = GetTemplateChild(nameof(Handle)) as Border;
            HandleTranslateTransform = GetTemplateChild(nameof(HandleTranslateTransform)) as TranslateTransform;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            UpdateGradient();
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Left:
                case Key.PageDown:
                    MoveHandle(-1);
                    break;
                case Key.Right:
                case Key.PageUp:
                    MoveHandle(1);
                    break;
            }
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider && args is { OldValue : double oldValue, })
            {
                slider.OnValueChanged(oldValue);
            }
        }

        private static void OnMinimumPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider)
            {
                slider.OnMinimumChanged();
            }
        }

        private static void OnMaximumPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider)
            {
                slider.OnMaximumChanged();
            }
        }

        private void OnValueChanged(double oldValue)
        {
            if (_isValueUpdating)
            {
                return;
            }

            try
            {
                _isValueUpdating = true;

                CoerceValue();

                var newValue = Value;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (oldValue == newValue)
                {
                    return;
                }

                UpdateHandleCurrentPosition();

                RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent));
            }
            finally
            {
                _isValueUpdating = false;
            }
        }

        private void OnMinimumChanged()
        {
            CoerceMaximum();
            CoerceValue();
        }

        private void OnMaximumChanged()
        {
            CoerceMinimum();
            CoerceValue();
        }

        private void CoerceValue()
        {
            var value = Value;
            if (value < Minimum)
            {
                SetCurrentValue(ValueProperty, Minimum);
            }
            else if (value > Maximum)
            {
                SetCurrentValue(ValueProperty, Maximum);
            }
        }

        private void CoerceMinimum()
        {
            var maximum = Maximum;
            if (Minimum > maximum)
            {
                SetCurrentValue(MinimumProperty, maximum);
            }
        }

        private void CoerceMaximum()
        {
            var minimum = Minimum;
            if (Maximum < minimum)
            {
                SetCurrentValue(MaximumProperty, minimum);
            }
        }

        private void SliderCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (SliderCanvas == null)
            {
                return;
            }

            UpdateHandleNewPosition(args.GetPosition(SliderCanvas).X);

            _isDragging = true;
            SliderCanvas.CaptureMouse();
            args.Handled = true;
        }

        private void SliderCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (SliderCanvas == null)
            {
                return;
            }

            _isDragging = false;
            SliderCanvas.ReleaseMouseCapture();
            args.Handled = true;
        }

        private void SliderCanvas_MouseMove(object sender, MouseEventArgs args)
        {
            if (SliderCanvas == null)
            {
                return;
            }

            if (!_isDragging || args.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            UpdateHandleNewPosition(args.GetPosition(SliderCanvas).X);

            args.Handled = true;
        }

        private void SliderCanvas_MouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (SliderCanvas == null)
            {
                return;
            }

            if (_isDragging)
            {
                return;
            }

            MoveHandle(Math.Sign(args.Delta));
        }

        private void SliderCanvas_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (SliderCanvas == null)
            {
                return;
            }

            UpdateHandleCurrentPosition();
        }

        private void MoveHandle(int sign)
        {
            if (sign == 0)
            {
                return;
            }

            var isLargeStep = (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0;
            SetCurrentValue(ValueProperty, Value + sign * (isLargeStep ? 10 : 1));
        }

        private void UpdateHandleCurrentPosition()
        {
            if (_isDragging)
            {
                return;
            }

            var (offset, width) = GetLayoutInfo();
            if (width <= 0)
            {
                return;
            }

            if (HandleTranslateTransform != null)
            {
                HandleTranslateTransform.X = ValueToHandlePosition(Value, offset, width, Minimum, Maximum);
            }
        }

        private void UpdateHandleNewPosition(double mousePosition)
        {
            var (offset, width) = GetLayoutInfo();
            if (width <= 0)
            {
                return;
            }

            if (HandleTranslateTransform != null)
            {
                HandleTranslateTransform.X = MousePositionToHandlePosition(mousePosition, offset, width);
            }
            SetCurrentValue(ValueProperty, MousePositionToValue(mousePosition, offset, width, Minimum, Maximum));
        }

        private (double offset, double width) GetLayoutInfo()
        {
            if (SliderCanvas == null || Handle == null)
            {
                return default;
            }

            var offset = Handle.Width / 2;
            var width = SliderCanvas.ActualWidth - Handle.Width;
            return (offset, width);
        }

        private static double ValueToHandlePosition(double value, double offset, double width, double min, double max)
        {
            return offset + width * ((value - min) / (max - min));
        }

        private static double MousePositionToHandlePosition(double mousePosition, double offset, double width)
        {
            if (mousePosition < offset)
            {
                mousePosition = offset;
            }
            if (mousePosition > width + offset)
            {
                mousePosition = width + offset;
            }
            return mousePosition;
        }

        private static double MousePositionToValue(double mousePosition, double offset, double width, double min, double max)
        {
            var value = ((mousePosition - offset) / width) switch
            {
                < 0 => 0,
                > 1 => 1,
                var v => v,
            };
            return min + value * (max - min);
        }

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        private class GradientContext
        {
            private readonly IColorState _colorState;
            private readonly ColorFactory<double> _stopColorFactory;

            public GradientContext(IColorState colorState, int stopCount, ColorFactory<double> stopColorFactory)
            {
                _colorState = colorState;
                _stopColorFactory = stopColorFactory;

                Brush = CreateBrush(stopCount);
            }

            public LinearGradientBrush Brush { get; }

            public LinearGradientBrush Update(double minimum, double maximum)
            {
                var stops = Brush.GradientStops;
                var stopCount = stops.Count;

                var delta = maximum - minimum;

                for (var i = 0; i < stopCount; i++)
                {
                    var value = i / (double)(stopCount - 1) * delta + minimum;
                    stops[i].Color = _stopColorFactory.Invoke(value, _colorState);
                }

                return Brush;
            }

            private LinearGradientBrush CreateBrush(int stopCount)
            {
                var stops = Enumerable.Range(0, stopCount).Select(x => new GradientStop(DefaultGradientColor, x / (double)(stopCount - 1)));
                return new LinearGradientBrush(new GradientStopCollection(stops), new Point(0, 0), new Point(1, 0));
            }
        }
    }
}
