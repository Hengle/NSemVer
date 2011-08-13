namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System;
	using System.Collections.Generic;
	using NSemVer.Visitors.Context;

	public interface IBreakingChangeDefinitionsProvider
	{
		IDictionary<ApiBreakType, Func<TypeChange, TypeChangeContext, bool>> BreakingTypeChanges { get; }
		
		IDictionary<ApiBreakType, Func<MethodGroupChange, MethodGroupChangeContext, bool>> BreakingMethodGroupChanges { get; }

		IDictionary<ApiBreakType, Func<MethodChange, MethodChangeContext, bool>> BreakingMethodChanges { get; }

		IDictionary<ApiBreakType, Func<ParameterChange, ParameterChangeContext, bool>> BreakingParameterChanges { get; }
	}
}