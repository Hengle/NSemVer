namespace NSemVer.Tests
{
	using System;
	using System.CodeDom.Compiler;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using NUnit.Framework;
	using StoryQ;

	public abstract class NSemVerTestBase
	{
		protected readonly Feature Fixture;
		private const string OldAssemblyDir = "V1";
		private const string NewAssemblyDir = "V2";

		protected NSemVerTestBase()
		{
			Fixture = new Story("Checking for API changes")
				.InOrderTo("determine API changes")
				.AsA("developer")
				.IWant("compare previous assembly version to current");

			EnsureDirectoryCreated(OldAssemblyDir);
			EnsureDirectoryCreated(NewAssemblyDir);
		}

		protected string OldAssemblyPath { get; private set; }

		protected string NewAssemblyPath { get; private set; }

		protected void OldAssemblyWith(string source)
		{
			OldAssemblyPath = CompileAssembly(Path.Combine(OldAssemblyDir, "TestAssembly.dll"), source);
		}

		protected void NewAssemblyWith(string source)
		{
			NewAssemblyPath = CompileAssembly(Path.Combine(NewAssemblyDir, "TestAssembly.dll"), source);
		}

		protected string GenerateScenarioName(MethodBase method)
		{
			// TODO: Turn Pascal cased name into one with spaces
			return method.Name;
		}

		protected Condition GivenContext(string previousCode, string currentCode)
		{
			return Fixture
				.WithScenario(GenerateScenarioName(MethodBase.GetCurrentMethod()))
				.Given(OldAssemblyWith, previousCode)
				.And(NewAssemblyWith, currentCode);
		}

		private static string CompileAssembly(string fileName, string source)
		{
			var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
			var outputFilePath = Path.Combine(baseDir, fileName);

			if (File.Exists(outputFilePath))
			{
				File.Delete(outputFilePath);
			}

			var parameters = new CompilerParameters { GenerateExecutable = false, OutputAssembly = fileName };
			var codeProvider = CodeDomProvider.CreateProvider("CSharp");
			CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, source);

			Assert.AreEqual(0, results.Errors.Count, string.Join(Environment.NewLine, results.Errors.Cast<CompilerError>().Select(x => x.ToString())));

			return Path.GetFullPath(outputFilePath);
		}

		private static void EnsureDirectoryCreated(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}