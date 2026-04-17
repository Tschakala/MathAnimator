using System.Windows.Media;

namespace MathAnimator.Model
{
    public class ThemeSettings
    {
        public Color BackgroundColor { get; set; } = Colors.Black;
        public Color GridColor { get; set; } = Color.FromRgb(40, 40, 40);
        public Color AxisColor { get; set; } = Colors.White;
        public Color CurveColor { get; set; } = Colors.Red;

        public double LineThickness { get; set; } = 1.5;
    }
}