namespace MathAnimator.Rendering
{
    public class AnimationController
    {
        private double _time;

        private readonly double _aSpeed;
        private readonly double _bSpeed;
        private readonly double _cSpeed;

        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public AnimationController(
            double aSpeed,
            double bSpeed,
            double cSpeed)
        {
            _aSpeed = aSpeed;
            _bSpeed = bSpeed;
            _cSpeed = cSpeed;
        }

        public void Update()
        {
            _time += 1.0 / 60.0; // Zeit läuft ~60 FPS

            A = _time * _aSpeed;
            B = _time * _bSpeed;
            C = _time * _cSpeed;
        }
    }
}