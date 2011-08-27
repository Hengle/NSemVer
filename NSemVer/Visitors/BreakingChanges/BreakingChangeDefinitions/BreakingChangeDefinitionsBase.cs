namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public abstract class BreakingChangeDefinitionsBase<TChange, TChangeContext> : IBreakingChangeDefinitions<TChange, TChangeContext>
	{
		private readonly List<Func<TChange, TChangeContext, IEnumerable<BreakingChange>>> _funcs = new List<Func<TChange, TChangeContext, IEnumerable<BreakingChange>>>();

		public IEnumerable<BreakingChange> GetBreakingChanges(TChange change, TChangeContext context)
		{
			return _funcs.SelectMany(func => func(change, context));
		}

		protected void Add<TBreakingChangeItem>(
			ApiBreakType breakType,
			Func<TChange, TChangeContext, TBreakingChangeItem> breakingItemsSelector,
			Func<TBreakingChangeItem, TChangeContext, string> descriptionGenerator)
		{
			_funcs.Add((change, ctx) =>
			{
				TBreakingChangeItem breakingChangeItem = breakingItemsSelector(change, ctx);
				return ReferenceEquals(breakingChangeItem, null)
					? new BreakingChange[0]
					: new[] { new BreakingChange(breakType, descriptionGenerator(breakingChangeItem, ctx), change) };
			});
		}

		protected void Add<TBreakingChangeItem>(
			ApiBreakType breakType,
			Func<TChange, TChangeContext, IEnumerable<TBreakingChangeItem>> breakingItemsSelector,
			Func<TBreakingChangeItem, TChangeContext, string> descriptionGenerator)
		{
			_funcs.Add((change, ctx) =>
			{
				var breakingChanges = breakingItemsSelector(change, ctx)
					.Select(item => new BreakingChange(breakType, descriptionGenerator(item, ctx), change));

				return breakingChanges;
			});
		}
	}
}