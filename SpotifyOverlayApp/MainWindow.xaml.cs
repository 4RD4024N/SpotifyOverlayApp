using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using NAudio.CoreAudioApi;

namespace SpotifyOverlayNoAPI
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer updateTimer;
        private DispatcherTimer hideTimer;
        private TaskbarIcon trayIcon;
        private MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
        private DispatcherTimer rgbTimer;
        private byte rgbStep = 0;

        private string lastSongTitle = "";
        private float lastVolume = -1;
        private bool isVisibleNow = false;
        private Point dragStartPoint;
        private bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
        }
        private void SongText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                dragStartPoint = e.GetPosition(this);
                isDragging = true;
                this.DragMove();
            }
        }
        private void AnimateRgb(object sender, EventArgs e)
        {
            if (!(Application.Current.Resources["IsRgbTheme"] is bool isRgb) || !isRgb)
                return;

            rgbStep++;
            if (rgbStep > 255) rgbStep = 0;

            // Renk dönüşümü (HSV bazlı geçiş yerine lineer RGB geçişi)
            byte r = (byte)(Math.Sin(rgbStep * 0.05) * 127 + 128);
            byte g = (byte)(Math.Sin(rgbStep * 0.05 + 2) * 127 + 128);
            byte b = (byte)(Math.Sin(rgbStep * 0.05 + 4) * 127 + 128);

            var color1 = System.Windows.Media.Color.FromRgb(r, g, b);
            var color2 = System.Windows.Media.Color.FromRgb((byte)(255 - r), (byte)(255 - g), (byte)(255 - b));
            var color3 = System.Windows.Media.Color.FromRgb(g, b, r);

            Application.Current.Resources["RgbColor1"] = color1;
            Application.Current.Resources["RgbColor2"] = color2;
            Application.Current.Resources["RgbColor3"] = color3;
        }


        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - f * saturation));
            byte t = (byte)(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromRgb(v, t, p),
                1 => Color.FromRgb(q, v, p),
                2 => Color.FromRgb(p, v, t),
                3 => Color.FromRgb(p, q, v),
                4 => Color.FromRgb(t, p, v),
                _ => Color.FromRgb(v, p, q),
            };
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rgbTimer = new DispatcherTimer();
            rgbTimer.Interval = TimeSpan.FromMilliseconds(100);
            rgbTimer.Tick += AnimateRgb;
            rgbTimer.Start();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Opacity = 1;
                SongText.Text = "🎵 Tasarım Şarkısı";
                VolumeText.Text = "🔊 Ses: %50";
                return;
            }

            this.Left = 10;
            this.Top = 10;

            trayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon("icon.ico"),
                ToolTipText = "Şarkı Overlay",
                Visibility = Visibility.Visible
            };

            trayIcon.TrayLeftMouseDown += (s, args) =>
            {
                this.Visibility = (this.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
            };

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            updateTimer.Tick += CheckForSongChange;
            updateTimer.Start();

            hideTimer = new DispatcherTimer();
            hideTimer.Interval = TimeSpan.FromSeconds(5);
            hideTimer.Tick += (s, ev) => { FadeOut(); hideTimer.Stop(); };
        }

        private void CheckForSongChange(object sender, EventArgs e)
        {
            string currentTitle = GetSpotifyTitle();
            float volume = GetSystemVolume();

            VolumeText.Text = $"🔊 Volume : %{volume:F0}";

            // Smooth volume animation
            double newVolume = volume;
            double currentValue = VolumeBar.Value;
            double delta = Math.Abs(newVolume - currentValue);
            double durationMs = Math.Max(60, 300 - delta * 3);

            var animation = new DoubleAnimation
            {
                From = currentValue,
                To = newVolume,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };
            VolumeBar.BeginAnimation(ProgressBar.ValueProperty, animation);

            bool volumeChanged = Math.Abs(volume - lastVolume) > 0.5;
            lastVolume = volume;

            if (string.IsNullOrEmpty(currentTitle) || currentTitle == "Spotify")
            {
                FadeOut();
                return;
            }

            if (currentTitle != lastSongTitle)
            {
                lastSongTitle = currentTitle;
                SongText.Text = $"🎵 {currentTitle.Replace("Spotify - ", "")}";
                FadeIn();
                hideTimer.Stop();
                hideTimer.Start();
            }
            else if (volumeChanged)
            {
                SongText.Text = $"🎵 {currentTitle.Replace("Spotify - ", "")}";
                FadeIn();
                hideTimer.Stop();
                hideTimer.Start();
            }

            UpdatePlaybackStatus(currentTitle);
        }

        private void UpdatePlaybackStatus(string title)
        {
            if (string.IsNullOrEmpty(title) || title == "Spotify" || title == "Spotify Premium")
            {
                
                var shrink = (Storyboard)this.Resources["PausedShrink"];
                shrink.Begin();
            }
            else
            {
                
                var grow = (Storyboard)this.Resources["PlayingGrow"];
                grow.Begin();
            }
        }

        private void FadeIn()
        {
            if (!isVisibleNow)
            {
                isVisibleNow = true;
                this.Visibility = Visibility.Visible;
                var fadeIn = (Storyboard)this.Resources["FadeIn"];
                fadeIn.Begin(this);
            }
        }

        private void FadeOut()
        {
            if (isVisibleNow)
            {
                isVisibleNow = false;
                var fadeOut = (Storyboard)this.Resources["FadeOut"];
                fadeOut.Completed += (s, e) => { this.Visibility = Visibility.Hidden; };
                fadeOut.Begin(this);
            }
        }

        private string GetSpotifyTitle()
        {
            foreach (var proc in Process.GetProcessesByName("Spotify"))
            {
                if (!string.IsNullOrWhiteSpace(proc.MainWindowTitle))
                    return proc.MainWindowTitle;
            }
            return null;
        }

        private float GetSystemVolume()
        {
            var device = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow == null || !settingsWindow.IsLoaded)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Owner = this;
                settingsWindow.Show();
            }
            else
            {
                settingsWindow.Activate();
            }
        }

        private SettingsWindow settingsWindow;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
