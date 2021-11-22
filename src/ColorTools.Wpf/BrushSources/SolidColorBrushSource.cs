using System.Windows;
using System.Windows.Media;

namespace Koda.ColorTools.Wpf.BrushSources
{
    public class SolidColorBrushSource : BrushSource
    {
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(nameof(Brush), typeof(SolidColorBrush), typeof(SolidColorBrushSource),
                new PropertyMetadata(new SolidColorBrush(DefaultColor), OnBrushPropertyChanged));

        public SolidColorBrushSource()
        {
            Value = Brush;
        }

        public SolidColorBrush Brush
        {
            get => (SolidColorBrush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        private static void OnBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SolidColorBrushSource brushSource)
            {
                brushSource.Value = brushSource.Brush;
            }
        }
    }
}
