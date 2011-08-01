namespace NSemVer.Tests.Visitors.BreakingChanges
{
	using System.Reflection;
	using NSemVer.Visitors.BreakingChanges;
	using NUnit.Framework;

	public class MethodTests
	{
		public class AddingInstanceMethodTests : BreakingChangeVisitorTestsBase
		{
			private readonly string _previousCode;
			private readonly string _currentCode;
			private readonly string _consumerCode;

			public AddingInstanceMethodTests()
			{
				_previousCode = @"
					namespace Api
					{
						<class-visibility> class Foo
						{
							public void Bar() { }
						}
					}";

				_currentCode = @"
					namespace Api
					{
						<class-visibility> class Foo
						{
							public void Bar() { }
							<method-visibility> void Baz() { }
						}
					}";

				_consumerCode = @"
					namespace ApiConsumer
					{
						using Api;

						public class Consumer
						{
							public void Example()
							{
								new Foo().Baz();
							}
						}

						public static class ConsumerExtensions
						{
							public static void Baz(this Foo foo)
							{
							}
						}
					}
				";
			}

			[Test]
			public void AddingPublicInstanceMethod()
			{
				string previousCode = _previousCode.Replace("<class-visibility>", "public");
				string currentCode = _currentCode.Replace("<class-visibility>", "public").Replace("<method-visibility>", "public");

				GivenContext(previousCode, currentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
					.When(ApiChangesDetermined)
					.And(BreakingChangeVisitorVisitsChanges)
					.Then(SingleBreakingChangeReasonIs, ApiBreakType.NewInstanceMethod)
					.And(SingleChangeMethodIs, "System.Void Api.Foo::Baz()")
					.And(ExampleNonBreakingButSemanticallyDifferentConsumerCodeIs, _consumerCode)
					.ExecuteWithReport();
			}

			[TestCase("internal", "public")]
			[TestCase("public", "internal")]
			[TestCase("public", "protected")]
			[TestCase("public", "private")]
			public void AddingNonPublicInstanceMethod(string classVisibility, string methodVisibility)
			{
				string previousCode = _previousCode.Replace("<class-visibility>", classVisibility);
				string currentCode = _currentCode.Replace("<class-visibility>", classVisibility).Replace("<method-visibility>", methodVisibility);

				GivenContext(previousCode, currentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
					.When(ApiChangesDetermined)
					.And(BreakingChangeVisitorVisitsChanges)
					.Then(NoBreakingChanges)
					.ExecuteWithReport();
			}
		}

		public class AddingMethodOverloadWithNewInterfaceParameterTests : BreakingChangeVisitorTestsBase
		{
			private readonly string _previousCode;
			private readonly string _currentCode;
			private readonly string _brokenConsumerCode;

			public AddingMethodOverloadWithNewInterfaceParameterTests()
			{
				_previousCode = @"
					namespace Api
					{
						using System.Collections;

						<class-visibility> class Foo
						{
							public void Bar(IEnumerable x) { }
						}
					}";

				_currentCode = @"
					namespace Api
					{
						using System;
						using System.Collections;

						<class-visibility> class Foo
						{
							public void Bar(IEnumerable x) { }
							<method-visibility> void Bar(ICloneable x) { }
						}
					}";

				_brokenConsumerCode = @"
					namespace ApiConsumer
					{
						using Api;

						public class Consumer
						{
							public void Example()
							{
								new Foo().Bar(new int[0]);
							}
						}
					}
				";
			}

			[Test]
			public void AddingPublicMethodOverloadWithInterfaceBasedParameter()
			{
				string previousCode = _previousCode.Replace("<class-visibility>", "public");
				string currentCode = _currentCode.Replace("<class-visibility>", "public").Replace("<method-visibility>", "public");

				GivenContext(previousCode, currentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
					.When(ApiChangesDetermined)
					.And(BreakingChangeVisitorVisitsChanges)
					.Then(SingleBreakingChangeReasonIs, ApiBreakType.MethodOverloadedWithInterfaceBasedParameter)
					.And(SingleChangeMethodIs, "System.Void Api.Foo::Bar(System.ICloneable)")
					.And(ExampleBrokenConsumerCodeIs, _brokenConsumerCode)
					.ExecuteWithReport();
			}

			[TestCase("internal", "public")]
			[TestCase("public", "internal")]
			[TestCase("public", "protected")]
			[TestCase("public", "private")]
			public void AddingNonPublicMethodOverloadWithInterfaceBasedParameter(string classVisibility, string methodVisibility)
			{
				string previousCode = _previousCode.Replace("<class-visibility>", classVisibility);
				string currentCode = _currentCode.Replace("<class-visibility>", classVisibility).Replace("<method-visibility>", methodVisibility);

				GivenContext(previousCode, currentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
					.When(ApiChangesDetermined)
					.And(BreakingChangeVisitorVisitsChanges)
					.Then(NoBreakingChanges)
					.ExecuteWithReport();
			}
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