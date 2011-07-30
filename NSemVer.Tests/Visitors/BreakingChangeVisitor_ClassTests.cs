namespace NSemVer.Tests.Visitors
{
	using NSemVer.Visitors;
	using NUnit.Framework;

	public class BreakingChangeVisitor_ClassTests : BreakingChangeVisitorTestsBase
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
				.And(BreakingChangeVisitorVisitsChanges)
				.Then(SingleBreakingChangeReasonIs, BreakingChangeType.PublicTypeRemoved)
				.And(SingleChangeTypeIs, "Namespace1.Class1")
				.ExecuteWithReport();
		}
	}
}