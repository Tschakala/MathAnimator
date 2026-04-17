
using System;
using System.Windows.Media.Imaging;
using MathAnimator.Model;

namespace MathAnimator.Rendering
{
	public class GraphRenderer
	{
		private readonly int _width;
		private readonly int _height;

        private ThemeSettings Theme => AppState.Theme;

        private const double DEFAULT_MIN = -20;
        private const double DEFAULT_MAX = 20;

        private double _worldMin = DEFAULT_MIN;
        private double _worldMax = DEFAULT_MAX;



        public GraphRenderer(int width, int height)
        {
            _width = width;
            _height = height;
        }


        public void Zoom(double factor)
        {
            double center = (_worldMin + _worldMax) / 2.0;
            double halfRange = (_worldMax - _worldMin) / 2.0;

            halfRange /= factor;

            _worldMin = center - halfRange;
            _worldMax = center + halfRange;
        }


        public unsafe void Render(
            WriteableBitmap bitmap,
            Func<double, double, double, double, double> func,
            double a,
            double b,
            double c)
        {
            bitmap.Lock();
            byte* buffer = (byte*)bitmap.BackBuffer;

            // ✅ Hintergrund schwarz

            for (int i = 0; i < _width * _height * 4; i += 4)
            {

                buffer[i + 0] = Theme.BackgroundColor.B;
                buffer[i + 1] = Theme.BackgroundColor.G;
                buffer[i + 2] = Theme.BackgroundColor.R;
                buffer[i + 3] = 255;
            }

            // ✅ Koordinatensystem zeichnen
            DrawAxesAndGrid(buffer);

            int? lastX = null;
            int? lastY = null;

            for (int x = 0; x < _width; x++)
            {
                double worldX = Map(x, 0, _width, _worldMin, _worldMax);
                double y = func(worldX, a, b, c);

                int py = (int)(_height / 2 - y * 10);

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

            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, _width, _height));
            bitmap.Unlock();
        }

		private static double Map(
			double v, double a1, double a2, double b1, double b2)
		{
			return b1 + (v - a1) * (b2 - b1) / (a2 - a1);
		}

        private unsafe void DrawLine(
            byte* buffer,
            int x0, int y0,
            int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;

            var color = AppState.Theme.CurveColor;

            while (true)
            {
                if (x0 >= 0 && x0 < _width && y0 >= 0 && y0 < _height)
                {
                    int index = (y0 * _width + x0) * 4;

                    buffer[index + 0] = color.B;
                    buffer[index + 1] = color.G;
                    buffer[index + 2] = color.R;
                    buffer[index + 3] = 255;
                }

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
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
            bitmap.Lock();
            byte* buffer = (byte*)bitmap.BackBuffer;

            for (int i = 0; i < _width * _height * 4; i++)
                buffer[i] = 0;

            // ✅ Koordinatensystem
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

            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, _width, _height));
            bitmap.Unlock();
        }


        private unsafe void DrawAxesAndGrid(byte* buffer)
        {
            int bytesPerPixel = 4;

            // Mittelpunkt (0,0)
            int centerX = _width / 2;
            int centerY = _height / 2;

            // ✅ Gitter (alle 10 Pixel)
            for (int x = 0; x < _width; x += 50)
            {
                for (int y = 0; y < _height; y++)
                {
                    int index = (y * _width + x) * bytesPerPixel;
                    buffer[index + 0] = Theme.GridColor.B;
                    buffer[index + 1] = Theme.GridColor.G;
                    buffer[index + 2] = Theme.GridColor.R;
                    buffer[index + 3] = 255;
                }
            }

            for (int y = 0; y < _height; y += 50)
            {
                for (int x = 0; x < _width; x++)
                {
                    int index = (y * _width + x) * bytesPerPixel;
                    buffer[index + 0] = Theme.GridColor.B;
                    buffer[index + 1] = Theme.GridColor.G;
                    buffer[index + 2] = Theme.GridColor.R;
                    buffer[index + 3] = 255;
                }
            }

            // ✅ X‑Achse (weiß)
            for (int x = 0; x < _width; x++)
            {
                int index = (centerY * _width + x) * bytesPerPixel;
                buffer[index + 0] = Theme.AxisColor.B;
                buffer[index + 1] = Theme.AxisColor.G;
                buffer[index + 2] = Theme.AxisColor.R;
                buffer[index + 3] = 255;
            }

            // ✅ Y‑Achse (weiß)
            for (int y = 0; y < _height; y++)
            {
                int index = (y * _width + centerX) * bytesPerPixel;
                buffer[index + 0] = Theme.AxisColor.B;
                buffer[index + 1] = Theme.AxisColor.G;
                buffer[index + 2] = Theme.AxisColor.R;
                buffer[index + 3] = 255;
            }
        }


        public void ResetZoom()
        {
            _worldMin = DEFAULT_MIN;
            _worldMax = DEFAULT_MAX;
        }

    }
}
