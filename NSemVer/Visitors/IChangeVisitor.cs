namespace NSemVer.Visitors
{
	public interface IChangeVisitor
	{
		void Visit(AssemblyChanges changes);
	}
}