namespace NSemVer.Visitors
{
	using System.Collections.Generic;

	public static class VisitorExtensions
	{
		public static void Visit(this AssemblyChanges assemblyChanges, IChangeVisitor visitor)
		{
			visitor.Visit(assemblyChanges);
		}

		/*
		public static void Visit(this IEnumerable<ModuleChange> moduleChanges, IChangeVisitor visitor)
		{
			foreach (var moduleChange in moduleChanges)
			{
				visitor.Visit(moduleChange);

				if (moduleChange.ChangeType == ChangeType.Matched)
				{
					moduleChange.TypeChanges.Visit(visitor);
				}
			}
		}

		public static void Visit(this IEnumerable<TypeChange> typeChanges, IChangeVisitor visitor)
		{
			foreach (var typeChange in typeChanges)
			{
				visitor.Visit(typeChange);

				if (typeChange.ChangeType == ChangeType.Matched)
				{
					typeChange.MethodGroupChanges.Visit(visitor);
				}
			}
		}

		public static void Visit(this IEnumerable<MethodGroupChange> methodGroupChanges, IChangeVisitor visitor)
		{
			foreach (var methodGroupChange in methodGroupChanges)
			{
				visitor.Visit(methodGroupChange);

				if (methodGroupChange.ChangeType == ChangeType.Matched)
				{
					methodGroupChange.MethodChanges.Visit(visitor);
				}
			}
		}

		public static void Visit(this IEnumerable<MethodChange> methodChanges, IChangeVisitor visitor)
		{
			foreach (var methodChange in methodChanges)
			{
				visitor.Visit(methodChange);

				if (methodChange.ChangeType == ChangeType.Matched)
				{
					methodChange.ParameterChanges.Visit(visitor);
				}
			}
		}

		public static void Visit(this IEnumerable<ParameterChange> parameterChanges, IChangeVisitor visitor)
		{
			foreach (var parameterChange in parameterChanges)
			{
				visitor.Visit(parameterChange);
			}
		}*/
	}
}