using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TH.Core.Tools.Form.Models;

namespace TH.WebUI.Tools.Form.ModelBinders
{
    public class FormPostBinder : IModelBinder
    {
        private const string key_token = "__RequestVerificationToken";
        private const string prefix_metadata = "__metadata";
        private const string prefix_data = "__data";

        /// <summary> Creates Model from Request. </summary>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Provider
            DictionaryValueProvider<object> provider = GetDictionaryValueProvider(bindingContext);
            if (provider == null)
                return null;

            // Check Type
            bool multiple = (bindingContext.ModelType == typeof(FormPost)) ? false : true;

            if (multiple)
            {
                List<FormPost> list = new List<FormPost>();

                List<string> prefixes = new List<string>();

                string prefixFormat = provider.ContainsPrefix(bindingContext.ModelName + "[0]") ? bindingContext.ModelName + "[{0}]" : "[{0}]";

                int index = 0;
                while (provider.ContainsPrefix(string.Format(prefixFormat, index)))
                {
                    string prefix = string.Format(prefixFormat, index);

                    string _key_token = prefix + "." + key_token;
                    string _prefix_metadata = prefix + "." + prefix_metadata;
                    string _prefix_data = prefix + "." + prefix_data;

                    FormPost formPost = new FormPost();

                    // Retrieve RequestVerificationToken
                    formPost.RequestVerificationToken = ParseToken(bindingContext, provider, _key_token);

                    // Retrieve Metadata
                    formPost.Metadata = ParseMetadata(bindingContext, provider, _prefix_metadata);

                    // Retrieve Data
                    formPost.Data = ParseData(bindingContext, provider, _prefix_data);

                    list.Add(formPost);

                    index++;
                }

                return list.AsEnumerable();
            }
            else
            {
                string _key_token = key_token;
                string _prefix_metadata = prefix_metadata;
                string _prefix_data = prefix_data;

                if (provider.ContainsPrefix(bindingContext.ModelName))
                {
                    _key_token = bindingContext.ModelName + "." + key_token;
                    _prefix_metadata = bindingContext.ModelName + "." + prefix_metadata;
                    _prefix_data = bindingContext.ModelName + "." + prefix_data;
                }

                FormPost formPost = new FormPost();

                // Retrieve RequestVerificationToken
                formPost.RequestVerificationToken = ParseToken(bindingContext, provider, _key_token);

                // Retrieve Metadata
                formPost.Metadata = ParseMetadata(bindingContext, provider, _prefix_metadata);

                // Retrieve Data
                formPost.Data = ParseData(bindingContext, provider, _prefix_data);

                return formPost;
            }
        }

        /// <summary> Get value provider. </summary>
        private DictionaryValueProvider<object> GetDictionaryValueProvider(ModelBindingContext bindingContext)
        {
            ValueProviderCollection vpc = bindingContext.ValueProvider as ValueProviderCollection;

            DictionaryValueProvider<object> vp = vpc.Where(x => x.GetType() == typeof(DictionaryValueProvider<object>)).FirstOrDefault() as DictionaryValueProvider<object>;

            return vp;
        }

        /// <summary> Get value. </summary>
        private string GetValue(ModelBindingContext bindingContext, string key)
        {
            var result = bindingContext.ValueProvider.GetValue(key);
            return (result == null) ? null : result.AttemptedValue;
        }

        /// <summary> Parse Token. </summary>
        private string ParseToken(ModelBindingContext bindingContext, DictionaryValueProvider<object> provider, string key)
        {
            return this.GetValue(bindingContext, key);
        }

        /// <summary> Parse Metadata values. </summary>
        private Metadata ParseMetadata(ModelBindingContext bindingContext, DictionaryValueProvider<object> provider, string prefix)
        {
            Metadata metadata = new Metadata();

            IDictionary<string, string> keys = provider.GetKeysFromPrefix(prefix);

            foreach (string shortKey in keys.Keys)
            {
                string key = prefix + "." + shortKey;

                string value = GetValue(bindingContext, key);

                metadata[shortKey] = value;
            }

            return metadata;
        }

        /// <summary> Parse Data values. </summary>
        private Dictionary<string, string> ParseData(ModelBindingContext bindingContext, DictionaryValueProvider<object> provider, string prefix)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            IDictionary<string, string> keys = provider.GetKeysFromPrefix(prefix);

            foreach (string shortKey in keys.Keys)
            {
                string key = prefix + "." + shortKey;

                string value = GetValue(bindingContext, key);

                data[shortKey] = value;
            }

            return data;
        }

        /// <summary> Parses a single FormPost object from json. </summary>
        public static FormPost FromJson(JToken json)
        {
            if (json == null)
                return null;

            try
            {
                FormPost obj = new FormPost();

                obj.RequestVerificationToken = json[key_token] != null ? json[key_token].Value<string>() : null;

                JEnumerable<JProperty> properties = json.Children<JProperty>();

                obj.Metadata = new Metadata();
                foreach (JProperty prop in properties.Where(x => x.Name.StartsWith(prefix_metadata)))
                {
                    obj.Metadata[prop.Name.Substring(prefix_metadata.Length + 1)] = prop.Value.Value<object>();
                }

                obj.Data = new Dictionary<string, string>();
                foreach (JProperty prop in properties.Where(x => x.Name.StartsWith(prefix_data)))
                {
                    obj.Data[prop.Name.Substring(prefix_data.Length + 1)] = prop.Value.Value<string>();
                }

                return obj;
            }
            catch
            {
                return null;
            }
        }
    }
}