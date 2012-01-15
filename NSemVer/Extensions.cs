namespace NSemVer
{
	using System.Globalization;
	using System.IO;
	using Mono.Cecil;
	using NSemVer.Visitors;

	public static class Extensions
	{
		public static AssemblyChanges GetChanges(this AssemblyChangesBuilder changeBuilder, Stream olderAssemblyStream, Stream newerAssemblyStream)
		{
			return changeBuilder.GetChanges(
				AssemblyDefinition.ReadAssembly(olderAssemblyStream),
				AssemblyDefinition.ReadAssembly(newerAssemblyStream));
		}

		public static void Visit(this AssemblyChanges changes, params IChangeVisitor[] visitors)
		{
			foreach (IChangeVisitor changeVisitor in visitors)
			{
				changeVisitor.Visit(changes);
			}
		}
		
		public static string FormatInvariant(this string s, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, s, args);
		}
	}
}