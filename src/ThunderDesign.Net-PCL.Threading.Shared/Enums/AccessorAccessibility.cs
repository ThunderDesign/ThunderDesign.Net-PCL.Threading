namespace ThunderDesign.Net.Threading.Enums
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    public enum AccessorAccessibility
    {
        Public,
        Private,
        Protected,
        Internal,
        ProtectedInternal,
        PrivateProtected
    }
#endif
}