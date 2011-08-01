namespace NSemVer.Visitors.BreakingChanges
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

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
					methodGroupChange => methodGroupChange.GetAllNewMethodChanges().Any(methodChange => methodChange.Method.IsPubliclyVisible())
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
}