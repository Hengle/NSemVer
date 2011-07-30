namespace NSemVer.Tests
{
	using System.Linq;
	using NUnit.Framework;

	public abstract class ApiChangeTestBase : NSemVerTestBase
	{
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