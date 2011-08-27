namespace NSemVer.Visitors.BreakingChanges
{
	using System.Collections.Generic;
	using System.Linq;
	using NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions;
	using NSemVer.Visitors.Context;

	public class BreakingChangeVisitor : VisitorBase
	{
		private readonly BreakingChangeDefinitionsProvider _breakingChangeDefinitionsProvider;
		private readonly List<BreakingChange> _breakingChanges = new List<BreakingChange>();

		public BreakingChangeVisitor()
			: this (new BreakingChangeDefinitionsProvider())
		{
		}

		public BreakingChangeVisitor(BreakingChangeDefinitionsProvider breakingChangeDefinitionsProvider)
		{
			_breakingChangeDefinitionsProvider = breakingChangeDefinitionsProvider;
		}

		public bool StopOnFirstBreakingChange { get; set; }

		public IEnumerable<BreakingChange> BreakingChanges
		{
			get { return _breakingChanges; }
		}

		protected override bool Visit(TypeChange typeChange, TypeChangeContext typeChangeContext)
		{
			NextAction nextAction = Visit(_breakingChangeDefinitionsProvider.BreakingTypeChanges, typeChange, typeChange.ChangeType, typeChangeContext);
			return nextAction == NextAction.VisitChildTypes
			       	? base.Visit(typeChange, typeChangeContext)
			       	: nextAction == NextAction.VisitNextSibling ? true : false;
		}

		protected override bool Visit(MethodGroupChange methodGroupChange, MethodGroupChangeContext methodGroupChangeContext)
		{
			NextAction nextAction = Visit(_breakingChangeDefinitionsProvider.BreakingMethodGroupChanges, methodGroupChange, methodGroupChange.ChangeType, methodGroupChangeContext);
			return nextAction == NextAction.VisitChildTypes
				? base.Visit(methodGroupChange, methodGroupChangeContext)
				: nextAction == NextAction.VisitNextSibling ? true : false;
		}

		protected override bool Visit(MethodChange methodChange, MethodChangeContext methodChangeContext)
		{
			NextAction nextAction = Visit(_breakingChangeDefinitionsProvider.BreakingMethodChanges, methodChange, methodChange.ChangeType, methodChangeContext);
			return nextAction == NextAction.VisitChildTypes
				? base.Visit(methodChange, methodChangeContext)
				: nextAction == NextAction.VisitNextSibling ? true : false;
		}

		protected override bool Visit(ParameterChange parameterChange, ParameterChangeContext parameterChangeContext)
		{
			NextAction nextAction = Visit(_breakingChangeDefinitionsProvider.BreakingParameterChanges, parameterChange, parameterChange.ChangeType, parameterChangeContext);
			return nextAction == NextAction.VisitChildTypes
				? base.Visit(parameterChange, parameterChangeContext)
				: nextAction == NextAction.VisitNextSibling ? true : false;
		}

		private NextAction Visit<TChange, TChangeContext>(
			IBreakingChangeDefinitions<TChange, TChangeContext> breakingChangeDefinitions,
			TChange typeChange,
			ChangeType changeType,
			TChangeContext typeChangeContext)
		{
			ChangesResult result = EvaluateAndStoreBreakingChanges(breakingChangeDefinitions, typeChange, typeChangeContext);

			if (result == ChangesResult.NoBreakingChanges && changeType == ChangeType.Matched)
			{
				return NextAction.VisitChildTypes;
			}

			return (StopOnFirstBreakingChange && result == ChangesResult.SomeBreakingChanges)
				? NextAction.Stop
				: NextAction.VisitNextSibling;
		}

		private ChangesResult EvaluateAndStoreBreakingChanges<TChange, TChangeContext>(
			IBreakingChangeDefinitions<TChange, TChangeContext> breakingChangeDefinitions,
			TChange change,
			TChangeContext changeContext)
		{
			var breakingChanges = breakingChangeDefinitions.GetBreakingChanges(change, changeContext);

			if (StopOnFirstBreakingChange)
			{
				var breakingChange = breakingChanges.FirstOrDefault();
				if (breakingChange != null)
				{
					_breakingChanges.Add(breakingChange);
					return ChangesResult.SomeBreakingChanges;
				}

				return ChangesResult.NoBreakingChanges;
			}

			_breakingChanges.AddRange(breakingChanges);
			return breakingChanges.Any() ? ChangesResult.SomeBreakingChanges : ChangesResult.NoBreakingChanges;
		}

		private enum ChangesResult
		{
			NoBreakingChanges,
			SomeBreakingChanges
		}

		private enum NextAction
		{
			VisitChildTypes,
			VisitNextSibling,
			Stop
		}
	}
}