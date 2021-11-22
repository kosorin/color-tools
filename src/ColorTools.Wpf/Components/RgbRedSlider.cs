namespace Koda.ColorTools.Wpf.Components
{
    public class RgbRedSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Rgb.R / 255);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new RgbColor(Value * 255, picker.Rgb.G, picker.Rgb.B);
        }
    }
}
