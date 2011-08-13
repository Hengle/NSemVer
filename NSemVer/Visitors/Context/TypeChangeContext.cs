namespace NSemVer.Visitors.Context
{
	public class TypeChangeContext : ModuleChangeContext
	{
		private readonly ModuleChange _parentModuleChange;

		public TypeChangeContext(AssemblyChanges assemblyChanges, ModuleChange parentModuleChange)
			: base(assemblyChanges)
		{
			_parentModuleChange = parentModuleChange;
		}

		public ModuleChange ParentModuleChange
		{
			get { return _parentModuleChange; }
		}
	}
}