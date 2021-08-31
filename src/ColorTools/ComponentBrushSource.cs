using System.Windows.Media;

namespace ColorTools
{
    public abstract class ComponentBrushSource
    {
        protected static readonly Color DefaultColor = Colors.Transparent;

        protected ComponentBrushSource(IColorState colorState)
        {
            ColorState = colorState;
        }

        private ComponentBrushSource()
        {
            ColorState = null!;
        }

        public static ComponentBrushSource Empty { get; } = new EmptyBrushSource();

        public abstract Brush Brush { get; }

        protected IColorState ColorState { get; }

        public abstract void Update();

        private class EmptyBrushSource : ComponentBrushSource
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
