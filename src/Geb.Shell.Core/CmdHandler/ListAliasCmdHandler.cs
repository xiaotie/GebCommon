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
    public class ListAliasCmdHandler : AbstractCmdHandler
    {
        public ListAliasCmdHandler(Context cxt, String fullCmdString, String mark, IList<String> args)
            : base(cxt, fullCmdString, mark, args)
        {
        }

        public override String Run()
        {
            return Context.ListAlias();
        }

    }

}
