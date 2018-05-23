//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Text;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using ProtoBuf;

//namespace Orc.Util
//{
//    public class ObjectSerializer
//    {
//        public static Byte[] Buffer;

//        static ObjectSerializer()
//        {
//            Buffer = new Byte[1024];
//        }

//        public static void Save<T>(T obj, String fullFilePath) where T : class
//        {
//            SaveGZipXml<T>(obj, fullFilePath);
//        }

//        public static void SaveXml<T>(T obj, String fullFilePath) where T : class
//        {
//            if (obj == null) throw new ArgumentNullException("obj");
//            if (fullFilePath == null) throw new ArgumentNullException("fullFilePath");
//            if (File.Exists(fullFilePath))
//                File.Delete(fullFilePath);
//            using (StreamWriter writer = new StreamWriter(fullFilePath))
//            {
//                XmlSerializer xs = new XmlSerializer(typeof(T));
//                xs.Serialize(writer, obj);
//                writer.Close();
//            }
//        }

//        public static void SaveGZipXml<T>(T obj, String fullFilePath) where T : class
//        {
//            if (obj == null) throw new ArgumentNullException("obj");
//            if (fullFilePath == null) throw new ArgumentNullException("fullFilePath");
//            if (File.Exists(fullFilePath))
//                File.Delete(fullFilePath);
//            String xmlString = GetXmlString<T>(obj);
//            using (MemoryStream ms = CompressString(xmlString))
//            {
//                using (FileStream fs = File.OpenWrite(fullFilePath))
//                {
//                    Int32 length = 0;
//                    while ((length = ms.Read(Buffer, 0, Buffer.Length)) > 0)
//                    {
//                        fs.Write(Buffer, 0, length);
//                    }
//                }
//            }
//        }

//        public static void SaveProtoBuf<T>(String filePath, T instance)
//        {
//            if (File.Exists(filePath)) File.Delete(filePath);
//            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
//            {
//                ProtoBuf.Serializer.Serialize<T>(fs, instance);
//            }
//        }

//        public static T ParseProtoBuf<T>(String filePath)
//        {
//            if (File.Exists(filePath) == false) return default(T);

//            using(FileStream fs = new FileStream(filePath, FileMode.Open))
//            {
//                return ProtoBuf.Serializer.Deserialize<T>(fs);
//            }
//        }

//        public static String GetXmlString<T>(T obj) where T : class
//        {
//            StringWriter sw = new StringWriter();
//            XmlSerializer xs = new XmlSerializer(typeof(T));
//            xs.Serialize(sw, obj);
//            return sw.ToString();
//        }

//        public static T ParseXml<T>(String fullFilePath)
//            where T : class
//        {
//            if (!File.Exists(fullFilePath))
//                return null;

//            try
//            {
//                return ParseXmlFromString<T>(File.ReadAllText(fullFilePath));
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public static T Parse<T>(String fullFilePath)
//            where T : class
//        {
//            return ParseGZipXml<T>(fullFilePath);
//        }

//        public static T ParseGZipXml<T>(String fullFilePath)
//            where T : class
//        {
//            if (!File.Exists(fullFilePath))
//                return null;

//            try
//            {
//                Byte[] buffer = File.ReadAllBytes(fullFilePath);
//                using (MemoryStream ms = new MemoryStream(buffer))
//                {
//                    ms.Position = 0;
//                    using (GZipStream zs = DecompressStream(ms))
//                    {
//                        using (XmlTextReader xmlReader = new XmlTextReader(zs))
//                        {
//                            return ParseXmlReader<T>(xmlReader);
//                        }
//                    }
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public static T ParseXmlFromString<T>(String xmlString)
//            where T : class
//        {
//            try
//            {
//                using (StringReader reader = new StringReader(xmlString))
//                {
//                    using (XmlTextReader xmlReader = new XmlTextReader(reader))
//                    {
//                        return ParseXmlReader<T>(xmlReader);
//                    }
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public static T ParseXmlReader<T>(XmlTextReader xmlReader)
//            where T : class
//        {
//            try
//            {
//                xmlReader.WhitespaceHandling = WhitespaceHandling.All;
//                XmlSerializer xs = new XmlSerializer(typeof(T));
//                object obj = xs.Deserialize(xmlReader);
//                return obj as T;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public static void SaveBin<T>(T obj, String fullFilePath) where T : class
//        {
//            using (FileStream fs = new FileStream(fullFilePath, FileMode.Create))
//            {
//                BinaryFormatter bf = new BinaryFormatter();
//                bf.Serialize(fs, obj);
//            }
//        }

//        public static T ParseBin<T>(String fullFilePath) where T : class
//        {
//            try
//            {
//                using (FileStream fs = new FileStream(fullFilePath, FileMode.Open))
//                {
//                    BinaryFormatter bf = new BinaryFormatter();
//                    return bf.Deserialize(fs) as T;
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public static MemoryStream CompressString(String inputString)
//        {
//            Byte[] buffer = Encoding.Unicode.GetBytes(inputString);
//            MemoryStream ms = new MemoryStream();
//            using (GZipStream cpStream = new GZipStream(ms, CompressionMode.Compress, true))
//            {
//                try
//                {
//                    cpStream.Write(buffer, 0, buffer.Length);
//                }
//                catch { }
//                cpStream.Close();
//            }
//            ms.Position = 0;
//            return ms;
//        }

//        public static MemoryStream CompressStream(Stream inputStream)
//        {
//            MemoryStream ms = new MemoryStream();
//            using (GZipStream cpStream = new GZipStream(ms, CompressionMode.Compress, true))
//            {
//                Int32 index = 0;
//                try
//                {
//                    while ((index = inputStream.Read(Buffer, 0, Buffer.Length)) > 0)
//                    {
//                        cpStream.Write(Buffer, 0, index);
//                    }
//                }
//                catch { }
//                cpStream.Close();
//            }
//            ms.Position = 0;
//            return ms;
//        }

//        public static GZipStream DecompressStream(MemoryStream inputStream)
//        {
//            GZipStream zs = new GZipStream(inputStream, CompressionMode.Decompress);
//            return zs;
//        }
//    }
//}
