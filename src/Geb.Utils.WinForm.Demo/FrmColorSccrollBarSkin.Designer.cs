namespace Geb.Utils.WinForm.Demo
{
    partial class FrmColorSccrollBarSkin
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
            this.tbDemo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbDemo
            // 
            this.tbDemo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDemo.Location = new System.Drawing.Point(13, 13);
            this.tbDemo.Multiline = true;
            this.tbDemo.Name = "tbDemo";
            this.tbDemo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDemo.Size = new System.Drawing.Size(657, 455);
            this.tbDemo.TabIndex = 0;
            // 
            // FrmColorSccrollBarSkin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 480);
            this.Controls.Add(this.tbDemo);
            this.Name = "FrmColorSccrollBarSkin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ColorSccrollBarSkin Demo";
            this.Load += new System.EventHandler(this.FrmColorSccrollBarSkin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDemo;
    }
}