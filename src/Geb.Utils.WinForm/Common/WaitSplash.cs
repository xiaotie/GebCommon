using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Geb.Utils.WinForm
{
	public class WaitSplash
	{
		public static readonly Int32 DefaultSleepIntervalMilliSeconds = 100;
		public Boolean Running { get; private set; }
		public Int32 SleepIntervalMilliSeconds { get; set; }
		public Int32 BoxSize { get; set; }

		public String Caption { get; set; }

		public WaitSplash()
		{
			SleepIntervalMilliSeconds = DefaultSleepIntervalMilliSeconds;
			Caption = "请等待";
		}

		public void Start()
		{
			Running = true;
			Thread thread = new Thread(new ThreadStart(this.ThreadStart_RunWaitSplash));
			thread.IsBackground = true;
			thread.Start();
		}

		public void Stop()
		{
			Running = false;
		}

		private void ThreadStart_RunWaitSplash()
		{
			if (Running)
			{
				Thread.Sleep(200);
				if (Running == false) return;
				FrmWaitSplash ws = new FrmWaitSplash(this);
				ws.BoxSize = this.BoxSize;
				if (Running == false) return;
				ws.ShowDialog();
			}
		}
	}
}
