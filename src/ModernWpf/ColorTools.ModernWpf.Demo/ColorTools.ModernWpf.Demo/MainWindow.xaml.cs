using System;
using System.Windows;
using System.Windows.Media;

namespace ColorTools.ModernWpf.Demo
{
    public partial class MainWindow 
    {
        private readonly Random _random;
        private bool _state;

        public MainWindow()
        {
            InitializeComponent();

            _random = new Random();
        }

        private void OriginalColor_OnClick(object sender, RoutedEventArgs e)
        {
            if (Picker.OriginalColor.HasValue)
            {
                Picker.OriginalColor = null;
            }
            else
            {
                Picker.OriginalColor = Color.FromArgb((byte)_random.Next(50, 250), (byte)_random.Next(50, 250), (byte)_random.Next(50, 250), (byte)_random.Next(50, 250));
            }
        }

        private void SelectedColor_OnClick(object sender, RoutedEventArgs e)
        {
            if (_state)
            {
                _state = false;
                Picker.SelectedColor = null;
            }
            else
            {
                _state = true;
                Picker.SelectedColor = Color.FromArgb((byte)_random.Next(50, 250), (byte)_random.Next(50, 250), (byte)_random.Next(50, 250), (byte)_random.Next(50, 250));
            }
        }
    }
}
