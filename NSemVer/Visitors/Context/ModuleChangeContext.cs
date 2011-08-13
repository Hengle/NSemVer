namespace NSemVer.Visitors.Context
{
	public class ModuleChangeContext
	{
		private readonly AssemblyChanges _assemblyChanges;

		public ModuleChangeContext(AssemblyChanges assemblyChanges)
		{
			_assemblyChanges = assemblyChanges;
		}

		public AssemblyChanges AssemblyChanges
		{
			get { return _assemblyChanges; }
		}
	}
}