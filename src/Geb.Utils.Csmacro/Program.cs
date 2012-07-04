using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Geb.Utils.Csmacro
{
    class Program
    {
        static Regex includeReg = new Regex("#region\\s+include.+\\s+#endregion");
        static Regex mixinReg = new Regex("(?<=#region\\s+mixin\\s)[\\s|\\S]+(?=#endregion)");
        
        /// <summary>
        /// Csmacro [dir|filePath]
        /// 
        /// 语法：
        ///     #region include ""
        ///     #endregion
        ///     
        /// </summary>
        /// <param name="args"></param>
        static void 
            #region include<>
                Main
            #endregion
            (string[] args)
        {
            if (args.Length != 1)
            {
                PrintHelp();
                return;
            }

            String filePath = args[0];

            Path.GetDirectoryName(filePath);
            String dirName = Path.GetDirectoryName(filePath);
#if DEBUG
            Console.WriteLine("dir:" + dirName);
#endif
            String fileName = Path.GetFileName(filePath);
#if DEBUG
            Console.WriteLine("file:" + fileName);
#endif

            if (String.IsNullOrEmpty(fileName))
            {
                Csmacro(new DirectoryInfo(dirName));
            }
            else
            {
                if (fileName.EndsWith(".cs") == false)
                {
                    Console.WriteLine("Csmacro只能处理后缀为.cs的源程序.");
                }
                else
                {
                    Csmacro(new FileInfo(fileName));
                }
            }

            Console.WriteLine("[Csmacro]:处理完毕.");

#if DEBUG
            Console.ReadKey();
#endif
        }

        static void Csmacro(DirectoryInfo di)
        {
            Console.WriteLine("[Csmacro]:进入目录" + di.FullName);

            foreach (FileInfo fi in di.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                Csmacro(fi);
            }
        }

        static void Csmacro(FileInfo fi)
        {
            String fullName = fi.FullName;
            if (fi.Exists == false)
            {
                Console.WriteLine("[Csmacro]:文件不存在-" + fullName);
            }
            else if (fullName.EndsWith("_Csmacro.cs"))
            {
                return;
            }
            else
            {
                String text = File.ReadAllText(fullName);

                DirectoryInfo parrentDirInfo = fi.Directory;

                MatchCollection mc = includeReg.Matches(text);
                if (mc == null || mc.Count == 0) return;
                else
                {
                    Console.WriteLine("[Csmacro]:处理文件" + fullName);

                    StringBuilder sb = new StringBuilder();

                    Int32 from = 0;
                    foreach (Match item in mc)
                    {
                        sb.Append(text.Substring(from, item.Index - from));
                        from = item.Index + item.Length;
                        sb.Append(Csmacro(parrentDirInfo, item.Value));
                    }

                    sb.Append(text.Substring(from, text.Length - from));

                    String newName = fullName.Substring(0, fullName.Length - 3) + "_Csmacro.cs";
                    if (File.Exists(newName))
                    {
                        Console.WriteLine("[Csmacro]:删除旧文件" + newName);
                    }
                    File.WriteAllText(newName, sb.ToString());
                    Console.WriteLine("[Csmacro]:生成文件" + newName);
                }
            }
        }

        static String Csmacro(DirectoryInfo currentDirInfo, String text)
        {
            String outfilePath = text.Replace("#region", String.Empty).Replace("#endregion", String.Empty).Replace("include",String.Empty).Replace("\"",String.Empty).Trim();
            try
            {
                if (Path.IsPathRooted(outfilePath) == false)
                {
                    outfilePath = currentDirInfo.FullName + @"\" + outfilePath;
                }
                
                FileInfo fi = new FileInfo(outfilePath);
                if (fi.Exists == false)
                {
                    Console.WriteLine("[Csmacro]:文件" + fi.FullName + "不存在.");
                    return text;
                }
                else
                {
                    return GetMixinCode(File.ReadAllText(fi.FullName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Csmacro]:出现错误(" + outfilePath + ")-" + ex.Message);
            }
            finally
            {
            }
            return text;
        }

        static String GetMixinCode(String txt)
        {
            Match m = mixinReg.Match(txt);
            if (m.Success == true)
            {
                return m.Value;
            }
            else return String.Empty;
        }

        static void PrintHelp()
        {
            Console.WriteLine("Csmacro [dir|filePath]");
        }
    }
}
