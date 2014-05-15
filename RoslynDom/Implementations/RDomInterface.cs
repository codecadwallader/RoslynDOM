﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInterface : RDomSyntaxNodeBase<InterfaceDeclarationSyntax>, IInterface
    {
        internal RDomInterface(InterfaceDeclarationSyntax rawItem,
            IEnumerable<ITypeMember> members) : base(rawItem)
        { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return this.AttributesFrom();
            }
        }

        public override string Name
        {
            get { return TypedRawItem.NestedNameFrom(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.QualifiedNameFrom(); }
        }

        public string OriginalName
        {
            get
            {
                return this.OriginalNameFrom();
            }
        }
    }
}
