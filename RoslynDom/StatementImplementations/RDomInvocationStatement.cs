using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cm = System.ComponentModel;

namespace RoslynDom
{
    public class RDomInvocationStatement : RDomBase<IInvocationStatement, ISymbol>, IInvocationStatement
    {
        /// <summary>
        /// Constructor to use when creating a RoslynDom from scratch
        /// </summary>
        /// <param name="expression">
        /// Expression to invoke
        /// </param>
        public RDomInvocationStatement(IInvocationExpression expression, bool suppressNewLine = false)
            : base()
        {
            _invocation = expression;
            if (!suppressNewLine)
            { Whitespace2Set.Add(new Whitespace2(LanguageElement.EndOfLine, "", "\r\n", "")); }
        }

        [cm.EditorBrowsable(cm.EditorBrowsableState.Never)]
        public RDomInvocationStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomInvocationStatement(RDomInvocationStatement oldRDom)
            : base(oldRDom)
        {
            _invocation = (IInvocationExpression)oldRDom.Invocation.Copy();
        }

        public override IEnumerable<IDom> Children
        { get { return new List<IDom>() { Invocation }; } }

        [Required]
        private IInvocationExpression _invocation;

        public IInvocationExpression Invocation
        {
            get { return _invocation; }
            set { SetProperty(ref _invocation, value); }
        }

        public string MethodName
        {
            get { return Invocation.MethodName; }
            set { Invocation.MethodName = value; }
        }

        public RDomCollection<IReferencedType> TypeArguments
        { get { return Invocation.TypeArguments; } }

        public RDomCollection<IArgument> Arguments
        { get { return Invocation.Arguments; } }
    }
}