/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Geb.Shell.Core.CmdHandler;

namespace Geb.Shell.Core
{
    public class CmdDispatcher
    {
        public static readonly String[] SPLITS = { " ", "\t", "\n" };
        public static readonly String[] CMDS = 
        {
            "debug",
            "undebug",
            "help",
            "asms",
            "cd",
            "cdc",
            "ls",
            "lsc",
            "grep",
            "ping",
            "import",
            "use",
            "unuse",
            "dirc",
            "dir",
            "my",
            "alias"
        };

        public Context Context { get; private set; }
        public String InputCmdString { get; private set; }
        public CmdDispatcher(Context cxt, String cmd)
        {
            Context = cxt;
            InputCmdString = cmd;
        }

        public static Assembly Find(String filePath)
        {
            return AppDomain.CurrentDomain.Load(filePath);
        }

        public void Dispatch()
        {
            String[] results = InputCmdString.Split(SPLITS, StringSplitOptions.None);
            if(results.Length == 0) return;

            String cmd = results[0];
            String mark = String.Empty;
            IList<String> args = new List<String>();
            
            Int32 argIndex = 1;

            if (results.Length > 1 && results[1].StartsWith("-"))
            {
                argIndex ++;
                mark = results[1];
            }

            for(;argIndex < results.Length;argIndex++)
            {
                args.Add(results[argIndex]);
            }

            String argString = String.Empty;
            foreach (String a in args)
            {
                argString += " " + a;
            }

            argString = argString.Trim();

            switch (cmd.ToLower())
                {
                    case "debug":   // 开启debug开关
                        Context.Debug = true;
                        break;
                    case "undebug": // 关闭debug开关
                        Context.Debug = false;
                        break;
                    case "help":
                        Context.PrintHelp();
                        break;
                    case "asms":    // 列出加载的程序集
                        Context.ListAsms();
                        break;
                    case "cd":
                        if(!String.IsNullOrEmpty(argString))
                            FileSystem.ChDir(Path.GetFullPath(argString));
                        break;
                    case "ls":      // Unix下的ls命令
                        Context.ExecuteDosCmd("dir " + mark + argString);
                        break;
                    case "dir":
                    case "grep":
                    case "ping":
                        Context.ExecuteDosCmd(InputCmdString);
                        break;
                    case "import":  // 导入程序集
                        new ImportAsmCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    case "use":     // 引入命名空间
                        new UseNamespaceCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    case "unuse":   // 移除引入的命名空间
                        new RemoveNamespaceCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    case "cdc":     // 改变命名空间
                        new CdClassCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    case "lsc":     // 列出命名空间的内容
                    case "dirc":
                        new ListClassCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    case "my":      // 列出用户变量
                        new ListInstanceCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    case "alias":   // 列出alias列表
                        new ListAliasCmdHandler(Context, InputCmdString, mark, args).Run();
                        break;
                    default:
                        String fullCmd = Context.GetFullCmd(cmd);
                        if (fullCmd != null)    // 处理 alias
                        {
                            if (mark != null) fullCmd += " " + mark;
                            if (args != null && args.Count > 0)
                            {
                                foreach(String s in args)
                                {
                                    fullCmd += " " + s;
                                }
                            }

                            Context.Invoke(fullCmd);
                        }
                        else                   // 编译代码并运行
                        {
                            new CscCmdHandler(Context, InputCmdString).Run();
                        }
                        break;
                }

            return;
        }
    }
}
