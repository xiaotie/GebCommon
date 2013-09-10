using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
    public class Command : IDisposable
    {
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
