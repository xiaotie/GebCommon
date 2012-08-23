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
    public class ObjectHelper<T>
    {
        private String m_objName;

        public Context Context { get; private set; }

        public T Obj
        {
            get 
            {
                Object obj = Context[m_objName];
                return (T)obj; 
            }
            set { Context[m_objName] = value; }
        }

        public ObjectHelper(Context cxt, String objName)
        {
            m_objName = objName;
            Context = cxt;
            
        }
    }

    public class CscCmdHandler : AbstractCmdHandler
    {
        public CscCmdHandler(Context cxt, String code)
            : this(cxt, code, String.Empty, null)
        {
        }

        public CscCmdHandler(Context cxt, String fullCmdString,String mark, IList<String> args)
            : base(cxt, fullCmdString, mark, args)
        {
        }

        public override String Run()
        {
            if (String.IsNullOrEmpty(InputCmdString)) return String.Empty;

            StringBuilder sb = new StringBuilder();

            String inputCmdString = InputCmdString.Trim();
            Regex re;

            // 处理未初始化的环境变量
            re = new Regex(@"^(\$)(\w)+");
            if (inputCmdString != null)
            {
                Match m = re.Match(inputCmdString);
                if (m != null && m.Length > 1)
                {
                    String outArgName = inputCmdString.Substring(m.Index, m.Length).Substring(1);
                    if (Context[outArgName] == null)
                    {
                        String innerArgName = "orcShellTempArg_" + outArgName;
                        inputCmdString = "var " + inputCmdString.Replace("$" + outArgName, innerArgName);
                        inputCmdString += ";Save(\"" + outArgName + "\"," + innerArgName + ");";
                    }
                }
            }

            // 处理其它环境变量
            re = new Regex(@"(\$)(\w)+");
            IDictionary<String, String> ArgsList = new Dictionary<String, String>();
            if (inputCmdString != null)
            {
                MatchCollection mc = re.Matches(inputCmdString);
                if (mc != null)
                {
                    foreach (Match m in mc)
                    {
                        if (m.Length > 1)
                        {
                            String outArgName = inputCmdString.Substring(m.Index, m.Length).Substring(1);
                            if (!ArgsList.ContainsKey(outArgName))
                            {
                                Object obj = Context[outArgName];
                                if (obj == null) throw new Exception("不存在环境变量" + outArgName);

                                String innerArgName = String.Format(@"(new Geb.Shell.Core.CmdHandler.ObjectHelper<{0}>(Context,""{1}"")).Obj", obj.GetType(), outArgName);
                                ArgsList.Add(outArgName, innerArgName);
                            }
                        }
                    }
                }

                foreach (String outArg in ArgsList.Keys)
                {
                    inputCmdString = inputCmdString.Replace("$" + outArg, ArgsList[outArg]);
                }
            }

            String fullCmd = BuildFullCmd(inputCmdString);

            CompilerResults cr = Context.CodeProvider.CompileAssemblyFromSource(Context.CreateCompilerParameters(), fullCmd);

            if (cr.Errors.HasErrors)
            {
                Boolean recompileSwitch = true;

                foreach (CompilerError err in cr.Errors)
                {
                    //CS0201 : Only assignment, call, increment, decrement, and new object expressions can be
                    //used as a statement
                    if ((!err.IsWarning) && (!err.ErrorNumber.Equals("CS0201")))
                    {
                        recompileSwitch = false;
                        break;
                    }
                }

                // 重新编译
                if (recompileSwitch)
                {
                    String dynaName = "orcShellTempArg_Dynamic_" + DateTime.Now.Ticks.ToString();
                    inputCmdString = String.Format("  var {0} = ", dynaName) + inputCmdString;
                    inputCmdString += ";\n  " + dynaName + ".p();";

                    fullCmd = BuildFullCmd(inputCmdString);
                    cr = Context.CodeProvider.CompileAssemblyFromSource(Context.CreateCompilerParameters(), fullCmd);
                }

                if (cr.Errors.HasErrors)
                {
                    sb.AppendLine("编译错误：");
                    sb.AppendLine(OutputCode(inputCmdString));
                    foreach (CompilerError err in cr.Errors)
                    {
                        Console.WriteLine(err.ErrorNumber);

                        if (Context.Debug)
                        {
                            sb.AppendLine(String.Format("line {0}: {1}", err.Line, err.ErrorText));
                        }
                        else
                        {
                            sb.AppendLine(err.ErrorText);
                        }
                    }

                    return sb.ToString();
                }
            }

            if (Context.Debug)
            {
                sb.AppendLine(inputCmdString);
            }

            Assembly assem = cr.CompiledAssembly;
            Object dynamicObject = assem.CreateInstance("Geb.Shell.Core.Dynamic.DynamicClass");
            Type t = assem.GetType("Geb.Shell.Core.Dynamic.DynamicClass");
            MethodInfo minfo = t.GetMethod("MethodInstance");
            minfo.Invoke(dynamicObject, new Object[] { Context });

            return sb.ToString();
        }

        private String OutputCode(String text)
        {
            String line = Context.ConsoleLine;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(line);
            sb.AppendLine(text);
            sb.AppendLine(line);
            return sb.ToString();
        }

        private String BuildFullCmd(String inputCmdString)
        {
            String fullCmd = String.Empty;
            
            foreach (String ns in Context.Namespaces.Keys)
            {
                fullCmd += "                using " + ns + ";";
            }

            if (Context.TypeManager.Now != Context.TypeManager.Root)
            {
                String location = Context.TypeManager.Now.FullName;
                if (Context.Namespaces[location] == null)
                {
                    fullCmd += "                using " + location + ";";
                }
            }


            fullCmd += @"
                namespace Geb.Shell.Core.Dynamic 
                { 
                    public class DynamicClass
                    {
                        public Geb.Shell.Core.Context Context;

                        public void Save(String name, Object obj)
                        {
                            Context.Save(name,obj);
                        }

                        public Object My(String name)
                        {
                            return Context[name];
                        }

                        public void MethodInstance(Context context)
                        {
                            Context = context;
                            " + inputCmdString + @";
                        }
                    }
                }";
            return fullCmd;
        }
    }
}
