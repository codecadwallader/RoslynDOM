﻿using System;

namespace RoslynDom.Tests.Walkthrough_1_code
{
   public class Bar
   {
      private string firstName;
      private string lastName;

      public string Foo()
      {
         var ret = lastName;
         ret = Foo();
         ret = "xyz";

         // Comment and whitespace
         var xx = new String('a', 4);
         ret = "abc" + Foo();
         if (!string.IsNullOrEmpty(firstName))
         { ret = firstName + lastName; }
         // comment
         var x = ", ";
         uint y = 42;
         x = lastName + x + firstName;
         Foo2(x);
         return x;
      }

      private void Foo2(string dummy)
      {
         ulong x = 3;
         Console.WriteLine("Making up silly code to evaluate");
      }

      public string FooBar
      {
         get
         {
            ushort z = 432;
            return z.ToString();
         }
      }

      /// <summary>
      /// This is a test
      /// </summary>
      /// <param name="dummy">With a dummy parameter</param>
      public void Foo3(string dummy)
      {
         var x2 = 3;
         var x3 = x2;
         Console.WriteLine("Making up silly code to evaluate");
      }
   }
}