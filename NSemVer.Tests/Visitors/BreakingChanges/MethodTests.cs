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
			private readonly string _semanticallyChangedConsumerCode;

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

				_semanticallyChangedConsumerCode = @"
					namespace ApiConsumer
					{
						using Api;

						public class Consumer
						{
							public void Example()
							{
								// Compiled against V1: will call ConsumerExtensions.Baz extension method
								// Compiled against V2: will call Foo.Baz method
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
					.Then(BreakingChangeCountIs, 1)
					.And(BreakingMethodChangeDetected, ApiBreakType.NewInstanceMethod, "System.Void Api.Foo::Baz()")
					.And(ExampleNonBreakingButSemanticallyDifferentConsumerCodeIs, _semanticallyChangedConsumerCode)
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

		public class AddingMethodOverloadWithNewInterfaceBasedParameterTests : BreakingChangeVisitorTestsBase
		{
			private readonly string _previousCode;
			private readonly string _currentCode;
			private readonly string _brokenConsumerCode;

			public AddingMethodOverloadWithNewInterfaceBasedParameterTests()
			{
				_previousCode = @"
					namespace Api
					{
						using System;

						<class-visibility> class Foo
						{
							public void Bar(IComparable x) { }
						}
					}";

				_currentCode = @"
					namespace Api
					{
						using System;

						<class-visibility> class Foo
						{
							public void Bar(IComparable x) { }
							<method-visibility> void Bar(IFormattable x) { }
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
								int i = 99;
								new Foo().Bar(i); // ambiguous call - which overload to use?
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
					.Then(BreakingChangeCountIs, 2)
					.And(BreakingMethodOverloadChangeDetected, ApiBreakType.NewInstanceMethod, "System.Void Api.Foo::Bar(System.IFormattable)")
					.And(BreakingMethodOverloadChangeDetected, ApiBreakType.MethodOverloadedWithInterfaceBasedParameter, "System.Void Api.Foo::Bar(System.IFormattable)")
					.And(ExampleBrokenConsumerCodeIs, _brokenConsumerCode, "error CS0121: The call is ambiguous")
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
		
		public class ChangingMethodSignatureTests : BreakingChangeVisitorTestsBase
		{
			private readonly string _previousCode;
			private readonly string _currentCode;
			private readonly string _brokenConsumerCode;

			public ChangingMethodSignatureTests()
			{
				_previousCode = @"
					namespace Api
					{
						using System;

						<class-visibility> class Foo
						{
							<method-visibility> void Bar(int i) { }
						}
					}";

				_currentCode = @"
					namespace Api
					{
						using System;

						<class-visibility> class Foo
						{
							<method-visibility> bool Bar(int i) { return default(bool); }
						}
					}";

				_brokenConsumerCode = @"
					namespace ApiConsumer
					{
						using Api;
						using System;

						public class Consumer
						{
							public void Example()
							{
								Action<int> test = new Foo().Bar;
							}
						}
					}
				";
			}

			[Test]
			public void ChangingPublicMethodReturnType()
			{
				string previousCode = _previousCode.Replace("<class-visibility>", "public").Replace("<method-visibility>", "public");
				string currentCode = _currentCode.Replace("<class-visibility>", "public").Replace("<method-visibility>", "public");

				GivenContext(previousCode, currentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
					.When(ApiChangesDetermined)
					.And(BreakingChangeVisitorVisitsChanges)
					.Then(BreakingChangeCountIs, 2)
					.And(BreakingMethodOverloadChangeDetected, ApiBreakType.MethodReturnTypeChanged, "System.Boolean Api.Foo::Bar(System.Int32)")
					.And(BreakingMethodOverloadChangeDetected, ApiBreakType.NewInstanceMethod, "System.Boolean Api.Foo::Bar(System.Int32)")
					.And(ExampleBrokenConsumerCodeIs, _brokenConsumerCode, "error CS0407: 'bool Api.Foo.Bar(int)' has the wrong return type")
					.ExecuteWithReport();
			}

			[TestCase("internal", "public")]
			[TestCase("public", "internal")]
			[TestCase("public", "protected")]
			[TestCase("public", "private")]
			public void ChangingNonPublicMethodReturnType(string classVisibility, string methodVisibility)
			{
				string previousCode = _previousCode.Replace("<class-visibility>", classVisibility).Replace("<method-visibility>", methodVisibility);
				string currentCode = _currentCode.Replace("<class-visibility>", classVisibility).Replace("<method-visibility>", methodVisibility);

				GivenContext(previousCode, currentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
					.When(ApiChangesDetermined)
					.And(BreakingChangeVisitorVisitsChanges)
					.Then(NoBreakingChanges)
					.ExecuteWithReport();
			}
		}
	}
}