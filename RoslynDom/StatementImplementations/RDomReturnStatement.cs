using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;
using cm = System.ComponentModel;

namespace RoslynDom
{
    public class RDomReturnStatement : RDomBase<IReturnStatement, ISymbol>, IReturnStatement
    {
        /// <summary>
        /// Constructor to use when creating a RoslynDom from scratch
        /// </summary>
        /// <param name="expression">
        /// Expression to return
        /// </param>
        public RDomReturnStatement(IExpression expression, bool suppressNewLine = false)
            : base()
        {
            _return = expression;
            if (!suppressNewLine)
            { Whitespace2Set.Add(new Whitespace2(LanguageElement.EndOfLine, "", "\r\n", "")); }
            Whitespace2Set.Add(new Whitespace2(LanguageElement.ReturnKeyword, "", " ", ""));
        }

        [cm.EditorBrowsable(cm.EditorBrowsableState.Never)]
        public RDomReturnStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomReturnStatement(RDomReturnStatement oldRDom)
            : base(oldRDom)
        {
            if (oldRDom.Return != null)
            { _return = oldRDom.Return.Copy(); }
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                if (Return != null)
                { list.Add(Return); }
                return list;
            }
        }

        public IExpression Return
        {
            get { return _return; }
            set { SetProperty(ref _return, value); }
        }

        private IExpression _return;
    }
}