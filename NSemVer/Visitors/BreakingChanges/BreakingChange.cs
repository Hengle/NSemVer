namespace NSemVer.Visitors.BreakingChanges
{
	public class BreakingChange
	{
		private readonly ApiBreakType _breakType;
		private readonly string _description;
		private readonly object _change;

		public BreakingChange(ApiBreakType breakType, string description, object change)
		{
			_breakType = breakType;
			_description = description;
			_change = change;
		}

		public ApiBreakType BreakType
		{
			get { return _breakType; }
		}

		public string Description
		{
			get { return _description; }
		}

		public object Change
		{
			get { return _change; }
		}
	}
}