namespace NSemVer.Tests.Visitors
{
	using System.Linq;
	using NSemVer.Visitors;
	using NUnit.Framework;
	using StoryQ;

	public class BreakingChangeVisitorTestsBase : NSemVerTestBase
	{
		private BreakingChangeResult[] _breakingChanges;

		protected BreakingChangeVisitor Sut { get; set; }

		protected override Condition GivenContext(string previousCode, string currentCode)
		{
			return base.GivenContext(previousCode, currentCode)
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

		protected void SingleBreakingChangeReasonIs(BreakingChangeType breakingChangeType)
		{
			Assert.AreEqual(breakingChangeType, _breakingChanges.Single().BreakingChangeType);
		}

		protected void SingleChangeTypeIs(string namespaceQualifiedTypeName)
		{
			var typeChange = (TypeChange)_breakingChanges.Single().Change;

			Assert.AreEqual(namespaceQualifiedTypeName, typeChange.Type.FullName);
		}
	}
}