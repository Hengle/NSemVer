namespace NSemVer
{
	using System.Globalization;
	using NSemVer.Visitors;

	public static class Extensions
	{
		public static void Visit(this AssemblyChanges changes, params IChangeVisitor[] visitors)
		{
			foreach (var changeVisitor in visitors)
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