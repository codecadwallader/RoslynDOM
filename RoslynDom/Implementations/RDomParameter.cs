using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoslynDom
{
    public class RDomParameter : RDomBase<IParameter, IParameterSymbol>, IParameter
    {
        private AttributeCollection _attributes = new AttributeCollection();

        public RDomParameter(string name, string typeName,
                        int ordinal = 0, bool isOut = false, bool isRef = false,
                        bool isParamArray = false)
            : this(name, name, null,
                  LiteralKind.Unknown, null, ordinal, isOut, isRef, isParamArray)
        { }

        public RDomParameter(string name, IReferencedType type,
                        int ordinal = 0, bool isOut = false, bool isRef = false,
                        bool isParamArray = false)
            : this(name, type, null, LiteralKind.Unknown, null, ordinal, isOut, isRef, isParamArray)
        { }

        public RDomParameter(string name, string typeName,
                       object defaultValue, LiteralKind defaultValueType, string defaultConstantIdentifier,
                       int ordinal = 0, bool isOut = false, bool isRef = false,
                       bool isParamArray = false)
            : this(name, defaultValue, defaultValueType, defaultConstantIdentifier, ordinal, isOut, isRef, isParamArray)
        {
            _type = new RDomReferencedType(this, typeName, true);
        }

        public RDomParameter(string name, IReferencedType type,
                     object defaultValue, LiteralKind defaultValueType, string defaultConstantIdentifier,
                     int ordinal = 0, bool isOut = false, bool isRef = false,
                     bool isParamArray = false)
            : this(name, defaultValue, defaultValueType, defaultConstantIdentifier, ordinal, isOut, isRef, isParamArray)
        {
            _type = type;
        }

        private RDomParameter(string name,
                   object defaultValue, LiteralKind defaultValueType, string defaultConstantIdentifier,
                   int ordinal = 0, bool isOut = false, bool isRef = false,
                   bool isParamArray = false)
            : base()
        {
            _name = name;
            _ordinal = ordinal;
            _isOut = isOut;
            _isRef = isRef;
            _isParamArray = isParamArray;
            _isOptional = defaultValueType != LiteralKind.Unknown;
            if (IsOptional)
            {
                _defaultValue = defaultValue;
                _defaultValueType = defaultValueType;
                _defaultConstantIdentifier = defaultConstantIdentifier;
            }
        }

        public RDomParameter(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomParameter(RDomParameter oldRDom)
            : base(oldRDom)
        {
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            _name = oldRDom.Name;
            if (oldRDom.Type != null) { _type = oldRDom.Type.Copy(); }
            _ordinal = oldRDom.Ordinal;
            _isOut = oldRDom.IsOut;
            _isRef = oldRDom.IsRef;
            _isParamArray = oldRDom.IsParamArray;
            _isOptional = oldRDom.IsOptional;
            _defaultValue = oldRDom.DefaultValue;
            _defaultValueType = oldRDom.DefaultValueType;
            _defaultConstantIdentifier = oldRDom.DefaultConstantIdentifier;
        }

        public AttributeCollection Attributes
        { get { return _attributes; } }

        public override object RequestValue(string propertyName)
        {
            if (propertyName == "TypeName")
            { return Type.Name; }
            return base.RequestValue(propertyName);
        }

        private string _name;

        [Required]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private IReferencedType _type;

        [Required]
        public IReferencedType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private int _ordinal;

        public int Ordinal
        {
            get { return _ordinal; }
            set { SetProperty(ref _ordinal, value); }
        }

        private bool _isOut;

        public bool IsOut
        {
            get { return _isOut; }
            set { SetProperty(ref _isOut, value); }
        }

        private bool _isRef;

        public bool IsRef
        {
            get { return _isRef; }
            set { SetProperty(ref _isRef, value); }
        }

        private bool _isParamArray;

        public bool IsParamArray
        {
            get { return _isParamArray; }
            set { SetProperty(ref _isParamArray, value); }
        }

        private bool _isOptional;

        public bool IsOptional
        {
            get { return _isOptional; }
            set { SetProperty(ref _isOptional, value); }
        }

        private object _defaultValue;

        public object DefaultValue
        {
            get { return _defaultValue; }
            set { SetProperty(ref _defaultValue, value); }
        }

        private LiteralKind _defaultValueType;

        public LiteralKind DefaultValueType
        {
            get { return _defaultValueType; }
            set { SetProperty(ref _defaultValueType, value); }
        }

        private string _defaultConstantIdentifier;

        public string DefaultConstantIdentifier
        {
            get { return _defaultConstantIdentifier; }
            set { SetProperty(ref _defaultConstantIdentifier, value); }
        }
    }
}