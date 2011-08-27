namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions.Impl
{
	using System.Linq;
	using Mono.Cecil;
	using NSemVer.Visitors.Context;

	public class BreakingMethodGroupChanges : BreakingChangeDefinitionsBase<MethodGroupChange, MethodGroupChangeContext>
	{
		public BreakingMethodGroupChanges()
		{
			Add(ApiBreakType.InstanceMethodAdded,
				(methodGroupChange, ctx) => methodGroupChange
				                            	.GetAllMethodChangesOfType(ChangeType.Added)
				                            	.Select(methodChange => methodChange.Method)
				                            	.Where(method => method.IsPubliclyVisible()),
				(publicMethodAdded, ctx) => "Added public method '{0}'".FormatInvariant(publicMethodAdded.FullName));

			Add(ApiBreakType.InstanceMethodRemoved,
				(methodGroupChange, ctx) => methodGroupChange
				                            	.GetAllMethodChangesOfType(ChangeType.Removed)
				                            	.Select(methodChange => methodChange.Method)
				                            	.Where(method => method.IsPubliclyVisible()),
				(publicMethodRemoved, ctx) => "Removed public method '{0}'".FormatInvariant(publicMethodRemoved.FullName));

			Add(ApiBreakType.MethodReturnTypeChanged,
				(methodGroupChange, ctx) =>
				{
					var allAddedOrUpdatedMethods = methodGroupChange.MethodChanges
						.Where(x => x.Method.IsPubliclyVisible() && x.ChangeType != ChangeType.Removed)
						.Select(x => x.Method)
						.ToArray();

					var removedMethods = methodGroupChange.MethodChanges
						.Where(x => x.Method.IsPubliclyVisible() && x.ChangeType == ChangeType.Removed)
						.Select(x => x.Method);

					return removedMethods
						.SelectMany(removedMethod => allAddedOrUpdatedMethods
						                             	.FindOverridesByParameterTypes(removedMethod)
						                             	.Where(m => m.ReturnType.FullName != removedMethod.ReturnType.FullName)
						                             	.Select(newMethodVersion => new { OldMethod = removedMethod, NewMethod = newMethodVersion }));
				},
				(changedMethodPair, ctx) => "Return type of public method '{0}' changed from '{1}' to '{2}'"
				                            .FormatInvariant(changedMethodPair.NewMethod.FullName, changedMethodPair.OldMethod.ReturnType.FullName, changedMethodPair.NewMethod.ReturnType.FullName));

			Add(ApiBreakType.MethodOverloadedWithInterfaceBasedParameter,
				(methodGroupChange, ctx) =>
				{
					if (methodGroupChange.ChangeType != ChangeType.Matched)
						return new MethodDefinition[0];

					return methodGroupChange.MethodChanges
						.Where(methodChange => methodChange.ChangeType == ChangeType.Added &&
						                       methodChange.Method.IsPubliclyVisible() &&
						                       methodChange.GetNewParameters(methodGroupChange).Any(x => x.ParameterType.Resolve().IsInterface))
						.Select(methodChange => methodChange.Method);
				},
				(newMethodOverload, ctx) => "Added new public method '{0}' which overloads another public method by adding at least 1 interface-based parameter"
											.FormatInvariant(newMethodOverload.FullName));
		}
	}
}