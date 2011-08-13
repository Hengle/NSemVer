namespace NSemVer.Visitors.Context
{
	public class MethodChangeContext : MethodGroupChangeContext
	{
		private readonly MethodGroupChange _parentMethodGroupChange;

		public MethodChangeContext(AssemblyChanges assemblyChanges, ModuleChange parentModuleChange, TypeChange parentTypeChange, MethodGroupChange parentMethodGroupChange)
			: base(assemblyChanges, parentModuleChange, parentTypeChange)
		{
			_parentMethodGroupChange = parentMethodGroupChange;
		}

		public MethodGroupChange ParentMethodGroupChange
		{
			get { return _parentMethodGroupChange; }
		}
	}
}