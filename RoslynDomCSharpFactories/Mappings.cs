﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
    internal static class Mappings
    {
        private static List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>> LiteralKindMap = new List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>>()
        {
            Tuple.Create(SyntaxKind.StringLiteralToken,  SyntaxKind.StringLiteralExpression,      LiteralKind.String),
            Tuple.Create(SyntaxKind.IdentifierToken,     SyntaxKind.SimpleMemberAccessExpression, LiteralKind.MemberAccess),
            Tuple.Create(SyntaxKind.NumericLiteralToken, SyntaxKind.NumericLiteralExpression,     LiteralKind.Numeric),
            Tuple.Create(SyntaxKind.NullKeyword,         SyntaxKind.NullLiteralExpression,        LiteralKind.Null),
            Tuple.Create(SyntaxKind.TrueKeyword,         SyntaxKind.TrueKeyword,                  LiteralKind.Boolean),
            Tuple.Create(SyntaxKind.FalseKeyword,        SyntaxKind.FalseKeyword,                 LiteralKind.Boolean),
            Tuple.Create(SyntaxKind.DefaultKeyword,      SyntaxKind.DefaultExpression ,           LiteralKind.Default),
            Tuple.Create(SyntaxKind.TypeOfKeyword,       SyntaxKind.TypeOfExpression,             LiteralKind.Type),
        };

        public static LiteralKind LiteralKindFromSyntaxKind(SyntaxKind kind)
        {
            foreach (var tuple in LiteralKindMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item3; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromLiteralKind(LiteralKind literalKind, object value)
        {
            if (literalKind == LiteralKind.Boolean)
            {
                if ((bool)value) { return SyntaxKind.TrueLiteralExpression; }
                return SyntaxKind.FalseLiteralExpression;
            }
            foreach (var tuple in LiteralKindMap)
            {
                if (tuple.Item3 == literalKind)
                { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<SyntaxKind, SyntaxKind, AssignmentOperator>> assignmentOpMap = new List<Tuple<SyntaxKind, SyntaxKind, AssignmentOperator>>()
        {
             Tuple.Create( SyntaxKind.SimpleAssignmentExpression,     SyntaxKind.EqualsToken,                        AssignmentOperator.Equals ),
             Tuple.Create( SyntaxKind.AddAssignmentExpression,        SyntaxKind.PlusEqualsToken,                    AssignmentOperator.AddAssignment),
             Tuple.Create( SyntaxKind.SubtractAssignmentExpression,   SyntaxKind.MinusEqualsToken,                   AssignmentOperator.SubtractAssignment),
             Tuple.Create( SyntaxKind.MultiplyAssignmentExpression,   SyntaxKind.AsteriskEqualsToken,                AssignmentOperator.MultiplyAssignment),
             Tuple.Create( SyntaxKind.ModuloAssignmentExpression,     SyntaxKind.PercentEqualsToken,                 AssignmentOperator.ModuloAssignment),
             Tuple.Create( SyntaxKind.DivideAssignmentExpression,     SyntaxKind.SlashEqualsToken,                   AssignmentOperator.DivideAssignment),
             Tuple.Create( SyntaxKind.AndAssignmentExpression,        SyntaxKind.AmpersandEqualsToken,               AssignmentOperator.AndAssignment),
             Tuple.Create( SyntaxKind.ExclusiveOrAssignmentExpression,SyntaxKind.CaretEqualsToken,                   AssignmentOperator.ExclusiveOrAssignment),
             Tuple.Create( SyntaxKind.OrAssignmentExpression,         SyntaxKind.BarEqualsToken,                     AssignmentOperator.OrAssignment),
             Tuple.Create( SyntaxKind.LeftShiftAssignmentExpression,  SyntaxKind.GreaterThanGreaterThanEqualsToken,  AssignmentOperator.LeftShiftAssignment),
             Tuple.Create( SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.LessThanLessThanEqualsToken,        AssignmentOperator.RightShiftAssignment)
        };

        public static AssignmentOperator AssignmentOperatorFromCSharpKind(SyntaxKind kind)
        {
            foreach (var tuple in assignmentOpMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item3; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromAssignmentOperator(AssignmentOperator op)
        {
            foreach (var tuple in assignmentOpMap)
            {
                if (tuple.Item3 == op) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxTokenKindFromAssignmentOperator(AssignmentOperator op)
        {
            foreach (var tuple in assignmentOpMap)
            {
                if (tuple.Item3 == op) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static AssignmentOperator AssignmentOperatorFromTokenKind(SyntaxKind kind)
        {
            foreach (var tuple in assignmentOpMap)
            {
                if (tuple.Item2 == kind) { return tuple.Item3; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<SyntaxKind, Operator>> operatorMap = new List<Tuple<SyntaxKind, Operator>>()
        {
             Tuple.Create( SyntaxKind.PlusToken                      , Operator.Plus),
             Tuple.Create( SyntaxKind.MinusToken                     , Operator.Minus),
             Tuple.Create( SyntaxKind.AsteriskToken                  , Operator.Asterisk),
             Tuple.Create( SyntaxKind.SlashToken                     , Operator.Slash),
             Tuple.Create( SyntaxKind.PercentToken                   , Operator.Percent),
             Tuple.Create( SyntaxKind.AmpersandToken                 , Operator.Ampersand),
             Tuple.Create( SyntaxKind.BarToken                       , Operator.Bar),
             Tuple.Create( SyntaxKind.CaretToken                     , Operator.Caret),
             Tuple.Create( SyntaxKind.LessThanLessThanToken          , Operator.LessThanLessThan),
             Tuple.Create( SyntaxKind.GreaterThanGreaterThanToken    , Operator.GreaterThanGreaterThan),
             Tuple.Create( SyntaxKind.EqualsEqualsToken              , Operator.EqualsEquals),
             Tuple.Create( SyntaxKind.ExclamationEqualsToken         , Operator.ExclamationEquals),
             Tuple.Create( SyntaxKind.GreaterThanToken               , Operator.GreaterThan),
             Tuple.Create( SyntaxKind.LessThanToken                  , Operator.LessThan),
             Tuple.Create( SyntaxKind.GreaterThanEqualsToken         , Operator.GreaterThanEquals),
             Tuple.Create( SyntaxKind.LessThanEqualsToken            , Operator.LessThanEquals)
        };

        public static Operator OperatorFromCSharpKind(SyntaxKind kind)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromOperator(Operator op)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item2 == op) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<Accessibility, AccessModifier>> accessModifierMap = new List<Tuple<Accessibility, AccessModifier>>()
        {
             Tuple.Create(Accessibility.Private             , AccessModifier.Private),
             Tuple.Create(Accessibility.ProtectedAndInternal, AccessModifier.ProtectedAndInternal),
             Tuple.Create(Accessibility.ProtectedAndFriend  , AccessModifier.ProtectedAndFriend ),
             Tuple.Create(Accessibility.Protected           , AccessModifier.Protected ),
             Tuple.Create(Accessibility.Internal            , AccessModifier.Internal ),
             Tuple.Create(Accessibility.Friend              , AccessModifier.Friend ),
             Tuple.Create(Accessibility.NotApplicable       , AccessModifier.None),
             Tuple.Create(Accessibility.ProtectedOrInternal , AccessModifier.ProtectedOrInternal ),
             Tuple.Create(Accessibility.ProtectedOrFriend   , AccessModifier.ProtectedOrFriend),
             Tuple.Create(Accessibility.Public              , AccessModifier.Public )
        };

        public static Accessibility AccessibilityFromAccessModifier(AccessModifier accessModifier)
        {
            foreach (var tuple in accessModifierMap)
            {
                if (tuple.Item2 == accessModifier) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

        public static AccessModifier AccessModifierFromAccessibility(Accessibility accessibility)
        {
            foreach (var tuple in accessModifierMap)
            {
                if (tuple.Item1 == accessibility) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<SyntaxKind, string, string>> typeAliasMap = new List<Tuple<SyntaxKind, string, string>>()
        {
             Tuple.Create(SyntaxKind.SByteKeyword,   "sbyte"  ,"SByte"),
             Tuple.Create(SyntaxKind.ShortKeyword,   "short"  ,"Int16"),
             Tuple.Create(SyntaxKind.IntKeyword,     "int"    ,"Int32"),
             Tuple.Create(SyntaxKind.LongKeyword,    "long"   ,"Int64"),
             Tuple.Create(SyntaxKind.ByteKeyword,    "byte"   ,"Byte"),
             Tuple.Create(SyntaxKind.UShortKeyword,  "ushort" ,"UInt16"),
             Tuple.Create(SyntaxKind.UIntKeyword,    "uint"   ,"UInt32"),
             Tuple.Create(SyntaxKind.ULongKeyword,   "ulong"  ,"UInt64"),
             Tuple.Create(SyntaxKind.DecimalKeyword, "decimal","Decimal"),
             Tuple.Create(SyntaxKind.FloatKeyword,   "float"  ,"Single"),
             Tuple.Create(SyntaxKind.DoubleKeyword,  "double" ,"Double"),
             Tuple.Create(SyntaxKind.BoolKeyword,    "bool"   ,"Boolean"),
             Tuple.Create(SyntaxKind.StringKeyword,  "string" ,"String"),
             Tuple.Create(SyntaxKind.CharKeyword,    "char"   ,"Char")
        };

        public static bool IsTypeAlias(SyntaxKind type)
        {
            return typeAliasMap.Any(x => x.Item1 == type);
        }

        public static string SystemTypeFromAlias(string alias)
        {
            foreach (var tuple in typeAliasMap)
            {
                if (tuple.Item2 == alias) { return tuple.Item3; }
            }
            throw new InvalidOperationException();
        }

        public static string AliasFromSystemType(string systemTypeName)
        {
            foreach (var tuple in typeAliasMap)
            {
                if (tuple.Item3 == systemTypeName) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<SyntaxKind, Variance>> GenericVarianceMap = new List<Tuple<SyntaxKind, Variance>>()
        {
            Tuple.Create( SyntaxKind.InKeyword,     Variance.In),
            Tuple.Create( SyntaxKind.OutKeyword,             Variance.Out)
        };

        public static Variance VarianceFromVarianceKind(SyntaxKind kind)
        {
            foreach (var tuple in GenericVarianceMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            return Variance.None;
        }

        public static SyntaxKind VarianceKindFromVariance(Variance variance)
        {
            foreach (var tuple in GenericVarianceMap)
            {
                if (tuple.Item2 == variance)
                { return tuple.Item1; }
            }
            return SyntaxKind.None;
        }
    }
}