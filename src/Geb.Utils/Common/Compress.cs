using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Geb.Utils
{
    public class CompressHelper
    {
        public static byte[] Compress(String content)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, content);
                ms.Seek(0, 0);
                byte[] bytes = ms.ToArray();
                return Compress(bytes);
            }
        }

        public static String DecompressToString(byte[] data)
        {
            data = Decompress(data);
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, 0);
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms).ToString();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            
            using (MemoryStream memoryStream = new MemoryStream())
            {
                CompressionMode compressionMode = CompressionMode.Compress;
                using (GZipStream gZipStream = new GZipStream(memoryStream, compressionMode, true))
                {
                    gZipStream.Write(data, 0, data.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            GZipStream gZipStream;
            using (MemoryStream inputMemoryStream = new MemoryStream())
            {
                inputMemoryStream.Write(data, 0, data.Length);
                inputMemoryStream.Position = 0;

                CompressionMode compressionMode = CompressionMode.Decompress;
                gZipStream = new GZipStream(inputMemoryStream, compressionMode, true);

                using (MemoryStream outputMemoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[1024];
                    int byteRead = -1;
                    byteRead = gZipStream.Read(buffer, 0, buffer.Length);

                    while (byteRead > 0)
                    {
                        outputMemoryStream.Write(buffer, 0, byteRead);
                        byteRead = gZipStream.Read(buffer, 0, buffer.Length);
                    }

                    gZipStream.Close();
                    return outputMemoryStream.ToArray();
                }
            }
        }
    }

}
