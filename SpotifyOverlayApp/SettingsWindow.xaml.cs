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
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

                MessageBox.Show("Color change applied!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Invalid HEX code. Ex: #FF112233", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
