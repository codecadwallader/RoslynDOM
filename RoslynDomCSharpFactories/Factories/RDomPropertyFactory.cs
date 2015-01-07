﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
    public class RDomPropertyTypeMemberFactory
          : RDomBaseSyntaxNodeFactory<RDomProperty, PropertyDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomPropertyTypeMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                    _whitespaceLookup.Add(LanguageElement.AccessorGroupStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.AccessorGroupEndDelimiter, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.Add(LanguageElement.NewSlot, SyntaxKind.NewKeyword);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.OopModifiers);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as PropertyDeclarationSyntax;
            var newItem = new RDomProperty(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.AccessorList, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            var type = OutputContext.Corporation
                            .Create(syntax.Type, newItem, model)
                            .FirstOrDefault()
                            as IReferencedType;
            newItem.PropertyType = type;

            var propSymbol = newItem.Symbol as IPropertySymbol;
            Guardian.Assert.IsNotNull(propSymbol, nameof(propSymbol));

            var getAccessorSyntax = syntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.GetAccessorDeclaration).FirstOrDefault();
            var setAccessorSyntax = syntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.SetAccessorDeclaration).FirstOrDefault();
            if (getAccessorSyntax != null)
            { newItem.GetAccessor = OutputContext.Corporation.CreateSpecial<IAccessor>(getAccessorSyntax, newItem, model).FirstOrDefault(); }
            if (setAccessorSyntax != null)
            { newItem.SetAccessor = OutputContext.Corporation.CreateSpecial<IAccessor>(setAccessorSyntax, newItem, model).FirstOrDefault(); }

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IProperty;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
            var returnType = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(itemAsT.ReturnType).First();
            var node = SyntaxFactory.PropertyDeclaration(returnType, nameSyntax);
            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
            node = node.WithModifiers(modifiers);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            var accessors = SyntaxFactory.List<AccessorDeclarationSyntax>();
            var getAccessorSyntax = RDom.CSharp.GetSyntaxGroup(itemAsT.GetAccessor).FirstOrDefault();
            if (getAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)getAccessorSyntax); }

            var setAccessorSyntax = RDom.CSharp.GetSyntaxGroup(itemAsT.SetAccessor).FirstOrDefault();
            if (setAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)setAccessorSyntax); }

            var accessorList = SyntaxFactory.AccessorList(accessors);
            accessorList = BuildSyntaxHelpers.AttachWhitespace(accessorList, itemAsT.Whitespace2Set, WhitespaceLookup);
            if (accessors.Any()) { node = node.WithAccessorList(accessorList); }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

            return node.PrepareForBuildSyntaxOutput(item, OutputContext);
        }
    }
}