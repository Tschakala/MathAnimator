using System.Collections.Generic;

namespace MathAnimator.Model
{
	public class LibraryFolder
	{
		public string Name { get; set; } = "Neuer Ordner";
		public List<FunctionDefinition> Functions { get; set; } = new();

		public override string ToString()
		{
			return $"{Name} ({Functions.Count})";
		}
	}
}