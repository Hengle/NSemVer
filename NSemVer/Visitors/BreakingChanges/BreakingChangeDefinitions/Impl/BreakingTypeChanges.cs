namespace NSemVer.Visitors.BreakingChanges.BreakingChangeDefinitions.Impl
{
	using NSemVer.Visitors.Context;

	public class BreakingTypeChanges : BreakingChangeDefinitionsBase<TypeChange, TypeChangeContext>
	{
		public BreakingTypeChanges()
		{
			Add(ApiBreakType.TypeRemoved,
				(change, context) => change.ChangeType == ChangeType.Removed && change.Type.IsPublic ? change.Type : null,
				(typeRemoved, context) => "Removed public type '{0}'".FormatInvariant(typeRemoved.FullName));
		}
	}
}