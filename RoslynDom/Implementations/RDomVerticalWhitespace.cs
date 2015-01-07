using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomVerticalWhitespace : RDomDetail<IVerticalWhitespace>, IVerticalWhitespace
    {
        public RDomVerticalWhitespace(IDom parent, SyntaxTrivia trivia, int count, bool isElastic = false)
            : base(parent, StemMemberKind.Whitespace, MemberKind.Whitespace, trivia)
        {
            _count = count;
            _isElastic = isElastic;
        }

        internal RDomVerticalWhitespace(RDomVerticalWhitespace oldRDom)
            : base(oldRDom)
        {
            _count = oldRDom.Count;
            _isElastic = oldRDom.IsElastic;
        }

        // TODO: This is not going to be updated by the generator, consider how this affects the RoslynDom
        private int _count;

        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        private bool _isElastic;

        public bool IsElastic
        {
            get { return _isElastic; }
            set { SetProperty(ref _isElastic, value); }
        }
    }
}