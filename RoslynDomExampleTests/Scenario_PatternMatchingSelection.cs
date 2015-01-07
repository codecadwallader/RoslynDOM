﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;
using System.Collections.Generic;
using System.Linq;

// Common scenarios for loading the DOM are in RoslynDomLoad of this RoslynDomExampleTests project
namespace RoslynDomExampleTests
{
   /// <summary>
   ///
   /// </summary>
   /// <remarks>
   /// There are a set of scenarios that require either determining if a file matches a pattern
   /// (as in the code first work) or a subpart matches.
   /// <br/>
   /// In exploring this, I felt the concept of root classes was important. Just give me the root classes
   /// whether or not there are surrounding namespaces.
   /// <br/>
   /// This is one of the scenarios where I think annotations/design time attributes will be important, and
   /// almost certainly public ones.
   /// </remarks>
   [TestClass]
   public class Scenario_PatternMatchingSelection
   {
      #region Syntax

      [TestMethod]
      public void Can_select_root_classes_by_attribute()
      {
         var csharpCode = @"
using System
[A(), B]
public class Foo  {}
namespace Namespace1
{
   [C]
   [D, E]
   public class Foo1
   {
      [F]
      public class Foo3{}
   }
   public class Foo2{}
}
";

         var root = RDom.CSharp.Load(csharpCode);
         var rootAttributes = GetRootAttributeNames(root).ToArray();
         Assert.AreEqual(5, rootAttributes.Count());
         Assert.AreEqual("A", rootAttributes[0]);
         Assert.AreEqual("B", rootAttributes[1]);
         Assert.AreEqual("C", rootAttributes[2]);
         Assert.AreEqual("D", rootAttributes[3]);
         Assert.AreEqual("E", rootAttributes[4]);
         Assert.IsTrue(HasAttribute(root, "A"));
         Assert.IsTrue(HasAttribute(root, "B"));
         Assert.IsTrue(HasAttribute(root, "C"));
         Assert.IsTrue(HasAttribute(root, "D"));
         Assert.IsTrue(HasAttribute(root, "E"));
         Assert.IsFalse(HasAttribute(root, "F"));
      }

      private IEnumerable<string> GetRootAttributeNames(IRoot root)
      {
         var classAttributeNames = from x in root.RootClasses
                                   from a in x.Attributes.Attributes
                                   select a.Name;
         return classAttributeNames;
      }

      private bool HasAttribute(IRoot root, string name)
      {
         var attributeNames = GetRootAttributeNames(root);
         var matches = attributeNames.Where(x => x == name);
         return (matches.Count() > 0);
      }

      [TestMethod]
      public void Can_get_and_retrieve_public_annotations()
      {
         var csharpCode = @"
            //[[ file: kad_Test4(val1 = ""George"", val2 = 43) ]]
            //[[ kad_Test1(val1 : ""Fred"", val2 : 40) ]]
            using Foo;

            //[[ kad_Test2(""Bill"", val2 : 41) ]]
            //[[ kad_Test3(val1 =""Percy"", val2 : 42) ]]
            public class MyClass
            { }
            ";
         var root = RDom.CSharp.Load(csharpCode);

         var using1 = root.UsingDirectives.First();
         var kad_Test1 = root.GetMembers().OfType<IPublicAnnotation>().First(x => x.Name == "kad_Test1");
         var kad_Test2 = root.GetMembers().OfType<IPublicAnnotation>().First(x => x.Name == "kad_Test2");
         var kad_Test3 = root.GetMembers().OfType<IPublicAnnotation>().First(x => x.Name == "kad_Test3");
         var kad_Test4 = root.GetMembers().OfType<IPublicAnnotation>().First(x => x.Name == "kad_Test4");
         Assert.AreEqual("Fred", kad_Test1.GetValue<string>("val1"));
         Assert.AreEqual("Fred", kad_Test1.GetValue("val1"));
         Assert.AreEqual(40, kad_Test1.GetValue<int>("val2"));
         Assert.AreEqual(40, kad_Test1.GetValue("val2"));

         var class1 = root.RootClasses.First();
         Assert.AreEqual("Bill", kad_Test2.GetValue(""));
         Assert.AreEqual(41, kad_Test2.GetValue("val2"));
         Assert.AreEqual("Percy", kad_Test3.GetValue("val1"));
         Assert.AreEqual(42, kad_Test3.GetValue("val2"));
         Assert.AreEqual(42, kad_Test3.GetValue<int>("val2"));

         Assert.AreEqual("George", kad_Test4.GetValue("val1"));
         Assert.AreEqual(43, kad_Test4.GetValue("val2"));
      }

      #endregion Syntax
   }
}