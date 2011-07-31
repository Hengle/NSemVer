namespace NSemVer.Visitors.BreakingChanges
{
	public class BreakingChangeResult
	{
		private readonly ApiBreakType _breakType;
		private readonly object _change;

		public BreakingChangeResult(ApiBreakType breakType, object change)
		{
			_breakType = breakType;
			_change = change;
		}

		public ApiBreakType BreakType
		{
			get { return _breakType; }
		}

		public object Change
		{
			get { return _change; }
		}
	}
}