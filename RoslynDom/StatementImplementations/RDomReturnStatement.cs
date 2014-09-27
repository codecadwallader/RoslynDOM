﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
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
      public RDomReturnStatement(IExpression expression)
       : this(null, null, null)
      {
         Return = expression;
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
            { Return = oldRDom.Return.Copy(); }
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

        //public override IEnumerable<IDom> Descendants
        //{ get { return Children; } }

        public IExpression Return { get; set; }
    }
}
