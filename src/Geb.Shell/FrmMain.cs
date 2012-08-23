using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geb.Shell
{
    using Geb.Shell.Core;

    public partial class FrmMain : Form
    {
        static Context cxt;
        static String Prefix = ">>> ";
        static List<String> Outputs = new List<string>();

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            cxt = new Context();
        }

        private void Output(String cmd, String output)
        {
            Outputs.Add(Prefix + cmd);
            Outputs.Add(output);

            StringBuilder sb = new StringBuilder();
            foreach (String item in Outputs)
            {
                sb.AppendLine(item);
            }
            this.tbOutput.Text = sb.ToString();
            this.tbOutput.SelectionStart = this.tbOutput.Text.Length;
            this.tbOutput.SelectionLength = 0;
            this.tbOutput.ScrollToCaret();
        }

        private void AcceptTab()
        {
            int idx = this.tbInput.SelectionStart - 1;
            int start = idx + 1;
            for (int i = idx; i >= 0; i--)
            {
                Char c = this.tbInput.Text[i];
                if (Char.IsLetterOrDigit(c) == false)
                {
                    break;
                }
                start = i;
            }

            if (start >= idx) return;

            String prefix = this.tbInput.Text.Substring(start, idx - start + 1);
            if (String.IsNullOrEmpty(prefix) == false)
            {
                DirectoryInfo di = new DirectoryInfo("./");
                DirectoryInfo[] dirs = di.GetDirectories(prefix + "*");
                FileInfo[] files = di.GetFiles(prefix + "*");
                if (dirs.Length == 1 && files.Length == 0)
                {
                }
            }
        }

        private void tbInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)    // 回车
            {
                String cmd = this.tbInput.Text.Trim();
                if (String.IsNullOrEmpty(cmd) == false)
                {
                    Output(cmd, cxt.Invoke(cmd));
                }
                e.Handled = true;
                this.tbInput.Text = "";
            }
            else if (e.KeyChar == 9)    // tab
            {
                AcceptTab();
                e.Handled = true;
            }
        }
    }
}
