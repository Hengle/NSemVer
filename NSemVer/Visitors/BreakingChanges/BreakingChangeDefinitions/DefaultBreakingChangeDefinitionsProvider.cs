namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class DefaultBreakingChangeDefinitionsProvider : BreakingChangeDefinitionsProviderBase
	{
		public DefaultBreakingChangeDefinitionsProvider()
			: base(CreateBreakingTypeChanges(), CreateBreakingMethodGroupChanges(), CreateBreakingMethodChanges(), CreateBreakingParameterChangeChanges())
		{
		}

		private static IDictionary<ApiBreakType, Func<TypeChange, bool>> CreateBreakingTypeChanges()
		{
			return new Dictionary<ApiBreakType, Func<TypeChange, bool>>
			{
				{ ApiBreakType.TypeRemoved, change => change.Type.IsPublic && change.ChangeType == ChangeType.Removed },
			};
		}

		private static IDictionary<ApiBreakType, Func<MethodGroupChange, bool>> CreateBreakingMethodGroupChanges()
		{
			return new Dictionary<ApiBreakType, Func<MethodGroupChange, bool>>
			{
				{
					ApiBreakType.NewInstanceMethod,
					methodGroupChange => methodGroupChange.GetAllNewMethodChanges().Any(methodChange => methodChange.Method.IsPubliclyVisible())
				},
				{
					ApiBreakType.MethodReturnTypeChanged,
					methodGroupChange =>
					{
						var allAddedOrUpdatedMethods = methodGroupChange.MethodChanges
							.Where(x => x.Method.IsPubliclyVisible() && x.ChangeType != ChangeType.Removed)
							.Select(x => x.Method)
							.ToArray();
						var removedMethods = methodGroupChange.MethodChanges
							.Where(x => x.Method.IsPubliclyVisible() && x.ChangeType == ChangeType.Removed)
							.Select(x => x.Method);
						return removedMethods.Any(removedMethod => allAddedOrUpdatedMethods
																	.FindOverridesByParameterTypes(removedMethod)
							                      					.Any(m => m.ReturnType.FullName != removedMethod.ReturnType.FullName));
					}
				},
				{
					ApiBreakType.MethodOverloadedWithInterfaceBasedParameter,
					methodGroupChange => methodGroupChange.ChangeType == ChangeType.Matched && // ChangeType.Matched => Same named method or methods existed in previous version
					                     methodGroupChange.MethodChanges.Any(methodChange => 
					                                                         methodChange.ChangeType == ChangeType.Added &&
					                                                         methodChange.Method.IsPubliclyVisible() &&
					                                                         methodChange.GetNewParameters(methodGroupChange).Any(x => x.ParameterType.Resolve().IsInterface)
					                     )
				},
			};
		}

		private static IDictionary<ApiBreakType, Func<MethodChange, bool>> CreateBreakingMethodChanges()
		{
			return new Dictionary<ApiBreakType, Func<MethodChange, bool>>
			{
			};
		}

		private static IDictionary<ApiBreakType, Func<ParameterChange, bool>> CreateBreakingParameterChangeChanges()
		{
			return new Dictionary<ApiBreakType, Func<ParameterChange, bool>>
			{
			};
		}
	}
}