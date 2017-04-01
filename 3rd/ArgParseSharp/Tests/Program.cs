using ArgParseSharp;
using System;

namespace Tests
{
	class MainClass
	{
		static int Main(string[] args) {
			ArgParser.AddArgument('a', "alpha", argType: ArgumentType.Named, consumption: 1);
			ArgParser.AddArgument(
				"NUM",
				argType: ArgumentType.Positional,
				consumption: -1,
				dataType: typeof(int)
			);

			foreach (var argPair in ArgParser.ParseArgs(new string[] {"--alpha", "7", "1", "55"})) {
				Console.WriteLine(argPair.Value);
			}

			return 0;
		}
	}
}
