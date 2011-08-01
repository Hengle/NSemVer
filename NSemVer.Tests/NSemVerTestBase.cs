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

		protected AssemblyChanges AssemblyChanges { get; private set; }

		protected void OldAssemblyWith(string source)
		{
			OldAssemblyPath = CompileAssemblyAssertNoErrors(Path.Combine(OldAssemblyDir, "TestAssembly.dll"), source);
		}

		protected void NewAssemblyWith(string source)
		{
			NewAssemblyPath = CompileAssemblyAssertNoErrors(Path.Combine(NewAssemblyDir, "TestAssembly.dll"), source);
		}

		protected string GenerateScenarioName(MethodBase method)
		{
			// TODO: Turn Pascal cased name into one with spaces
			return method.Name;
		}

		protected virtual Condition GivenContext(string previousCode, string currentCode, string scenarioName)
		{
			return Fixture
				.WithScenario(scenarioName)
				.Given(OldAssemblyWith, previousCode)
				.And(NewAssemblyWith, currentCode);
		}

		protected void ApiChangesDetermined()
		{
			using (var assembly1Stream = File.Open(OldAssemblyPath, FileMode.Open))
			using (var assembly2Stream = File.Open(NewAssemblyPath, FileMode.Open))
			{
				var changeBuilder = new ChangeBuilder();

				AssemblyChanges = changeBuilder.GetChanges(assembly1Stream, assembly2Stream);
			}
		}

		protected Tuple<CompilerResults, string> CompileAssembly(string outputAssemblyFileName, string source, params string[] referencedAssemblies)
		{
			string outputFilePath = GetOutputFilePath(outputAssemblyFileName);
			EnsureFileDeleted(outputFilePath);

			var parameters = new CompilerParameters { GenerateExecutable = false, OutputAssembly = outputAssemblyFileName };
			parameters.ReferencedAssemblies.Add("System.Core.dll");
			parameters.ReferencedAssemblies.AddRange(referencedAssemblies);

			var codeProvider = CodeDomProvider.CreateProvider("CSharp");
			var results = codeProvider.CompileAssemblyFromSource(parameters, source);

			return Tuple.Create(results, outputFilePath);
		}

		protected static string ConvertCompilerErrorMessagesToString(CompilerErrorCollection compilerErrorCollection)
		{
			return string.Join(Environment.NewLine, compilerErrorCollection.Cast<CompilerError>().Select(x => x.ToString()));
		}

		private static void EnsureFileDeleted(string outputFilePath)
		{
			if (File.Exists(outputFilePath))
			{
				File.Delete(outputFilePath);
			}
		}

		private static string GetOutputFilePath(string fileName)
		{
			var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
			return Path.Combine(baseDir, fileName);
		}

		private string CompileAssemblyAssertNoErrors(string fileName, string source)
		{
			Tuple<CompilerResults, string> results = CompileAssembly(fileName, source);

			Assert.AreEqual(0, results.Item1.Errors.Count, ConvertCompilerErrorMessagesToString(results.Item1.Errors));

			return Path.GetFullPath(results.Item2);
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