﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
    public class RDomVariableDeclarationFactory
        : RDomBaseSyntaxNodeFactory<RDomVariableDeclaration, VariableDeclaratorSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomVariableDeclarationFactory(RDomCorporation corporation)
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
                    _whitespaceLookup.Add(LanguageElement.EqualsAssignmentOperator, SyntaxKind.EqualsToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        public override Type[] SupportedSyntaxNodeTypes
        { get { return new Type[] { typeof(VariableDeclarationSyntax), typeof(CatchDeclarationSyntax) }; } }

        public override Type[] SpecialExplicitDomTypes
        { get { return new[] { typeof(IVariableDeclaration) }; } }

        protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var rawVariableDeclaration = syntaxNode as VariableDeclarationSyntax;
            if (rawVariableDeclaration != null)
            {
                return CreateFromVariableDeclaration(rawVariableDeclaration, syntaxNode, parent, model,
                    (s, p, m) => new RDomDeclarationStatement(s, p, m));
            }

            var rawCatchDeclaration = syntaxNode as CatchDeclarationSyntax;
            if (rawCatchDeclaration != null)
            {
                return new IMisc[] { SetupNewVariable(VariableKind.Catch,
                     new RDomVariableDeclaration(rawCatchDeclaration, parent, model),
                     rawCatchDeclaration.Type, rawCatchDeclaration, parent, model) };
            }

            var rawForEachSyntax = syntaxNode as ForEachStatementSyntax;
            if (rawForEachSyntax != null)
            {
                return new IMisc[] { SetupNewVariable(VariableKind.ForEach ,
                    new RDomVariableDeclaration(rawForEachSyntax, parent, model),
                    rawForEachSyntax.Type, rawForEachSyntax, parent, model ) };
            }

            var rawForSyntax = syntaxNode as ForStatementSyntax;
            if (rawForSyntax != null)
            {
                return CreateFromVariableDeclaration(rawForSyntax.Declaration, syntaxNode, parent, model,
                         (s, p, m) => new RDomVariableDeclaration(s, p, m));
            }

            throw new InvalidOperationException();
        }

        private IEnumerable<IMisc> CreateFromVariableDeclaration(
                VariableDeclarationSyntax syntax, SyntaxNode syntaxNode, IDom parent, SemanticModel model,
                Func<SyntaxNode, IDom, SemanticModel, RDomBaseVariable> makeNewDelegate)
        {
            var list = new List<IMisc>();
            var declarators = syntax.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = SetupNewVariable(VariableKind.Local,
                    makeNewDelegate(decl, parent, model),
                    syntax.Type, decl, parent, model);
                var rDomItem = newItem as IRoslynDom;
                list.Add(newItem);
                CreateFromWorker.StoreWhitespace(newItem, decl, LanguagePart.Current, WhitespaceLookup);
                if (decl.Initializer != null)
                {
                    var equalsClause = decl.Initializer;
                    newItem.Initializer = OutputContext.Corporation.CreateSpecial<IExpression>(equalsClause.Value, newItem, model).FirstOrDefault();
                    //newItem.Initializer = (IExpression)OutputContext.Corporation.Create(equalsClause.Value, newItem, model).FirstOrDefault();
                    CreateFromWorker.StandardInitialize(newItem.Initializer, decl, parent, model, OutputContext);
                    CreateFromWorker.StoreWhitespaceForToken(newItem, decl.Initializer.EqualsToken, LanguagePart.Current, LanguageElement.EqualsAssignmentOperator);
                    CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, decl.Initializer, LanguagePart.Current, LanguageElement.Expression);
                }
            }
            return list;
        }

        public IVariable SetupNewVariable(VariableKind variableKind, RDomBaseVariable newItem, TypeSyntax typeSyntax,
            SyntaxNode node, IDom parent, SemanticModel model)
        {
            CreateFromWorker.StandardInitialize(newItem, node, parent, model, OutputContext);
            newItem.Name = newItem.TypedSymbol.Name;
            var declaredType = typeSyntax.ToString();
            var returnType = OutputContext.Corporation
                            .Create(typeSyntax, newItem, model)
                            .FirstOrDefault()
                            as IReferencedType;
            newItem.Type = returnType;
            newItem.VariableKind = variableKind;

            newItem.IsImplicitlyTyped = (declaredType == "var");
            if (!newItem.IsImplicitlyTyped
                  && newItem.Type.TypeArguments.Count() == 0
                  && declaredType != newItem.Type.Name)
            {
                var test = Mappings.AliasFromSystemType(newItem.Type.Name);
                if (declaredType == test) newItem.IsAliased = true;
            }
            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IVariableDeclaration;
            TypeSyntax typeSyntax;
            if (itemAsT.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDom.CSharp.GetSyntaxNode(itemAsT.Type)); }
            switch (itemAsT.VariableKind)
            {
                //case VariableKind.Unknown:
                //{ throw new NotImplementedException(); }
                // This is NOT symmetric. The local declaration does not call back to this class for BuildSyntax
                case VariableKind.Local:
                    { return BuildSyntaxLocal(itemAsT, typeSyntax); }
                //case VariableKind.Using:
                //    return BuildSyntaxUsing(itemAsT, typeSyntax);
                //case VariableKind.Catch:
                //{ return BuildSyntaxCatch(itemAsT, typeSyntax); }
                //case VariableKind.For:
                //{ return BuildSyntaxFor(itemAsT, typeSyntax); }
                //case VariableKind.ForEach:
                //{ return BuildSyntaxForEach(itemAsT, typeSyntax); }
                default:
                    { throw new InvalidOperationException(); }
            }
        }

        private IEnumerable<SyntaxNode> BuildSyntaxLocal(IVariableDeclaration itemAsT, TypeSyntax typeSyntax)
        {
            var node = SyntaxFactory.VariableDeclarator(itemAsT.Name);
            if (itemAsT.Initializer != null)
            {
                var expressionSyntax = (ExpressionSyntax)RDom.CSharp.GetSyntaxNode(itemAsT.Initializer);
                expressionSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(expressionSyntax, itemAsT.Whitespace2Set[LanguageElement.Expression]);
                var equalsToken = SyntaxFactory.Token(SyntaxKind.EqualsToken);
                equalsToken = BuildSyntaxHelpers.AttachWhitespaceToToken(equalsToken, itemAsT.Whitespace2Set[LanguageElement.EqualsAssignmentOperator]);
                var equalsValueClause = SyntaxFactory.EqualsValueClause(equalsToken, expressionSyntax);
                //equalsValueClause = BuildSyntaxHelpers.AttachWhitespace(equalsValueClause, itemAsT.Whitespace2Set, WhitespaceLookup);
                node = node.WithInitializer(equalsValueClause);
            }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            // Not a statement, so should not call PrepareForBuildSyntaxOutput
            return new SyntaxNode[] { node };
        }

        //private IEnumerable<SyntaxNode> BuildSyntaxFor(IVariableDeclaration itemAsT, TypeSyntax typeSyntax)
        //{
        //    var ret = BuildSyntaxLocal(itemAsT, typeSyntax);

        //    return ret;
        //}

        //private IEnumerable<SyntaxNode> BuildSyntaxCatch(IVariableDeclaration itemAsT, TypeSyntax typeSyntax)
        //{
        //    throw new NotImplementedException();
        //}
    }
}