using System.Windows;
using System.Windows.Controls;

namespace SpotifyOverlayNoAPI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selected &&
                selected.Tag is string paletteKey)
            {
                var brush = (System.Windows.Media.Brush)Application.Current.Resources[paletteKey];
                Application.Current.Resources["CurrentGradient"] = brush;
                ThemeManager.SaveTheme(paletteKey);
            }
        }

        private void ApplyCustomHex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hex = HexInput.Text.Trim();

                if (!hex.StartsWith("#"))
                    hex = "#" + hex;

                ThemeManager.ApplyGradientFromHex(hex);
                ThemeManager.SaveTheme(hex);

                MessageBox.Show("Renk uygulandı!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Geçersiz HEX kodu. Örnek: #FF112233", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
