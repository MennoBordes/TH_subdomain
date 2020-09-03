using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TH.Core.Tools.Form.Models;

namespace TH.WebUI.Tools.Form.ModelBinders
{
    public class MetadataBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string prefix = "__metadata";
            DictionaryValueProvider<object> provider = GetDictionaryValueProvider(bindingContext);
            IDictionary<string, string> keys = provider.GetKeysFromPrefix(prefix);

            Metadata metadata = new Metadata();

            foreach (string shortKey in keys.Keys)
            {
                string key = prefix + "." + shortKey;

                string value = GetValue(bindingContext, key);

                metadata[shortKey] = value;
            }

            return metadata;
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
    }
}