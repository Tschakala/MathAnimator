using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MathAnimator
{
    public partial class MainWindow : Window
    {
        private readonly Stack<UserControl> _navigationStack = new();
        private bool _isFullscreen = false;
        private WindowStyle _previousWindowStyle;
        private WindowState _previousWindowState;
        private ResizeMode _previousResizeMode;

        public MainWindow()
        {
            InitializeComponent();
            KeyDown += OnKeyDown;
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
        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F11)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                _previousWindowStyle = WindowStyle;
                _previousWindowState = WindowState;
                _previousResizeMode = ResizeMode;

                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;

                _isFullscreen = true;
            }
            else
            {
                WindowStyle = _previousWindowStyle;
                ResizeMode = _previousResizeMode;
                WindowState = _previousWindowState;

                _isFullscreen = false;
            }
        }
    }
}