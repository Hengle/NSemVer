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

				Console.WriteLine("Comparing:");
				Console.WriteLine("Old: {0}".FormatInvariant(Path.GetFullPath(oldAssemblyPath)));
				Console.WriteLine("New: {0}".FormatInvariant(Path.GetFullPath(newAssemblyPath)));

				var breakingChanges = CompareAssemblies(oldAssemblyPath, newAssemblyPath);

				if (arguments.QuickAndQuietMode)
				{
					if (breakingChanges.Any())
					{
						Environment.ExitCode = 1;
						return;
					}
				}
				else
				{
					if (breakingChanges.Any())
					{
						Console.WriteLine("Found breaking changes:");

						foreach (var breakingChange in breakingChanges)
						{
							Console.WriteLine("  [{0}] {1}".FormatInvariant(breakingChange.BreakType, breakingChange.Description));
						}
					}
				}
				return;
			}
			catch (CommandLineException exception)
			{
				Console.WriteLine(exception.ArgumentHelp.Message);
				Console.WriteLine(exception.ArgumentHelp.GetHelpText(System.Console.BufferWidth));
			} 
		}

		private static IEnumerable<BreakingChange> CompareAssemblies(string oldAssemblyPath, string newAssemblyPath)
		{
			using (var oldAssembly = File.Open(oldAssemblyPath, FileMode.Open))
			using (var newAssembly = File.Open(newAssemblyPath, FileMode.Open))
			{
				var changesBuilder = new AssemblyChangesBuilder();
				AssemblyChanges changes = changesBuilder.GetChanges(oldAssembly, newAssembly);
				
				var breakingChangeVisitor = new BreakingChangeVisitor();
				changes.Visit(breakingChangeVisitor);

				return breakingChangeVisitor.BreakingChanges;
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
