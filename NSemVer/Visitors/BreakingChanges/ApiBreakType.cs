namespace NSemVer.Visitors.BreakingChanges
{
	public enum ApiBreakType
	{
		TypeRemoved,
		MethodOverloadedWithInterfaceBasedParameter,
		MethodReturnTypeChanged,
		InstanceMethodAdded,
		InstanceMethodRemoved
	}
}