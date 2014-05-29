﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomProperty : RDomSyntaxNodeBase<PropertyDeclarationSyntax, IPropertySymbol>, IProperty
    {
        internal RDomProperty(PropertyDeclarationSyntax rawItem) : base(rawItem) { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return this.AttributesFrom();
            }
        }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public override string QualifiedName
        {
            get
            {
                // TODO: Manage static member's qualified names
                throw new InvalidOperationException("You can't get qualified name for an instance property");
            }
        }
        
        public override string Namespace
        {
            get
            {
                throw new InvalidOperationException("You can't get namespace for an instance method");
            }
        }

        public IReferencedType ReturnType
        {
                get
                {
                    return new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences , TypedSymbol.Type);
                }
            }

        public bool IsAbstract
        {
            get
            {
                return Symbol.IsAbstract;
            }
        }

        public bool IsVirtual
        {
            get
            {
                return Symbol.IsVirtual;
            }
        }

        public bool IsOverride
        {
            get
            {
                return Symbol.IsOverride;
            }
        }

        public bool IsSealed
        {
            get
            {
                return Symbol.IsSealed;
            }
        }

        public bool IsStatic
        {
            get
            {
                return Symbol.IsStatic;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This is to support VB, C# does not have parameters on properties. Property parameters
        /// in VB are generally used for indexing, which is managed by "default" in C#
        /// <br/>
        /// Can't test until VB is active
        /// </remarks>
        public IEnumerable<IParameter> Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
