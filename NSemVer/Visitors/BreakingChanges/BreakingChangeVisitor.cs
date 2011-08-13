namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions;
	using NSemVer.Visitors.Context;

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

		protected override void Visit(TypeChange typeChange, TypeChangeContext typeChangeContext)
		{
			if (0 == DetermineBreakingChanges(_breakingChangeDefinitionsProvider.BreakingTypeChanges, typeChange, typeChangeContext) &&
				typeChange.ChangeType == ChangeType.Matched)
			{
				base.Visit(typeChange, typeChangeContext);
			}
		}

		protected override void Visit(MethodGroupChange methodGroupChange, MethodGroupChangeContext methodGroupChangeContext)
		{
			if (0 == DetermineBreakingChanges(_breakingChangeDefinitionsProvider.BreakingMethodGroupChanges, methodGroupChange, methodGroupChangeContext) &&
				methodGroupChange.ChangeType == ChangeType.Matched)
			{
				base.Visit(methodGroupChange, methodGroupChangeContext);
			}
		}

		protected override void Visit(MethodChange methodChange, MethodChangeContext methodChangeContext)
		{
			if (0 == DetermineBreakingChanges(_breakingChangeDefinitionsProvider.BreakingMethodChanges, methodChange, methodChangeContext) &&
				methodChange.ChangeType == ChangeType.Matched)
			{
				base.Visit(methodChange, methodChangeContext);
			}
		}

		protected override void Visit(ParameterChange parameterChange, ParameterChangeContext parameterChangeContext)
		{
			if (0 == DetermineBreakingChanges(_breakingChangeDefinitionsProvider.BreakingParameterChanges, parameterChange, parameterChangeContext) &&
				parameterChange.ChangeType == ChangeType.Matched)
			{
				base.Visit(parameterChange, parameterChangeContext);
			}
		}

		private int DetermineBreakingChanges<TChange, TChangeContext>(
			IEnumerable<KeyValuePair<ApiBreakType, Func<TChange, TChangeContext, bool>>> breakingTypeChangeFuncs,
			TChange change,
			TChangeContext context)
		{
			var breakingChanges = breakingTypeChangeFuncs
				.Where(breakingTypeChangeFunc => breakingTypeChangeFunc.Value(change, context))
				.Select(keyValuePair => new BreakingChangeResult(keyValuePair.Key, change))
				.ToList();

			_breakingChanges.AddRange(breakingChanges);

			return breakingChanges.Count;
		}
	}
}