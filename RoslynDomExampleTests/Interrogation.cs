﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;
using System.Linq;

namespace RoslynDomExampleTests
{
    /// <summary>
    /// Summary description for Interrogation
    /// </summary>
    [TestClass]
    public class Interrogation
    {
        [TestMethod]
        public void Interrogate_using_statements()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var usings = root.UsingDirectives.ToArray();
            Assert.AreEqual(4, usings.Count());
            Assert.AreEqual("System", usings[0].Name);
            Assert.AreEqual("System.Diagnostics.CodeAnalysis", usings[1].Name);
            Assert.AreEqual("System.Diagnostics.Tracing", usings[2].Name);
            Assert.AreEqual("System.Linq", usings[3].Name);
        }

        [TestMethod]
        public void Interrogate_non_empty_namespaces()
        {
            // Nonempty namespaces are anticipated to be the primary namespace access mechanism.
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspaces = root.Namespaces.ToArray();
            Assert.AreEqual(2, nspaces.Count());
            Assert.AreEqual("testing", nspaces[0].Name);
            Assert.AreEqual("testing.Namespace3.testing", nspaces[0].QualifiedName);
            Assert.AreEqual("Namespace2", nspaces[1].Name);
        }

        [TestMethod]
        public void Interrogate_child_namespaces()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspaces = root.ChildNamespaces.ToArray();
            Assert.AreEqual(3, nspaces.Count());
            Assert.AreEqual("testing", nspaces[0].Name);
            Assert.AreEqual("testing", nspaces[1].Name);
            Assert.AreEqual("testing.Namespace1", nspaces[1].ChildNamespaces.First().QualifiedName);
            Assert.AreEqual("Namespace2", nspaces[2].Name);
        }

        [TestMethod]
        public void Interrogate_all_namespaces()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspaces = root.DescendantNamespaces.ToArray();
            Assert.AreEqual(6, nspaces.Count());
            Assert.AreEqual("testing", nspaces[0].Name);
            Assert.AreEqual("Namespace3", nspaces[1].Name);
            Assert.AreEqual("testing.Namespace3.testing", nspaces[2].QualifiedName);
            Assert.AreEqual("testing", nspaces[3].QualifiedName);
            Assert.AreEqual("testing.Namespace1", nspaces[4].QualifiedName);
            Assert.AreEqual("Namespace2", nspaces[5].Name);
        }

        [TestMethod]
        public void Interrogate_classes_in_namespaces()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var classes = nspace.Classes.ToArray();
            Assert.AreEqual(2, classes.Count());
            Assert.AreEqual("FooClass", classes[0].Name);
            Assert.AreEqual("BarClass", classes[1].Name);
        }

        [TestMethod]
        public void Interrogate_classes_in_root()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var classes = root.RootClasses.ToArray();
            Assert.AreEqual(3, classes.Count());
            Assert.AreEqual("FooClass1", classes[0].Name);
            Assert.AreEqual("FooClass", classes[1].Name);
            Assert.AreEqual("BarClass", classes[2].Name);
        }

        [TestMethod]
        public void Interrogate_structs_in_namespace()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var structures = nspace.Structures.ToArray();
            Assert.AreEqual(1, structures.Count());
            Assert.AreEqual("FooStruct", structures[0].Name);
        }

        [TestMethod]
        public void Interrogate_interfaces_in_namespace()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var interfaces = nspace.Interfaces.ToArray();
            Assert.AreEqual(1, interfaces.Count());
            Assert.AreEqual("FooInterface", interfaces[0].Name);
        }

        [TestMethod]
        public void Interrogate_enums_in_namespace()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var enums = nspace.Enums.ToArray();
            Assert.AreEqual(1, enums.Count());
            Assert.AreEqual("FooEnum", enums[0].Name);
        }

        [TestMethod]
        public void Interrogate_methods_in_class()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var class1 = root.Namespaces.Last().Classes.First();
            var methods = class1.Methods.ToArray();
            Assert.AreEqual(1, methods.Count());
            Assert.AreEqual("FooMethod", methods[0].Name);
        }

        [TestMethod]
        public void Interrogate_fields_in_class()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var class1 = root.Namespaces.Last().Classes.First();
            var fields = class1.Fields.ToArray();
            Assert.AreEqual(1, fields.Count());
            Assert.AreEqual("FooField", fields[0].Name);
        }

        [TestMethod]
        public void Interrogate_properties_in_class()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var class1 = root.Namespaces.Last().Classes.First();
            var properties = class1.Properties.ToArray();
            Assert.AreEqual(1, properties.Count());
            Assert.AreEqual("FooProperty", properties[0].Name);
        }

        [TestMethod]
        public void Interrogate_parameters_for_method()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var class1 = root.Namespaces.Last().Classes.First();
            var method = class1.Methods.First();
            var parameters = method.Parameters.ToArray();
            Assert.AreEqual(2, parameters.Count());
            Assert.AreEqual("FooMethod", method.Name);
            Assert.AreEqual("String", method.ReturnType.Name);
            Assert.AreEqual(AccessModifier.Public, method.AccessModifier);
            Assert.IsFalse(method.IsAbstract);
            Assert.IsFalse(method.IsExtensionMethod);
            Assert.IsFalse(method.IsOverride);
            Assert.IsFalse(method.IsSealed);
            Assert.IsFalse(method.IsStatic);
            Assert.IsFalse(method.IsVirtual);
            Assert.IsFalse(method.IsVirtual);
            Assert.AreEqual("bar1", parameters[0].Name);
            Assert.AreEqual("Int32", parameters[0].Type.Name);
            Assert.AreEqual(0, parameters[0].Ordinal);
            Assert.AreEqual("bar2", parameters[1].Name);
            Assert.AreEqual("String", parameters[1].Type.Name);
            Assert.AreEqual(1, parameters[1].Ordinal);
            Assert.IsFalse(parameters[1].IsOptional);
            Assert.IsFalse(parameters[1].IsOut);
            Assert.IsFalse(parameters[1].IsParamArray);
        }

        [TestMethod]
        public void Interrogate_attributes_for_class()
        {
            IRoot root = RDom.CSharp.LoadFromFile(@"..\..\TestFile.cs");
            var class1 = root.Namespaces.Last().Classes.First();
            var attributes = class1.Attributes.Attributes.ToArray();
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("ExcludeFromCodeCoverage", attributes[0].Name);
            Assert.AreEqual("EventSource", attributes[1].Name);
            Assert.AreEqual("Name", attributes[1].AttributeValues.First().Name);
            Assert.AreEqual("George", attributes[1].AttributeValues.First().Value);
            Assert.AreEqual(LiteralKind.String, attributes[1].AttributeValues.First().ValueType);
        }
    }
}