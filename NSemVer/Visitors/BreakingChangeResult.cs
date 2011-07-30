namespace NSemVer.Visitors
{
	public class BreakingChangeResult
	{
		private readonly BreakingChangeType _breakingChangeType;
		private readonly object _change;

		public BreakingChangeResult(BreakingChangeType breakingChangeType, object change)
		{
			_breakingChangeType = breakingChangeType;
			_change = change;
		}

		public BreakingChangeType BreakingChangeType
		{
			get { return _breakingChangeType; }
		}

		public object Change
		{
			get { return _change; }
		}
	}
}