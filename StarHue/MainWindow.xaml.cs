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
using System.Collections.Generic;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace StarHue
{
    public partial class MainWindow : Window
    {
        public const string appName = "StarHue";
        public const string appVersion = "2.0";
        public const string appDeveloper = "justinnas";

        private DispatcherTimer CursorTrackTimer;
        private CursorFollower CursorFollower;

        private byte?[] RGBValues = new byte?[3];
        private string HEXValue = "";

        public Dictionary<string, string> colors = new Dictionary<string, string>();
        private bool isColorSaved = false;
        private string savedColorFilePath = "";

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

            this.Cursor = Cursors.Cross;

            Color color = GetColor(position);

            var colorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));

            CursorFollower.SetPosition(position);
            CursorFollower.SetMiniColorDisplay(colorBrush);

            if (Control.MouseButtons.HasFlag(System.Windows.Forms.MouseButtons.Left))
            {
                PickColorButton.IsEnabled = true;
                CursorFollower.Close();
                CursorTrackTimer.Stop();
                this.Cursor = Cursors.Arrow;
                SaveColorContextMenuButton.Header = "Save color";
                OpenSavedColorContextMenuButton.Visibility = Visibility.Collapsed;
                ColorSelected(position);
            }

            if (Control.MouseButtons.HasFlag(System.Windows.Forms.MouseButtons.Right) || Keyboard.IsKeyDown(Key.Escape))
            {
                PickColorButton.IsEnabled = true;
                CursorFollower.Close();
                CursorTrackTimer.Stop();
                SetInfoMessage();
                this.Cursor = Cursors.Arrow;
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

            ColorDisplay.Background = colorBrush;

            RGBValues[0] = color.R;
            RGBValues[1] = color.G;
            RGBValues[2] = color.B;

            UpdateRGBText(color.R, color.G, color.B);

            HEXValue = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            UpdateHEXText(HEXValue);

            SetInfoMessage();
            this.Title = $"StarHue - #{HEXValue}";
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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CopyHEXButton_Click(object sender, RoutedEventArgs e)
        {
            if (HEXValue != "")
            {
                Clipboard.SetText($"#{HEXValue}");

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
                Clipboard.SetText($"{RGBValues[0]}, {RGBValues[1]}, {RGBValues[2]}");

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
                Clipboard.SetText(RGBValues[0].ToString());

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
                Clipboard.SetText(RGBValues[1].ToString());

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
                Clipboard.SetText(RGBValues[2].ToString());

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
                Clipboard.SetText(HEXValue);

                SetInfoMessage("copy", $"{HEXValue}");
            }
            else
            {
                SetInfoMessage("noCopy");
            }
        }

        private void SaveColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (HEXValue == "")
            {
                SetInfoMessage("noCopy");
                return;
            }

            // Only load colors from CSV after the first export click, to avoid unnecessary loading during app startup

            if (colors.Count == 0)
            {
                try
                {
                    colors = ColorNamer.LoadColorsFromCsv(@"values\colornames.csv");
                }
                catch
                {
                    // Do nothing, defaults to empty dictionary
                }
            }

            string colorName = "";
            string inputHex = $"#{HEXValue}";

            try
            {
                colorName = ColorNamer.FindClosestColorName(inputHex, colors);
            }
            catch
            {
                // Do nothing, defaults to empty
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "HTML files (*.html)|*.html",
                DefaultExt = ".html",
                FileName = $"{(string.IsNullOrEmpty(colorName) ? "" : colorName + " ")}{inputHex}.html"
            };


            if (saveFileDialog.ShowDialog() == true)
            {
                savedColorFilePath = saveFileDialog.FileName;

                ColorExporter.ExportToHTML(inputHex, savedColorFilePath);
                isColorSaved = true;
                OpenSavedColorContextMenuButton.Visibility = Visibility.Visible;
                SaveColorContextMenuButton.Header = "Save color again";
                SetInfoMessage("colorSaved");
            }
            else
            {
                SetInfoMessage("colorNotSaved");
            }
        }

        private void OpenSavedColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (isColorSaved)
            {
                try
                {
                    System.Diagnostics.Process.Start(savedColorFilePath);
                }
                catch
                {
                    // Handle case where the file path is invalid or the file doesn't exist
                    // Sets the context menu to default mode
                    SetInfoMessage("colorNotSaved");
                    SaveColorContextMenuButton.Header = "Save color";
                    OpenSavedColorContextMenuButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AppIcon_Click(object sender, RoutedEventArgs e)
        {
            if (InfoMessageLabel.Content.ToString() == $"{appName}")
                SetInfoMessage("copyrightMessage");
            else if (InfoMessageLabel.Content.ToString() == $"v{appVersion} © {appDeveloper}")
                SetInfoMessage();
        }

        private void InfoMessage_Click(object sender, RoutedEventArgs e)
        {
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
                    InfoMessageLabel.Content = $"v{appVersion} © {appDeveloper}";
                    break;
                case "colorSaved":
                    InfoMessageLabel.Content = $"Color saved!";
                    break;
                case "colorNotSaved":
                    InfoMessageLabel.Content = $"Color not saved!";
                    break;
                default:
                    InfoMessageLabel.Content = $"{appName}";
                    break;
            }
        }
    }
}