using System.Windows.Media;

namespace ColorTools
{
    public delegate Color ColorFactory<in TValue>(TValue value, IColorState state);
}
