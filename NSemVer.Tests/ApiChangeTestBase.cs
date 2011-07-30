namespace NSemVer.Tests
{
	using System.IO;
	using System.Linq;
	using NUnit.Framework;

	public abstract class ApiChangeTestBase : NSemVerTestBase
	{
		protected AssemblyChanges AssemblyChanges { get; private set; }

		protected void CheckingPublicApiChanges()
		{
			using (var assembly1Stream = File.Open(OldAssemblyPath, FileMode.Open))
			using (var assembly2Stream = File.Open(NewAssemblyPath, FileMode.Open))
			{
				var changeBuilder = new ChangeBuilder();

				AssemblyChanges = changeBuilder.GetChanges(assembly1Stream, assembly2Stream);
			}
		}

		protected void HasTypeChange(ChangeType changeType, string namespaceQualifiedTypeName)
		{
			var matchCount = AssemblyChanges.ModuleChanges
				.SelectMany(x => x.TypeChanges)
				.Count(x => x.ChangeType == changeType && x.Type.FullName == namespaceQualifiedTypeName);

			Assert.AreEqual(1, matchCount);
		}

		protected void HasParameterChange(ChangeType changeType, string parameterName)
		{
			var matchCount = AssemblyChanges.ModuleChanges
				.SelectMany(x => x.TypeChanges)
				.SelectMany(x => x.MethodChanges)
				.SelectMany(x => x.ParameterChanges)
				.Count(x => x.ChangeType == changeType && x.Parameter.Name == parameterName);

			Assert.AreEqual(1, matchCount);
		}
	}
}