namespace MathAnimator.Model
{
    public class FunctionDefinition
    {
        public GraphMode Mode { get; set; }

        // Für Funktionsmodus
        public string Formula { get; set; } = "";

        // Für Parametermodus
        public string XFormula { get; set; } = "";
        public string YFormula { get; set; } = "";

        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public override string ToString()
        {
            return Mode == GraphMode.Function
                ? $"y = {Formula}"
                : $"x(t) = {XFormula}, y(t) = {YFormula}";
        }
    }
}