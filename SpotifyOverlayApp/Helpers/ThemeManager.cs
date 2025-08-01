using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SpotifyOverlayNoAPI
{
    public static class ThemeManager
    {
        private static readonly string ConfigPath = "theme.json";
        public static bool IsRgbMode = false;
        public static DispatcherTimer rgbTimer;
        private static Color lastColor = GetInitialColor();
        private static Color GetRandomColor()
        {
            // Renk farkı küçük tutularak yumuşak geçiş sağlanır
            byte newR = (byte)Math.Clamp(lastColor.R + new Random().Next(-20, 21), 0, 255);
            byte newG = (byte)Math.Clamp(lastColor.G + new Random().Next(-20, 21), 0, 255);
            byte newB = (byte)Math.Clamp(lastColor.B + new Random().Next(-20, 21), 0, 255);

            lastColor = Color.FromRgb(newR, newG, newB);
            return lastColor;
        }

        private static Color GetInitialColor()
        {
            return Color.FromRgb(120, 120, 255); // Başlangıç rengi
        }


        public static void StartRgbAnimation()
        {
            if (rgbTimer != null) return;

            rgbTimer = new DispatcherTimer();
            rgbTimer.Interval = TimeSpan.FromMilliseconds(120);
            rgbTimer.Tick += (s, e) =>
            {


                rgbTimer.Tick += (s, e) =>
                {
                    var c1 = GetRandomColor();
                    var c2 = GetRandomColor();
                    var c3 = GetRandomColor();

                    var brush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    brush.GradientStops.Add(new GradientStop(c1, 0.0));
                    brush.GradientStops.Add(new GradientStop(c2, 0.5));
                    brush.GradientStops.Add(new GradientStop(c3, 1.0));

                    Application.Current.Resources["CurrentGradient"] = brush;
                };

            };
            rgbTimer.Start();
            IsRgbMode = true;
        }


        public static void StopRgbAnimation()
        {
            if (rgbTimer != null)
            {
                rgbTimer.Stop();
                rgbTimer = null;
                IsRgbMode = false;
            }
        }
        private static Brush GetRandomBrush()
        {
            Random rand = new Random();
            return new SolidColorBrush(Color.FromRgb(
                (byte)rand.Next(100, 255),
                (byte)rand.Next(100, 255),
                (byte)rand.Next(100, 255)));
        }

        public static void ApplyGradientFromHex(string hex)
        {
            try
            {
                StopRgbAnimation();

                Color baseColor = (Color)ColorConverter.ConvertFromString(hex);
                Color lighter = ChangeColorBrightness(baseColor, 0.2f);
                Color darker = ChangeColorBrightness(baseColor, -0.2f);

                var gradient = new LinearGradientBrush();
                gradient.GradientStops.Add(new GradientStop(lighter, 0));
                gradient.GradientStops.Add(new GradientStop(darker, 1));

                Application.Current.Resources["CurrentGradient"] = gradient;
            }
            catch
            {
                Application.Current.Resources["CurrentGradient"] = Brushes.DarkSlateGray;
            }
        }

        private static Color ChangeColorBrightness(Color color, float factor)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            r = Clamp(r + factor, 0, 1);
            g = Clamp(g + factor, 0, 1);
            b = Clamp(b + factor, 0, 1);

            return Color.FromArgb(color.A,
                                  (byte)(r * 255),
                                  (byte)(g * 255),
                                  (byte)(b * 255));
        }

        private static float Clamp(float val, float min, float max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        public static void ApplySavedTheme()
        {
            if (!File.Exists(ConfigPath)) return;

            try
            {
                var json = File.ReadAllText(ConfigPath);
                var config = JsonSerializer.Deserialize<ThemeConfig>(json);

                if (config == null || string.IsNullOrEmpty(config.Theme)) return;

                if (config.Theme == "RGB")
                {
                    StartRgbAnimation();
                }
                else if (Application.Current.Resources.Contains(config.Theme))
                {
                    StopRgbAnimation();
                    Application.Current.Resources["CurrentGradient"] = Application.Current.Resources[config.Theme];
                }
                else
                {
                    StopRgbAnimation();
                    var converter = new BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(config.Theme);
                    Application.Current.Resources["CurrentGradient"] = brush;
                }
            }
            catch
            {
                // geçersiz json veya hex
            }
        }

        public static void SaveTheme(string themeKey)
        {
            var config = new ThemeConfig { Theme = themeKey };
            var json = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigPath, json);
        }
    }

    public class ThemeConfig
    {
        public string Theme { get; set; }
    }
}
