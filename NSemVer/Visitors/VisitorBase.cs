namespace NSemVer.Visitors
{
	using NSemVer.Visitors.Context;

	public abstract class VisitorBase : IChangeVisitor
	{
		public virtual void Visit(AssemblyChanges assemblyChanges)
		{
			var context = new ModuleChangeContext(assemblyChanges);

			foreach (var moduleChange in assemblyChanges.ModuleChanges)
			{
				Visit(moduleChange, context);
			}
		}

		protected virtual void Visit(ModuleChange moduleChange, ModuleChangeContext moduleChangeContext)
		{
			var typeChangeContext = new TypeChangeContext(moduleChangeContext.AssemblyChanges, moduleChange);

			foreach (var typeChange in moduleChange.TypeChanges)
			{
				Visit(typeChange, typeChangeContext);
			}
		}

		protected virtual void Visit(TypeChange typeChange, TypeChangeContext typeChangeContext)
		{
			var methodGroupChangeContext = new MethodGroupChangeContext(
				typeChangeContext.AssemblyChanges,
				typeChangeContext.ParentModuleChange,
				typeChange);

			foreach (var methodGroupChange in typeChange.MethodGroupChanges)
			{
				Visit(methodGroupChange, methodGroupChangeContext);
			}
		}

		protected virtual void Visit(MethodGroupChange methodGroupChange, MethodGroupChangeContext methodGroupChangeContext)
		{
			var methodChangeContext = new MethodChangeContext(
				methodGroupChangeContext.AssemblyChanges,
				methodGroupChangeContext.ParentModuleChange,
				methodGroupChangeContext.ParentTypeChange,
				methodGroupChange);

			foreach (var methodChange in methodGroupChange.MethodChanges)
			{
				Visit(methodChange, methodChangeContext);
			}
		}

		protected virtual void Visit(MethodChange methodChange, MethodChangeContext methodChangeContext)
		{
			var context = new ParameterChangeContext(
				methodChangeContext.AssemblyChanges,
				methodChangeContext.ParentModuleChange,
				methodChangeContext.ParentTypeChange,
				methodChangeContext.ParentMethodGroupChange,
				methodChange);

			foreach (var parameterChange in methodChange.ParameterChanges)
			{
				Visit(parameterChange, context);
			}
		}

		protected virtual void Visit(ParameterChange parameterChange, ParameterChangeContext parameterChangeContext)
		{
		}
	}
}