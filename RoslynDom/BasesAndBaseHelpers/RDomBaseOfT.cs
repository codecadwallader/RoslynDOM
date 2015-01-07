using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System;
using System.Linq;
using System.Reflection;

namespace RoslynDom
{
   // Provides a Roslyn specific non-generic base class
   public abstract class RoslynRDomBase : RDomBase, IRoslynHasSymbol
   {
      protected RoslynRDomBase()
         : base()
      { }

      protected RoslynRDomBase(IDom oldIDom)
         : base(oldIDom)
      { }

      public abstract ISymbol Symbol { get; }

      public override object RequestValue(string propertyName)
      {
         if (ReflectionUtilities.CanGetProperty(this, propertyName))
         {
            var value = ReflectionUtilities.GetPropertyValue(this, propertyName);
            return value;
         }
         return null;
      }
   }

   public abstract class RDomBase<T> : RoslynRDomBase, IDom<T>
         where T : class, IDom<T>
   {
      protected RDomBase()
         : base()
      { }

      protected RDomBase(T oldIDom)
         : base(oldIDom)
      { }

      public virtual T Copy()
      {
         var type = this.GetType();
         var constructor = type.GetTypeInfo()
             .DeclaredConstructors
             .Where(x => x.GetParameters().Count() == 1
             && typeof(T).IsAssignableFrom(x.GetParameters().First().ParameterType))
             .FirstOrDefault();
         if (constructor == null) throw new InvalidOperationException("Missing constructor for clone");
         var newItem = constructor.Invoke(new object[] { this });
         return (T)newItem;
      }

      protected override sealed bool SameIntentInternal<TLocal>(TLocal other)
      {
         var otherAsT = other as T;
         var thisAsT = this as T;
         if (otherAsT == null) return false;
         if (!CheckSameIntent(otherAsT)) { return false; }
         if (!StandardSameIntent.CheckSameIntent(thisAsT, otherAsT)) return false; ;
         return true;
      }

      /// <summary>
      /// Derived classes can override this if the RoslynDom.Common implementations aren't working.
      /// Do NOT override if the problem can be solved in the RoslynDom.Common implementations (SameIntent_xxx)
      /// </summary>
      /// <param name="other"></param>
      ///
      /// <returns></returns>
      protected virtual bool CheckSameIntent(T other)
      {
         return true;
      }
   }
}