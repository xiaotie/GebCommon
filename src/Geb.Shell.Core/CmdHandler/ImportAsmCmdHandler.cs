/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Geb.Shell.Core.CmdHandler
{
    public class ImportAsmCmdHandler : AbstractCmdHandler
    {
        public ImportAsmCmdHandler(Context cxt, String fullCmdString, String mark, IList<String> args)
            : base(cxt, fullCmdString, mark, args)
        {
        }

        public override String Run()
        {
            if (Args != null && Args.Count == 1)
            {
                  String path =  Args[0].Trim();
                  return ImportAsm(path); ;
            }
            return String.Empty;
        }

        public String ImportAsm(String path)
        {
            Assembly a;
            if (Mark != null && Mark.Contains('f'))
            {
                a = Assembly.LoadFile(System.IO.Path.GetFullPath(path));
            }
            else
            {
                a = Assembly.Load(path);
                if (a == null)
                {
                    a = Assembly.LoadFile(System.IO.Path.GetFullPath(path));
                }
            }

            if (a == null)
            {
                return path + " not found.";
            }
            else
            {
                Context.ImportAsm(a);
                return path + " imported.";
            }
        }

    }
}
