﻿using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRoslynDom : IDom
    {
        ISymbol Symbol { get; }

        SyntaxNode TypedSyntax { get; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public interface IRoslynDom<T, TSymbol> : IDom<T>, IRoslynDom
        where TSymbol : ISymbol
        where T : IDom<T>
    {
        TSymbol TypedSymbol { get; }
    }
}