using System;

namespace MathAnimator
{
    public class AnimationController
    {
        public double Time { get; private set; }

        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        private readonly double _a0;
        private readonly double _b0;
        private readonly double _c0;

        private readonly double _aSpeed;
        private readonly double _bSpeed;
        private readonly double _cSpeed;

        private DateTime _lastTime;

        public AnimationController(double a, double b, double c)
        {
            _a0 = a;
            _b0 = b;
            _c0 = c;

            _aSpeed = a;
            _bSpeed = b;
            _cSpeed = c;

            A = a;
            B = b;
            C = c;

            _lastTime = DateTime.Now;
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            double deltaTime = (now - _lastTime).TotalSeconds;
            _lastTime = now;

            Time += deltaTime;

            A = _a0 + _aSpeed * Time;
            B = _b0 + _bSpeed * Time;
            C = _c0 + _cSpeed * Time;
        }
    }
}