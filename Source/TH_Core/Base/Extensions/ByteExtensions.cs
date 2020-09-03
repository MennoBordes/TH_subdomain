using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TH.Core.Base.Extensions
{

    public static class ByteExtensions
    {
        /// <summary> Converts this string to byte array. 
        /// <para> No special encoding is used. </para>
        /// </summary>
        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary> Converts this string to byte array. 
        /// <para> Specify an encoding (ANSI, Unicode, Unicode big endian, UTF-8). </para>
        /// </summary>
        public static byte[] GetBytes(this string str, Encoding encoding)
        {
            // NULL check...
            byte[] bytes = encoding.GetBytes(str);
            return bytes;
        }

        /// <summary> Converts this byte array to a string. 
        /// <para> No special encoding is used. </para>
        /// </summary>
        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary> Converts this byte array to a string. 
        /// <para> Specify an encoding (ANSI, Unicode, Unicode big endian, UTF-8). </para>
        /// </summary>
        public static string GetString(this byte[] bytes, Encoding encoding)
        {
            // NULL check...
            string str = encoding.GetString(bytes);
            return str;
        }


        //=== Zip

        /// <summary> Compress string to GZip. </summary>
        public static byte[] CompressGZip(string content)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (Stream fs = ToStream(content))
                using (Stream csStream = new GZipStream(memory, CompressionMode.Compress))
                {
                    byte[] buffer = new byte[1024];
                    int nRead;
                    while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        csStream.Write(buffer, 0, nRead);
                    }
                }
                return memory.ToArray();
            }
        }

        // not finished
        ///// <summary> Decompress string from GZip. </summary>
        //public string DecompressGZip(byte[] bytes)
        //{
        //    using (Stream fd = File.Create("gj.new.txt"))
        //    using (Stream fs = File.OpenRead("gj.zip"))
        //    using (Stream csStream = new GZipStream(fs, CompressionMode.Decompress))
        //    {
        //        byte[] buffer = new byte[1024];
        //        int nRead;
        //        while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            fd.Write(buffer, 0, nRead);
        //        }
        //    }
        //}

        /// <summary> Creates stream from string. </summary>
        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
