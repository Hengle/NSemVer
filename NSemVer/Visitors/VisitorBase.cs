namespace NSemVer.Visitors
{
	using System.Linq;
	using NSemVer.Visitors.Context;

	public abstract class VisitorBase : IChangeVisitor
	{
		public virtual void Visit(AssemblyChanges assemblyChanges)
		{
			var context = new ModuleChangeContext(assemblyChanges);

			foreach (var moduleChange in assemblyChanges.ModuleChanges)
			{
				if (!Visit(moduleChange, context))
				{
					return;
				}
			}
		}

		protected virtual bool Visit(ModuleChange moduleChange, ModuleChangeContext moduleChangeContext)
		{
			var typeChangeContext = new TypeChangeContext(moduleChangeContext.AssemblyChanges, moduleChange);

			return moduleChange.TypeChanges.All(typeChange => Visit(typeChange, typeChangeContext));
		}

		protected virtual bool Visit(TypeChange typeChange, TypeChangeContext typeChangeContext)
		{
			var methodGroupChangeContext = new MethodGroupChangeContext(
				typeChangeContext.AssemblyChanges,
				typeChangeContext.ParentModuleChange,
				typeChange);

			return typeChange.MethodGroupChanges.All(methodGroupChange => Visit(methodGroupChange, methodGroupChangeContext));
		}

		protected virtual bool Visit(MethodGroupChange methodGroupChange, MethodGroupChangeContext methodGroupChangeContext)
		{
			var methodChangeContext = new MethodChangeContext(
				methodGroupChangeContext.AssemblyChanges,
				methodGroupChangeContext.ParentModuleChange,
				methodGroupChangeContext.ParentTypeChange,
				methodGroupChange);

			return methodGroupChange.MethodChanges.All(methodChange => Visit(methodChange, methodChangeContext));
		}

		protected virtual bool Visit(MethodChange methodChange, MethodChangeContext methodChangeContext)
		{
			var context = new ParameterChangeContext(
				methodChangeContext.AssemblyChanges,
				methodChangeContext.ParentModuleChange,
				methodChangeContext.ParentTypeChange,
				methodChangeContext.ParentMethodGroupChange,
				methodChange);

			return methodChange.ParameterChanges.All(parameterChange => Visit(parameterChange, context));
		}

		protected virtual bool Visit(ParameterChange parameterChange, ParameterChangeContext parameterChangeContext)
		{
			return true;
		}
	}
}