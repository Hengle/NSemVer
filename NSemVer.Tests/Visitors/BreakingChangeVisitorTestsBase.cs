namespace NSemVer.Tests.Visitors
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

		protected void NoBreakingChanges()
		{
			Assert.AreEqual(0, _breakingChanges.Length);
		}

		protected void SingleBreakingChangeReasonIs(ApiBreakType breakType)
		{
			Assert.AreEqual(1, _breakingChanges.Length);
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
	}
}