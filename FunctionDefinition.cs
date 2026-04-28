namespace MathAnimator.Model
{
    public class FunctionDefinition
    {
        public GraphMode Mode { get; set; }

        public string Formula { get; set; } = "";

        public string XFormula { get; set; } = "";
        public string YFormula { get; set; } = "";

        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public override string ToString()
        {
            string paramsText = $"a = {A}, b = {B}, c = {C}";

            return Mode == GraphMode.Function
                ? $"y = {Formula}   |   {paramsText}"
                : $"x(t) = {XFormula}, y(t) = {YFormula}   |   {paramsText}";
        }
    }
}