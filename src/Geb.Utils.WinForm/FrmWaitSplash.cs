using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Geb.Utils.WinForm
{
	public partial class FrmWaitSplash : Form
	{
		private WaitSplash Manager { get; set; }

		public Int32 BoxSize 
		{
			get { return this.Size.Width; }
			set 
			{
				if (value > 0)
				{
					this.Size = new Size(value, value);
					this.loadingBox.Size = this.Size;
				}
			}
		}

		public FrmWaitSplash(WaitSplash manager)
		{
			if (manager == null) throw new ArgumentNullException("manager");
			Manager = manager;
			InitializeComponent();
		}

		private void WaitSplash_Load(object sender, EventArgs e)
		{
			this.Text = Manager.Caption;
			BackgroundWorker bw = new BackgroundWorker();
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerAsync();
		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			while (Manager.Running == true)
			{
				Thread.Sleep(Manager.SleepIntervalMilliSeconds);
			}
		}

		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Close();
		}
	}
}
