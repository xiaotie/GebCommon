using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
	public partial class TextBoxListener : UserControl
	{
		public Int32 MaxLines
		{
			get { return Receiver.MaxLines; }
			set { Receiver.MaxLines = value; }
		}
		private MessageReceiver Receiver { get; set; }
		public TextBoxListener()
		{
			InitializeComponent();
			Receiver = new MessageReceiver();
		}

		public void Listen(IMessageStub stub)
		{
			if (stub == null) throw new ArgumentNullException("stub");
			stub.MessageReceived += new EventHandler<MessageEventArgs>(stub_MessageReceived);
		}

		private void stub_MessageReceived(object sender, MessageEventArgs e)
		{
			Receive(e.Message);
		}

		public void Receive(String msg)
		{
			Receiver.Receive(msg);

			if (this.IsHandleCreated == true)
			{
				this.Invoke(new Action(this.RefreshText));
			}
		}

		private void RefreshText()
		{
			this.textBox.Text = Receiver.GetMessage();
			this.textBox.Focus();
			this.textBox.SelectionStart = this.textBox.Text.Length;
			this.textBox.ScrollToCaret();
		}
	}
}
