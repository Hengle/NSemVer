namespace NSemVer.Visitors.Context
{
	public class MethodGroupChangeContext : TypeChangeContext
	{
		private readonly TypeChange _parentTypeChange;

		public MethodGroupChangeContext(AssemblyChanges assemblyChanges, ModuleChange parentModuleChange, TypeChange parentTypeChange)
			: base(assemblyChanges, parentModuleChange)
		{
			_parentTypeChange = parentTypeChange;
		}

		public TypeChange ParentTypeChange
		{
			get { return _parentTypeChange; }
		}
	}
}