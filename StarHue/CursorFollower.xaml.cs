using System.Windows;
using System.Windows.Media;

namespace StarHue
{
    public partial class CursorFollower : Window
    {
        public CursorFollower()
        {
            // This component does not have fully transparent background, but this
            // almost-transparent background is gone before a color is picked,
            // so the color at the end is accurate

            InitializeComponent();
            this.Cursor = System.Windows.Input.Cursors.Cross;
        }

        public void SetPosition(System.Drawing.Point position)
        {
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(new System.Windows.Point(position.X, position.Y));
            Left = mouse.X - ActualWidth / 2;
            Top = mouse.Y - ActualHeight / 2;
        }

        public void SetMiniColorDisplay(SolidColorBrush colorBrush)
        {
            MiniColorDisplay.Background = colorBrush;
        }
    }
}
