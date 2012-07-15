/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

using Geb.Shell.Core;

namespace Geb.Shell
{
    class Program
    {
        static StringBuilder cmdCache;
        static Context cxt;
        static String prefix;
        static Program()
        {
            cmdCache = new StringBuilder();
            cxt = new Context();
            prefix = ">> ";
        }

        static void Main(string[] args)
        {
            Console.Title = "OcrShell (by xiaotie@gmail.com [blog: http://xiaotie.cnblogs.com])";
            Console.Write(prefix);
            while ( true )
            {
                ConsoleKeyInfo ki = Console.ReadKey();
                switch (ki.Key)
                {
                    case ConsoleKey.Enter:
                        if (AcceptKeyEnter() == false) goto END;
                        break;
                    case ConsoleKey.Tab:
                        AcceptKeyTap();
                        break;
                    default:
                        cmdCache.Append(ki.KeyChar);
                        break;
                }
            }

        END:
            return;
        }

        static Boolean AcceptKeyEnter()
        {
            String cmd = cmdCache.ToString().Trim();
            if (cmd == "exit")
                return false;
            else if (!String.IsNullOrEmpty(cmd))
            {
                Console.Write(Environment.NewLine);
                cxt.Invoke(cmd);
                Console.Write(Environment.NewLine);
                Console.Write(prefix);
                cmdCache = new StringBuilder();
            }
            return true;
        }

        static void AcceptKeyTap()
        {
            Console.SetCursorPosition(Console.CursorLeft - 2, Console.CursorTop);
            String cmd = cmdCache.ToString().Trim();
            String[] tokens = cmd.Split(' ');
            if (tokens.Length == 0) return;

            String token = tokens[tokens.Length - 1].Trim();
            if (String.IsNullOrEmpty(token) == true) return;

            List<String> finds = cxt.IntelligenceComplete(token);
            if (finds.Count == 0)
            {
                Console.Beep();
            }
            else if (finds.Count == 1)
            {
                Console.SetCursorPosition(Console.CursorLeft - token.Length, Console.CursorTop);
                Console.Write(finds[0]);
                cmdCache = new StringBuilder();
                for (int i = 0; i < tokens.Length - 1; i++)
                {
                    cmdCache.Append(tokens[i]);
                }
                cmdCache.Append(finds[0]);
            }
            else
            {
                Console.Write(Environment.NewLine);
                foreach (var item in finds)
                {
                    Console.Write(item);
                    Console.Write("  ");
                }
                Console.Write(Environment.NewLine);
                Console.Write(prefix);
                Console.Write(cmdCache.ToString());
            }
        }
    }
}
