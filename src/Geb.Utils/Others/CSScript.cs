using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Geb.Utils
{
    public class CSScript
    {
        /// <summary>
        /// 获取 path 环境变量中的最后一个。
        /// </summary>
        /// <returns></returns>
        public static String GetLastPath()
        {
            String pathes = Environment.GetEnvironmentVariable("path");
            String[] list = pathes.Split(';');
            return list.Length > 0 ? list[list.Length - 1] : String.Empty;
        }

        public static String GetFile(String fileName)
        {
            return GetLastPath() + Path.DirectorySeparatorChar + fileName;
        }

        public static void ForEach(Action<String> onFullFileName, String searchPattern = "*")
        {
            if (onFullFileName == null) return;
            if (searchPattern == null) searchPattern = "*";

            String path = GetLastPath();
            String[] files = Directory.GetFiles(path, searchPattern);
            foreach (String file in files)
            {
                onFullFileName(file);
            }
        }
    }
}
