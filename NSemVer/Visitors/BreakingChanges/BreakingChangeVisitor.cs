namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;

	public class BreakingChangeVisitor : IChangeVisitor
	{
		private readonly IBreakingChangeDefinitionsProvider _breakingChangeDefinitionsProvider;
		private readonly List<BreakingChangeResult> _breakingChanges = new List<BreakingChangeResult>();

		public BreakingChangeVisitor(IBreakingChangeDefinitionsProvider breakingChangeDefinitionsProvider)
		{
			_breakingChangeDefinitionsProvider = breakingChangeDefinitionsProvider;
		}

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
			Visit(_breakingChangeDefinitionsProvider.BreakingTypeChanges, change);
		}

		public void Visit(MethodGroupChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingMethodGroupChanges, change);
		}

		public void Visit(MethodChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingMethodChanges, change);
		}

		public void Visit(ParameterChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingParameterChanges, change);
		}

		private void Visit<TChange>(IEnumerable<KeyValuePair<ApiBreakType, Func<TChange, bool>>> breakingTypeChangeFuncs, TChange change)
		{
			foreach (var breakingTypeChangeFunc in breakingTypeChangeFuncs)
			{
				if (breakingTypeChangeFunc.Value(change))
				{
					_breakingChanges.Add(new BreakingChangeResult(breakingTypeChangeFunc.Key, change));
				}
			}
		}
	}
}