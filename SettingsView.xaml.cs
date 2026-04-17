using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathAnimator
{
    public partial class SettingsView : UserControl
    {
        private readonly MainWindow _host;

        public SettingsView(MainWindow host)
        {
            InitializeComponent();
            _host = host;
        }

        private void ApplyTheme(Color bg, Color fg, Color accent)
        {
            Application.Current.Resources["ThemeBackground"] = new SolidColorBrush(bg);
            Application.Current.Resources["ThemeForeground"] = new SolidColorBrush(fg);
            Application.Current.Resources["ThemeAccent"] = new SolidColorBrush(accent);
        }

        private void OnNeon(object sender, RoutedEventArgs e)
        {
            ApplyTheme(Colors.Black, Colors.White, Colors.Lime);
        }

        private void OnDark(object sender, RoutedEventArgs e)
        {
            ApplyTheme(Color.FromRgb(20, 20, 20), Colors.White, Colors.Orange);
        }

        private void OnClassic(object sender, RoutedEventArgs e)
        {
            ApplyTheme(Colors.Black, Colors.White, Color.FromRgb(58, 122, 254));
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            _host.GoBack();
        }

        private void OnWhite(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["ThemeBackground"] =
                new SolidColorBrush(Colors.White);

            Application.Current.Resources["ThemeForeground"] =
                new SolidColorBrush(Colors.Black);

            Application.Current.Resources["ThemeAccent"] =
                new SolidColorBrush(Color.FromRgb(58, 122, 254));

            Application.Current.Resources["ThemeAccentHover"] =
                new SolidColorBrush(Color.FromRgb(47, 101, 217));

            Application.Current.Resources["ThemeAccentPressed"] =
                new SolidColorBrush(Color.FromRgb(36, 78, 176));

            Application.Current.Resources["ThemeDisabled"] =
                new SolidColorBrush(Color.FromRgb(180, 180, 180));

            Application.Current.Resources["ThemeCurve"] =
                new SolidColorBrush(Colors.Red);
        }

        private void OnCrazy(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["ThemeBackground"] =
                new SolidColorBrush(Colors.Black);

            Application.Current.Resources["ThemeForeground"] =
                new SolidColorBrush(Colors.White);

            Application.Current.Resources["ThemeAccent"] =
                new SolidColorBrush(Colors.HotPink);

            Application.Current.Resources["ThemeAccentHover"] =
                new SolidColorBrush(Colors.Cyan);

            Application.Current.Resources["ThemeAccentPressed"] =
                new SolidColorBrush(Colors.Lime);

            Application.Current.Resources["ThemeDisabled"] =
                new SolidColorBrush(Colors.Gray);

            // ⚠️ Curve-Farbe wird NICHT statisch gesetzt
            // -> kommt aus dem Renderer (Regenbogen!)
        }

        private void OnEik(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["ThemeBackground"] =
                new SolidColorBrush(Colors.DarkGreen);

            Application.Current.Resources["ThemeForeground"] =
                new SolidColorBrush(Colors.Black);

            Application.Current.Resources["ThemeAccent"] =
                new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Application.Current.Resources["ThemeAccentHover"] =
                new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Application.Current.Resources["ThemeAccentPressed"] =
                new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Application.Current.Resources["ThemeDisabled"] =
                new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Application.Current.Resources["ThemeCurve"] =
                new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
    }
}