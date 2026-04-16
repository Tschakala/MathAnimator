using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathAnimator.MathCore;
using MathAnimator.Rendering;
using MathAnimator;

namespace MathAnimator
{
    public partial class AnimationView : UserControl
    {
        private readonly MainWindow _host;
        private WriteableBitmap _bitmap;
        private GraphRenderer _renderer;

        private Func<double, double, double, double, double>? _func;
        private Func<double, double, double, double, double>? _fx;
        private Func<double, double, double, double, double>? _fy;

        private double _a;
        private double _b;
        private double _c;

        private GraphMode _mode;

        private AnimationController _animation;




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

            _host = host;
            _mode = mode;

            _a = a;
            _b = b;
            _c = c;

            int width = 900;
            int height = 500;

            _bitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgra32, null);

            Surface.Source = _bitmap;
            _renderer = new GraphRenderer(width, height);

            if (_mode == GraphMode.Function)
                _func = MathParser.Parse(formula);
            else
            {
                _fx = MathParser.Parse(xFormula);
                _fy = MathParser.Parse(yFormula);
            }

            _animation = new AnimationController(_a, _b, _c);
            CompositionTarget.Rendering += OnRender;
        }


        private void OnRender(object? sender, EventArgs e)
        {
            _animation.Update();

            double t = _animation.Time;
            double a = _animation.A;
            double b = _animation.B;
            double c = _animation.C;

            if (_mode == GraphMode.Function && _func != null)
            {
                _renderer.Render(
                    _bitmap,
                    _func,
                    a, b, c
                );
            }
            else if (_mode == GraphMode.Parametric && _fx != null && _fy != null)
            {
                _renderer.RenderParametric(
                    _bitmap,
                    _fx,
                    _fy,
                    a, b, c,
                    0,
                    t,
                    0.02
                );
            }
        }


        private void OnBack(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnRender;
            _host.GoBack();
        }
    }
}