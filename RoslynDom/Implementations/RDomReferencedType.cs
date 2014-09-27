﻿using Microsoft.CodeAnalysis;
using RoslynDom.Common;
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
      /// <br/>
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
      public RDomReferencedType(string name, bool displayAlias = false, bool isArray = false,
                  params IReferencedType[] typeArgs)
         : this(null, null, null)
      {
         Name = StringUtilities.NameFromQualifiedName(name);
         Namespace = StringUtilities.NamespaceFromQualifiedName(name);
         DisplayAlias = displayAlias;
         IsArray = isArray;
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
      { Initialize(); }

      internal RDomReferencedType(RDomReferencedType oldRDom)
          : base(oldRDom)
      {
         Initialize();
         Name = oldRDom.Name;
         Namespace = oldRDom.Namespace;
         DisplayAlias = oldRDom.DisplayAlias;
         IsArray = oldRDom.IsArray;
         ContainingType = oldRDom.ContainingType;
         TypeArguments.AddOrMoveRange(RoslynDomUtilities.CopyMembers(oldRDom._typeArguments));
      }

      private void Initialize()
      {
         _typeArguments = new RDomCollection<IReferencedType>(this);
      }

      public string Name { get; set; }
      public bool DisplayAlias { get; set; }
      public bool IsArray { get; set; }

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

      public string Namespace { get; set; }

      public RDomCollection<IReferencedType> TypeArguments
      { get { return _typeArguments; } }

      public INamedTypeSymbol ContainingType { get; set; }
   }
}
