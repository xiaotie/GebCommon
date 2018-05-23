using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;

namespace Geb.Utils
{
    public class Command : IDisposable
    {
        /// <summary>
        /// Linux 或 macosx 下，有的命令行会以 \r 来换行，用 OutputDataReceived 会接收不到数据
        /// </summary>
        public class StreamHelper
        {
            public bool Stopped;
            public Action<String> OnMsg;

            /// <summary>
            /// 除了 '\n' 外，另一个断句分隔符
            /// </summary>
            public Char ExtSplitter = '\r';

            public Process Owner;

            private StringBuilder sbOut = new StringBuilder();
            private StringBuilder sbErr = new StringBuilder();

            public void Start()
            {
                new Thread(new ThreadStart(this.RunParseStdOut)).Start();
                new Thread(new ThreadStart(this.RunParseStdErr)).Start();
            }

            private void RunParseStdOut()
            {
                while (Stopped == false)
                {
                    FlushStdOut();
                }

                FlushStdOut();
            }

            private void RunParseStdErr()
            {
                while (Stopped == false)
                {
                    FlushStdErr();
                }

                FlushStdErr();
            }

            private void FlushStdOut(bool flushAll = false)
            {
                StreamReader sOut = Owner.StandardOutput;
                if (sOut.EndOfStream == true)
                    return;

                int val = sOut.Read();
                if (val >= 0)
                {
                    Char c = (Char)val;
                    ParseStr(sbOut, c, flushAll);
                }
            }

            private void FlushStdErr(bool flushAll = false)
            {
                StreamReader sErr = Owner.StandardError;
                if (sErr.EndOfStream == true)
                    return;

                int val = sErr.Read();
                if (val >= 0)
                {
                    Char c = (Char)val;
                    ParseStr(sbErr, c, flushAll);
                }
            }

            private void ParseStr(StringBuilder sb, Char c, bool flushAll = false)
            {
                if (flushAll == true)
                {
                    sb.Append(c);
                    String msg = sb.ToString();
                    if(sb.Length > 0) sb.Remove(0, sb.Length);
                    if (OnMsg != null)
                        OnMsg(msg);
                    return;
                }

                sb.Append(c);
                if (c == '\n' || c == ExtSplitter)
                {
                    String msg = sb.ToString();
                    if (sb.Length > 0) sb.Remove(0, sb.Length);

                    if (OnMsg != null)
                        OnMsg(msg);
                }
            }
        }

        private Action<String> OnMsg;
        private Process _process;

        public void KillProcess()
        {
            if (_process != null)
            {
                try
                {
                    _process.Kill();
                }
                catch
                {
                }
                finally
                {
                    _process = null;
                }
            }
        }

        public String ExecuteCmd(String fileName, String arguments, Action<String> onMsg)
        {
            OnMsg = onMsg;

            Process process = new Process();     //创建进程对象
            if (_process != null)
            {
                KillProcess();
            }

            _process = process;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;      //设定需要执行的命令
            startInfo.Arguments = arguments;   //设定参数，其中的“/C”表示执行完命令后马上退出
            startInfo.UseShellExecute = false;     //不使用系统外壳程序启动
            startInfo.RedirectStandardInput = false;   //不重定向输入
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;   //重定向输出
            startInfo.CreateNoWindow = true;     //不创建窗口

            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);

            process.StartInfo = startInfo;
            try
            {
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();
                process.Close();
                process.Dispose();
                _process = null;
                return FetchCacheOutputs();
            }
            catch (Exception e)
            {
                if (OnMsg != null) OnMsg(e.Message);
                return e.ToString();
            }
            finally
            {
                if (process != null)
                    process.Close();
            }

            return String.Empty;
        }

        /// <summary>
        /// 使用除了\n外，附加新的断句分隔符进行断句，当遇到 \n 或者新断句分隔符时，认为是一条消息，调用 onMsg 回调
        /// </summary>
        /// <returns>The cmd.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="arguments">Arguments.</param>
        /// <param name="onMsg">On message.</param>
        /// <param name="extSplitter">Ext splitter.</param>
        public String ExecuteCmd(String fileName, String arguments, Action<String> onMsg, Char extSplitter)
        {
            OnMsg = onMsg;

            Process process = new Process();     //创建进程对象
            if (_process != null)
            {
                KillProcess();
            }

            _process = process;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;      //设定需要执行的命令
            startInfo.Arguments = arguments;   //设定参数，其中的“/C”表示执行完命令后马上退出
            startInfo.UseShellExecute = false;     //不使用系统外壳程序启动
            startInfo.RedirectStandardInput = false;   //不重定向输入
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;   //重定向输出
            startInfo.CreateNoWindow = true;     //不创建窗口

            process.StartInfo = startInfo;
            StreamHelper helper = new StreamHelper();
            helper.Owner = process;
            helper.OnMsg = onMsg;
            helper.ExtSplitter = extSplitter;

            try
            {
                process.Start();
                helper.Start();
                process.WaitForExit();
                helper.Stopped = true;

                process.Close();
                process.Dispose();
                _process = null;
                return FetchCacheOutputs();
            }
            catch (Exception e)
            {
                if (OnMsg != null) OnMsg(e.Message);
                return e.ToString();
            }
            finally
            {
                if (process != null)
                    process.Close();
            }

            return String.Empty;
        }

        private List<String> CacheOutputs = new List<String>();

        private String FetchCacheOutputs()
        {
            StringBuilder sb = new StringBuilder();
            lock (this)
            {
                foreach (String item in CacheOutputs)
                {
                    sb.AppendLine(item);
                }
                CacheOutputs.Clear();
            }
            return sb.ToString();
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                lock (this)
                {
                    CacheOutputs.Add(e.Data);
                    if (OnMsg != null) OnMsg(e.Data);
                }
            }
        }

        public void Dispose()
        {
            this.KillProcess();
        }
    }
}
