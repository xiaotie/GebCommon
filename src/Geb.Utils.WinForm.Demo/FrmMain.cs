using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm.Demo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnScrollBarDemo_Click(object sender, EventArgs e)
        {
            FrmColorSccrollBarSkin frm = new FrmColorSccrollBarSkin();
            frm.ShowDialog();
        }
    }
}
