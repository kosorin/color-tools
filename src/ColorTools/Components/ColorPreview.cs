using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColorTools.BrushSources;

namespace ColorTools.Components
{
    [TemplatePart(Name = nameof(OriginalColorBorder), Type = typeof(Border))]
    public class ColorPreview : ColorComponent
    {
        public static readonly DependencyProperty OriginalColorProperty =
            DependencyProperty.Register(nameof(OriginalColor), typeof(Color?), typeof(ColorPreview), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedColorBrushSourceProperty =
            DependencyProperty.Register(nameof(SelectedColorBrushSource), typeof(BrushSource), typeof(ColorPreview), new PropertyMetadata(null));

        private Border? _originalColorBorder;

        static ColorPreview()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPreview), new FrameworkPropertyMetadata(typeof(ColorPreview)));
        }

        public Color? OriginalColor
        {
            get => (Color?)GetValue(OriginalColorProperty);
            set => SetValue(OriginalColorProperty, value);
        }

        public BrushSource SelectedColorBrushSource
        {
            get => (BrushSource)GetValue(SelectedColorBrushSourceProperty);
            set => SetValue(SelectedColorBrushSourceProperty, value);
        }

        public Border? OriginalColorBorder
        {
            get => _originalColorBorder;
            set
            {
                if (_originalColorBorder != null)
                {
                    _originalColorBorder.MouseLeftButtonDown -= OnOriginalColorBorderMouseLeftButtonDown;
                }

                _originalColorBorder = value;

                if (_originalColorBorder != null)
                {
                    _originalColorBorder.MouseLeftButtonDown += OnOriginalColorBorderMouseLeftButtonDown;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OriginalColorBorder = GetTemplateChild(nameof(OriginalColorBorder)) as Border;
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            if (OriginalColor.HasValue)
            {
                picker.ReplaceSelectedColor(OriginalColor.Value);
            }
        }

        private void OnOriginalColorBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            TryUpdatePicker();
        }
    }
}
