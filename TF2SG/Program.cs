using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SayGenerator
{
	class Program
	{
		private const string field_inputFileDefaultPath  = "./say.txt";
		private const string field_readmeFileDefaultPath  = "./readme.txt";
		private const string field_outputFileDefaultPath = "./say.cfg";

		static void Main(string[] param_arguments)
		{
			bool generateInputTemplate = false;
			string inputFilePath = field_inputFileDefaultPath;
			string outputFilePath = field_outputFileDefaultPath;

			// Deal with specified arguments
			for (int argumentIndex = 0; argumentIndex < param_arguments.Length; argumentIndex++)
			{
				// Help
				if (param_arguments[argumentIndex] == "-h")
				{
					PrintHelp();
					continue;
				}
				// Generate input template
				if (param_arguments[argumentIndex] == "-g")
				{
					generateInputTemplate = true;
					continue;
				}
				// Input path
				if (param_arguments[argumentIndex] == "-i")
				{
					argumentIndex++;
					if (argumentIndex >= param_arguments.Length)
					{
						Console.WriteLine("Flag -o must be followed by a path.");
						PrintHelp();
						return;
					}
					inputFilePath = param_arguments[argumentIndex].Trim(new char[] { '\'', '"', ' ', '\t', '\r', '\n' });
					continue;
				}
				// Output path
				if (param_arguments[argumentIndex] == "-o")
				{
					argumentIndex++;
					if (argumentIndex >= param_arguments.Length)
					{
						Console.WriteLine("Flag -o must be followed by a path.");
						PrintHelp();
						return;
					}
					outputFilePath = param_arguments[argumentIndex].Trim(new char[] { '\'', '"', ' ', '\t', '\r', '\n' });
					continue;
				}
				Console.WriteLine("Unknown flag " + param_arguments[argumentIndex] + ".");
				PrintHelp();
				return;
			}
			// If no arguments were specified
			if (param_arguments.Length == 0)
			{
				generateInputTemplate = !File.Exists(field_inputFileDefaultPath);
			}

			// Generate input template and quit
			if (generateInputTemplate)
			{
				CreateInputTemplate(field_inputFileDefaultPath);
				CreateChainTemplates(field_inputFileDefaultPath);
				CreateReadMe(field_readmeFileDefaultPath);
				return;
			}
			// Generate the say.cfg file

			Dictionary<int, Block> blocks = new Dictionary<int, Block>();
			Dictionary<int, Block> subblocks = new Dictionary<int, Block>();
			Dictionary<int, Phrase> phrases = new Dictionary<int, Phrase>();
			Dictionary<int, Chain> chains = new Dictionary<int, Chain>();

			// Phrases
			{
				string[] lines = File.ReadAllLines(inputFilePath);
				for (int linesIndex = 0; linesIndex < lines.Length; linesIndex++)
				{
					try
					{
						string line = lines[linesIndex];
						line = line.Trim(new[] { ' ', '\t', '\r', '\n' });

						// Empty
						if (line.Length == 0)
						{
							continue;
						}
						// Comments
						if (line.StartsWith("//"))
						{
							continue;
						}
						// Blocks
						if (line.StartsWith("block") && line[6] == ' ')
						{
							if (line.Contains("\""))
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": block cannot have doublequotes in it.");
								return;
							}
							int index = Int32.Parse(line.Substring(5, 1));
							if (phrases.ContainsKey(index))
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": phrase with index " + index + " already exists.");
								return;
							}
							line = line.Substring(7);

							string description = null;

							string[] parts = line.Split(';');
							if (parts.Length > 2)
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": too many semicolons (" + (parts.Length - 1) + ").");
								return;
							}
							if (parts[parts.Length - 1].Length != 0)
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": last semicolon should be followed by nothing and terminate the string.");
								return;
							}
							if (parts.Length >= 1)
							{
								description = parts[0];
							}

							Block block = new Block(index, description);
							blocks.Add(index, block);
							continue;
						}
						// Sub Blocks
						if (line.StartsWith("block") && line[7] == ' ')
						{
							if (line.Contains("\""))
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": block cannot have doublequotes in it.");
								return;
							}
							int index = Int32.Parse(line.Substring(5, 2));
							if (phrases.ContainsKey(index))
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": phrase with index " + index + " already exists.");
								return;
							}
							line = line.Substring(8);

							string description = null;

							string[] parts = line.Split(';');
							if (parts.Length > 2)
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": too many semicolons (" + (parts.Length - 1) + ").");
								return;
							}
							if (parts[parts.Length - 1].Length != 0)
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": last semicolon should be followed by nothing and terminate the string.");
								return;
							}
							if (parts.Length >= 1)
							{
								description = parts[0];
							}

							Block block = new Block(index, description);
							subblocks.Add(index, block);
							continue;
						}
						// Phrases
						if (line.StartsWith("phrase"))
						{
							if (line.Contains("\""))
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": phrase cannot have doublequotes in it.");
								return;
							}
							int index = Int32.Parse(line.Substring(6, 3));
							if (phrases.ContainsKey(index))
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": phrase with index " + index + " already exists.");
								return;
							}
							line = line.Substring(10);

							string text = null;
							string description = null;

							string[] parts = line.Split(';');
							if (parts.Length > 3)
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": too many semicolons (" + (parts.Length - 1) + ").");
								return;
							}
							if (parts[parts.Length - 1].Length != 0)
							{
								Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": last semicolon should be followed by nothing and terminate the string.");
								return;
							}
							if (parts.Length >= 1)
							{
								text = parts[0];
							}
							if (parts.Length >= 2)
							{
								description = parts[1];
							}

							Phrase phrase = new Phrase(index, text, description);
							phrases.Add(index, phrase);
							continue;
						}
					}
					catch (Exception exception)
					{
						Console.WriteLine("Error in file " + inputFilePath + " on line " + linesIndex + ": " + exception.Message);
						return;
					}
				}
			}

			// Chains
			{
				List<string> chainFilePaths = GetChainFilePaths(inputFilePath);
				for (int i = 0; i < chainFilePaths.Count; i++)
				{
					string chainFilePath = chainFilePaths[i];
					Chain chain;
					if (!File.Exists(chainFilePath))
					{
						chain = new Chain(i, "", new List<string>());
						chains.Add(i, chain);
						continue;
					}

					string title = "";
					List<string> lines = new List<string>(File.ReadAllLines(chainFilePath));

					for (int linesIndex = 0; linesIndex < lines.Count; linesIndex++)
					{
						if (lines[linesIndex].Contains("\""))
						{
							Console.WriteLine("Error in file " + chainFilePath + " on line " + linesIndex + ": phrase cannot have doublequotes in it.");
							return;
						}
						if (lines[linesIndex].Contains(";"))
						{
							Console.WriteLine("Error in file " + chainFilePath + " on line " + linesIndex + ": phrase cannot have semicolon in it.");
							return;
						}
					}
					if (lines.Count > 0)
					{
						title = lines[0];
						lines.RemoveAt(0);
					}

					chain = new Chain(i, title, lines);
					chains.Add(i, chain);
				}
			}

			CreateOutput(outputFilePath, blocks, subblocks, phrases, chains);
		}
		static void PrintHelp()
		{
			Console.WriteLine("");
			Console.WriteLine("TF2SG (TeamFortress 2 Say Generator)");
			Console.WriteLine("Author:  Vladimir Iodynis (c) 2016");
			Console.WriteLine("License: Public Domain");
			Console.WriteLine("");
			Console.WriteLine("Usage:");
			Console.WriteLine("\ttf2sg.exe [-g] [-i input] [-o output]");
			Console.WriteLine("");
			Console.WriteLine("Flags:");
			Console.WriteLine("\t-h\tPrint this");
			Console.WriteLine("\t-g\tGenerate an input file " + field_inputFileDefaultPath + ". Ignores -i and -o flags");
			Console.WriteLine("\t-i\tUse specified file as input\t\tFiles with \".00\" - \".29\" before extension will be used for chains");
			Console.WriteLine("\t-o\tUse specified file for output");
			Console.WriteLine("");
			Console.WriteLine("Executing with no flags if " + field_inputFileDefaultPath + " does not exist equals to:");
			Console.WriteLine("\ttf2sg.exe -g");
			Console.WriteLine("Executing with no flags if " + field_inputFileDefaultPath + " exists equals to:");
			Console.WriteLine("\ttf2sg.exe -i " + field_inputFileDefaultPath + " -o " + field_outputFileDefaultPath);
		}
		static void CreateReadMe(string param_filePath)
		{
			if (File.Exists(param_filePath))
			{
				return;
			}

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("This is a readme file for TF2SG (TeamFortress 2 Say Generator)");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("Author:  Vladimir Iodynis (c) 2016");
			stringBuilder.AppendLine("License: Public Domain");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("Usage:");
			stringBuilder.AppendLine("1. Run tf2sg.exe. It will create say.txt for phrased and say.00.txt - say.29 for chained phrases.");
			stringBuilder.AppendLine("2. Edit say.txt with your phrases.");
			stringBuilder.AppendLine("2. Edit say.00.txt - say.29.txt with your chained phrases.");
			stringBuilder.AppendLine("3. Run tf2sg.exe. It will create say.cfg.");
			stringBuilder.AppendLine("4. Copy say.cfg to your .../Steam/SteamApps/common/Team Fortress 2/tf/cfg/ directory.");
			stringBuilder.AppendLine("5. Add \"exec say.cfg\" line (with no quotes) to your .../Steam/SteamApps/common/Team Fortress 2/tf/cfg/all.cfg file.");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("Tips:");
			stringBuilder.AppendLine("If you already use \"developer\" or any of \"con_filter\" commands there would be conflicts. Be 99% sure.");
			stringBuilder.AppendLine("If you already use numpad keys (numpad 0-9, numpad ./*-+ and numpad Enter) there would be conflicts.");
			stringBuilder.AppendLine("");

			File.WriteAllText(param_filePath, stringBuilder.ToString());
		}
		static List<string> GetChainFilePaths(string param_filePath)
		{
			List<string> chainFilePaths = new List<string>();
			for (int i = 0; i < 30; i++)
			{
				chainFilePaths.Add(Path.GetFileNameWithoutExtension(param_filePath) + "." + i.ToString("D2") + Path.GetExtension(param_filePath));
			}
			return chainFilePaths;
		}
		static void CreateChainTemplates(string param_filePath)
		{
			List<string> chainFilePaths = GetChainFilePaths(param_filePath);
			foreach (string chainFilePath in chainFilePaths)
			{
				CreateChainTemplate(chainFilePath);
			}
		}
		static void CreateChainTemplate(string param_filePath)
		{
			if (File.Exists(param_filePath))
			{
				return;
			}

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("");

			File.WriteAllText(param_filePath, stringBuilder.ToString());
		}
		static void CreateInputTemplate(string param_filePath)
		{
			if (File.Exists(param_filePath))
			{
				return;
			}

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// This is an input file for TF2SG (TeamFortress 2 Say Generator)");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// License: Public Domain");
			stringBuilder.AppendLine("// Vladimir Iodynis (c) 2016");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Blocks");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Syntax:");
			stringBuilder.AppendLine("// blockX block description;");
			stringBuilder.AppendLine("// blockXX block description;");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Warning:");
			stringBuilder.AppendLine("// No ; or \" allowed (kind of Source engine limits).");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Examples:");
			stringBuilder.AppendLine("// block00 happy    smilies;");
			stringBuilder.AppendLine("// block01 suicidal smilies;");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("");

			for (int index1 = 0; index1 <= 9; index1++)
			{
				stringBuilder.AppendLine("block" + index1 + " ;");
			}

			for (int index1 = 0; index1 <= 9; index1++)
			{
				for (int index2 = 0; index2 <= 9; index2++)
				{
					stringBuilder.AppendLine("block" + index1 + index2 + " ;");
				}
			}

			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Phrases");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Syntax:");
			stringBuilder.AppendLine("// phraseXXX phrase itself;phrase description;");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Warning:");
			stringBuilder.AppendLine("// Unicode symbols are fucked up when displaying tips on the screen.");
			stringBuilder.AppendLine("// Therefore use descriptions for complex smilies to keep tips helpful.");
			stringBuilder.AppendLine("// Also no ; or \" allowed (kind of Source engine limits).");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Examples:");
			stringBuilder.AppendLine("// phrase000 da da;;");
			stringBuilder.AppendLine("// phrase861 XD;;");
			stringBuilder.AppendLine("// phrase332 ʕ•́ᴥ•̀ʔ;bear;");
			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("");

			for (int index1 = 0; index1 <= 9; index1++)
			{
				for (int index2 = 0; index2 <= 9; index2++)
				{
					for (int index3 = 0; index3 <= 9; index3++)
					{
						stringBuilder.AppendLine("phrase" + index1 + index2 + index3 + " ;;");
					}
				}
			}

			File.WriteAllText(param_filePath, stringBuilder.ToString());
		}
		static void CreateOutput(string param_fileName, Dictionary<int, Block> param_blocks, Dictionary<int, Block> param_subblocks, Dictionary<int, Phrase> param_phrases, Dictionary<int, Chain> param_chains)
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("");

			// Stuff

			stringBuilder.AppendLine("//");
			stringBuilder.AppendLine("// Generated by TF2SG (TeamFortress 2 Say Generator)");
			stringBuilder.AppendLine("// Author:  Vladimir Iodynis (c) 2016");
			stringBuilder.AppendLine("// License: Public Domain");
			stringBuilder.AppendLine("//");

			stringBuilder.AppendLine("");

			stringBuilder.AppendLine("// Unbind keys that will be used");
			stringBuilder.AppendLine("unbind KP_DEL;");
			stringBuilder.AppendLine("unbind KP_HOME;");
			stringBuilder.AppendLine("unbind KP_UPARROW;");
			stringBuilder.AppendLine("unbind KP_PGUP;");
			stringBuilder.AppendLine("unbind KP_LEFTARROW;");
			stringBuilder.AppendLine("unbind KP_5;");
			stringBuilder.AppendLine("unbind KP_RIGHTARROW;");
			stringBuilder.AppendLine("unbind KP_END;");
			stringBuilder.AppendLine("unbind KP_DOWNARROW;");
			stringBuilder.AppendLine("unbind KP_PGDN;");
			stringBuilder.AppendLine("unbind KP_INS;");
			stringBuilder.AppendLine("unbind KP_SLASH;");
			stringBuilder.AppendLine("unbind KP_MULTIPLY;");
			stringBuilder.AppendLine("unbind KP_MINUS;");
			stringBuilder.AppendLine("unbind KP_PLUS;");
			stringBuilder.AppendLine("unbind KP_ENTER;");

			stringBuilder.AppendLine("");

			stringBuilder.AppendLine("// Set filter settings");
			stringBuilder.AppendLine("con_filter_text \"|||||||||||||\";       // Something to never give a match");
			stringBuilder.AppendLine("con_filter_text_out \"\";                // ");
			stringBuilder.AppendLine("con_notifytime 8;                      // How long to display text");
			stringBuilder.AppendLine("contimes 12;                           // Number of lines to display. Value of 12 displays 10 lines o_O");
			stringBuilder.AppendLine("con_nprint_bgalpha 50;                 // Con_NPrint background alpha.");
			stringBuilder.AppendLine("con_nprint_bgborder 5;                 // Con_NPrint border size.");

			stringBuilder.AppendLine("");

			// Phrases

			stringBuilder.AppendLine("// Phrases");
			stringBuilder.AppendLine("alias -phrase \"reset_numpad;\";");
			foreach (Phrase phrase in param_phrases.Values)
			{
				stringBuilder.AppendLine("alias +phrase" + phrase.Index.ToString("D3") + " \"-tip; say " + phrase.Text + ";\";");
			}

			stringBuilder.AppendLine("");

			// Tips

			stringBuilder.AppendLine("// Tips");
			stringBuilder.AppendLine("alias -tip    \"con_filter_enable 0; developer 0;\";");
			stringBuilder.Append("alias +tip    \"developer 1; ");
			for (int index1 = 0; index1 <= 9; index1++)
			{
				int index = index1;
				if (!param_blocks.ContainsKey(index))
				{
					continue;
				}
				Block block = param_blocks[index];
				stringBuilder.Append("echo " + index1 + ". - " + block.Description + "; ");
			}
			stringBuilder.AppendLine("con_filter_enable 2;\";");
			
			for (int index1 = 0; index1 <= 9; index1++)
			{
				stringBuilder.Append("alias +tip_" + index1 + "  \"developer 1; ");
				for (int index2 = 0; index2 <= 9; index2++)
				{
					int index = index1 * 10 + index2;
					if (!param_subblocks.ContainsKey(index))
					{
						continue;
					}
					Block block = param_subblocks[index];
					stringBuilder.Append("echo " + index2 + ". - " + block.Description + "; ");
				}
				stringBuilder.AppendLine("con_filter_enable 2;\";");
			}

			for (int index1 = 0; index1 <= 9; index1++)
			{
				for (int index2 = 0; index2 <= 9; index2++)
				{
					stringBuilder.Append("alias +tip_" + index1 + index2 + " \"developer 1; ");
					for (int index3 = 0; index3 <= 9; index3++)
					{
						int index = index1 * 100 + index2 * 10 + index3;
						if (!param_phrases.ContainsKey(index))
						{
							continue;
						}
						Phrase phrase = param_phrases[index];
						stringBuilder.Append("echo " + index3 + ". - " + phrase.Description + "; ");
					}
					stringBuilder.AppendLine("con_filter_enable 2;\";");
				}
			}
			for (int index1 = 0; index1 <= 2; index1++)
			{
				stringBuilder.Append("alias +tip_c" + index1 + "  \"developer 1; ");
				for (int index2 = 0; index2 <= 9; index2++)
				{
					int index = index1 * 10 + index2;
					if (!param_chains.ContainsKey(index))
					{
						continue;
					}
					Chain chain = param_chains[index];
					stringBuilder.Append("echo " + index2 + ". - " + chain.Title + "; ");
				}
				stringBuilder.AppendLine("con_filter_enable 2;\";");
			}

			stringBuilder.AppendLine("");

			// Blocks
			
			for (int index1 = 0; index1 <= 9; index1++)
			{
				stringBuilder.AppendLine("alias +block" + index1 + "  \"-tip;\";");
				stringBuilder.Append("alias -block" + index1 + "  \"+tip_" + index1 + "; ");
				for (int index2 = 0; index2 <= 9; index2++)
				{
					stringBuilder.Append("alias +key" + index2 + " +block" + index1 + index2 + "; alias -key" + index2 + " -block" + index1 + index2 + "; ");
				}
				stringBuilder.AppendLine("\";");
			}
			for (int index1 = 0; index1 <= 9; index1++)
			{
				for (int index2 = 0; index2 <= 9; index2++)
				{
					stringBuilder.AppendLine("alias +block" + index1 + index2 + " \"-tip;\";");
					stringBuilder.Append("alias -block" + index1 + index2 + " \"+tip_" + index1 + index2 + ";");
					for (int index3 = 0; index3 <= 9; index3++)
					{
						stringBuilder.Append("alias +key" + index3 + " +phrase" + index1 + index2 + index3 + ";");
						stringBuilder.Append("alias -key" + index3 + " -phrase;");
					}
					stringBuilder.AppendLine("\";");
				}
			}

			stringBuilder.AppendLine("");

			// Songs

			stringBuilder.AppendLine("// Songs");
			foreach (Chain chain in param_chains.Values)
			{
				stringBuilder.AppendLine("alias +chain" + chain.Index.ToString("D2") + "            \"" + 
						"alias +chainskip +chain"  + chain.Index.ToString("D2") + "_0000_skip; " +
						"alias +chainprint +chain" + chain.Index.ToString("D2") + "_0000_print;\";");
				stringBuilder.AppendLine("alias -chain" + chain.Index.ToString("D2") + "            \";\"");
				for (int lineIndex = 0; lineIndex < chain.Lines.Count; lineIndex++)
				{
					stringBuilder.AppendLine("alias +chain" + chain.Index.ToString("D2") + "_" + lineIndex.ToString("D4")       + "_skip  \"" +
						"alias +chainskip +chain"      + chain.Index.ToString("D2") + "_" + (lineIndex + 1).ToString("D4") + "_skip; " +
						"alias +chainprint +chain" + chain.Index.ToString("D2") + "_" + (lineIndex + 1).ToString("D4") + "_print;\"");
					stringBuilder.AppendLine("alias -chain" + chain.Index.ToString("D2") + "_" + lineIndex.ToString("D4") + "_skip  \";\"");
					stringBuilder.AppendLine("alias +chain" + chain.Index.ToString("D2") + "_" + lineIndex.ToString("D4")       + "_print \"" +
						"alias +chainskip +chain"      + chain.Index.ToString("D2") + "_" + (lineIndex + 1).ToString("D4") + "_skip; " +
						"alias +chainprint +chain"     + chain.Index.ToString("D2") + "_" + (lineIndex + 1).ToString("D4") + "_print; " +
						"say " + chain.Lines[lineIndex] + ";\";");
					stringBuilder.AppendLine("alias -chain" + chain.Index.ToString("D2") + "_" + lineIndex.ToString("D4") + "_print \";\"");
				}
				stringBuilder.AppendLine("alias +chain" + chain.Index.ToString("D2") + "_" + chain.Lines.Count.ToString("D4") + "_skip  \";\";");
				stringBuilder.AppendLine("alias -chain" + chain.Index.ToString("D2") + "_" + chain.Lines.Count.ToString("D4") + "_skip  \";\";");
				stringBuilder.AppendLine("alias +chain" + chain.Index.ToString("D2") + "_" + chain.Lines.Count.ToString("D4") + "_print \";\";");
				stringBuilder.AppendLine("alias -chain" + chain.Index.ToString("D2") + "_" + chain.Lines.Count.ToString("D4") + "_print \";\";");
			}

			stringBuilder.AppendLine("alias +chainskip  \";\";");
			stringBuilder.AppendLine("alias +chainprint \";\";");
			stringBuilder.AppendLine("alias -chainskip  \";\";");
			stringBuilder.AppendLine("alias -chainprint \";\";");
			
			for (int index1 = 0; index1 <= 2; index1++)
			{
				stringBuilder.Append("alias +key_chain" + index1 + " \"+tip_c" + index1 + "");
				for (int index2 = 0; index2 <= 9; index2++)
				{
					stringBuilder.Append(";alias +key" + index2 + " +chain" + index1 + index2 + "; alias -key" + index2 + " -chain" + index1 + index2 + "; ");
				}
				stringBuilder.AppendLine("\";");
				stringBuilder.AppendLine("alias -key_chain" + index1 + " \"reset_numpad; -tip;\"");
			}
			stringBuilder.AppendLine("alias +key_chainskip +chainskip;");
			stringBuilder.AppendLine("alias -key_chainskip -chainskip;");
			stringBuilder.AppendLine("alias +key_chainprint +chainprint;");
			stringBuilder.AppendLine("alias -key_chainprint -chainprint;");

			stringBuilder.AppendLine("");

			// Stuff

			stringBuilder.AppendLine("// Resetter");
			stringBuilder.Append("alias reset_numpad \"alias +help +helpon; ");
			for (int index1 = 0; index1 <= 9; index1++)
			{
				stringBuilder.Append("alias +key" + index1 + " +block" + index1 + "; alias -key" + index1 + " -block" + index1 + "; ");
			}
			stringBuilder.AppendLine("\";");
			//stringBuilder.Append("alias reset_numpad \"alias +help +helpon;alias +key0 +block0; alias -key0 -block0; alias +key1 +block1; alias -key1 -block1; alias +key2 +block2; alias -key2 -block2; alias +key3 +block3; alias -key3 -block3; alias +key4 +block4; alias -key4 -block4; alias +key5 +block5; alias -key5 -block5; alias +key6 +block6; alias -key6 -block6; alias +key7 +block7; alias -key7 -block7; alias +key8 +block8; alias -key8 -block8; alias +key9 +block9; alias -key9 -block9;\";");

			stringBuilder.AppendLine("");

			stringBuilder.AppendLine("// Helper");
			stringBuilder.AppendLine("alias +helpon  \"alias -help -helpon;  reset_numpad; -tip;\";");
			stringBuilder.AppendLine("alias -helpon  \"alias +help +helpoff;               +tip;\";");
			stringBuilder.AppendLine("alias +helpoff \"alias -help -helpoff; reset_numpad; -tip;\";");
			stringBuilder.AppendLine("alias -helpoff \"alias +help +helpon;                -tip;\";");
			//stringBuilder.AppendLine("alias helpon \"reset_numpad;+tip;alias help helpoff;\"");
			//stringBuilder.AppendLine("alias helpoff \"-tip;alias help helpon;\"");
			//stringBuilder.AppendLine("alias help helpon");

			stringBuilder.AppendLine("");

			stringBuilder.AppendLine("// Reset");
			stringBuilder.AppendLine("reset_numpad;");

			stringBuilder.AppendLine("");

			stringBuilder.AppendLine("// Bind");
			stringBuilder.AppendLine("bind KP_DEL        +help;");
			stringBuilder.AppendLine("bind KP_INS        +key0;");
			stringBuilder.AppendLine("bind KP_END        +key1;");
			stringBuilder.AppendLine("bind KP_DOWNARROW  +key2;");
			stringBuilder.AppendLine("bind KP_PGDN       +key3;");
			stringBuilder.AppendLine("bind KP_LEFTARROW  +key4;");
			stringBuilder.AppendLine("bind KP_5          +key5;");
			stringBuilder.AppendLine("bind KP_RIGHTARROW +key6;");
			stringBuilder.AppendLine("bind KP_HOME       +key7;");
			stringBuilder.AppendLine("bind KP_UPARROW    +key8;");
			stringBuilder.AppendLine("bind KP_PGUP       +key9;");
			stringBuilder.AppendLine("bind KP_SLASH      +key_chain0;");
			stringBuilder.AppendLine("bind KP_MULTIPLY   +key_chain1;");
			stringBuilder.AppendLine("bind KP_MINUS      +key_chain2;");
			stringBuilder.AppendLine("bind KP_PLUS       +key_chainskip;");
			stringBuilder.AppendLine("bind KP_ENTER      +key_chainprint;");

			stringBuilder.AppendLine("");

			File.WriteAllText(param_fileName, stringBuilder.ToString());
		}
	}
}
