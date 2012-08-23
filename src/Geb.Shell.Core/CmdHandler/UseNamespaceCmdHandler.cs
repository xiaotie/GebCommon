/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geb.Shell.Core.CmdHandler
{
    public class UseNamespaceCmdHandler : AbstractCmdHandler
    {
        public UseNamespaceCmdHandler(Context cxt, String fullCmdString, String mark, IList<String> args)
            : base(cxt, fullCmdString, mark, args)
        {
        }

        public override String Run()
        {
            if (Args == null || Args.Count == 0)
            {
                Context.ListNamespace();
            }
            else if (Args.Count == 1)
            {
                String ns = Args[0].Trim();

                if (!String.IsNullOrEmpty(ns))
                {
                    Context.AddNamespace(ns);
                    return Context.ListNamespace();
                }
            }

            return String.Empty;
        }
    }

    public class RemoveNamespaceCmdHandler : AbstractCmdHandler
    {
        public RemoveNamespaceCmdHandler(Context cxt, String fullCmdString, String mark, IList<String> args)
            : base(cxt, fullCmdString, mark, args)
        {
        }

        public override String Run()
        {
            if (Args != null && Args.Count == 1)
            {
                String ns = Args[0].Trim();

                if (!String.IsNullOrEmpty(ns))
                {
                    Context.RemoveNamespace(ns);
                    return Context.ListNamespace();
                }
            }
            return Environment.NewLine;
        }
    }
}
