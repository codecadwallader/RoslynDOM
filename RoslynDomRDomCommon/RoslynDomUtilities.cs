﻿using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom
{
    public static class RoslynDomUtilities
    {
        public static IEnumerable<INamespace> GetDescendantNamespaces(
            IStemContainer stemContainer,
            bool includeSelf = false)
        {
            var ret = new List<INamespace>();
            if (includeSelf)
            {
                var nspace = stemContainer as INamespace;
                if (nspace != null) ret.Add(nspace);
            }
            foreach (var child in stemContainer.ChildNamespaces)
            {
                ret.AddRange(GetDescendantNamespaces(child, true));
            }
            return ret;
        }

        public static IEnumerable<INamespace> GetNonEmptyNamespaces(
            IStemContainer stemContainer)
        {
            return GetDescendantNamespaces(stemContainer)
                .Where(x => x.StemMembers.Where(y => y.StemMemberKind != StemMemberKind.Namespace).Count() != 0);
        }

        //public static IEnumerable<T> CopyMembers<T>(IEnumerable<T> members)
        //{
        //   var ret = new List<T>();
        //   if (members != null)
        //   {
        //      foreach (var member in members)
        //      {
        //         ret.Add(Copy(member));
        //      }
        //   }
        //   return ret;
        //}

        //public static T Copy<T>(T oldItem)
        //{
        //   var type = oldItem.GetType();
        //   var constructors = type.GetTypeInfo()
        //       .DeclaredConstructors
        //       .Where(x => x.GetParameters().Count() == 1
        //       && type.IsAssignableFrom(x.GetParameters().First().ParameterType));
        //   Guardian.Assert.RDomHasOneCloneContructor(constructors, type);
        //   var constructor = constructors.FirstOrDefault();
        //   var newItem = constructor.Invoke(new object[] { oldItem });
        //   return (T)newItem;
        //}

        public static string GetNamespace(IDom item)
        {
            var ret = "";
            if (item == null) { return ret; }
            var itemAsNamespace = item as INamespace;
            if (itemAsNamespace != null) { ret += itemAsNamespace.Name; }
            if (item.Parent == null || item.Parent is IRoot) { return ret; }
            var parentNamespace = GetNamespace(item.Parent);
            if (!string.IsNullOrEmpty(parentNamespace))
            { ret = parentNamespace + (string.IsNullOrEmpty(ret) ? "" : "." + ret); }
            return ret;
        }
    }
}