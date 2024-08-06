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
            Left = position.X - 100;
            Top = position.Y - 100;
        }

        public void SetMiniColorDisplay(SolidColorBrush colorBrush)
        {
            MiniColorDisplay.Background = colorBrush;
        }
    }
}
