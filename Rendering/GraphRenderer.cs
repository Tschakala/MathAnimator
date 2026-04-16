
using System;
using System.Windows.Media.Imaging;

namespace MathAnimator.Rendering
{
	public class GraphRenderer
	{
		private readonly int _width;
		private readonly int _height;

		private readonly byte _r;
		private readonly byte _g;
		private readonly byte _b;


		public GraphRenderer(int width, int height, byte r = 255, byte g = 0, byte b = 0)
		{
			_width = width;
			_height = height;

			_r = r;
			_g = g;
			_b = b;
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

			// Bildschirm löschen
			for (int i = 0; i < _width * _height * 4; i++)
			{
				buffer[i] = 0;
			}

            // Funktionsgraph zeichnen
            int? lastX = null;
            int? lastY = null;

            for (int x = 0; x < _width; x++)
            {
                double worldX = Map(x, 0, _width, -20, 20);
                double y = func(worldX, a, b, c);

                int py = (int)(_height / 2 - y * 10);
                if (py < 0 || py >= _height)
                {
                    lastX = null;
                    lastY = null;
                    continue;
                }

                if (lastX.HasValue && lastY.HasValue)
                {
                    DrawLine(buffer,
                             lastX.Value, lastY.Value,
                             x, py);
                }

                lastX = x;
                lastY = py;
            }

            bitmap.AddDirtyRect(
				new System.Windows.Int32Rect(0, 0, _width, _height));
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

            while (true)
            {
                if (x0 >= 0 && x0 < _width && y0 >= 0 && y0 < _height)
                {
                    int index = (y0 * _width + x0) * 4;

                    buffer[index + 0] = _b;
                    buffer[index + 1] = _g;
                    buffer[index + 2] = _r;
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

    }
}
