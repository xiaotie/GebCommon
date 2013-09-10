namespace Geb.Utils.WinForm
{
    partial class FrmImageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmImageBox));
            this.pbMain = new System.Windows.Forms.PictureBox();
            this.lbSize = new System.Windows.Forms.Label();
            this.lbUrl = new System.Windows.Forms.LinkLabel();
            this.btnNormalSize = new System.Windows.Forms.RadioButton();
            this.btnZoomSize = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.SuspendLayout();
            // 
            // pbMain
            // 
            this.pbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbMain.Location = new System.Drawing.Point(12, 12);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(513, 472);
            this.pbMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMain.TabIndex = 0;
            this.pbMain.TabStop = false;
            // 
            // lbSize
            // 
            this.lbSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSize.AutoSize = true;
            this.lbSize.Location = new System.Drawing.Point(13, 491);
            this.lbSize.Name = "lbSize";
            this.lbSize.Size = new System.Drawing.Size(65, 12);
            this.lbSize.TabIndex = 1;
            this.lbSize.Text = "(图像尺寸)";
            // 
            // lbUrl
            // 
            this.lbUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbUrl.AutoSize = true;
            this.lbUrl.Location = new System.Drawing.Point(352, 491);
            this.lbUrl.Name = "lbUrl";
            this.lbUrl.Size = new System.Drawing.Size(173, 12);
            this.lbUrl.TabIndex = 2;
            this.lbUrl.TabStop = true;
            this.lbUrl.Text = "集异璧实验室(www.geblab.com)";
            this.lbUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbUrl_LinkClicked);
            // 
            // btnNormalSize
            // 
            this.btnNormalSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNormalSize.AutoSize = true;
            this.btnNormalSize.Location = new System.Drawing.Point(560, 12);
            this.btnNormalSize.Name = "btnNormalSize";
            this.btnNormalSize.Size = new System.Drawing.Size(71, 16);
            this.btnNormalSize.TabIndex = 3;
            this.btnNormalSize.Text = "原始大小";
            this.btnNormalSize.UseVisualStyleBackColor = true;
            this.btnNormalSize.CheckedChanged += new System.EventHandler(this.btnNormalSize_CheckedChanged);
            // 
            // btnZoomSize
            // 
            this.btnZoomSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomSize.AutoSize = true;
            this.btnZoomSize.Checked = true;
            this.btnZoomSize.Location = new System.Drawing.Point(560, 35);
            this.btnZoomSize.Name = "btnZoomSize";
            this.btnZoomSize.Size = new System.Drawing.Size(71, 16);
            this.btnZoomSize.TabIndex = 4;
            this.btnZoomSize.TabStop = true;
            this.btnZoomSize.Text = "自动缩放";
            this.btnZoomSize.UseVisualStyleBackColor = true;
            this.btnZoomSize.CheckedChanged += new System.EventHandler(this.btnZoomSize_CheckedChanged);
            // 
            // FrmImageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 522);
            this.Controls.Add(this.btnZoomSize);
            this.Controls.Add(this.btnNormalSize);
            this.Controls.Add(this.lbUrl);
            this.Controls.Add(this.lbSize);
            this.Controls.Add(this.pbMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrmImageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "图像浏览器";
            this.Load += new System.EventHandler(this.FrmImageBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMain;
        private System.Windows.Forms.Label lbSize;
        private System.Windows.Forms.LinkLabel lbUrl;
        private System.Windows.Forms.RadioButton btnNormalSize;
        private System.Windows.Forms.RadioButton btnZoomSize;
    }
}