namespace Geb.Utils.WinForm.Demo
{
    partial class FrmMain
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
            this.btnScrollBarDemo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnScrollBarDemo
            // 
            this.btnScrollBarDemo.Location = new System.Drawing.Point(35, 23);
            this.btnScrollBarDemo.Name = "btnScrollBarDemo";
            this.btnScrollBarDemo.Size = new System.Drawing.Size(205, 27);
            this.btnScrollBarDemo.TabIndex = 0;
            this.btnScrollBarDemo.Text = "ColorSccrollBarSkin";
            this.btnScrollBarDemo.UseVisualStyleBackColor = true;
            this.btnScrollBarDemo.Click += new System.EventHandler(this.btnScrollBarDemo_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 416);
            this.Controls.Add(this.btnScrollBarDemo);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnScrollBarDemo;
    }
}

