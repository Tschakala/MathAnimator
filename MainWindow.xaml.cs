using System.Windows;

namespace MathAnimator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowView(new StartView(this));
        }

        public void ShowView(UIElement view)
        {
            MainContent.Content = view;
        }
    }
}