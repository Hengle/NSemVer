namespace NSemVer.Tests.Visitors.BreakingChanges
{
	using System.Linq;
	using NSemVer.Visitors;
	using NSemVer.Visitors.BreakingChanges;
	using NUnit.Framework;
	using StoryQ;

	public class BreakingChangeVisitorTestsBase : NSemVerTestBase
	{
		private BreakingChangeResult[] _breakingChanges;

		protected BreakingChangeVisitor Sut { get; set; }

		protected override Condition GivenContext(string previousCode, string currentCode, string scenarioName)
		{
			return base.GivenContext(previousCode, currentCode, scenarioName)
				.And(BreakingChangeVisitor);
		}

		protected void BreakingChangeVisitor()
		{
			Sut = new BreakingChangeVisitor();
		}

		protected void BreakingChangeVisitorVisitsChanges()
		{
			AssemblyChanges.Visit(Sut);

			_breakingChanges = Sut.BreakingChanges.ToArray();
		}

		protected void NoBreakingChanges()
		{
			Assert.AreEqual(0, _breakingChanges.Length);
		}

		protected void SingleBreakingChangeReasonIs(ApiBreakType breakType)
		{
			Assert.AreEqual(1, _breakingChanges.Length, string.Format("Did not find expected breaking change type '{0}'", breakType));
			Assert.AreEqual(breakType, _breakingChanges[0].BreakType);
		}

		protected void SingleChangeTypeIs(string namespaceQualifiedTypeName)
		{
			Assert.AreEqual(1, _breakingChanges.Length);
			var typeChange = (TypeChange)_breakingChanges[0].Change;

			Assert.AreEqual(namespaceQualifiedTypeName, typeChange.Type.FullName);
		}

		protected void SingleChangeMethodIs(string namespaceQualifiedMethodName)
		{
			var methodChange = (MethodChange)_breakingChanges.Single().Change;

			Assert.AreEqual(namespaceQualifiedMethodName, methodChange.Method.FullName);
		}

		protected void ExampleNonBreakingButSemanticallyDifferentConsumerCodeIs(string brokenConsumerCode)
		{
			AssertNoConsumerAssemblyCompilationErrors(brokenConsumerCode, OldAssemblyPath);
			AssertNoConsumerAssemblyCompilationErrors(brokenConsumerCode, NewAssemblyPath);
		}

		protected void ExampleBrokenConsumerCodeIs(string brokenConsumerCode)
		{
			AssertNoConsumerAssemblyCompilationErrors(brokenConsumerCode, OldAssemblyPath);
			AssertSomeConsumerAssemblyCompilationErrors(brokenConsumerCode, NewAssemblyPath);
		}

		private void AssertNoConsumerAssemblyCompilationErrors(string consumerCode, string apiAssemblyPath)
		{
			var results = CompileAssembly("ConsumerAssembly.dll", consumerCode, apiAssemblyPath);

			Assert.AreEqual(0, results.Item1.Errors.Count, ConvertCompilerErrorMessagesToString(results.Item1.Errors));
		}

		private void AssertSomeConsumerAssemblyCompilationErrors(string consumerCode, string apiAssemblyPath)
		{
			var results = CompileAssembly("ConsumerAssembly.dll", consumerCode, apiAssemblyPath);

			Assert.Greater(results.Item1.Errors.Count, 0);
		}
	}
}