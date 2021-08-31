using System.Windows.Media;

namespace ColorTools
{
    public abstract class BrushSource
    {
        protected static readonly Color DefaultColor = Colors.Transparent;

        protected BrushSource(IColorState colorState)
        {
            ColorState = colorState;
        }

        private BrushSource()
        {
            ColorState = null!;
        }

        public static BrushSource Empty { get; } = new EmptyBrushSource();

        public abstract Brush Brush { get; }

        protected IColorState ColorState { get; }

        public abstract void Update();

        private class EmptyBrushSource : BrushSource
        {
            public EmptyBrushSource()
            {
                Brush = new SolidColorBrush(DefaultColor);
            }

            public override Brush Brush { get; }

            public override void Update()
            {
            }
        }
    }
}
