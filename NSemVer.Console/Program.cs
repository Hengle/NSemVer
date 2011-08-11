namespace NSemVer.Console
{
	using System;
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

				using (var oldAssembly = File.Open(arguments.OldAssembly, FileMode.Open))
				using (var newAssembly = File.Open(arguments.OldAssembly, FileMode.Open))
				{
					AssemblyChanges changes = new ChangeBuilder().GetChanges(oldAssembly, newAssembly);

					var breakingChangeVisitor = new BreakingChangeVisitor();
					breakingChangeVisitor.Visit(changes);

					if (arguments.QuickAndQuietMode)
					{
						if (breakingChangeVisitor.BreakingChanges.Any())
						{
							Environment.ExitCode = 1;
							return;
						}
					}
					else
					{
						foreach (var breakingChange in breakingChangeVisitor.BreakingChanges)
						{
							
						}
					}
				}
			}
			catch (CommandLineException exception)
			{
				Console.WriteLine(exception.ArgumentHelp.Message);
				Console.WriteLine(exception.ArgumentHelp.GetHelpText(System.Console.BufferWidth));
			} 
		}
	}
}
