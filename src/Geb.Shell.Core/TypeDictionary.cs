/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Geb.Shell.Core
{
    public class TypeDictionary
    {
        public String Name { get; private set; }

        public TypeDictionary Parent { get; set; }

        public String FullName{get;set;}

        public SortedDictionary<String, TypeDictionary> SubTypeDictionary { get; set; }

        public SortedDictionary<String, Type> Types { get; set; }

        public void ListType(String match)
        {
            Regex re = null;

            if (match != null)
            {
                re = new Regex(match);
            }

            foreach (Type t in Types.Values)
            {
                String name = t.Name;
                if (re != null)
                {
                    if (!re.IsMatch(name)) continue;
                }

                String typeClass = String.Empty;

                if (t.IsClass) typeClass += "C";
                else
                {
                    if (t.IsInterface) typeClass += "I";
                    else typeClass += "S";
                }

                Console.WriteLine(typeClass + ":\t" + Context.EnsureAtLeastLength(name,20) + "\t" + t.FullName);
            }
        }

        public void ListDir(String match)
        {
            Regex re = null;

            if (match != null)
            {
                re = new Regex(match);
            }

            foreach (TypeDictionary dic in SubTypeDictionary.Values)
            {
                String name = dic.Name;
                if (re != null)
                {
                    if (!re.IsMatch(name)) continue;
                }
                Console.WriteLine("N:\t" + Context.EnsureAtLeastLength(name, 20) + "\t" + dic.FullName);
            }
        }

        public TypeDictionary(String name)
        {
            Name = name;
            FullName = name;
            SubTypeDictionary = new SortedDictionary<string, TypeDictionary>();
            Types = new SortedDictionary<string, Type>();
        }
    }
}
