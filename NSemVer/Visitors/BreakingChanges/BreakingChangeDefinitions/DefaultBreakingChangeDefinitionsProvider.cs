namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NSemVer.Visitors.Context;

	public class DefaultBreakingChangeDefinitionsProvider : BreakingChangeDefinitionsProviderBase
	{
		public DefaultBreakingChangeDefinitionsProvider()
			: base(CreateBreakingTypeChanges(), CreateBreakingMethodGroupChanges(), CreateBreakingMethodChanges(), CreateBreakingParameterChangeChanges())
		{
		}

		private static IDictionary<ApiBreakType, Func<TypeChange, TypeChangeContext, bool>> CreateBreakingTypeChanges()
		{
			return new Dictionary<ApiBreakType, Func<TypeChange, TypeChangeContext, bool>>
			{
				{
					ApiBreakType.TypeRemoved,
					(change, ctx) => change.Type.IsPublic && change.ChangeType == ChangeType.Removed
				},
			};
		}

		private static IDictionary<ApiBreakType, Func<MethodGroupChange, MethodGroupChangeContext, bool>> CreateBreakingMethodGroupChanges()
		{
			return new Dictionary<ApiBreakType, Func<MethodGroupChange, MethodGroupChangeContext, bool>>
			{
				{
					ApiBreakType.NewInstanceMethod,
					(methodGroupChange, ctx) =>
					{
						return
							ctx.ParentTypeChange.ChangeType != ChangeType.Added &&
							methodGroupChange.GetAllMethodChangesOfType(ChangeType.Added).Any(methodChange => methodChange.Method.IsPubliclyVisible());
					}
				},

				//{
				//    ApiBreakType.InstanceMethodRemoved,
				//    methodGroupChange =>
				//    {
				//        IEnumerable<MethodChange> removedMethods = methodGroupChange.GetAllMethodChangesOfType(ChangeType.Removed).ToArray();
				//        return removedMethods.Any(methodChange => methodChange.Method.IsPubliclyVisible());
				//    }
				//},

				{
					ApiBreakType.MethodReturnTypeChanged,
					(methodGroupChange, ctx) =>
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
					(methodGroupChange, ctx) =>
						methodGroupChange.ChangeType == ChangeType.Matched && // ChangeType.Matched => Same named method or methods existed in previous version
					    methodGroupChange.MethodChanges.Any(methodChange => 
					                                        methodChange.ChangeType == ChangeType.Added &&
					                                        methodChange.Method.IsPubliclyVisible() &&
					                                        methodChange.GetNewParameters(methodGroupChange).Any(x => x.ParameterType.Resolve().IsInterface)
					    )
				},
			};
		}

		private static IDictionary<ApiBreakType, Func<MethodChange, MethodChangeContext, bool>> CreateBreakingMethodChanges()
		{
			return new Dictionary<ApiBreakType, Func<MethodChange, MethodChangeContext, bool>>
			{
			};
		}

		private static IDictionary<ApiBreakType, Func<ParameterChange, ParameterChangeContext, bool>> CreateBreakingParameterChangeChanges()
		{
			return new Dictionary<ApiBreakType, Func<ParameterChange, ParameterChangeContext, bool>>
			{
			};
		}
	}
}