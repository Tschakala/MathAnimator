using System;
using System.Collections.Generic;
using System.Text;

namespace _2d_admination
{
    internal class Function
    {
        string _formula;
        double _a;
        double _b;
        double _c;
        int _frames;

        public Function(string formula, double a, double b, double c, int frames)
        {
            _formula = formula;
            _a = a;
            _b = b;
            _c = c;
            _frames = frames;
        }

        public string getformula()
        {
            return _formula;
        }

        public double geta()
        {
            return _a;
        }

        public double getb()
        {
            return _b;
        }

        public double getc()
        {
            return _c;
        }

        public int getframes()
        {
            return _frames;
        }

        public override string ToString()
        {
            if (_a != 0 && _b != 0 && _c != 0)
            {
                return _formula + "\n\tFrames: " + _frames + "   |   a: " + _a + "   |   b: " + _b + "   |   c: " + _c;
            }
            else if (_a != 0 && _b != 0)
            {
                return _formula + "\n\tFrames: " + _frames + "   |   a: " + _a + "   |   b: " + _b;
            }
            else if (_a != 0)
            {
                return _formula + "\n\tFrames: " + _frames + "   |   a: " + _a;
            }
            return _formula + "\n\tFrames: " + _frames;
        }
    }
}
