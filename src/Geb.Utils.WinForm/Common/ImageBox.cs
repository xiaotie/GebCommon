using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Utils
{
    using Geb.Utils.WinForm;

    public class ImageBox
    {
        public static void Show(Bitmap bmp, String title = null)
        {
            ShowCore(bmp, title, false);
        }

        public static void ShowDialog(Bitmap bmp, String title = null)
        {
            ShowCore(bmp, title, true);
        }

        private static void ShowCore(Bitmap bmp, String title = null, Boolean isDialog = false)
        {
            FrmImageBox box = new FrmImageBox(bmp);
            if (title != null) box.Text = title;
            if (isDialog == true) box.ShowDialog(); else box.Show();
        }
    }
}
