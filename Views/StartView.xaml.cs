using System.Windows;
using System.Windows.Controls;

namespace MathAnimator
{
    public partial class StartView : UserControl
    {
        private readonly MainWindow _host;

        public StartView(MainWindow host)
        {
            InitializeComponent();
            _host = host;
        }

        private void OnInput(object sender, RoutedEventArgs e)
        {
            _host.ShowView(new InputView(_host));
        }

        private void OnSettings(object sender, RoutedEventArgs e)
        {
            _host.ShowView(new SettingsView(_host));
        }

        private void OnLibrary(object sender, RoutedEventArgs e)
        {
            _host.ShowView(new LibraryView(_host));
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}