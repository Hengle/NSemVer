namespace NSemVer.Visitors.BreakingChanges
{
	public enum ApiBreakType
	{
		TypeRemoved,
		NewInstanceMethod,
		MethodOverloadedWithInterfaceBasedParameter,
		MethodReturnTypeChanged,
		InstanceMethodRemoved
	}
}