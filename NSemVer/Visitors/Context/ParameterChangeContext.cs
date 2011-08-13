namespace NSemVer.Visitors.Context
{
	public class ParameterChangeContext : MethodChangeContext
	{
		private readonly MethodChange _parentMethodChange;

		public ParameterChangeContext(AssemblyChanges assemblyChanges, ModuleChange parentModuleChange, TypeChange parentTypeChange, MethodGroupChange parentMethodGroupChange, MethodChange parentMethodChange)
			: base(assemblyChanges, parentModuleChange, parentTypeChange, parentMethodGroupChange)
		{
			_parentMethodChange = parentMethodChange;
		}

		public MethodChange ParentMethodChange
		{
			get { return _parentMethodChange; }
		}
	}
}