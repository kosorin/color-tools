namespace ColorTools
{
    public interface IColorStateSubscriber
    {
        void HandleColorStateChanged(IColorState colorState);
    }
}
