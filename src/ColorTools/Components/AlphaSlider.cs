namespace ColorTools.Components
{
    public class AlphaSlider : ColorSlider
    {
        protected override void OnAlphaChanged(IColorPicker picker, double alpha)
        {
            base.OnAlphaChanged(picker, alpha);

            SetCurrentValue(ValueProperty, alpha);
        }

        protected override void UpdatePicker(IColorPicker picker)
        {
            picker.Alpha = Value;
        }
    }
}
