﻿using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface ITryStatement : IStatement, IDom<ITryStatement>, IStatementBlock
    {
        IEnumerable<ICatchStatement> Catches { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
           "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Finally",
          Justification = "Because this represents a finally clause, it's seems an appropriate name")]
        IFinallyStatement Finally { get; }
    }

    public interface ICatchStatement : IStatement, IHasCondition, IStatementBlock, IDom<ICatchStatement>
    {
        // There will always be an exception type. There might also be a variable
        IVariableDeclaration Variable { get; set; }

        IReferencedType ExceptionType { get; set; }
    }

    public interface IFinallyStatement : IStatement, IDom<IFinallyStatement>, IStatementBlock
    {
    }
}