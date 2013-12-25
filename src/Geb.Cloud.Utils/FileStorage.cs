using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Geb.Cloud.Utils
{
    /// <summary>
    /// 文件存储类。将一个文件，根据文件名称的hash存储到baseDir目录下的不同的bucket里。
    /// </summary>
    public class FileStorage
    {
        private static Object SyncRoot = new Object();

        public String BaseDir { get; set; }

        public Boolean IsExist(String fileName)
        {
            if (String.IsNullOrEmpty(fileName) == true) return false;
            int hash = Math.Abs(fileName.GetHashCode());
            hash = hash % 10000;
            int bucket1 = hash / 100;
            int bucket2 = hash % 100;
            String fullPath = Path.Combine(BaseDir, bucket1.ToString(), bucket2.ToString(), fileName);
            return File.Exists(fullPath);
        }

        public void Write(String fileName, byte[] data)
        {
            if (String.IsNullOrEmpty(fileName) == true)
            {
                throw new ArgumentNullException("fileName");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            int hash = Math.Abs(fileName.GetHashCode());
            hash = hash % 10000;
            int bucket1 = hash / 100;
            int bucket2 = hash % 100;
            String path1 = Path.Combine(BaseDir, bucket1.ToString());
            String path2 = Path.Combine(BaseDir, bucket1.ToString(),bucket2.ToString());
            if (Directory.Exists(path1) == false)
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(path1);
                }
            }

            if (Directory.Exists(path2) == false)
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(path2);
                }
            }

            String fullPath = Path.Combine(BaseDir, bucket1.ToString(), bucket2.ToString(), fileName);
            File.WriteAllBytes(fullPath, data);
        }

        public byte[] ReadFile(String fileName)
        {
            if (String.IsNullOrEmpty(fileName) == true)
            {
                throw new ArgumentNullException("fileName");
            }

            int hash = Math.Abs(fileName.GetHashCode());
            hash = hash % 10000;
            int bucket1 = hash / 100;
            int bucket2 = hash % 100;

            String fullPath = Path.Combine(BaseDir, bucket1.ToString(), bucket2.ToString(), fileName);
            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }
            else
            {
                return null;
            }
        }
    }
}
