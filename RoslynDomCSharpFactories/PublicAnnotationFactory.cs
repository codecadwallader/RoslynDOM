﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
    public class PublicAnnotationFactory
        : RDomBaseSyntaxTriviaFactory<IPublicAnnotation>
    {
        public override IDom CreateFrom(SyntaxTrivia trivia, IDom parent, OutputContext context)
        {
            var tuple = context.Corporation.CreateFromWorker.ExtractComment(trivia.ToFullString());
            var str = GetMatch(tuple.Item2);
            if (string.IsNullOrWhiteSpace(str)) return null;
            var target = str.SubstringBefore(":");
            var attribSyntax = GetAnnotationStringAsAttribute(str);
            // Reuse the evaluation work done in attribute to follow same rules
            var tempAttribute = context.Corporation.Create(attribSyntax, null, null).FirstOrDefault() as IAttribute;
            var newPublicAnnotation = new RDomPublicAnnotation(parent, trivia, tempAttribute.Name.ToString());
            newPublicAnnotation.Target = target;
            newPublicAnnotation.Whitespace2Set.AddRange(tempAttribute.Whitespace2Set);
            foreach (var attributeValue in tempAttribute.AttributeValues)
            {
                newPublicAnnotation.AddItem(attributeValue.Name ?? "", attributeValue.Value);
            }
            return newPublicAnnotation;
        }

        public override IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom publicAnnotation, OutputContext context)
        {
            throw new NotImplementedException();
        }

        #region Private methods to support public annotations

        private string GetMatch(string comment)
        {
            return comment.SubstringAfter("[[").SubstringBefore("]]").Trim();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(System.String,System.String,Microsoft.CodeAnalysis.CSharp.CSharpParseOptions,System.Threading.CancellationToken)")]
        private static AttributeSyntax GetAnnotationStringAsAttribute(string str)
        {
            // Trick Roslyn into thinking it's an attribute
            str = "[" + str + "] public class {}";
            var tree = CSharpSyntaxTree.ParseText(str);
            var attrib = tree.GetRoot().DescendantNodes()
                        .Where(x => x.CSharpKind() == SyntaxKind.Attribute)
                        .FirstOrDefault();
            return attrib as AttributeSyntax;
        }

        //private IEnumerable<RDomPublicAnnotation> GetPublicAnnotations(CompilationUnitSyntax syntaxRoot, OutputContext context)
        //{
        //   var ret = new List<RDomPublicAnnotation>();
        //   var nodes = syntaxRoot.ChildNodes();
        //   foreach (var node in nodes)
        //   {
        //      ret.AddRange(GetPublicAnnotationFromFirstToken(node, true, context));
        //   }
        //   return ret;
        //}

        //private IEnumerable<RDomPublicAnnotation> GetPublicAnnotations(SyntaxNode node, OutputContext context)
        //{
        //   return GetPublicAnnotationFromFirstToken(node, false, context);
        //}

        //private IEnumerable<RDomPublicAnnotation> GetPublicAnnotationFromFirstToken(
        //           SyntaxNode node, bool isRoot, OutputContext context)
        //{
        //   var ret = new List<RDomPublicAnnotation>();
        //   var firstToken = node.GetFirstToken();
        //   if (firstToken != default(SyntaxToken))
        //   {
        //      ret.AddRange(GetPublicAnnotationFromToken(firstToken, isRoot, context));
        //   }
        //   return ret;
        //}

        //private IEnumerable<RDomPublicAnnotation> GetPublicAnnotationFromToken(
        //       SyntaxToken token, bool isRoot, OutputContext context)
        //{
        //   var ret = new List<RDomPublicAnnotation>();
        //   var trivias = token.LeadingTrivia
        //                     .Where(x => x.CSharpKind() == SyntaxKind.SingleLineCommentTrivia);
        //   foreach (var trivia in trivias)
        //   {
        //      var str = GetPublicAnnotationAsString(trivia);
        //      var strRoot = GetSpecialRootAnnotation(str);
        //      if (isRoot)
        //      { str = strRoot; }
        //      else
        //      { str = string.IsNullOrWhiteSpace(strRoot) ? str : ""; }
        //      if (!string.IsNullOrWhiteSpace(str))
        //      {
        //         var attribSyntax = GetAnnotationStringAsAttribute(str);
        //         // Reuse the evaluation work done in attribute to follow same rules
        //         var tempAttribute = context.Corporation.Create(attribSyntax, null, null).FirstOrDefault() as IAttribute;
        //         var newPublicAnnotation = new RDomPublicAnnotation(parent, trivia, tempAttribute.Name.ToString());
        //         newPublicAnnotation.Whitespace2Set.AddRange(tempAttribute.Whitespace2Set);
        //         foreach (var attributeValue in tempAttribute.AttributeValues)
        //         {
        //            newPublicAnnotation.AddItem(attributeValue.Name ?? "", attributeValue.Value);
        //         }
        //         ret.Add(newPublicAnnotation);
        //      }
        //   }
        //   return ret;
        //}

        private static string GetPublicAnnotationAsString(SyntaxTrivia trivia)
        {
            var str = trivia.ToString().Trim();
            if (!str.StartsWith("//", StringComparison.Ordinal)) throw new InvalidOperationException("Unexpected comment format");
            str = str.SubstringAfter("//").SubstringAfter("[[").SubstringBefore("]]").Trim();
            return str;
        }

        private static string GetSpecialRootAnnotation(string str)
        {
            str = str.Trim();

            if (str.StartsWith("file:", StringComparison.Ordinal))
            { return str.SubstringAfter("file:"); }
            if (str.StartsWith("root:", StringComparison.Ordinal))
            { return str.SubstringAfter("root:"); }
            return null;
        }

        #endregion Private methods to support public annotations
    }
}