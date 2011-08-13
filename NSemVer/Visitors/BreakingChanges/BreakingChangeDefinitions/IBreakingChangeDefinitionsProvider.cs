namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System;
	using System.Collections.Generic;

	public interface IBreakingChangeDefinitionsProvider
	{
		IDictionary<ApiBreakType, Func<TypeChange, bool>> BreakingTypeChanges { get; }
		
		IDictionary<ApiBreakType, Func<MethodGroupChange, bool>> BreakingMethodGroupChanges { get; }

		IDictionary<ApiBreakType, Func<MethodChange, bool>> BreakingMethodChanges { get; }

		IDictionary<ApiBreakType, Func<ParameterChange, bool>> BreakingParameterChanges { get; }
	}
}