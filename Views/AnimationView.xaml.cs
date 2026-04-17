using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathAnimator.MathCore;
using MathAnimator.Rendering;

namespace MathAnimator
{
    public partial class AnimationView : UserControl
    {
        private readonly MainWindow _host;
        private readonly WriteableBitmap _bitmap;
        private readonly GraphRenderer _renderer;

        private readonly Func<double, double, double, double, double>? _func;
        private readonly Func<double, double, double, double, double>? _fx;
        private readonly Func<double, double, double, double, double>? _fy;

        private readonly AnimationController _animation;
        private readonly GraphMode _mode;

        private DateTime _lastRender = DateTime.MinValue;

        public AnimationView(
            MainWindow host,
            GraphMode mode,
            string formula,
            string xFormula,
            string yFormula,
            double a,
            double b,
            double c)
        {
            InitializeComponent();
            Loaded += (_, _) => Focus();

            _host = host;
            _mode = mode;

            int width = 900;
            int height = 500;

            _bitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgra32, null);

            Surface.Source = _bitmap;
            _renderer = new GraphRenderer(width, height);

            if (_mode == GraphMode.Function)
            {
                _func = MathParser.Parse(formula);
            }
            else
            {
                _fx = MathParser.Parse(xFormula);
                _fy = MathParser.Parse(yFormula);
            }

            _animation = new AnimationController(a, b, c);
            CompositionTarget.Rendering += OnRender;
        }

        private void OnRender(object? sender, EventArgs e)
        {
            if ((DateTime.Now - _lastRender).TotalMilliseconds < 33)
                return;

            _lastRender = DateTime.Now;
            _animation.Update();

            bool crazyMode = IsCrazyMode();

            if (_mode == GraphMode.Function && _func != null)
            {
                _renderer.Render(
                    _bitmap,
                    _func,
                    _animation.A,
                    _animation.B,
                    _animation.C
                );
            }
            else if (_mode == GraphMode.Parametric && _fx != null && _fy != null)
            {
                double tStep = crazyMode ? 0.05 : 0.02;

                _renderer.RenderParametric(
                    _bitmap,
                    _fx,
                    _fy,
                    _animation.A,
                    _animation.B,
                    _animation.C,
                    0,
                    _animation.Time,
                    tStep
                );
            }

            DrawAxisLabels();
        }

        private static bool IsCrazyMode()
        {
            return Application.Current?.Resources["ThemeAccent"] is SolidColorBrush brush
                   && brush.Color == Colors.HotPink;
        }

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            _renderer.Zoom(e.Delta > 0 ? 1.1 : 0.9);
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.R)
                _renderer.ResetZoom();
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnRender;
            _host.GoBack();
        }

        private void DrawAxisLabels()
        {
            AxisOverlay.Children.Clear();

            double min = _renderer.WorldMin;
            double max = _renderer.WorldMax;
            double range = max - min;

            double step = GraphRenderer.GetNiceStep(range / 100);

            if (min <= 0 && max >= 0)
            {
                double yPixel = AxisOverlay.ActualHeight / 2 + 6;

                for (double x = Math.Ceiling(min / step) * step; x <= max; x += step)
                {
                    double px = (x - min) / range * AxisOverlay.ActualWidth;
                    if (px < 0 || px > AxisOverlay.ActualWidth) continue;

                    var tb = new TextBlock
                    {
                        Text = x.ToString("0.##"),
                        FontSize = 11,
                        Foreground = Brushes.White
                    };

                    Canvas.SetLeft(tb, px + 2);
                    Canvas.SetTop(tb, yPixel);
                    AxisOverlay.Children.Add(tb);
                }
            }

            if (min <= 0 && max >= 0)
            {
                for (double y = Math.Ceiling(min / step) * step; y <= max; y += step)
                {
                    double py = AxisOverlay.ActualHeight -
                                ((y - min) / range * AxisOverlay.ActualHeight);

                    if (py < 0 || py > AxisOverlay.ActualHeight) continue;

                    var tb = new TextBlock
                    {
                        Text = y.ToString("0.##"),
                        FontSize = 11,
                        Foreground = Brushes.White
                    };

                    Canvas.SetLeft(tb, 4);
                    Canvas.SetTop(tb, py - 8);
                    AxisOverlay.Children.Add(tb);
                }
            }
        }
    }
}