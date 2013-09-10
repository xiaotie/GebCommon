using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
    public partial class FrmAddUrl : Form
    {
        public List<String> Urls = new List<String>();

        public FrmAddUrl()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            String urls = this.tbUrls.Text;
            String[] list = urls.Split('\r');
            foreach (String item in list)
            {
                if (String.IsNullOrEmpty(item.Trim()) == false)
                {
                    Urls.Add(item);
                }
            }
            this.Close();
        }

        private void AddUrlForm_Load(object sender, EventArgs e)
        {
            this.tbUrls.Enhance();
        }
    }
}
