namespace NSemVer.Console
{
	using System.Globalization;

	public static class Extensions
	{
		public static string FormatInvariant(this string s, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, s, args);
		}
	}
}