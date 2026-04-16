using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MathAnimator
{
    public partial class MainWindow : Window
    {
        private readonly Stack<UserControl> _navigationStack = new();

        public MainWindow()
        {
            InitializeComponent();
            ShowView(new StartView(this), clearStack: true);
        }

        public void ShowView(UserControl view, bool clearStack = false)
        {
            if (clearStack)
                _navigationStack.Clear();
            else if (Content is UserControl current)
                _navigationStack.Push(current);

            Content = view;
        }

        public void GoBack()
        {
            if (_navigationStack.Count > 0)
            {
                Content = _navigationStack.Pop();
            }
        }
    }
}