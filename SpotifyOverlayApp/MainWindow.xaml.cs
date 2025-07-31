using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using NAudio.CoreAudioApi;

namespace SpotifyOverlayNoAPI
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private TaskbarIcon trayIcon;
        private MMDeviceEnumerator devEnum = new MMDeviceEnumerator();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Mouse geçirme
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        // Pencere yüklenince
        private void FadeOutIfVisible()
        {
            if (isCurrentlyVisible)
            {
                isCurrentlyVisible = false;
                var fadeOut = (Storyboard)this.Resources["FadeOut"];
                fadeOut.Begin(this);
            }
        }

        private void FadeInIfHidden()
        {
            if (!isCurrentlyVisible)
            {
                isCurrentlyVisible = true;
                var fadeIn = (Storyboard)this.Resources["FadeIn"];
                fadeIn.Begin(this);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = 10;
            this.Top = 10;

            // Animasyonlu görünme
            Storyboard fadeIn = (Storyboard)this.Resources["FadeIn"];
            fadeIn.Begin(this);

            // Tray setup
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

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += UpdateOverlay;
            timer.Start();
        }


        // Verileri güncelle
        private bool isCurrentlyVisible = true; // pencere açık mı

        private void UpdateOverlay(object sender, EventArgs e)
        {
            string title = GetSpotifyTitle();
            float volume = GetSystemVolume();

            VolumeText.Text = $"🔊 Ses: %{volume:F0}";

            if (string.IsNullOrEmpty(title))
            {
                SongText.Text = "Spotify çalışmıyor";
                FadeOutIfVisible();
            }
            else if (title == "Spotify") // Şarkı durdu
            {
                SongText.Text = "Şarkı duraklatıldı";
                FadeOutIfVisible();
            }
            else
            {
                SongText.Text = $"🎵 {title.Replace("Spotify - ", "")}";
                FadeInIfHidden();
            }
        }


        // Spotify başlığı oku
        private string GetSpotifyTitle()
        {
            foreach (var proc in Process.GetProcessesByName("Spotify"))
            {
                if (!string.IsNullOrWhiteSpace(proc.MainWindowTitle))
                    return proc.MainWindowTitle;
            }
            return null;
        }

        // Sistem sesi oku (NAudio)
        private float GetSystemVolume()
        {
            var device = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
        }

        // Mouse geçirme destekleri
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
