using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace TH.Core.Base.Extensions
{
    public static class XmlExtensions
    {
        /// <summary> Serializes this object to a string. </summary>
        public static string XmlSerialize(this object obj, bool indent = true, Type[] extraTypes = null, bool includeXmlDeclaration = true, bool checkCharacters = true)
        {
            // Check
            if (obj == null)
                return null;

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = indent,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8,
                CheckCharacters = checkCharacters
            };

            StreamReader sr = null;

            try
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    xmlWriterSettings.OmitXmlDeclaration = !includeXmlDeclaration;

                    using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                    {
                        XmlSerializer serializer = new XmlSerializer(obj.GetType(), extraTypes);
                        serializer.Serialize(xmlWriter, obj);

                        memoryStream.Position = 0;

                        sr = new StreamReader(memoryStream);

                        return sr.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (sr != null)
                    sr.Dispose();
            }

            #region Simple
            //XmlSerializer serializer = new XmlSerializer(obj.GetType());

            //using (StringWriter textWriter = new StringWriter())
            //{
            //    serializer.Serialize(textWriter, obj);

            //    return textWriter.ToString();
            //}
            #endregion
        }

        /// <summary> Deserializes this xml string to an object. </summary>
        public static T DeserializeObject<T>(this string xml, Type[] extraTypes = null)
        {
            // Check
            if (xml == null)
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(xml)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), extraTypes);

                using (StreamReader xmlStreamReader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    return (T)xmlSerializer.Deserialize(xmlStreamReader);
                }
            }

            #region Simple
            //XmlSerializer serializer = new XmlSerializer(typeof(T));

            //object obj;
            //using (TextReader reader = new StringReader(xml))
            //{
            //    obj = serializer.Deserialize(reader);
            //}

            //return (T)obj;
            #endregion
        }

        /// <summary> Checks whether the given string is an xml document. </summary>
        public static bool IsXml(this string value)
        {
            // Check
            if (string.IsNullOrEmpty(value))
                return false;

            // Remove whitespaces
            value = value.Trim();

            // If has not xml tags
            if (!value.StartsWith("<") && !value.EndsWith(">"))
                return false;

            // Try parse
            try
            {
                ParseXmlHelper(value);
                return true;
            }
            catch (XmlException ex)
            {
                return false;
            }
        }

        ///<summary>Parse xml</summary>
        public static XmlDocument ParseXml(this string value, bool isAddRootElement = true)
        {
            // Try parse
            try
            {
                return ParseXmlHelper(value);
            }
            catch (XmlException ex)
            {
                return null;
            }
        }

        ///<summary>Helper parse xml</summary>
        private static XmlDocument ParseXmlHelper(string value, bool isAddRootElement = true)
        {
            // Try parse
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(string.Format(isAddRootElement ? "<root>{0}</root>" : "{0}", value));
                return xml;
            }
            catch (XmlException ex)
            {
                throw ex;
            }
        }

        /// <summary> Replaces all invalid xml chars (& , " , ' , > , < > ) with their equivalent escaped notations ( &amp; , &quot; , &apos; , &gt; , &lt;)</summary>
        public static string ReplaceInvalidXmlChars(string input, bool allowQuote = false)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string output = Regex.Replace(input, "&(?!amp;)(?!lt;)(?!gt;)(?!quot;)(?!apos;)", "&amp;");
            output = Regex.Replace(output, "<", "&lt;");
            output = Regex.Replace(output, ">", "&gt;");
            output = Regex.Replace(output, "\"", "&quot;");
            if (!allowQuote)
                output = Regex.Replace(output, "\'", "&apos;");

            return output;
        }

        /// <summary> Replaces escaped xml chars ( &amp; , &quot; , &apos; , &gt; , &lt;) with their equivalent unescaped notations (& , " , ' , > , < > )</summary>
        public static string ReplaceEscapedXmlChars(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string output = input.Replace("&amp;", "&");
            output = output.Replace("&lt;", "<");
            output = output.Replace("&gt;", ">");
            output = output.Replace("&quot;", "\"");
            output = output.Replace("&apos;", "\'");

            return output;
        }
    }
}
