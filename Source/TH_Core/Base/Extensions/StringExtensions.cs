using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace TH.Core.Base.Extensions
{
    /// <summary> Different string related extensions. </summary>
    public static class StringExtensions
    {
        //=== String

        /// <summary> Truncates the specified string.
        /// <para> Suggested usage is for managing database values. </para>
        /// </summary>
        /// <param name="maxLength"> Default value is related to the 'varchar(255)' setting. </param>
        public static string Truncate(this string input, int maxLength = 255)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return (input.Length > maxLength ? input.Substring(0, maxLength) : input);
        }

        /// <summary> Transfroms the first character into uppercase. </summary>
        public static string UppercaseFirst(this string input)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(input))
                return input;

            // Return char and concat substring.
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        /// <summary> Transfroms the first character into lowercase. </summary>
        public static string LowercaseFirst(this string input)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(input))
                return input;

            // Return char and concat substring.
            return char.ToLower(input[0]) + input.Substring(1);
        }

        /// <summary> Transfroms the string into 'camelCase'. </summary>
        public static string CamelCase(this string input)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(input))
                return input;

            string output = input;

            if (output.Contains(' '))
            {
                TextInfo ti = CultureInfo.InvariantCulture.TextInfo;

                output = ti.ToTitleCase(output);
            }

            output = output.Replace(" ", "");

            output = output.LowercaseFirst();

            return output;
        }

        /// <summary> Transfroms the string into 'PascalCase'. </summary>
        public static string PascalCase(this string input)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(input))
                return input;

            string output = input;

            if (output.Contains(' '))
            {
                TextInfo ti = CultureInfo.InvariantCulture.TextInfo;

                output = ti.ToTitleCase(output);
            }

            output = output.Replace(" ", "");

            return output;
        }

        /// <summary> Transforms pascalcase notation into a readable notation. 
        /// <para> Example: 'InsertSpaceBeforeUpperCase' to 'Insert Space Before Upper Case'. </para>
        /// </summary>
        public static string InsertSpaceBeforeUpperCase(this string input)
        {
            // Check
            if (string.IsNullOrEmpty(input))
                return input;

            // regex alternative:
            // string[] split =  Regex.Split(str, @"(?<!^)(?=[A-Z])");

            var result = string.Empty;

            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    // if not the first letter, insert space before uppercase
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += " ";
                    }
                }
                // start new word
                result += c;
            }

            return result;
        }

        /// <summary> Removes diacritics from a string. 
        /// <para> Note: Diacritics are replaced with normal variants. </para>
        /// </summary>
        public static string RemoveDiacritics(this string str)
        {
            if (str == null)
                return null;
            var chars =
                from c in str.Normalize(NormalizationForm.FormD).ToCharArray()
                let uc = CharUnicodeInfo.GetUnicodeCategory(c)
                where uc != UnicodeCategory.NonSpacingMark
                select c;

            var cleanStr = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

            return cleanStr;
        }

        /// <summary> Removes everything but 'a-z', 'A-Z' and '0-9'. </summary>
        public static string OnlyLettersAndNumbers(this string input)
        {
            if (input == null)
                return null;

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, "");
        }

        /// <summary> Removes everything but 'a-z', 'A-Z', '0-9' and ' ' . </summary>
        public static string OnlyLettersNumberAndsWhitespace(this string input)
        {
            if (input == null)
                return null;

            Regex rgx = new Regex("[^a-zA-Z0-9 ]");
            return rgx.Replace(input, "");
        }

        /// <summary> Removes everything but '0-9'. </summary>
        public static string OnlyNumbers(this string input)
        {
            if (input == null)
                return null;

            Regex rgx = new Regex("[^\\d]");
            return rgx.Replace(input, "");
        }

        /// <summary> Gets the inner string based on start- and end-string. </summary>
        /// <remarks> The part between the start- and end-string. </remarks>
        public static string GetInnerString(this string input, string start, string end)
        {
            // Check input
            if (input == null)
                return null;
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Find start
            int startIndex = 0;
            if (!string.IsNullOrEmpty(start))
            {
                if (!input.Contains(start))
                    return string.Empty;
                startIndex = input.IndexOf(start) + start.Length;
            }

            // Find end
            int endIndex = input.Length;
            if (!string.IsNullOrEmpty(end) && input.Contains(end))
                endIndex = input.IndexOf(end, startIndex);
            int length = endIndex - startIndex;

            // return substring
            return input.Substring(startIndex, length);
        }

        /// <summary> Gets the partial string based on starts-with and ends-with string. </summary>
        /// <remarks> The part between the start- and end-string INCL the start and endstring. </remarks>
        public static string GetPartialString(this string input, string startsWith, string endsWith)
        {
            // Check input
            if (input == null)
                return null;
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Find start
            int startIndex = 0;
            if (!string.IsNullOrEmpty(startsWith))
                startIndex = input.IndexOf(startsWith);
            if (startIndex == -1)
                return string.Empty;

            // Find end
            int endIndex = input.Length;
            if (!string.IsNullOrEmpty(endsWith))
                endIndex = input.IndexOf(endsWith, startIndex) + endsWith.Length;
            int length = endIndex - startIndex;

            // Return substring
            return input.Substring(startIndex, length);
        }

        /// <summary> Extracts substrings based on start and end characters. </summary>
        /// <param name="include"> Indicator if characters should be included in the output. </param>
        public static string[] ExtractSubStrings(this string input, char start, char end, bool include = false)
        {
            if (input == null)
                return null;

            // example: "< >" => "\<(.+?)\>"  

            string regex = "\\" + start + "(.+?)\\" + end;

            MatchCollection matches = Regex.Matches(input, regex);

            string[] values = new string[matches.Count];

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                if (include)
                {
                    values[i] = match.Value;
                }
                else
                {
                    values[i] = match.Value.Substring(1, match.Value.Length - 2);
                }
            }

            return values;
        }

        /// <summary> Checks whether the given string is numeric. </summary>
        /// <remarks>
        /// <para> Numeric strings consist of optional sign, any number of digits, optional decimal part and optional exponential part. 
        /// Thus +0123.45e6 is a valid numeric value. </para>
        /// <para> Hexadecimal (e.g. 0xf4c3b00c), Binary (e.g. 0b10100111001), Octal (e.g. 0777) notation is allowed too but only without sign, decimal and exponential part. </para>
        /// </remarks>
        public static bool IsNumeric(this string input)
        {
            #region Notes

            //  "123",      /* TRUE */
            //  "abc",      /* FALSE */
            //  "12.3",     /* TRUE */
            //  "+12.3",    /* TRUE */
            //  "-12.3",    /* TRUE */
            //  "1.23e2",   /* TRUE */
            //  "-1e23",    /* TRUE */
            //  "1.2ef",    /* FALSE */
            //  "0x0",      /* TRUE */
            //  "0xfff",    /* TRUE */
            //  "0xf1f",    /* TRUE */
            //  "0xf1g",    /* FALSE */
            //  "0123",     /* TRUE */
            //  "0999",     /* FALSE (not octal) */
            //  "+0999",    /* TRUE (forced decimal) */
            //  "0b0101",   /* TRUE */
            //  "0b0102"    /* FALSE */

            #endregion

            Regex isNumericRegex =
            new Regex("^(" +
                /*Hex*/ @"0x[0-9a-f]+" + "|" +
                /*Bin*/ @"0b[01]+" + "|" +
                /*Oct*/ @"0[0-7]*" + "|" +
                /*Dec*/ @"((?!0)|[-+]|(?=0+\.))(\d*\.)?\d+(e\d+)?" +
                        ")$");

            return isNumericRegex.IsMatch(input);
        }

        /// <summary> Check whether the given string is an integer. </summary>
        public static bool IsInteger(this string input)
        {
            int n;
            if (input != null && input.Length > 0 && int.TryParse(input, out n))
            {
                return true;
            }

            return false;
        }

        /// <summary> Check whether the given string is a decimal. </summary>
        public static bool IsDecimal(this string input)
        {
            decimal n;
            if (input != null && input.Length > 0 && decimal.TryParse(input, out n))
            {
                return true;
            }

            return false;
        }

        /// <summary> Check whether the given string is XHtml. </summary>
        public static bool IsXHTML(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                XElement x = XElement.Parse("<wrapper>" + input + "</wrapper>");
                return !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
            }
            catch
            {
                return true;
            }
        }


        /// <summary> Parses string to integer (success), or set to null (error). </summary>        
        public static int? ParseInteger(this string input)
        {
            int i;
            if (int.TryParse(input, out i))
            {
                return i;
            }
            else
            {
                return null;
            }
        }

        //=== NameValueCollection

        /// <summary> Creates a query string. 
        /// <para> Example: 'a=1&b=2' </para>
        /// </summary>
        public static string ToQueryString(this NameValueCollection collection, bool urlEncode = false)
        {
            StringBuilder s = new StringBuilder();

            if (collection != null)
            {
                for (int i = 0; i < collection.AllKeys.Length; i++)
                {
                    string key = collection.AllKeys[i];
                    string value = collection[key];

                    if (i > 0)
                    { s.Append("&"); }

                    if (urlEncode)
                    {
                        s.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                    }
                    else
                    {
                        s.AppendFormat("{0}={1}", key, value);
                    }
                }
            }

            return s.ToString();
        }

        /// <summary> Parses a query string. 
        /// <para> Example: 'a=1&b=2' </para>
        /// </summary>
        public static NameValueCollection ParseQueryString(this string input)
        {
            // Check
            if (string.IsNullOrEmpty(input))
                return null;

            NameValueCollection collection = HttpUtility.ParseQueryString(input);

            return collection;
        }

        /// <summary> Get typed value. </summary>
        public static T Value<T>(this NameValueCollection collection, string key)
        {
            string value = collection[key];

            Type type = typeof(T);

            object convertedValue = TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);

            return (T)convertedValue;

            //return default(T);
        }

        /// <summary> Decrypts a querystring and returns a collection. </summary>
        //public static NameValueCollection DecryptQueryString(this string queryString, string encryptionKey = null)
        //{
        //    NameValueCollection collection = new NameValueCollection();

        //    if (!string.IsNullOrWhiteSpace(queryString))
        //    {
        //        string decryptedUrl = TH.Core.Base.Common.Utilities.Encryption.URLDecrypt(queryString, key: encryptionKey);

        //        if (!string.IsNullOrWhiteSpace(decryptedUrl))
        //        {
        //            collection = HttpUtility.ParseQueryString(decryptedUrl);
        //        }
        //    }

        //    return collection;
        //}


        //=== Dictionary

        /// <summary> Create Query String. 
        /// <para> Example: 'a=1&b=2' </para>
        /// </summary>
        public static string ToQueryString(this Dictionary<string, object> collection, bool urlEncode = false)
        {
            StringBuilder s = new StringBuilder();

            if (collection != null)
            {
                int i = 0;
                foreach (KeyValuePair<string, object> pair in collection)
                {
                    string key = pair.Key;
                    string value = pair.Value != null ? Convert.ToString(pair.Value) : string.Empty;

                    if (i > 0)
                    { s.Append("&"); }

                    if (urlEncode)
                    {
                        s.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                    }
                    else
                    {
                        s.AppendFormat("{0}={1}", key, value);
                    }

                    i++;
                }
            }

            return s.ToString();
        }
    }
}
