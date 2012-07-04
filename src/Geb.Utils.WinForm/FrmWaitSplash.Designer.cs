namespace Geb.Utils.WinForm
{
	partial class FrmWaitSplash
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmWaitSplash));
			this.Worker = new System.ComponentModel.BackgroundWorker();
			this.loadingBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.loadingBox)).BeginInit();
			this.SuspendLayout();
			// 
			// loadingBox
			// 
			this.loadingBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.loadingBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.loadingBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingBox.ErrorImage = global::Geb.Utils.WinForm.Properties.Resources.loading_animation;
            this.loadingBox.Image = global::Geb.Utils.WinForm.Properties.Resources.loading_animation;
			this.loadingBox.ImageLocation = "";
            this.loadingBox.InitialImage = global::Geb.Utils.WinForm.Properties.Resources.loading_animation;
			this.loadingBox.Location = new System.Drawing.Point(0, 0);
			this.loadingBox.Name = "loadingBox";
			this.loadingBox.Size = new System.Drawing.Size(200, 200);
			this.loadingBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.loadingBox.TabIndex = 1;
			this.loadingBox.TabStop = false;
			// 
			// WaitSplashForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.ClientSize = new System.Drawing.Size(200, 200);
			this.Controls.Add(this.loadingBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "WaitSplashForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "WaitSplash";
			this.Load += new System.EventHandler(this.WaitSplash_Load);
			((System.ComponentModel.ISupportInitialize)(this.loadingBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public System.ComponentModel.BackgroundWorker Worker;
		private System.Windows.Forms.PictureBox loadingBox;

	}
}