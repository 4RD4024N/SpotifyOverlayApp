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


        private static double hue = 0;
        private static Color HslToRgb(double h, double s, double l)
        {
            h /= 360;
            double r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                Func<double, double, double, double> hueToRgb = (p, q, t) =>
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1.0 / 6) return p + (q - p) * 6 * t;
                    if (t < 1.0 / 2) return q;
                    if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
                    return p;
                };

                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;

                r = hueToRgb(p, q, h + 1.0 / 3);
                g = hueToRgb(p, q, h);
                b = hueToRgb(p, q, h - 1.0 / 3);
            }

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
        public static void UpdateRgbSpeed(int milliseconds)
        {
            if (rgbTimer != null)
            {
                rgbTimer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public static void StartRgbAnimation()
        {
            if (rgbTimer != null) return;

            rgbTimer = new DispatcherTimer();
            rgbTimer.Interval = TimeSpan.FromMilliseconds(60);
            rgbTimer.Tick += (s, e) =>
            {
                hue = (hue + 1) % 360; // 0 - 359 arasında döner
                var baseColor = HslToRgb(hue, 0.8, 0.6);
                var lighter = HslToRgb((hue + 20) % 360, 0.9, 0.7);
                var darker = HslToRgb((hue + 340) % 360, 0.9, 0.4);

                var brush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
            {
                new GradientStop(lighter, 0),
                new GradientStop(baseColor, 0.5),
                new GradientStop(darker, 1)
            }
                };

                Application.Current.Resources["CurrentGradient"] = brush;
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
