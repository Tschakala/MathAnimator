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
    }
}