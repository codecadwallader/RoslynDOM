﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomOperatorTypeMemberFactory
          : RDomTypeMemberFactory<RDomOperator, OperatorDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomOperatorTypeMemberFactory(RDomCorporation corporation)
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
                _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }
        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as OperatorDeclarationSyntax;
            var newItem = new RDomOperator(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            //var typeParameters = newItem.TypedSymbol.TypeParametersFrom();

            newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);
            newItem.Operator  = Mappings.OperatorFromCSharpKind(syntax.OperatorToken.CSharpKind());

            //newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.ReturnType);
            var returnType = Corporation
                             .CreateFrom<IMisc>(syntax.ReturnType, newItem, model)
                             .FirstOrDefault()
                             as IReferencedType;
            newItem.Type = returnType;

            newItem.IsStatic = newItem.Symbol.IsStatic;

            return newItem;
        }

            public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IOperator;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);

            var kind = Mappings.SyntaxKindFromOperator(itemAsT.Operator);
            var returnTypeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsT.Type).First();
            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
            var node = SyntaxFactory.OperatorDeclaration(returnTypeSyntax, nameSyntax)
                            .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            //node = node.WithBody(RoslynCSharpUtilities.MakeStatementBlock(itemAsT.Statements));
            node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup));

            return node.PrepareForBuildSyntaxOutput(item);
        }

    }


}
