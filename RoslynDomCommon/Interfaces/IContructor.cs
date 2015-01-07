namespace RoslynDom.Common
{
   public interface IConstructor :
       ITypeMember<IConstructor>,
       IStatementContainer,
       ICanBeStatic,
       IHasParameters
   {
      // NOTE: Constructors do NOT record a name. Consistent with VB and general usage.
      ConstructorInitializerType ConstructorInitializerType { get; set; }

      RDomCollection<IArgument> InitializationArguments { get; }
   }
}