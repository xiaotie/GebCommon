using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
    public partial class FrmDownload : Form
    {
        public List<Uri> InitUriList { get; set; }

        public BaseWebDownloader Downloader { get; set; }

        public class UrlRecord
        {
            public String Url { get; set; }
            public Int32 Status { get; set; } // 0 - 未下载; 1 - 已下载; -1 下载失败
            public String StatusNote
            {
                get
                {
                    if (Status == 0)
                        return "未下载";
                    else if (Status > 0)
                        return "已下载";
                    else return "下载失败";
                }
            }
        }

        public BindingList<UrlRecord> m_records = new BindingList<UrlRecord>();

        public FrmDownload()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmAddUrl form = new FrmAddUrl();
            form.ShowDialog();
            foreach (String item in form.Urls)
            {
                UrlRecord record = new UrlRecord { Url = item, Status = 0 };
                m_records.Add(record);
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = m_records;
            this.dgv.DataSource = bs;
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {
            this.dgv.UseDefaultMode00();
            if (InitUriList != null)
            {
                foreach (Uri item in InitUriList)
                {
                    UrlRecord record = new UrlRecord();
                    record.Url = item.ToString();
                    record.Status = 0;
                    m_records.Add(record);
                }
                BindingSource bs = new BindingSource();
                bs.DataSource = m_records;
                this.dgv.DataSource = bs;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            this.btnAdd.Enabled = false;
            this.btnDownload.Enabled = false;
            Stopped = false;
            this.worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.EnableControls(true);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartDownload();
        }

        private void StartDownload()
        {
            if (Downloader == null)
            {
                MessageBox.Show("没有指定Downloader实例.");
                return;
            }

            foreach (UrlRecord item in m_records)
            {
                if (Stopped == true) return;

                if (item.Status <= 0)
                {
                    item.Status = Downloader.Download(item.Url);
                }
            }
        }

        private bool Stopped = true;

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stopped = true;
        }
    }
}
