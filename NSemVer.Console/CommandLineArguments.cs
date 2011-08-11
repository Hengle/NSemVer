namespace NSemVer.Console
{
	using CmdLine;

	[CommandLineArguments(Program = "NSemVer.Console", Title = "Semantic version checker for .Net")]
	public class CommandLineArguments
	{
		[CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
		public bool Help { get; set; }

		[CommandLineParameter(Name = "old", ParameterIndex = 1, Required = true, Description = "Path to the old version of assembly")]
		public string OldAssembly { get; set; }

		[CommandLineParameter(Name = "new", ParameterIndex = 2, Required = true, Description = "Path to the new version of assembly")]
		public string NewAssembly { get; set; }

		[CommandLineParameter(Command = "q", Required = false, Description = "Quick and quiet mode. No console output and returns non-zero result on first breaking change encountered")]
		public bool QuickAndQuietMode { get; set; }
	}
}