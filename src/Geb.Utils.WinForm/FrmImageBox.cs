using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
    public partial class FrmImageBox : Form
    {
        public Bitmap Image;

        public Boolean ZoomImage;

        public FrmImageBox():this(null)
        {
        }

        public FrmImageBox(Bitmap bmp)
        {
            Image = bmp;
            InitializeComponent();
        }

        private void FrmImageBox_Load(object sender, EventArgs e)
        {
            this.pbMain.Image = Image;
            Refresh();
        }

        private void lbUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.geblab.com/");
        }

        private void btnNormalSize_CheckedChanged(object sender, EventArgs e)
        {
            this.ZoomImage = !this.btnNormalSize.Checked;
            Refresh();
        }

        private void btnZoomSize_CheckedChanged(object sender, EventArgs e)
        {
            this.ZoomImage = this.btnZoomSize.Checked;
            Refresh();
        }

        private void Refresh()
        {
            if (ZoomImage == true)
            {
                this.pbMain.SizeMode = PictureBoxSizeMode.Zoom;
                this.btnZoomSize.Checked = true;
            }
            else
            {
                this.pbMain.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            if (Image != null)
            {
                this.lbSize.Text = String.Format("({0},{1})", Image.Width, Image.Height);
            }
        }
    }
}
