using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MathAnimator.Rendering
{
    public class GraphRenderer
    {
        private readonly int _width;
        private readonly int _height;

        private const double DEFAULT_MIN = -20;
        private const double DEFAULT_MAX = 20;

        private double _worldMin = DEFAULT_MIN;
        private double _worldMax = DEFAULT_MAX;

        public double WorldMin => _worldMin;
        public double WorldMax => _worldMax;

        private readonly Color[] _rainbowCache = new Color[360];
        private double _lastRainbowTime = -1;

        public GraphRenderer(int width, int height)
        {
            _width = width;
            _height = height;
        }

        private static Color GetThemeColorSafe(string key, Color fallback)
        {
            try
            {
                if (Application.Current?.Resources.Contains(key) == true &&
                    Application.Current.Resources[key] is SolidColorBrush brush)
                {
                    return brush.Color;
                }
            }
            catch { }

            return fallback;
        }

        private static Color GetThemeColor(string key)
        {
            return GetThemeColorSafe(key, Colors.Magenta);
        }

        public void Zoom(double factor)
        {
            double center = (_worldMin + _worldMax) / 2.0;
            double halfRange = (_worldMax - _worldMin) / 2.0;
            halfRange /= factor;

            _worldMin = center - halfRange;
            _worldMax = center + halfRange;
        }

        public void ResetZoom()
        {
            _worldMin = DEFAULT_MIN;
            _worldMax = DEFAULT_MAX;
        }

        public unsafe void Render(
            WriteableBitmap bitmap,
            Func<double, double, double, double, double> func,
            double a,
            double b,
            double c)
        {
            UpdateRainbowCache();

            Color bg = GetThemeColor("ThemeBackground");

            bitmap.Lock();
            byte* buffer = (byte*)bitmap.BackBuffer;

            for (int i = 0; i < _width * _height * 4; i += 4)
            {
                buffer[i + 0] = bg.B;
                buffer[i + 1] = bg.G;
                buffer[i + 2] = bg.R;
                buffer[i + 3] = 255;
            }

            DrawAxesAndGrid(buffer);

            int? lastX = null;
            int? lastY = null;

            for (int x = 0; x < _width; x++)
            {
                double worldX = Map(x, 0, _width, _worldMin, _worldMax);
                double y = func(worldX, a, b, c);

                // ✅ ONLY REAL FIX
                int py = (int)Map(y, _worldMin, _worldMax, _height, 0);

                if (py < 0 || py >= _height)
                {
                    lastX = lastY = null;
                    continue;
                }

                if (lastX.HasValue)
                    DrawLine(buffer, lastX.Value, lastY!.Value, x, py);

                lastX = x;
                lastY = py;
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            bitmap.Unlock();
        }

        public unsafe void RenderParametric(
            WriteableBitmap bitmap,
            Func<double, double, double, double, double> fx,
            Func<double, double, double, double, double> fy,
            double a,
            double b,
            double c,
            double tStart,
            double tEnd,
            double tStep)
        {
            UpdateRainbowCache();

            bitmap.Lock();
            byte* buffer = (byte*)bitmap.BackBuffer;

            for (int i = 0; i < _width * _height * 4; i++)
                buffer[i] = 0;

            DrawAxesAndGrid(buffer);

            int? lastX = null;
            int? lastY = null;

            for (double t = tStart; t <= tEnd; t += tStep)
            {
                double xVal = fx(t, a, b, c);
                double yVal = fy(t, a, b, c);

                int px = (int)Map(xVal, _worldMin, _worldMax, 0, _width);
                int py = (int)Map(yVal, _worldMin, _worldMax, _height, 0);

                if (px < 0 || px >= _width || py < 0 || py >= _height)
                {
                    lastX = lastY = null;
                    continue;
                }

                if (lastX.HasValue)
                    DrawLine(buffer, lastX.Value, lastY!.Value, px, py);

                lastX = px;
                lastY = py;
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            bitmap.Unlock();
        }

        private static double Map(double v, double a1, double a2, double b1, double b2)
            => b1 + (v - a1) * (b2 - b1) / (a2 - a1);

        private unsafe void DrawLine(byte* buffer, int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            Color accent = GetThemeColorSafe("ThemeAccent", Colors.Blue);
            bool crazyMode = accent == Colors.HotPink;
            Color curveColor = GetThemeColorSafe("ThemeCurve", Colors.Red);

            while (true)
            {
                Color color = crazyMode
                    ? _rainbowCache[Math.Abs(x0) % 360]
                    : curveColor;

                int index = (y0 * _width + x0) * 4;
                buffer[index + 0] = color.B;
                buffer[index + 1] = color.G;
                buffer[index + 2] = color.R;
                buffer[index + 3] = 255;

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        private unsafe void DrawAxesAndGrid(byte* buffer)
        {
            Color grid = GetThemeColor("ThemeGrid");
            Color axis = GetThemeColor("ThemeForeground");

            int bpp = 4;

            double worldWidth = _worldMax - _worldMin;
            double unitsPerPixel = worldWidth / _width;

            double step = GetNiceStep(unitsPerPixel);
            double subStep = step / 5.0;

            bool drawSubGrid = step < worldWidth / 10.0;
            double usedStep = drawSubGrid ? subStep : step;

            double startX = Math.Floor(_worldMin / usedStep) * usedStep;
            int index = 0;

            for (double x = startX; x <= _worldMax; x += usedStep, index++)
            {
                int px = (int)Map(x, _worldMin, _worldMax, 0, _width);
                if (px < 0 || px >= _width) continue;

                bool major = drawSubGrid && index % 5 == 0;
                byte alpha = major ? (byte)90 : (byte)35;

                for (int y = 0; y < _height; y++)
                {
                    int i = (y * _width + px) * bpp;
                    buffer[i + 0] = (byte)(grid.B * alpha / 255);
                    buffer[i + 1] = (byte)(grid.G * alpha / 255);
                    buffer[i + 2] = (byte)(grid.R * alpha / 255);
                    buffer[i + 3] = 255;
                }
            }

            if (_worldMin <= 0 && _worldMax >= 0)
            {
                int cx = (int)Map(0, _worldMin, _worldMax, 0, _width);
                int cy = (int)Map(0, _worldMin, _worldMax, _height, 0);

                for (int y = 0; y < _height; y++)
                {
                    int i = (y * _width + cx) * bpp;
                    buffer[i + 0] = axis.B;
                    buffer[i + 1] = axis.G;
                    buffer[i + 2] = axis.R;
                    buffer[i + 3] = 255;
                }

                for (int x = 0; x < _width; x++)
                {
                    int i = (cy * _width + x) * bpp;
                    buffer[i + 0] = axis.B;
                    buffer[i + 1] = axis.G;
                    buffer[i + 2] = axis.R;
                    buffer[i + 3] = 255;
                }
            }
        }

        public static double GetNiceStep(double unitsPerPixel)
        {
            double exponent = Math.Pow(10, Math.Floor(Math.Log10(unitsPerPixel * 100)));
            double mantissa = (unitsPerPixel * 100) / exponent;

            if (mantissa < 2) return 1 * exponent;
            if (mantissa < 5) return 2 * exponent;
            return 5 * exponent;
        }

        private void UpdateRainbowCache()
        {
            double time = DateTime.Now.TimeOfDay.TotalSeconds;

            if (Math.Abs(time - _lastRainbowTime) > 0.05)
            {
                for (int i = 0; i < 360; i++)
                {
                    double t = time + i * 0.05;
                    _rainbowCache[i] = Color.FromRgb(
                        (byte)(128 + 127 * Math.Sin(t)),
                        (byte)(128 + 127 * Math.Sin(t + 2)),
                        (byte)(128 + 127 * Math.Sin(t + 4))
                    );
                }
                _lastRainbowTime = time;
            }
        }
    }
}