using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using Control = System.Windows.Forms.Control;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using System.Windows.Documents;
using System.Drawing;
using System.Windows.Forms;


namespace StarHue
{
    public partial class MainWindow : Window
    {
        private const string appName = "StarHue";
        private const string appVersion = "1.0";
        private const string appDeveloper = "justinnas";

        private DispatcherTimer CursorTrackTimer;
        private CursorFollower CursorFollower;

        private byte?[] RGBValues = new byte?[3];
        private string HEXValue = "";

        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            this.Deactivated += Window_Deactivated;

            SetInfoMessage();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }


        private Color GetColor(Point position)
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(position, new Point(0, 0), new Size(1, 1));
                }

                return bitmap.GetPixel(0, 0);
            }
        }

        private void StartTrackingMouse()
        {
            CursorTrackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            InitializeCursorFollower();
            PickColorButton.IsEnabled = false;

            SetInfoMessage("colorpick");

            CursorTrackTimer.Tick += Timer_Tick;
            CursorTrackTimer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Point position = Control.MousePosition;

            this.Cursor = System.Windows.Input.Cursors.Cross;

            Color color = GetColor(position);

            var colorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));

            CursorFollower.SetPosition(position);
            CursorFollower.SetMiniColorDisplay(colorBrush);

            if (Control.MouseButtons.HasFlag(MouseButtons.Left))
            {
                PickColorButton.IsEnabled = true;
                CursorFollower.Close();
                CursorTrackTimer.Stop();
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                ColorSelected(position);
            }

            if (Control.MouseButtons.HasFlag(MouseButtons.Right) || Keyboard.IsKeyDown(Key.Escape))
            {
                PickColorButton.IsEnabled = true;
                CursorFollower.Close();
                CursorTrackTimer.Stop();
                SetInfoMessage();
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        private void InitializeCursorFollower()
        {
            CursorFollower = new CursorFollower();
            CursorFollower.Show();
        }

        private void ColorSelected(Point position)
        {
            Color color = GetColor(position);

            var colorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));

            ColorDisplay.Fill = colorBrush;

            RGBValues[0] = color.R;
            RGBValues[1] = color.G;
            RGBValues[2] = color.B;

            UpdateRGBText(color.R, color.G, color.B);

            HEXValue = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            UpdateHEXText(HEXValue);

            SetInfoMessage();
        }

        public void UpdateRGBText(byte r, byte g, byte b)
        {
            if (RedHyperlink.Inlines.Count > 0)
                ((Run)RedHyperlink.Inlines.FirstInline).Text = r.ToString();
            if (GreenHyperlink.Inlines.Count > 0)
                ((Run)GreenHyperlink.Inlines.FirstInline).Text = g.ToString();
            if (BlueHyperlink.Inlines.Count > 0)
                ((Run)BlueHyperlink.Inlines.FirstInline).Text = b.ToString();
        }

        public void UpdateHEXText(string value)
        {
            if (HEXValueHyperlink.Inlines.Count > 0)
                ((Run)HEXValueHyperlink.Inlines.FirstInline).Text = value;
        }


        private void PickColorButton_Click(object sender, RoutedEventArgs e)
        {
            StartTrackingMouse();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CopyHEXButton_Click(object sender, RoutedEventArgs e)
        {
            if (HEXValue != "")
            {
                System.Windows.Clipboard.SetText($"#{HEXValue}");

                SetInfoMessage("copy", $"#{HEXValue}");
            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void CopyRGBButton_Click(object sender, RoutedEventArgs e)
        {
            if (RGBValues[0].HasValue && RGBValues[1].HasValue && RGBValues[2].HasValue)
            {
                System.Windows.Clipboard.SetText($"{RGBValues[0]}, {RGBValues[1]}, {RGBValues[2]}");

                SetInfoMessage("copy", $"{RGBValues[0]}, {RGBValues[1]}, {RGBValues[2]}");
            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void RGBRed_Click(object sender, RoutedEventArgs e)
        {
            if (RGBValues[0].HasValue)
            {
                System.Windows.Clipboard.SetText(RGBValues[0].ToString());

                SetInfoMessage("copy", $"{RGBValues[0]}");
            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void RGBGreen_Click(object sender, RoutedEventArgs e)
        {
            if (RGBValues[1].HasValue)
            {
                System.Windows.Clipboard.SetText(RGBValues[1].ToString());

                SetInfoMessage("copy", $"{RGBValues[1]}");

            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void RGBBlue_Click(object sender, RoutedEventArgs e)
        {
            if (RGBValues[2].HasValue)
            {
                System.Windows.Clipboard.SetText(RGBValues[2].ToString());

                SetInfoMessage("copy", $"{RGBValues[2]}");
            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void HEXValue_Click(object sender, RoutedEventArgs e)
        {
            if (HEXValue != "")
            {
                System.Windows.Clipboard.SetText(HEXValue);

                SetInfoMessage("copy", $"{HEXValue}");
            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void InfoMessage_Click(object sender, RoutedEventArgs e)
        {
            if (InfoMessageLabel.Content.ToString() == $"{appName}")
                SetInfoMessage("copyrightMessage");
            else if (InfoMessageLabel.Content.ToString() == $"{appName} v{appVersion} (c) {appDeveloper}")
                SetInfoMessage();
        }


        private void SetInfoMessage(string messageType = "", string message = "")
        {
            switch (messageType)
            {
                case "copy":
                    InfoMessageLabel.Content = $"Copied \"{message}\"";
                    break;
                case "noCopy":
                    InfoMessageLabel.Content = "Pick a color first!";
                    break;
                case "colorpick":
                    InfoMessageLabel.Content = "Right-Click or ESC to cancel";
                    break;
                case "message":
                    InfoMessageLabel.Content = message;
                    break;
                case "copyrightMessage":
                    InfoMessageLabel.Content = $"{appName} v{appVersion} (c) {appDeveloper}";
                    break;
                default:
                    InfoMessageLabel.Content = $"{appName}";
                    break;
            }
        }
    }
}