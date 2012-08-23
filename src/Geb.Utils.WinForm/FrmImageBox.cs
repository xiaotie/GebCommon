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
            if (Image != null)
            {
                this.lbSize.Text = String.Format("({0},{1})", Image.Width, Image.Height);
            }
        }
    }
}
