namespace TH.WebUI.Base.HtmlHelpers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Web.Mvc;

    /// <summary> Html helper for easy json access. </summary>
    public static class JsonHelper
    {
        /// <summary> Converts object to it's json representation </summary>
        /// <param name="camelCase"> If set to true, the 'camelCase' convention will be used. </param>
        public static MvcHtmlString Json(this HtmlHelper helper, object obj, bool camelCase = false, bool ignoreNullValues = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            if (camelCase)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            if (ignoreNullValues)
                settings.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(obj, settings);

            return new MvcHtmlString(json);
        }
    }
}