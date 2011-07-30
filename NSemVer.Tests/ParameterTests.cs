namespace NSemVer.Tests
{
	using NUnit.Framework;

	public class ParameterTests : ApiChangeTestBase
	{
		[Test]
		public void ParameterRemovedTest()
		{
			const string PreviousCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1(int param1) { }
					}
				}";

			const string CurrentCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1() { }
					}
				}";

			GivenContext(PreviousCode, CurrentCode)
				.When(CheckingPublicApiChanges)
				.Then(HasParameterChange, ChangeType.Removed, "param1")
				.ExecuteWithReport();
		}

		[Test]
		public void ParameterAddedTest()
		{
			const string PreviousCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1() { }
					}
				}";

			const string CurrentCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1(int param1) { }
					}
				}";

			GivenContext(PreviousCode, CurrentCode)
				.When(CheckingPublicApiChanges)
				.Then(HasParameterChange, ChangeType.Added, "param1")
				.ExecuteWithReport();
		}

		[Test]
		public void MultipleParametersAddedTest()
		{
			const string PreviousCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1() { }
					}
				}";

			const string CurrentCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1(int param1, string param2) { }
					}
				}";

			GivenContext(PreviousCode, CurrentCode)
				.When(CheckingPublicApiChanges)
				.And(HasParameterChange, ChangeType.Added, "param1")
				.And(HasParameterChange, ChangeType.Added, "param2")
				.ExecuteWithReport();
		}

		[Test]
		public void ParameterRenamedTest()
		{
			const string PreviousCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1(int param1) { }
					}
				}";

			const string CurrentCode = @"
				namespace Tests
				{
					public class Class1
					{
						public void Method1(int param2) { }
					}
				}";

			GivenContext(PreviousCode, CurrentCode)
				.When(CheckingPublicApiChanges)
				.Then(HasParameterChange, ChangeType.Removed, "param1")
				.And(HasParameterChange, ChangeType.Added, "param2")
				.ExecuteWithReport();
		}
	}
}