using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
	public partial class FrmErrorReport : Form
	{
		public String Title { get; set; }
		public String Content { get; set; }

		public FrmErrorReport()
		{
			Title = String.Empty;
			Content = String.Empty;
			InitializeComponent();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ErrorReportForm_Load(object sender, EventArgs e)
		{
			this.tbTitle.Text = this.Title;
			this.tbContent.Text = this.Content;
		}
	}
}
