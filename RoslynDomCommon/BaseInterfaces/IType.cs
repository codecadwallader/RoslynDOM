namespace RoslynDom.Common
{
    public interface IType :
         IDom,
         IHasAttributes,
         IHasNamespace,
         IStemMember,
         ITypeMember,
         ICanBeNested,
         IHasStructuredDocumentation
    {
        string MetadataName { get; set; }
    }

    public interface IType<T> : IType, ITypeMember<T>
        where T : IType<T>
    {
    }
}