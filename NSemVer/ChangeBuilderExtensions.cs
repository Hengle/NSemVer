namespace NSemVer
{
	using System.IO;
	using Mono.Cecil;

	public static class ChangeBuilderExtensions
	{
		public static AssemblyChanges GetChanges(this AssemblyChangesBuilder changeBuilder, Stream olderAssemblyStream, Stream newerAssemblyStream)
		{
			return changeBuilder.GetChanges(
				AssemblyDefinition.ReadAssembly(olderAssemblyStream),
				AssemblyDefinition.ReadAssembly(newerAssemblyStream));
		}
	}
}