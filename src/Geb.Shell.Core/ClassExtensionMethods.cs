/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Geb.Shell.Core
{
    public static class ClassExtensionMethods
    {
        public static void Print(this Object obj)
        {
            Console.WriteLine(obj);
        }

        public static void p(this Object obj)
        {
            Print( obj );
        }

        public static void P(this Object obj)
        {
            Print(obj);
        }

        public static void print(this Object obj)
        {
            Print(obj);
        }

        public static void methods(this Type t)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                Console.WriteLine("  " + mi);
            }
        }

        public static void methods(this Object obj)
        {
            if (obj == null) return;

            methods(obj.GetType());
        }

        public static void props(this Type t)
        {
            foreach (PropertyInfo pi in t.GetProperties())
            {
                Console.WriteLine("  " + pi);
            }
        }

        public static void props(this Object obj)
        {
            if (obj == null) return;

            props(obj.GetType());
        }

        public static void members(this Type t)
        {
            foreach (MemberInfo mi in t.GetMembers())
            {
                Console.WriteLine("  " + mi);
            }
        }

        public static void members(this Object obj)
        {
            if (obj == null) return;

            members(obj.GetType());
        }

        public static void creaters(this Type t)
        {
            foreach (ConstructorInfo ci in t.GetConstructors())
            {
                Console.WriteLine("  " + ci);
            }
        }

        public static void creaters(this Object obj)
        {
            if (obj == null) return;

            creaters(obj.GetType());
        }

        public static int match(this String pattern, String text, Int32 startat)
        {
            Regex re = new Regex(pattern);
            Match m = re.Match(text,startat);
            if (m == null) return -1;
            else return m.Index;
        }

        public static int match(this String pattern, String text)
        {
            return match(pattern, text, 0);
        }
    }

    public static class ClassExtensionMethodsForIEnumerable
    {
        public static void PrintAll(this IEnumerable e, String splitString)
        {
            foreach (Object o in e)
            {
                Console.Write(o.ToString() + splitString);
            }
        }

        public static void PrintAllLine(this IEnumerable e)
        {
            foreach (Object o in e)
            {
                Console.WriteLine(o.ToString());
            }
        }
    }
}
