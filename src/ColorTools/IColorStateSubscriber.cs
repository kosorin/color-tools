namespace ColorTools
{
    public interface IColorStateSubscriber
    {
        void HandleColorStateChanged(ColorState colorState);
    }
}
