namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System.Collections.Generic;

	public interface IBreakingChangeDefinitions<in TChange, in TChangeContext>
	{
		IEnumerable<BreakingChange> GetBreakingChanges(TChange change, TChangeContext context);
	}
}