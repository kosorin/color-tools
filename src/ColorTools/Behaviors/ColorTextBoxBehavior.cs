using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ColorTools.Behaviors
{
    // Source: http://www.dutton.me.uk/2013-07-25/how-to-select-all-wpf-textbox-text-on-focus-using-an-attached-behavior/
    // Source: https://stackoverflow.com/questions/563195/bind-textbox-on-enter-key-press
    public class ColorTextBoxBehavior
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(ColorTextBoxBehavior), new PropertyMetadata(false, OnEnablePropertyChanged));

        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }

        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        private static void OnEnablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not TextBox textBox)
            {
                return;
            }

            if (args.NewValue is not bool value)
            {
                return;
            }

            if (value)
            {
                textBox.GotFocus += SelectAll;
                textBox.PreviewMouseDown += IgnoreMouseButton;
                textBox.PreviewKeyDown += HandleEnterKey;
            }
            else
            {
                textBox.GotFocus -= SelectAll;
                textBox.PreviewMouseDown -= IgnoreMouseButton;
                textBox.PreviewKeyDown -= HandleEnterKey;
            }
        }

        private static void SelectAll(object sender, RoutedEventArgs args)
        {
            if (args.OriginalSource is not TextBox textBox)
            {
                return;
            }

            textBox.SelectAll();
        }

        private static void IgnoreMouseButton(object sender, MouseButtonEventArgs args)
        {
            if (sender is not TextBox textBox || textBox.IsKeyboardFocusWithin)
            {
                return;
            }

            args.Handled = true;

            textBox.Focus();
        }

        private static void HandleEnterKey(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter && args.Source is DependencyObject obj)
            {
                UpdateTextSource(obj);
            }
        }

        private static void UpdateTextSource(DependencyObject obj)
        {
            if (BindingOperations.GetBindingExpression(obj, TextBox.TextProperty) is { } binding)
            {
                binding.UpdateSource();
            }
        }
    }
}
