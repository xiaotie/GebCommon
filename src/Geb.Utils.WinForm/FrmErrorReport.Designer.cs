namespace Geb.Utils.WinForm
{
	partial class FrmErrorReport
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.tbTitle = new System.Windows.Forms.TextBox();
			this.tbContent = new System.Windows.Forms.TextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "错误:";
			// 
			// tbTitle
			// 
			this.tbTitle.ForeColor = System.Drawing.Color.Red;
			this.tbTitle.Location = new System.Drawing.Point(48, 16);
			this.tbTitle.Multiline = true;
			this.tbTitle.Name = "tbTitle";
			this.tbTitle.ReadOnly = true;
			this.tbTitle.Size = new System.Drawing.Size(520, 48);
			this.tbTitle.TabIndex = 1;
			// 
			// tbContent
			// 
			this.tbContent.Location = new System.Drawing.Point(48, 80);
			this.tbContent.Multiline = true;
			this.tbContent.Name = "tbContent";
			this.tbContent.ReadOnly = true;
			this.tbContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbContent.Size = new System.Drawing.Size(520, 264);
			this.tbContent.TabIndex = 2;
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Location = new System.Drawing.Point(488, 360);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "关闭";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Location = new System.Drawing.Point(8, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "详细:";
			// 
			// ErrorReportForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(583, 391);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.tbContent);
			this.Controls.Add(this.tbTitle);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorReportForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "出错了";
			this.Load += new System.EventHandler(this.ErrorReportForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbTitle;
		private System.Windows.Forms.TextBox tbContent;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label label2;
	}
}