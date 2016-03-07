using System;
using System.Collections.Generic;
using System.Text;

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
