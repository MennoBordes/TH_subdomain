using System;

namespace TH.WebUI.Base.ModelBinding
{
    /// <summary> ID Parameter. 
    /// <para> Can handle ID references in multiple types. </para>
    /// </summary>
    public class IDParam
    {
        /// <summary> Raw Value. </summary>
        public object Raw { get; set; }

        /// <summary> Number (eg. database reference). </summary>
        public int? Number { get; set; }

        /// <summary> Custom (eg. complex references). </summary>
        public string Custom { get; set; }

        /// <summary> Indicates that the value was not specified. </summary>
        public bool IsNull { get; set; }

        /// <summary> Indicates if ID is a guid value. </summary>
        public bool IsGuid { get; set; }

        /// <summary> Returns a IDParam representation for specified Guid value. </summary>
        public static string ToString(Guid guid)
        {
            return guid != null ? ("guid-" + guid.ToString("N")) : null;
        }
    }
}