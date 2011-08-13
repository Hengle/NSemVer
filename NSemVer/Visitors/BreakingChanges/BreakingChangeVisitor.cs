namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;
	using NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions;

	public class BreakingChangeVisitor : VisitorBase
	{
		private readonly IBreakingChangeDefinitionsProvider _breakingChangeDefinitionsProvider;
		private readonly List<BreakingChangeResult> _breakingChanges = new List<BreakingChangeResult>();

		public BreakingChangeVisitor()
			: this (new DefaultBreakingChangeDefinitionsProvider())
		{
		}

		public BreakingChangeVisitor(IBreakingChangeDefinitionsProvider breakingChangeDefinitionsProvider)
		{
			_breakingChangeDefinitionsProvider = breakingChangeDefinitionsProvider;
		}

		public IEnumerable<BreakingChangeResult> BreakingChanges
		{
			get { return _breakingChanges; }
		}

		public override void Visit(TypeChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingTypeChanges, change);
			base.Visit(change);
		}

		public override void Visit(MethodGroupChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingMethodGroupChanges, change);
			base.Visit(change);
		}

		public override void Visit(MethodChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingMethodChanges, change);
			base.Visit(change);
		}

		public override void Visit(ParameterChange change)
		{
			Visit(_breakingChangeDefinitionsProvider.BreakingParameterChanges, change);
			base.Visit(change);
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