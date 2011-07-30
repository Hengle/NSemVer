namespace NSemVer.Visitors
{
	using System.Collections.Generic;

	public static class VisitorExtensions
	{
		public static void Visit(this AssemblyChanges assemblyChanges, IChangeVisitor changeVisitor)
		{
			assemblyChanges.ModuleChanges.Visit(changeVisitor);
		}

		public static void Visit(this IEnumerable<ModuleChange> changes, IChangeVisitor changeVisitor)
		{

		}

		public static void Visit(this IEnumerable<ParameterChange> changes, IChangeVisitor changeVisitor)
		{

		}
	}
}