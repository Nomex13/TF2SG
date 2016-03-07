using System;
using System.Collections.Generic;
using System.Text;

namespace SayGenerator
{
	public struct Chain
	{
		public readonly int Index;
		public readonly string Title;
		public readonly List<string> Lines;

		public Chain(int param_index, string param_title, List<string> param_lines)
		{
			Index = param_index;
			Title = !String.IsNullOrEmpty(param_title) ? param_title : "";
			Lines = new List<string>(param_lines);
		}
	}
}
