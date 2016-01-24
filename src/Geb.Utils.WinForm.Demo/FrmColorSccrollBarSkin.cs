using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm.Demo
{
    public partial class FrmColorSccrollBarSkin : Form
    {
        private ColorSccrollBarSkin _skin;
        public FrmColorSccrollBarSkin()
        {
            InitializeComponent();
        }

        private void FrmColorSccrollBarSkin_Load(object sender, EventArgs e)
        {
            String demo = "ColorSccrollBarSkin Demo";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
                sb.AppendLine(demo);
            this.tbDemo.Text = sb.ToString();
            _skin = new ColorSccrollBarSkin(this.tbDemo.Handle);
        }
    }
}
