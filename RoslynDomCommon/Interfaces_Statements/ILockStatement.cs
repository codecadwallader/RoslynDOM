﻿using System.Collections.Generic;
using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface ILockStatement : IStatement, IStatementBlock  , IDom<ILockStatement>
    {
        IExpression Expression { get; set; }
    }
}
