using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
	public class MessageReceiver
	{
		public Int32 MaxLines { get; set; }
		private LinkedList<MessageEventArgs> CacheMsgs { get; set; }
		private Object SyncRoot = new object();

		public MessageReceiver()
		{
			CacheMsgs = new LinkedList<MessageEventArgs>();
			MaxLines = 50;
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

		public event EventHandler<EventArgs> MessageEntered;

		protected virtual void OnMessageEntered(EventArgs e)
		{
			EventHandler<EventArgs> handler = MessageEntered;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public void Receive(String msg)
		{
			if (String.IsNullOrEmpty(msg)) return;
			String allMsg = String.Empty;
			MessageEventArgs me = new MessageEventArgs(msg);
			lock (SyncRoot)
			{
				if (CacheMsgs.Count >= MaxLines)
				{
					while (CacheMsgs.Count >= MaxLines)
					{
						CacheMsgs.RemoveFirst();
					}
				}
				CacheMsgs.AddLast(me);
			}
			OnMessageEntered(EventArgs.Empty);
		}

		public String GetMessage()
		{
			return GetMessage(MessageEventArgs.MessageMode.DateThreadMessage);
		}

		public String GetMessage(MessageEventArgs.MessageMode mode)
		{
			StringBuilder sb = new StringBuilder();
			List<MessageEventArgs> msgs = new List<MessageEventArgs>();
			lock (SyncRoot)
			{
				msgs.AddRange(CacheMsgs);
			}

			foreach (var s in msgs)
				sb.AppendLine(s.BuildString(mode));

			return sb.ToString();
		}
	}
}
