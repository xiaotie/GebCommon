/***************************************************************
 * Copyright: xiaotie@gmail.com(http://xiaotie.cnblogs.com)
 * *************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using System.IO;
using System.Xml;

namespace Geb.Shell.Core
{
    public class Context
    {
        public static String EnsureAtLeastLength(String input, Int32 length)
        {
            String output = input;
            if (input.Length < length)
            {
                int count = length - input.Length;
                for (int i = 0; i < count; i++)
                {
                    output += " ";
                }
            }

            return output;
        }

        public Boolean Debug { get; set; }

        public IDictionary<String, Assembly> Assemblys { get; set; }
        public TypeManager TypeManager { get; set; }
        public SortedDictionary<String, String> AliasCmds { get; set; }
        public SortedDictionary<String, Object> Instances {get;set;}
        public SortedDictionary<String, String> Namespaces { get; set; }
        public String ConsoleLine { get; private set; }

        private Dictionary<string, string> IntelligenceContext;

        public CSharpCodeProvider CodeProvider { get; set; }

        public Object this[String instanceName]
        {
            get
            {
                if(Instances.ContainsKey(instanceName))
                {
                    return Instances[instanceName];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if(Instances.ContainsKey(instanceName))
                {
                    Instances.Remove(instanceName);
                }
                Instances.Add(instanceName,value);
            }
        }

        public Context()
        {
            IntelligenceContext = new Dictionary<string, string>();
            this.AddIntelligenceContextWord(CmdDispatcher.CMDS);

            Debug = false;
            AliasCmds = new SortedDictionary<string, string>();
            LoadAliasCmds();

            CodeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });

            Instances = new SortedDictionary<String,Object>();
            TypeManager = new TypeManager();
            Namespaces = new SortedDictionary<string, string>();
            Assemblys = new Dictionary<String, Assembly>();
            InitNamespaces();
            InitHelpDoc();

            ConsoleLine = CreateConsoleWidthLine();

            Assembly[] al = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in al)
            {
                AddAssembly(a);
            }

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
        }

        public CompilerParameters CreateCompilerParameters()
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            if (Assemblys != null)
            {
                foreach (Assembly a in Assemblys.Values)
                {
                    cp.ReferencedAssemblies.Add(a.Location);
                }
            }
            return cp;
        }

        private void LoadAliasCmds()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "Alias.xml";
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNodeList xnl = doc["data"].ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    String sc = xn.Attributes["shortCmd"].Value;
                    String fc = xn.Attributes["fullCmd"].Value;
                    if (!AliasCmds.ContainsKey(sc))
                    {
                        AliasCmds.Add(sc, fc);
                    }
                }
            }
        }

        private void AddAssembly(Assembly a)
        {
            if (a != null)
            {
                Assemblys.Add(a.FullName, a);

                Type[] tl = a.GetTypes();

                foreach (Type t in tl)
                {
                    // 只添加 public 类型
                    if(t.IsPublic && !t.FullName.StartsWith("<PrivateImplementationDetails>"))
                        TypeManager.AddType(t);
                }
            }
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Assembly a = args.LoadedAssembly;
            if (!Assemblys.ContainsKey(a.FullName))
            {
                AddAssembly(a);
            }
        }

        public void Invoke(String cmd)
        {
            try
            {
                CmdDispatcher pt = new CmdDispatcher(this, cmd);
                pt.Dispatch();
            }
            catch (Exception e)
            {
                if (Debug)
                {
                    Console.WriteLine(e.ToString());
                }
                else
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Save(String argName, Object arg)
        {
            this[argName] = arg;
        }

        public void ListInstances()
        {
            foreach (String key in Instances.Keys)
            {
                Console.WriteLine(key + "\t" + Instances[key]);
            }
        }

        public void ListAlias()
        {
            foreach (String sc in AliasCmds.Keys)
            {
                Console.WriteLine(sc + "\t" + AliasCmds[sc]);
            }
        }

        public String GetFullCmd(String shortCmd)
        {
            if (AliasCmds.ContainsKey(shortCmd))
            {
                return AliasCmds[shortCmd];
            }
            return null;
        }

        public void AddNamespace(String ns)
        {
            if (!Namespaces.ContainsKey(ns))
            {
                Namespaces.Add(ns, null);
            }
        }

        public void RemoveNamespace(String ns)
        {
            if (Namespaces.ContainsKey(ns))
            {
                Namespaces.Remove(ns);
            }
        }

        public void ListNamespace()
        {
            Console.WriteLine("Namespaces using:");
            Console.WriteLine(ConsoleLine);
            foreach (String s in Namespaces.Keys)
            {
                Console.WriteLine(s);
            }
        }

        public void InitNamespaces()
        {
            AddNamespace("System");
            AddNamespace("System.IO");
            AddNamespace("System.Text");
            AddNamespace("System.Collections.Generic");
            AddNamespace("Orc.Shell.Core");
        }

        public static String CreateConsoleWidthLine()
        {
            Int32 count = Console.WindowWidth;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < count - 1; i++) sb.Append('-');

            return sb.ToString();
        }

        public XmlDocument HelpDoc { get; private set; }

        public void InitHelpDoc()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "CmdHelp.xml";
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                HelpDoc = doc;
            }
        }

        public void PrintHelp()
        {
            if (HelpDoc == null) return;

            XmlDocument doc = HelpDoc;
            XmlNodeList xnl = doc["cmds"].ChildNodes;
            if (xnl != null)
            {
                foreach (XmlNode n in xnl)
                {
                    if(n.Name.Equals("cmd"))
                    {
                        String cmd = n.Attributes["name"].Value;
                        String note = n.InnerText;
                        Console.WriteLine(cmd + "\t" + note);
                    }
                }
            }
        }

        public void ImportAsm(Assembly asm)
        {
            AppDomain.CurrentDomain.Load(asm.GetName());
        }

        public void ListAsms()
        {
            if (Assemblys != null)
            {
                Console.WriteLine("Assemblys Loaded:");
                Console.WriteLine(ConsoleLine);
                foreach (Assembly a in Assemblys.Values)
                {
                    Console.WriteLine(a.FullName);
                }
            }
        }

        /// <summary>
        /// 执行 doc 命令
        /// </summary>
        /// <param name="dosCommand"> doc 指令 </param>
        public void ExecuteDosCmd(String dosCommand)
        {
            string output = "";     //输出字符串
            if (!String.IsNullOrEmpty(dosCommand))
            {
                Process process = new Process();     //创建进程对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";      //设定需要执行的命令
                startInfo.Arguments = "/C " + dosCommand;   //设定参数，其中的“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;     //不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;   //不重定向输入
                startInfo.RedirectStandardOutput = true;   //重定向输出
                startInfo.CreateNoWindow = true;     //不创建窗口
                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
                process.StartInfo = startInfo;
                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }

        public List<String> IntelligenceComplete(String input)
        {
            List<String> find = new List<string>();
            foreach (var key in this.IntelligenceContext.Keys)
            {
                if (key.StartsWith(input))
                {
                    find.Add(this.IntelligenceContext[key]);
                }
            }
            find.Sort();
            return find;
        }

        public Boolean AddIntelligenceContextWord(String word)
        {
            if (String.IsNullOrEmpty(word) == true) return false;
            String lower = word.ToLower();
            if (this.IntelligenceContext.ContainsKey(lower) == false)
            {
                this.IntelligenceContext.Add(lower, word);
                return true;
            }
            return false;
        }

        public void AddIntelligenceContextWord(IEnumerable<String> words)
        {
            foreach (var w in words)
                this.AddIntelligenceContextWord(w);
        }

        public void RemoveIntelligenceContextWord(String word)
        {
            if (String.IsNullOrEmpty(word) == true) return;
            this.IntelligenceContext.Remove(word.ToLower());
        }

        public void ClearIntelligenceContext()
        {
            this.IntelligenceContext.Clear();
        }
    }
}
