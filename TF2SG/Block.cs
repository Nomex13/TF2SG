
//
// This is TeamFortress 2 Say Generator
//
// License: Public Domain
// Iodynis Vladimir (c) 2016
//

namespace SayGenerator
{
	public struct Block
	{
		public readonly int Index;
		public readonly string Description;

		public Block(int param_index, string param_description)
		{
			Index = param_index;
			Description = param_description ?? "...";
		}
	}
}
