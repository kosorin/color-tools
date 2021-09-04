namespace ColorTools.Components
{
    public class AlphaSlider : ColorSlider
    {
        protected override ColorPickerParts PickerParts => ColorPickerParts.Alpha;

        protected override void OnPickerChanged(IColorPicker picker, ColorPickerParts parts)
        {
            base.OnPickerChanged(picker, parts);

            SetCurrentValue(ValueProperty, picker.Alpha);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Alpha = Value;
        }
    }
}
