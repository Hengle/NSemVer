namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions.Impl;
	using NSemVer.Visitors.Context;

	public class BreakingChangeDefinitionsProvider
	{
		private readonly IBreakingChangeDefinitions<TypeChange, TypeChangeContext> _breakingTypeChanges; 
		private readonly IBreakingChangeDefinitions<MethodGroupChange, MethodGroupChangeContext> _breakingMethodGroupChanges;
		private readonly IBreakingChangeDefinitions<MethodChange, MethodChangeContext> _breakingMethodChanges;
		private readonly IBreakingChangeDefinitions<ParameterChange, ParameterChangeContext> _breakingParameterChanges;

		public BreakingChangeDefinitionsProvider()
			: this(new BreakingTypeChanges(), new BreakingMethodGroupChanges(), new BreakingMethodChanges(), new BreakingParameterChanges())
		{
			
		}

		public BreakingChangeDefinitionsProvider(
			IBreakingChangeDefinitions<TypeChange, TypeChangeContext> breakingTypeChanges,
			IBreakingChangeDefinitions<MethodGroupChange, MethodGroupChangeContext> breakingMethodGroupChanges,
			IBreakingChangeDefinitions<MethodChange, MethodChangeContext> breakingMethodChanges,
			IBreakingChangeDefinitions<ParameterChange, ParameterChangeContext> breakingParameterChanges)
		{
			_breakingTypeChanges = breakingTypeChanges;
			_breakingMethodGroupChanges = breakingMethodGroupChanges;
			_breakingMethodChanges = breakingMethodChanges;
			_breakingParameterChanges = breakingParameterChanges;
		}

		public virtual IBreakingChangeDefinitions<TypeChange, TypeChangeContext> BreakingTypeChanges
		{
			get { return _breakingTypeChanges; }
		}

		public virtual IBreakingChangeDefinitions<MethodGroupChange, MethodGroupChangeContext> BreakingMethodGroupChanges
		{
			get { return _breakingMethodGroupChanges; }
		}

		public virtual IBreakingChangeDefinitions<MethodChange, MethodChangeContext> BreakingMethodChanges
		{
			get { return _breakingMethodChanges; }
		}

		public virtual IBreakingChangeDefinitions<ParameterChange, ParameterChangeContext> BreakingParameterChanges
		{
			get { return _breakingParameterChanges; }
		}
	}
}