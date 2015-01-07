﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
    public class RDomPropertyAccessorMiscFactory
          : RDomBaseSyntaxNodeFactory<RDomPropertyAccessor, AccessorDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomPropertyAccessorMiscFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.AccessorStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.AccessorGetKeyword, SyntaxKind.GetKeyword);
                    _whitespaceLookup.Add(LanguageElement.AccessorSetKeyword, SyntaxKind.SetKeyword);
                    _whitespaceLookup.Add(LanguageElement.AccessorShortFormIndicator, SyntaxKind.SemicolonToken);
                    _whitespaceLookup.Add(LanguageElement.AccessorEndDelimiter, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                }
                return _whitespaceLookup;
            }
        }

        public override Type[] SpecialExplicitDomTypes
        { get { return new[] { typeof(IAccessor) }; } }

        protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as AccessorDeclarationSyntax;
            var parentProperty = parent as IProperty;
            var accessorType = (syntaxNode.CSharpKind() == SyntaxKind.GetAccessorDeclaration)
                                ? AccessorType.Get : AccessorType.Set;
            var newItem = new RDomPropertyAccessor(syntaxNode, accessorType, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);

            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Body, LanguagePart.Current, WhitespaceLookup);

            var newItemName = accessorType.ToString().ToLower() + "_" + parentProperty.Name;
            newItem.Name = newItemName;

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IAccessor;
            var parentProperty = item.Parent as RDomProperty; // .NET specific
            if (itemAsT == null || parentProperty == null) { throw new InvalidOperationException(); }
            var kind = (itemAsT.AccessorType == AccessorType.Get)
                        ? SyntaxKind.GetAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
            AccessorDeclarationSyntax node;
            if (itemAsT.Statements.Any() || !parentProperty.CanBeAutoProperty)
            {
                var statementBlock = (BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup);
                node = SyntaxFactory.AccessorDeclaration(kind, statementBlock);
            }
            else
            {
                node = SyntaxFactory.AccessorDeclaration(kind).WithSemicolonToken(
                            SyntaxFactory.Token(
                                SyntaxKind.SemicolonToken));
            }

            if (itemAsT.AccessModifier != parentProperty.AccessModifier)
            {
                var modifiers = item.BuildModfierSyntax();
                node = node.WithModifiers(modifiers);
            }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

            var attributeList = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributeList.Any()) { node = node.WithAttributeLists(attributeList); }

            return node.PrepareForBuildSyntaxOutput(item, OutputContext);
        }
    }
}