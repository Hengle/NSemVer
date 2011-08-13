namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System;
	using System.Collections.Generic;
	using NSemVer.Visitors.Context;

	public abstract class BreakingChangeDefinitionsProviderBase : IBreakingChangeDefinitionsProvider
	{
		private readonly IDictionary<ApiBreakType, Func<TypeChange, TypeChangeContext, bool>> _breakingTypeChanges;
		private readonly IDictionary<ApiBreakType, Func<MethodGroupChange, MethodGroupChangeContext, bool>> _breakingMethodGroupChanges;
		private readonly IDictionary<ApiBreakType, Func<MethodChange, MethodChangeContext, bool>> _breakingMethodChanges;
		private readonly IDictionary<ApiBreakType, Func<ParameterChange, ParameterChangeContext, bool>> _breakingParameterChanges;

		protected BreakingChangeDefinitionsProviderBase(
			IDictionary<ApiBreakType, Func<TypeChange, TypeChangeContext, bool>> breakingTypeChanges,
			IDictionary<ApiBreakType, Func<MethodGroupChange, MethodGroupChangeContext, bool>> breakingMethodGroupChanges,
			IDictionary<ApiBreakType, Func<MethodChange, MethodChangeContext, bool>> breakingMethodChanges,
			IDictionary<ApiBreakType, Func<ParameterChange, ParameterChangeContext, bool>> breakingParameterChanges)
		{
			_breakingTypeChanges = breakingTypeChanges;
			_breakingMethodGroupChanges = breakingMethodGroupChanges;
			_breakingMethodChanges = breakingMethodChanges;
			_breakingParameterChanges = breakingParameterChanges;
		}

		public IDictionary<ApiBreakType, Func<TypeChange, TypeChangeContext, bool>> BreakingTypeChanges { get { return _breakingTypeChanges; } }

		public IDictionary<ApiBreakType, Func<MethodGroupChange, MethodGroupChangeContext, bool>> BreakingMethodGroupChanges { get { return _breakingMethodGroupChanges; } }

		public IDictionary<ApiBreakType, Func<MethodChange, MethodChangeContext, bool>> BreakingMethodChanges { get { return _breakingMethodChanges; } }

		public IDictionary<ApiBreakType, Func<ParameterChange, ParameterChangeContext, bool>> BreakingParameterChanges { get { return _breakingParameterChanges; } }
	}
}