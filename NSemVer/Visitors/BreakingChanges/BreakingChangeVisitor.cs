namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;

	public class BreakingChangeVisitor : IChangeVisitor
	{
		private readonly IDictionary<ApiBreakType, Func<TypeChange, bool>> _breakingTypeChangeFuncs = CreateBreakingTypeChanges();
		private readonly IDictionary<ApiBreakType, Func<MethodGroupChange, bool>> _breakingMethodGroupChangeFuncs = CreateBreakingMethodGroupChanges();
		private readonly IDictionary<ApiBreakType, Func<MethodChange, bool>> _breakingMethodChangeFuncs = CreateBreakingMethodChanges();
		private readonly IDictionary<ApiBreakType, Func<ParameterChange, bool>> _breakingParameterChangeFuncs = CreateBreakingParameterChangeChanges();

		private readonly List<BreakingChangeResult> _breakingChanges = new List<BreakingChangeResult>();
		
		public IEnumerable<BreakingChangeResult> BreakingChanges
		{
			get { return _breakingChanges; }
		}

		public void Visit(AssemblyChanges change)
		{	
		}

		public void Visit(ModuleChange change)
		{	
		}

		public void Visit(TypeChange change)
		{
			Visit(_breakingTypeChangeFuncs, change);
		}

		public void Visit(MethodGroupChange change)
		{
			Visit(_breakingMethodGroupChangeFuncs, change);
		}

		public void Visit(MethodChange change)
		{
			Visit(_breakingMethodChangeFuncs, change);
		}

		public void Visit(ParameterChange change)
		{
			Visit(_breakingParameterChangeFuncs, change);
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
					methodGroupChange => methodGroupChange.GetAllNewMethodChanges().Any(methodChange => methodChange.IsPubliclyVisible())
				},

				{
				    ApiBreakType.MethodOverloadedWithInterfaceBasedParameter,
				    methodGroupChange => methodGroupChange.ChangeType == ChangeType.Matched && 
				                         methodGroupChange.MethodChanges.Any(methodChange => 
											methodChange.ChangeType == ChangeType.Added &&
				                            methodChange.IsPubliclyVisible() &&
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

		private void Visit<TChange>(IEnumerable<KeyValuePair<ApiBreakType, Func<TChange, bool>>> breakingTypeChangeFuncs, TChange change)
		{
			foreach (var breakingTypeChangeFunc in breakingTypeChangeFuncs)
			{
				if (breakingTypeChangeFunc.Value(change))
				{
					_breakingChanges.Add(new BreakingChangeResult(breakingTypeChangeFunc.Key, change));
				}
			}
		}
	}

	// I wish there was a way to declare private extension classes :(
	public static class Extensions
	{
		public static bool IsPubliclyVisible(this MethodChange change)
		{
			return change.Method.IsPublic && change.Method.DeclaringType.IsPublic;
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

		/*
		public static bool OverloadedParametersContains(this MethodChange methodChange, Func<ParameterReference, bool> predicate)
		{

			var newParameters = methodChange.Method.Parameters

			//return methodChange.Method.DeclaringType.Methods
			//    .Where(x => x.IsPublic)
			//    .Any(x => x.Name == methodChange.Method.Parameters.First().ParameterType.);
		}*/
	}
}