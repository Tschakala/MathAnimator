
using System;

namespace MathAnimator.Model
{
	public class FunctionDefinition
	{
		public string Formula { get; set; } = "";

		public double A { get; set; }
		public double B { get; set; }
		public double C { get; set; }

		public override string ToString()
		{
			return $"{Formula} | a={A}, b={B}, c={C}";
		}
	}
}
