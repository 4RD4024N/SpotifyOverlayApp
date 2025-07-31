using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using NAudio.CoreAudioApi;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace SpotifyOverlayNoAPI
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer updateTimer;
        private DispatcherTimer hideTimer;
        private TaskbarIcon trayIcon;
        private MMDeviceEnumerator devEnum = new MMDeviceEnumerator();

        private string lastSongTitle = "";
        private float lastVolume = -1;
        private bool isVisibleNow = false;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

            VolumeText.Text = $"🔊 Ses seviyesi: %{volume:F0}";
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
        }
        private void SetClickThrough(bool enable)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            if (enable)
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            else
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
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

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private void SettingsButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SetClickThrough(false); // Tıklanabilir hale getir
        }

        private void SettingsButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SetClickThrough(true); // Tekrar click-through
        }

        // SADECE SettingsWindow kullanan versiyon
        private SettingsWindow settingsWindow;

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
                settingsWindow.Activate(); // Varsa zaten göster
            }
        }



    }
}
