using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using cm = System.ComponentModel;

namespace RoslynDom
{
   public class RDomReferencedType : RDomBase<IReferencedType, ISymbol>, IReferencedType
   {
      // ITypeParameter not used because these are arguments, not declarations
      private RDomCollection<IReferencedType> _typeArguments;

      /// <summary>
      /// Constructor to use when creating a RoslynDom from scratch
      /// </summary>
      /// <param name="name">
      /// Name of the type. If it's a qualified name, output checks in factories will
      /// probably simplify
      /// <br />
      /// In the case of language aliases, please enter the framework name because RoslynDom
      /// is language independent and direct tree creation does not depend on a language. For
      /// example, enter "System.String" not "string" and "System.Int32", not "int"
      /// </param>
      /// <param name="displayAlias">
      /// When using a type that you wish to display as a language alias, enter true.
      /// Otherwise this is ignored
      /// </param>
      /// <param name="isArray">
      /// Supply true if the type is an array
      /// </param>
      /// <param name="typeArgs">
      /// Type arguments for the type being created
      /// </param>
      public RDomReferencedType(IDom parent, string name, bool displayAlias = false, bool isArray = false,
                                params IReferencedType[] typeArgs)
         : base(parent)
      {
         Initialize();
         _name = StringUtilities.NameFromQualifiedName(name);
         _namespace = StringUtilities.NamespaceFromQualifiedName(name);
         _displayAlias = displayAlias;
         _isArray = isArray;
         TypeArguments.AddOrMoveRange(typeArgs);
      }

      /// <summary>
      /// Constructor for use by factories
      /// </summary>
      /// <param name="rawItem">Underlying SyntaxNode</param>
      /// <param name="parent">Parent IDom, null for root</param>
      /// <param name="model">The semantic model for the syntax node to avoid re-creation</param>
      [cm.EditorBrowsable(cm.EditorBrowsableState.Never)]
      public RDomReferencedType(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         Initialize();
         var typeSymbol = Symbol as ITypeSymbol;
         if (typeSymbol != null)
         {
            // TODO: See if this is a bug in the current preview
            _metadataName = typeSymbol.ContainingNamespace + "." + typeSymbol.MetadataName;
         }
      }

      internal RDomReferencedType(RDomReferencedType oldRDom)
         : base(oldRDom)
      {
         Initialize();

         _name = oldRDom.Name;
         _namespace = oldRDom.Namespace;
         _displayAlias = oldRDom.DisplayAlias;
         _isArray = oldRDom.IsArray;
         _containingType = oldRDom.ContainingType;
         _typeArguments = oldRDom.TypeArguments.Copy(this);
      }

      private void Initialize()
      {
         _typeArguments = new RDomCollection<IReferencedType>(this);
      }

      private string _name;

      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private string _metadataName;

      [Required]
      public string MetadataName
      {
         get { return _metadataName; }
         set { SetProperty(ref _metadataName, value); }
      }

      private string _namespace;

      public string Namespace
      {
         get { return _namespace; }
         set { SetProperty(ref _namespace, value); }
      }

      private bool _displayAlias;

      public bool DisplayAlias
      {
         get { return _displayAlias; }
         set { SetProperty(ref _displayAlias, value); }
      }

      private bool _isArray;

      public bool IsArray
      {
         get { return _isArray; }
         set { SetProperty(ref _isArray, value); }
      }

      private INamedTypeSymbol _containingType;

      public INamedTypeSymbol ContainingType
      {
         get { return _containingType; }
         set { SetProperty(ref _containingType, value); }
      }

      public string QualifiedName
      {
         get
         {
            var containingTypename = (ContainingType == null)
                                        ? ""
                                        : ContainingType.Name + ".";
            var ns = (string.IsNullOrEmpty(Namespace))
                        ? ""
                        : Namespace + ".";
            return ns + containingTypename + Name;
         }
      }

      public RDomCollection<IReferencedType> TypeArguments
      { get { return _typeArguments; } }

      private IType _type;
      private bool _searchFailed;

      public IType Type
      {
         get
         {
            // TODO: Reset searchFailed when load is complete or block access until load is complete
            if (!_searchFailed && _type == null)
            {
               _type = GetTypeFromRoslynDom();
               if (_type == null)
               {
                  _type = GetTypeFromReferenced();
               }
            }
            return _type;
         }
      }

      private IType GetTypeFromReferenced()
      {
         var root = Ancestors.OfType<RDomRoot>().FirstOrDefault();
         if (root != null)
         {
            return root.FindByMetadataName(MetadataName);
         }
         return null;
      }

      private IType GetTypeFromRoslynDom()
      {
         var candidates = Roots
                           .SelectMany(x => x.Descendants
                              .OfType<IType>()
                              .OfType<RoslynRDomBase>()
                              .Where(y => y.Symbol == Symbol)
                           )
                           .ToList();
         var count = candidates.Count();
         if (count == 1)
         {
            return candidates.First() as IType;
         }
         else if (count > 1)
         {
            Guardian.AmbiguousType(Name);
            _searchFailed = true;
         }
         return null;
      }
   }
}