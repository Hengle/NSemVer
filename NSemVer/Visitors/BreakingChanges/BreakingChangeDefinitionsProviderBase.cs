namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;

	public abstract class BreakingChangeDefinitionsProviderBase : IBreakingChangeDefinitionsProvider
	{
		private readonly IDictionary<ApiBreakType, Func<TypeChange, bool>> _breakingTypeChanges;
		private readonly IDictionary<ApiBreakType, Func<MethodGroupChange, bool>> _breakingMethodGroupChanges;
		private readonly IDictionary<ApiBreakType, Func<MethodChange, bool>> _breakingMethodChanges;
		private readonly IDictionary<ApiBreakType, Func<ParameterChange, bool>> _breakingParameterChanges;

		protected BreakingChangeDefinitionsProviderBase(
			IDictionary<ApiBreakType, Func<TypeChange, bool>> breakingTypeChanges,
			IDictionary<ApiBreakType, Func<MethodGroupChange, bool>> breakingMethodGroupChanges,
			IDictionary<ApiBreakType, Func<MethodChange, bool>> breakingMethodChanges,
			IDictionary<ApiBreakType, Func<ParameterChange, bool>> breakingParameterChanges)
		{
			_breakingTypeChanges = breakingTypeChanges;
			_breakingMethodGroupChanges = breakingMethodGroupChanges;
			_breakingMethodChanges = breakingMethodChanges;
			_breakingParameterChanges = breakingParameterChanges;
		}

		public IDictionary<ApiBreakType, Func<TypeChange, bool>> BreakingTypeChanges { get { return _breakingTypeChanges; } }

		public IDictionary<ApiBreakType, Func<MethodGroupChange, bool>> BreakingMethodGroupChanges { get { return _breakingMethodGroupChanges; } }

		public IDictionary<ApiBreakType, Func<MethodChange, bool>> BreakingMethodChanges { get { return _breakingMethodChanges; } }

		public IDictionary<ApiBreakType, Func<ParameterChange, bool>> BreakingParameterChanges { get { return _breakingParameterChanges; } }
	}
}