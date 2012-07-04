using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
	public class FormConfig
	{
		public NotifyIcon NotifyIcon { get;private set; }
		public ContextMenuStrip NotifyIconMenuStrip { get;private set; }
		public Boolean EnableFormExit { get; set; }

		private Form Form { get; set; }

		/// <summary>
		/// 为窗体添加 NotifyIcon。并为其配置默认的右键菜单。双击NotifyIcon，激活窗体。
		/// </summary>
		/// <param name="form">窗体</param>
		public void AttachNotifyIcon(Form form)
		{
			AttachNotifyIcon(form, null, null, CreateNotifyIconMenuStrip());
		}

		/// <summary>
		/// 为窗体添加 NotifyIcon。双击NotifyIcon，激活窗体。
		/// </summary>
		/// <param name="form">窗体</param>
		/// <param name="title">NotifyIcon 的 Title，若为空，则使用窗体的 Text 作为 NotifyIcon 的 Title </param>
		/// <param name="icon">NotifyIcon 的 图标，若为空，则使用窗体的 图标作为 NotifyIcon 的图标 </param>
		/// <param name="notifyIconMenuStrip">NotifyIcon 的右键菜单</param>
		public void AttachNotifyIcon(Form form, String title, Icon icon, ContextMenuStrip notifyIconMenuStrip)
		{
			if (form == null) throw new ArgumentNullException("form");
			if (notifyIconMenuStrip == null) throw new ArgumentNullException("notifyIconMenuStrip");

			if (NotifyIcon == null) NotifyIcon = new NotifyIcon();
			if (NotifyIconMenuStrip == null) NotifyIconMenuStrip = CreateNotifyIconMenuStrip();
			NotifyIcon.ContextMenuStrip = NotifyIconMenuStrip;
			NotifyIcon.Text = title == null ? form.Text : title;
			NotifyIcon.Icon = icon == null? form.Icon : icon;
			NotifyIcon.DoubleClick += new EventHandler(NotifyIcon_DoubleClick);
			Form = form;
			Form.FormClosing += new FormClosingEventHandler(Form_FormClosing);
		}

		private void NotifyIcon_DoubleClick(object sender, EventArgs e)
		{
			ShowMainForm();
		}

		private ContextMenuStrip CreateNotifyIconMenuStrip()
		{
			ToolStripMenuItem itemShowForm = new ToolStripMenuItem("显示主窗口");
			ToolStripMenuItem itemCloseForm = new ToolStripMenuItem("退出");
			itemShowForm.Click += new EventHandler(ItemShowForm_Click);
			itemCloseForm.Click += new EventHandler(ItemCloseForm_Click);
			ContextMenuStrip cms = new ContextMenuStrip();
			cms.Items.Add(itemShowForm);
			cms.Items.Add(itemCloseForm);
			return cms;
		}


        public event EventHandler<FormClosingEventArgs> FormClosing;

        protected virtual void OnFormClosing(FormClosingEventArgs e)
        {
            EventHandler<FormClosingEventArgs> handler = FormClosing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

		private void ItemShowForm_Click(object sender, EventArgs e)
		{
			ShowMainForm();
		}

		private void ItemCloseForm_Click(object sender, EventArgs e)
		{
			if(Form!=null)
			{
				EnableFormExit = true;

                FormClosingEventArgs ce = new FormClosingEventArgs(CloseReason.FormOwnerClosing, false);
                this.OnFormClosing(ce);

                if (ce.Cancel == false)
                {
                    Form.Close();
                    NotifyIcon.Visible = false;
                }
			}
		}

		private void Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!EnableFormExit)
			{
				e.Cancel = true; // 取消关闭窗体
				Form.Hide();
				Form.ShowInTaskbar = false;
				NotifyIcon.Visible = true;//显示托盘图标
			}
		}

		private void ShowMainForm()
		{
			if (Form == null) return;

			Form.Show();
			if (Form.WindowState == FormWindowState.Minimized)
				Form.WindowState = FormWindowState.Normal;
			Form.Activate();
		}
	}
}
