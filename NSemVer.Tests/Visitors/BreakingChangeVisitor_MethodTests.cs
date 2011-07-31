namespace NSemVer.Tests.Visitors
{
	using NSemVer.Visitors.BreakingChanges;
	using NUnit.Framework;

	public class BreakingChangeVisitor_MethodTests : BreakingChangeVisitorTestsBase
	{
		[Test]
		public void AddingPublicInstanceMethod()
		{
			const string PreviousCode = @"
				namespace Tests
				{
					using System;
					using System.Collections;

					public class Foo
					{
						public void Bar() { }
					}
				}";

			const string CurrentCode = @"
				namespace Tests
				{
					using System;
					using System.Collections;

					public class Foo
					{
						public void Bar() { }
						public void Baz() { }
					}
				}";

			GivenContext(PreviousCode, CurrentCode)
				.When(ApiChangesDetermined)
				.And(BreakingChangeVisitorVisitsChanges)
				.Then(SingleBreakingChangeReasonIs, ApiBreakType.NewInstanceMethod)
				.And(SingleChangeMethodIs, "System.Void Tests.Foo::Baz()")
				.ExecuteWithReport();
		}

		[TestCase("internal", "public")]
		[TestCase("public", "internal")]
		[TestCase("public", "protected")]
		[TestCase("public", "private")]
		public void AddingNonPublicInstanceMethod(string classVisibility, string methodVisibility)
		{
			string PreviousCode = @"
				namespace Tests
				{
					using System;
					using System.Collections;

					<class-visibility> class Foo
					{
						public void Bar() { }
					}
				}".Replace("<class-visibility>", classVisibility);

			string CurrentCode = @"
				namespace Tests
				{
					using System;
					using System.Collections;

					<class-visibility> class Foo
					{
						public void Bar() { }
						<method-visibility> void Baz() { }
					}
				}".Replace("<class-visibility>", classVisibility).Replace("<method-visibility>", methodVisibility);

			GivenContext(PreviousCode, CurrentCode)
				.When(ApiChangesDetermined)
				.And(BreakingChangeVisitorVisitsChanges)
				.Then(NoBreakingChanges);
		}

		/*
		[TestCase("public", ChangeType.Breaking)]
		[TestCase("internal", ChangeType.NonBreaking)]
		public void AddingMethodOverload(string visibility, ChangeType changeType)
		{
			string PreviousCode = @"
				namespace Tests
				{
					using System;
					using System.Collections;

					<visibility> class Foo
					{
						public void Bar(IEnumerable x) { }
					}
				}"
				.Replace("<visibility>", visibility);

			string CurrentCode = @"
				namespace Tests
				{
					using System;
					using System.Collections;

					<visibility> class Foo
					{
						public void Bar(IEnumerable x) { }
						public void Bar(ICloneable x) { }
					}
				}"
				.Replace("<visibility>", visibility);

			Fixture
				.WithScenario(GenerateScenarioName(MethodBase.GetCurrentMethod()))
				.Given(OldAssemblyWith, PreviousCode)
				.And(NewAssemblyWith, CurrentCode)
				.When(CheckingForPublicApiChanges)
				.Then(Considered, changeType)
				.ExecuteWithReport();
		}

		 * */
	}
}