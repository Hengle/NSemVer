namespace NSemVer.Console
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using CmdLine;
	using NSemVer.Visitors.BreakingChanges;

	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var arguments = CommandLine.Parse<CommandLineArguments>();

				string oldAssemblyPath = arguments.OldAssembly;
				string newAssemblyPath = arguments.NewAssembly;

				if (!VerifyFileExists(oldAssemblyPath) || !VerifyFileExists(newAssemblyPath))
					return;

				CompareAssemblies(Console.Out, oldAssemblyPath, newAssemblyPath, arguments.QuickAndQuietMode);

				return;
			}
			catch (CommandLineException exception)
			{
				Console.WriteLine(exception.ArgumentHelp.Message);
				Console.WriteLine(exception.ArgumentHelp.GetHelpText(System.Console.BufferWidth));
			} 
		}

		private static void CompareAssemblies(TextWriter writer, string oldAssemblyPath, string newAssemblyPath, bool quickAndQuietMode)
		{
			writer.WriteLine("Comparing:");
			writer.WriteLine("Old: {0}".FormatInvariant(Path.GetFullPath(oldAssemblyPath)));
			writer.WriteLine("New: {0}".FormatInvariant(Path.GetFullPath(newAssemblyPath)));

			using (var oldAssembly = File.Open(oldAssemblyPath, FileMode.Open))
			using (var newAssembly = File.Open(newAssemblyPath, FileMode.Open))
			{
				AssemblyChanges changes = new ChangeBuilder().GetChanges(oldAssembly, newAssembly);

				var breakingChangeVisitor = new BreakingChangeVisitor();
				breakingChangeVisitor.Visit(changes);

				if (quickAndQuietMode)
				{
					if (breakingChangeVisitor.BreakingChanges.Any())
					{
						Environment.ExitCode = 1;
						return;
					}
				}
				else
				{
					List<BreakingChangeResult> breakingChangeResults = breakingChangeVisitor.BreakingChanges.ToList();

					writer.WriteLine("Found {0} breaking changes".FormatInvariant(breakingChangeResults.Count));

					// TODO: Use pretty printer visitor to display exact changes. Just dumping the enum name for now
					foreach (var result in breakingChangeResults)
					{
						writer.WriteLine("\t-{0}".FormatInvariant(result.BreakType));
					}
				}
			}
		}

		private static bool VerifyFileExists(string pathFromCmdLine)
		{
			if (!File.Exists(pathFromCmdLine))
			{
				Console.Error.WriteLine("Could not find assembly using path '{0}'".FormatInvariant(pathFromCmdLine));
				Environment.ExitCode = 2;
				return false;
			}

			return true;
		}
	}
}
