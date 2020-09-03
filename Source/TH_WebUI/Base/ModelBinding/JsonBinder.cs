using System;
using System.Collections.Generic;
using System.Linq;

namespace TH.WebUI.Base.ModelBinding
{
    using System.Web.Mvc;
    using Newtonsoft.Json.Linq;
    /// <summary> Modelbinder for the 'JArray' and 'JObject' objects. 
    /// <para> Note: this is a simple implementation, it expects the value to be stringified. </para>
    /// <para> For complex objects (un-stringified) a custom valueprovider needs to be implemented... </para>
    /// </summary>
    public class JsonBinder : IModelBinder
    {
        /// <summary> Target Model Type. </summary>
        public Type ModelType { get; set; }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //DictionaryValueProvider<object> provider = this.GetDictionaryValueProvider(bindingContext);
            //ValueProviderResult value = provider.GetValue(bindingContext.ModelName);

            object provider = this.FindValue(bindingContext);

            // Only proceed if value is indeed stringified
            if (provider == null)
            {
                return null;
            }

            if (provider is ValueProviderResult)
            {
                ValueProviderResult result = (ValueProviderResult)provider;

                return this.ParseFrom(bindingContext, result);
            }
            else if (provider is DictionaryValueProvider<object>)
            {
                // Only JObject from here
                if (bindingContext.ModelType != typeof(JObject))
                {
                    if (this.ModelType != null)
                    {
                        if (bindingContext.ModelType != this.ModelType)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                DictionaryValueProvider<object> dvp = (DictionaryValueProvider<object>)provider;

                IDictionary<string, string> keys = dvp.GetKeysFromPrefix(bindingContext.ModelName);
                if (keys.Count == 0)
                    return null;

                // TODO: build recursively (now supports only single layer
                JObject jObj = new JObject();

                foreach (KeyValuePair<string, string> pair in keys)
                {
                    //string name = pair.Key;
                    //string value = pair.Value;

                    //ValueProviderResult vpr1 = dvp.GetValue(name);
                    ValueProviderResult value = dvp.GetValue(pair.Value);

                    if (value != null)
                    {
                        //    string attempted = value.AttemptedValue;
                        //    object rawValue = value.RawValue;

                        jObj[pair.Key] = value.RawValue != null ? JToken.FromObject(value.RawValue) : null;
                    }

                    //if (dvp.ContainsPrefix(value))
                    //{
                    //    IDictionary<string, string> keys2 = dvp.GetKeysFromPrefix(value);
                    //}
                }

                return jObj;
            }

            return null;
        }


        //=== Helpers

        /// <summary> Looks for value provider result. </summary>
        private object FindValue(ModelBindingContext bindingContext)
        {
            ValueProviderCollection vpc = bindingContext.ValueProvider as ValueProviderCollection;

            foreach (object vp in vpc)
            {
                if (vp is FormValueProvider)
                {
                    FormValueProvider fvp = (FormValueProvider)vp;

                    ValueProviderResult vpr = fvp.GetValue(bindingContext.ModelName);

                    if (vpr != null)
                    {
                        return vpr;
                    }
                }
                else if (vp is DictionaryValueProvider<object>)
                {
                    DictionaryValueProvider<object> dvp = (DictionaryValueProvider<object>)vp;

                    ValueProviderResult vpr = dvp.GetValue(bindingContext.ModelName);

                    if (vpr != null)
                    {
                        return vpr;
                    }
                    else if (dvp.ContainsPrefix(bindingContext.ModelName))
                    {
                        return vp;
                    }
                }
            }

            return null;
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
            ValueProviderResult result = bindingContext.ValueProvider.GetValue(key);
            return (result == null) ? null : result.AttemptedValue;
        }

        /// <summary> Parses the json object from value provider result. </summary>
        private object ParseFrom(ModelBindingContext bindingContext, ValueProviderResult result)
        {
            //string attempted = result.AttemptedValue;
            //object rawValue = result.RawValue;

            if (result != null && result.AttemptedValue != null && result.AttemptedValue.Length > 0)
            {
                // Parse
                if (bindingContext.ModelType == typeof(JArray))
                {
                    return JArray.Parse(result.AttemptedValue);
                }
                else if (bindingContext.ModelType == typeof(JObject))
                {
                    return JObject.Parse(result.AttemptedValue);
                }
                else
                {
                    return JObject.Parse(result.AttemptedValue);
                }
            }

            return null;
        }
    }
}