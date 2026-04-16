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
        private WriteableBitmap _bitmap;
        private GraphRenderer _renderer;
        private Func<double, double, double, double, double> _func;
        private AnimationController _animation;


        public AnimationView(
            MainWindow host,
            string formula,
            double aSpeed,
            double bSpeed,
            double cSpeed)
        {
            InitializeComponent();

            _host = host;

            int width = 900;
            int height = 500;

            _bitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgra32, null);

            Surface.Source = _bitmap;

            _renderer = new GraphRenderer(width, height);
            _func = MathParser.Parse(formula);

            // ENDLOS-ANIMATION
            _animation = new AnimationController(
                aSpeed,
                bSpeed,
                cSpeed
            );

            CompositionTarget.Rendering += OnRender;
        }


        private void OnRender(object? sender, EventArgs e)
        {
            _animation.Update();
            _renderer.Render(_bitmap, _func, _animation.A, _animation.B, _animation.C);
        }


        private void OnBack(object sender, RoutedEventArgs e)
        {
            // GANZ wichtig: Rendering stoppen
            CompositionTarget.Rendering -= OnRender;

            // Zurück zum Startmenü
            _host.ShowView(new StartView(_host));

            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= OnRender;
            };

        }

    }
}