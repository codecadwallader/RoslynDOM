using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom
{
    public class RDomFinallyStatement : RDomStatementBlockBase<IFinallyStatement>, IFinallyStatement
    {
        public RDomFinallyStatement()
            : base()
        { }

        public RDomFinallyStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomFinallyStatement(RDomFinallyStatement oldRDom)
            : base(oldRDom)
        { }

        public override IEnumerable<IDom> Children
        {
            get
            { return base.Children.ToList(); }
        }
    }
}