using System.Windows;

namespace MathAnimator
{
    public partial class HelpWindow : Window
    {
        public HelpWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        private void OnOpenLink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }
    }
}
