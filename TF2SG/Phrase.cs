
//
// This is TeamFortress 2 Say Generator
//
// License: Public Domain
// Iodynis Vladimir (c) 2016
//

using System;

namespace SayGenerator
{
	public struct Phrase
	{
		public readonly int Index;
		public readonly string Text;
		public readonly string Description;

		public Phrase(int param_index, string param_text, string param_description)
		{
			Index = param_index;
			Text = !String.IsNullOrEmpty(param_text) ? param_text : "";
			Description = !String.IsNullOrEmpty(param_description) ? param_description : (!String.IsNullOrEmpty(param_text) ? param_text : "");
		}
	}
}
