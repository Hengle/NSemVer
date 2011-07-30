namespace NSemVer.Visitors
{
	public static class VisitorExtensions
	{
		public static void Visit(this AssemblyChanges assemblyChanges, IChangeVisitor visitor)
		{
			visitor.Visit(assemblyChanges);

			foreach (var moduleChange in assemblyChanges.ModuleChanges)
			{
				visitor.Visit(moduleChange);

				foreach (var typeChange in moduleChange.TypeChanges)
				{
					visitor.Visit(typeChange);

					foreach (var methodChange in typeChange.MethodChanges)
					{
						visitor.Visit(methodChange);

						foreach (var parameterChange in methodChange.ParameterChanges)
						{
							visitor.Visit(parameterChange);
						}
					}
				}
			}
		}
	}
}