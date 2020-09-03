using System;
using System.Collections.Generic;
using System.Linq;

namespace TH.WebUI.Base.HtmlHelpers
{
    public static class EnumHelper
    {
        /// <summary> 
        /// Returns dictinary name => value for given enum.
        /// Helps with passing constants/settings to js.
        /// </summary>
        public static Dictionary<string, int> ToDictionary<TEnum>()
        {
            var type = typeof(TEnum);

            return Enum.GetValues(type)
                .Cast<int>()
                .ToDictionary(value => Enum.GetName(type, value), value => value);
        }
    }
}