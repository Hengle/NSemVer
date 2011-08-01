namespace NSemVer.Tests.Visitors.BreakingChanges
{
	using System.Reflection;
	using NSemVer.Visitors.BreakingChanges;
	using NUnit.Framework;

	public class ClassTests : BreakingChangeVisitorTestsBase
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

			GivenContext(PreviousCode, CurrentCode, GenerateScenarioName(MethodBase.GetCurrentMethod()))
				.When(ApiChangesDetermined)
				.And(BreakingChangeVisitorVisitsChanges)
				.Then(SingleBreakingChangeReasonIs, ApiBreakType.TypeRemoved)
				.And(SingleChangedTypeIs, "Namespace1.Class1")
				.ExecuteWithReport();
		}
	}
}