namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;

	internal static class Extensions
	{
		public static bool IsPubliclyVisible(this MethodDefinition method)
		{
			return method.IsPublic && method.DeclaringType.IsPublic;
		}

		public static IEnumerable<MethodChange> GetAllNewMethodChanges(this MethodGroupChange methodGroupChange)
		{
			return methodGroupChange.ChangeType == ChangeType.Added
			       	? methodGroupChange.MethodChanges
			       	: methodGroupChange.ChangeType == ChangeType.Matched
			       	  	? methodGroupChange.MethodChanges.Where(x => x.ChangeType == ChangeType.Added)
			       	  	: new MethodChange[0];
		}

		public static IEnumerable<ParameterDefinition> GetNewParameters(this MethodChange methodChange, MethodGroupChange methodGroupChange)
		{
			Tuple<MethodDefinition, ParameterDefinition[]>[] newParametersByOriginalMethod = methodChange.Method.GetNewParametersByOriginalMethod(methodGroupChange);

			return newParametersByOriginalMethod
				.SelectMany(x => x.Item2)
				.Distinct(new ParameterDefinitionComparer());
		}

		public static Tuple<MethodDefinition, ParameterDefinition[]>[] GetNewParametersByOriginalMethod(this MethodDefinition method, MethodGroupChange methodGroupChange)
		{
			var existingMethods = methodGroupChange.MethodChanges
				.Where(x => x.ChangeType == ChangeType.Matched) // Only select methods that are still matched (I.e. that haven't been removed)
				.Select(x => x.Method);

			var methodParameterSet = new HashSet<ParameterDefinition>(method.Parameters);

			return existingMethods
				.Select(existingMethod => Tuple.Create(
					existingMethod,
					methodParameterSet.Except(new HashSet<ParameterDefinition>(existingMethod.Parameters), new ParameterDefinitionComparer()).ToArray()))
				.ToArray();
		}

		public static IEnumerable<MethodDefinition> FindOverridesByParameterTypes(this IEnumerable<MethodDefinition> methods, MethodDefinition candidateOverrideMethod)
		{
			Func<MethodDefinition, string> getParameterTypesAsString = method => String.Join(",", method.Parameters.Select(p => p.ParameterType.FullName));
			string candidateOverloadTypesString = getParameterTypesAsString(candidateOverrideMethod);
			return methods.Where(m => getParameterTypesAsString(m) == candidateOverloadTypesString);
		}

		private class ParameterDefinitionComparer : IEqualityComparer<ParameterDefinition>
		{
			public bool Equals(ParameterDefinition x, ParameterDefinition y)
			{
				return x.ParameterType.FullName == y.ParameterType.FullName;
			}

			public int GetHashCode(ParameterDefinition obj)
			{
				return obj.ParameterType.FullName.GetHashCode();
			}
		}
	}
}