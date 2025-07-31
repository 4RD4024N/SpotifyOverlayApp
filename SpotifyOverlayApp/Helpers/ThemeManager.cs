using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace SpotifyOverlayNoAPI
{
    public static class ThemeManager
    {
        private static readonly string ConfigPath = "theme.json";
        public static void ApplyGradientFromHex(string hex)
        {
            try
            {
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
                // Hatalı renk girilmişse fallback
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

                if (Application.Current.Resources.Contains(config.Theme))
                {
                    // Hazır tema (BluePalette vs.)
                    Application.Current.Resources["CurrentGradient"] = Application.Current.Resources[config.Theme];
                }
                else
                {
                    // Custom HEX
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

        private class ThemeConfig
        {
            public string Theme { get; set; }
        }
    }
}
