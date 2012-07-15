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
    public abstract class AbstractCmdHandler
    {
        public Context Context { get; private set; }
        public String InputCmdString { get; private set; }
        public String Mark { get; private set; }
        public IList<String> Args { get; private set; }
        public String OutputCmdString { get; set; }

        public AbstractCmdHandler(Context cxt, String fullCmdString, String mark, IList<String> args)
        {
            Context = cxt;
            InputCmdString = fullCmdString;
            Mark = mark;
            Args = args;
        }

        public void ListClass(String match)
        {
            Console.WriteLine(Context.TypeManager.Now.FullName);
            Console.WriteLine(Context.ConsoleLine);
            Context.TypeManager.Now.ListDir(match);
            Context.TypeManager.Now.ListType(match);
            Console.WriteLine(Context.ConsoleLine);
        }

        public void ShowLocation()
        {
            Console.WriteLine("Location: " + Context.TypeManager.Now.FullName);
        }

        public abstract void Run();
    }
}
