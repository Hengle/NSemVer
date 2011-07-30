namespace NSemVer.Tests
{
	using NUnit.Framework;

	public class ApiChanges_ClassTests : ApiChangeTestBase
	{
		[Test]
		public void PublicClassMovingNamespace()
		{
			const string PreviousCode = @"
				namespace Namespace1
				{
					public class Class1 { }
				}";

			const string CurrentCode = @"
				namespace Namespace2
				{
					public class Class1 { }
				}";

			GivenContext(PreviousCode, CurrentCode)
				.When(ApiChangesDetermined)
				.Then(HasTypeChange, ChangeType.Removed, "Namespace1.Class1")
				.And(HasTypeChange, ChangeType.Added, "Namespace2.Class1")
				.ExecuteWithReport();
		}
	}
}