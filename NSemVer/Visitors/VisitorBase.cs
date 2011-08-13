namespace NSemVer.Visitors
{
	public abstract class VisitorBase : IChangeVisitor
	{
		public virtual void Visit(AssemblyChanges change)
		{
			foreach (var moduleChange in change.ModuleChanges)
			{
				Visit(moduleChange);
			}
		}

		public virtual void Visit(ModuleChange change)
		{
			foreach (var typeChange in change.TypeChanges)
			{
				Visit(typeChange);
			}
		}

		public virtual void Visit(TypeChange change)
		{
			foreach (var methodGroupChange in change.MethodGroupChanges)
			{
				Visit(methodGroupChange);
			}
		}

		public virtual void Visit(MethodGroupChange change)
		{
			foreach (var methodChange in change.MethodChanges)
			{
				Visit(methodChange);
			}
		}

		public virtual void Visit(MethodChange change)
		{
			foreach (var parameterChange in change.ParameterChanges)
			{
				Visit(parameterChange);
			}
		}

		public virtual void Visit(ParameterChange change)
		{
		}
	}
}