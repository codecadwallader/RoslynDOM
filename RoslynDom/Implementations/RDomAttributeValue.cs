using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
    public class RDomAttributeValue
        : RDomBase<IAttributeValue, ISymbol>, IAttributeValue
    {
        public RDomAttributeValue(IDom parent, string name, AttributeValueStyle style = AttributeValueStyle.Positional,
                 object value = null, string valueConstantIdentifier = null, LiteralKind valueType = LiteralKind.Unknown)
            : base(parent)
        {
            _name = name;
            _style = style;
            _value = value;
            _valueConstantIdentifier = valueConstantIdentifier;
            _valueType = valueType;
        }

        public RDomAttributeValue(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomAttributeValue(
             RDomAttributeValue oldRDom)
            : base(oldRDom)
        {
            _name = oldRDom.Name;
            _style = oldRDom.Style;
            _value = oldRDom.Value;
            _valueConstantIdentifier = oldRDom.ValueConstantIdentifier;
            _valueType = oldRDom.ValueType;
            // TODO: manage type
            Type = oldRDom.Type;
        }

        private string _name;

        [Required]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private AttributeValueStyle _style;

        public AttributeValueStyle Style
        {
            get { return _style; }
            set { SetProperty(ref _style, value); }
        }

        private object _value;

        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private string _valueConstantIdentifier;

        public string ValueConstantIdentifier
        {
            get { return _valueConstantIdentifier; }
            set { SetProperty(ref _valueConstantIdentifier, value); }
        }

        private LiteralKind _valueType;

        public LiteralKind ValueType
        {
            get { return _valueType; }
            set { SetProperty(ref _valueType, value); }
        }

        private Type _type;

        public Type Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
    }
}