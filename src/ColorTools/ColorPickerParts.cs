using System;

namespace ColorTools
{
    [Flags]
    public enum ColorPickerParts
    {
        Color = 1 << 0,
        Alpha = 1 << 1,
        IsSet = 1 << 2,

        None = 0,
        All = Color | Alpha | IsSet,
    }
}
