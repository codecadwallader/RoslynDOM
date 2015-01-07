using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom
{
    public class RDomRootGroup : RDomBase<IRootGroup>, IRootGroup
    {
        private Compilation _compilation;
        private RDomCollection<IRoot> _roots;
        private ReferencedTypeCache _referencedTypeCache;

        public RDomRootGroup(Compilation compilation, IFactoryAccess factoryAccess)
        {
            _compilation = compilation;
            _referencedTypeCache = new ReferencedTypeCache(compilation, factoryAccess);
            _roots = new RDomCollection<IRoot>(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomRootGroup(RDomRootGroup oldRDom)
        {
            _roots = Roots.Copy(this);
        }

        internal IType FindByMetadataName(string metadataName)
        {
            return _referencedTypeCache.FindByMetadataName(metadataName);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                list.AddRange(_roots);
                return list;
            }
        }

        public bool HasSyntaxErrors
        { get { return Roots.Any(x => x.HasSyntaxErrors); } }

        public override object OriginalRawItem
        { get { return _compilation; } }

        public override object RawItem
        { get { return OriginalRawItem; } }

        public RDomCollection<IRoot> Roots
        { get { return _roots; } }

        public override ISymbol Symbol
        { get { return null; } }
    }
}