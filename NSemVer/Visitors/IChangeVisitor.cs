namespace NSemVer.Visitors
{
	public interface IChangeVisitor
	{
		void Visit(AssemblyChanges change);

		void Visit(ModuleChange change);

		void Visit(TypeChange change);

		void Visit(MethodGroupChange change);

		void Visit(MethodChange change);

		void Visit(ParameterChange change);
	}
}