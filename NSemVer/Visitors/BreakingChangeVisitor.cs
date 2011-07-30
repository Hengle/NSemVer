namespace NSemVer.Visitors
{
	using System.Collections.Generic;

	public enum BreakingChangeType
	{
		PublicTypeRemoved
	}

	public class BreakingChangeVisitor : IChangeVisitor
	{
		private readonly List<BreakingChangeResult> _breakingChanges = new List<BreakingChangeResult>();

		public IEnumerable<BreakingChangeResult> BreakingChanges
		{
			get { return _breakingChanges; }
		}

		public void Visit(AssemblyChanges change)
		{
			
		}

		public void Visit(ModuleChange change)
		{
			
		}

		public void Visit(TypeChange change)
		{
			if (change.ChangeType == ChangeType.Removed && change.Type.IsPublic)
			{
				_breakingChanges.Add(new BreakingChangeResult(BreakingChangeType.PublicTypeRemoved, change));
			}
		}

		public void Visit(MethodChange change)
		{
			
		}

		public void Visit(ParameterChange change)
		{
			
		}
	}
}