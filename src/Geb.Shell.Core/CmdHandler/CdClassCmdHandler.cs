/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Geb.Shell.Core.CmdHandler
{
    public class CdClassCmdHandler : AbstractCmdHandler
    {
        public CdClassCmdHandler(Context cxt, String fullCmdString, String mark, IList<String> args)
            : base(cxt, fullCmdString, mark, args)
        {
        }

        public override void Run()
        {
            String match = null;

            if (Args != null && Args.Count == 1)
            {
                match = Args[0].Trim();
            }

            if (String.IsNullOrEmpty(match) || match.Equals("."))
            {
                ShowLocation();
                return;
            }

            if (match.Equals(".."))
            {
                Context.TypeManager.StepUp();
                ShowLocation();
            }
            else
            {
                TypeDictionary matchDic = null;

                IList<TypeDictionary> matchList = new List<TypeDictionary>();
                foreach (TypeDictionary td in Context.TypeManager.Now.SubTypeDictionary.Values)
                {
                    if (td.Name.Equals(match))
                    {
                        matchDic = td;
                        Context.TypeManager.StepDown(match);
                        ShowLocation();
                        return;
                    }

                    if (td.Name.StartsWith(match))
                    {
                        matchList.Add(td);
                    }
                }

                if (matchList.Count == 0)
                {
                    Console.WriteLine("指定的命名空间不存在.");
                }
                else if (matchList.Count == 1)  // 当只有1个匹配项时，则进入该匹配项
                {
                    Context.TypeManager.StepDown(matchList[0].Name);
                    ShowLocation();
                    return;
                }
                else // 超过1个匹配项时，保留在当前命名空间，同时打印全部匹配项
                {
                    foreach (TypeDictionary td in matchList)
                    {
                        Console.WriteLine(Context.EnsureAtLeastLength(td.Name, 24));
                    }
                }
            }
        }
    }
}
