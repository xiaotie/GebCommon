using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Net;

namespace Geb.Utils.WinForm
{
    public class BaseWebDownloader
    {
        public virtual FileInfo ConvertUrlToFileInfo(String url)
        {
            Uri uri = new Uri(url);
            String lp = uri.LocalPath;
            FileInfo fi = new FileInfo("./" + lp);
            return fi;
        }

        public virtual Int32 Download(String url)
        {
            FileInfo fi = ConvertUrlToFileInfo(url);
            if (fi.Exists == true) return 1;

            DirectoryInfo di = fi.Directory;
            if (di.Exists == false)
            {
                di.Create();
            }
            try
            {
                WebClient client = new WebClient();
                Byte[] data = client.DownloadData(url);
                String filePath = fi.FullName;
                if (File.Exists(filePath)) File.Delete(filePath);
                File.WriteAllBytes(filePath, data);
                return 1;
            }
            catch
            {
            }

            return 0;
        }
    }
}
