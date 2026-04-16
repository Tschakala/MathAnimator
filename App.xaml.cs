using System.Windows;

namespace MathAnimator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var main = new MainWindow();
            main.Show();
        }
    }
}