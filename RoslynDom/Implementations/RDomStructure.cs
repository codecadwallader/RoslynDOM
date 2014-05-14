﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructure : RDomBaseClassOrStructure<StructDeclarationSyntax>, IStructure
    {
        internal RDomStructure(StructDeclarationSyntax rawItem,
           IEnumerable<ITypeMember> members)
            : base(rawItem, members)
        { }

        public override string Name
        {
            get { return TypedRawItem.Identifier.NameFrom(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.Identifier.QNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return TypedRawItem.Identifier.BestInContextNameFrom(); }
        }
    }
}
