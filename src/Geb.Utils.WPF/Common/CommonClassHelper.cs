using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Geb.Utils.WPF
{
    public static class CommonClassHelper
    {
        public static void OpenFile(this FrameworkElement element, Action<String> callbackOnFilePath, String filter = "所有文件|*.*")
        {
            String filePath;
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = filter;
            dlg.FileOk += (object sender, CancelEventArgs e) =>
            {
                filePath = dlg.FileName;
                if (callbackOnFilePath != null)
                    callbackOnFilePath(filePath);
            };
            dlg.ShowDialog();
        }

        public static void OpenImageFile(this FrameworkElement element, Action<String> callbackOnFilePath, String filter = "图像文件|*.bmp;*.jpg;*.gif;*.png")
        {
            OpenFile(element, callbackOnFilePath, filter);
        }
    }
}
