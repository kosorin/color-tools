namespace Koda.ColorTools.Wpf.Components
{
    public class HsbBrightnessSlider : ColorSlider
    {
        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Hsb.B / 100);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Color = new HsbColor(picker.Hsb.H, picker.Hsb.S, Value * 100);
        }
    }
}
