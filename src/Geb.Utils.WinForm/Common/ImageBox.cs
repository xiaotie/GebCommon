using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Utils
{
    using Geb.Utils.WinForm;

    public class ImageBox
    {
        public static void Show(Bitmap bmp, String title = null, bool zoom = true)
        {
            ShowCore(bmp, title, false, zoom);
        }

        public static void ShowDialog(Bitmap bmp, String title = null, bool zoom = true)
        {
            ShowCore(bmp, title, true, zoom);
        }

        private static void ShowCore(Bitmap bmp, String title = null, Boolean isDialog = false, bool zoom = true)
        {
            FrmImageBox box = new FrmImageBox(bmp);
            box.ZoomImage = zoom;
            if (title != null) box.Text = title;
            if (isDialog == true) box.ShowDialog(); else box.Show();
        }
    }
}
