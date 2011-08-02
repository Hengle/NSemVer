namespace NSemVer.Tests.Visitors.BreakingChanges
{
	using System.CodeDom.Compiler;
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
			BreakingChangeVisitor(new DefaultBreakingChangeDefinitionsProvider());
		}

		protected void BreakingChangeVisitor(IBreakingChangeDefinitionsProvider breakingChangeDefinitionsProvider)
		{
			Sut = new BreakingChangeVisitor(breakingChangeDefinitionsProvider);
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

		protected void BreakingChangeCountIs(int expectedCount)
		{
			Assert.AreEqual(expectedCount, _breakingChanges.Length);
		}

		protected void SingleBreakingChangeReasonIs(ApiBreakType breakType)
		{
			Assert.AreEqual(1, _breakingChanges.Length, string.Format("Expected breaking change type of {0}, got [{1}]", breakType, string.Join(",", _breakingChanges.Select(x => x.BreakType))));
			Assert.AreEqual(breakType, _breakingChanges[0].BreakType);
		}

		protected void SingleChangedTypeIs(string namespaceQualifiedTypeName)
		{
			Assert.AreEqual(1, _breakingChanges.Length);
			var typeChange = (TypeChange)_breakingChanges[0].Change;

			Assert.AreEqual(namespaceQualifiedTypeName, typeChange.Type.FullName);
		}

		protected void SingleChangedMethodIs(string namespaceQualifiedMethodName)
		{
			object change = _breakingChanges.Single().Change;

			if (change is MethodChange)
			{
				Assert.AreEqual(namespaceQualifiedMethodName, ((MethodChange)change).Method.FullName);
			}
			else
			{
				var methodGroupChange = (MethodGroupChange)change;
				Assert.AreEqual(namespaceQualifiedMethodName, methodGroupChange.MethodChanges.Single().Method.FullName);
			}
		}

		protected void BreakingMethodChangeDetected(ApiBreakType expectedBreakType, string namespaceQualifiedMethodName)
		{
			var containsChange = _breakingChanges
				.Where(x => x.BreakType == expectedBreakType && x.Change is MethodGroupChange)
				.Select(x => ((MethodGroupChange)x.Change))
				.Where(x => x.ChangeType == ChangeType.Added) // Only interested in new methods, not overloads
				.SelectMany(x => x.MethodChanges)
				.Any(x => x.Method.FullName == namespaceQualifiedMethodName);

			Assert.True(containsChange, string.Format("Did not find expected breaking method change type '{0}' for method '{1}'", expectedBreakType, namespaceQualifiedMethodName));
		}

		protected void BreakingMethodOverloadChangeDetected(ApiBreakType expectedBreakType, string namespaceQualifiedMethodName)
		{
			bool containsChange = _breakingChanges.Any(
				change => change.BreakType == expectedBreakType &&
						  change.Change is MethodGroupChange ? ((MethodGroupChange)change.Change).MethodChanges.Any(x => x.Method.FullName == namespaceQualifiedMethodName) : false);

			Assert.True(containsChange, string.Format("Did not find expected breaking method overload change type '{0}' for method '{1}'", expectedBreakType, namespaceQualifiedMethodName));
		}

		protected void ExampleNonBreakingButSemanticallyDifferentConsumerCodeIs(string brokenConsumerCode)
		{
			AssertNoConsumerAssemblyCompilationErrors(brokenConsumerCode, OldAssemblyPath);
			AssertNoConsumerAssemblyCompilationErrors(brokenConsumerCode, NewAssemblyPath);
		}

		protected void ExampleBrokenConsumerCodeIs(string brokenConsumerCode, string expectedErrorSubstring)
		{
			AssertNoConsumerAssemblyCompilationErrors(brokenConsumerCode, OldAssemblyPath);
			AssertSomeConsumerAssemblyCompilationErrors(brokenConsumerCode, NewAssemblyPath, expectedErrorSubstring);
		}

		private void AssertNoConsumerAssemblyCompilationErrors(string consumerCode, string apiAssemblyPath)
		{
			var results = CompileAssembly("ConsumerAssembly.dll", consumerCode, apiAssemblyPath);

			Assert.AreEqual(0, results.Item1.Errors.Count, ConvertCompilerErrorMessagesToString(results.Item1.Errors));
		}

		private void AssertSomeConsumerAssemblyCompilationErrors(string consumerCode, string apiAssemblyPath, string expectedErrorSubstring)
		{
			var results = CompileAssembly("ConsumerAssembly.dll", consumerCode, apiAssemblyPath);

			CompilerErrorCollection errors = results.Item1.Errors;
			Assert.Greater(errors.Count, 0);

			var compilerErrors = errors.Cast<CompilerError>().ToArray();
			Assert.True(
				compilerErrors.Any(x => x.ToString().Contains(expectedErrorSubstring)),
				string.Format("Could not find expected error text substring '{0}' in errors collection: {1}", expectedErrorSubstring, ConvertCompilerErrorMessagesToString(errors)));
		}
	}
}