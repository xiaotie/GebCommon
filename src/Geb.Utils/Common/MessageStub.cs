using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
	public class MessageEventArgs : EventArgs
	{
		public enum MessageMode
		{
			Message,
			DateThreadMessage,
		}

		public String Message { get; private set; }
		public String Type { get; set; }
		public Int32 ThreadId { get; set; }
		public DateTime CreateTime { get; set; }

		public MessageEventArgs(String msg)
		{
			Message = msg;
			Type = String.Empty;
			ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			CreateTime = DateTime.Now;
		}

		public String BuildString(MessageMode mode)
		{
			switch (mode)
			{
				case MessageMode.DateThreadMessage:
					return String.Format("[{0}]-Thread[{1}]: {2}", CreateTime.ToLongTimeString(), ThreadId.ToString(), Message);
				default:
					return Message;
			}
		}
	}

	public interface IMessageStub
	{
		event EventHandler<MessageEventArgs> MessageReceived;
	}

	public class MessageStub : IMessageStub
	{
		public event EventHandler<MessageEventArgs> MessageReceived;

		protected virtual void OnMessageReceived(MessageEventArgs e)
		{
			EventHandler<MessageEventArgs> handler = MessageReceived;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}
