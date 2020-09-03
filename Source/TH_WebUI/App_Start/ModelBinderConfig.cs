using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI
{
    public class ModelBinderConfig
    {
        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            // Model => FormPost
            binders.Add(typeof(TH.Core.Tools.Form.Models.FormPost), new TH.WebUI.Tools.Form.ModelBinders.FormPostBinder());
            binders.Add(typeof(IEnumerable<TH.Core.Tools.Form.Models.FormPost>), new TH.WebUI.Tools.Form.ModelBinders.FormPostBinder());

            // Model => Metadata
            binders.Add(typeof(TH.Core.Tools.Form.Models.Metadata), new TH.WebUI.Tools.Form.ModelBinders.MetadataBinder());

            // Model => JArray
            binders.Add(typeof(Newtonsoft.Json.Linq.JArray), new TH.WebUI.Base.ModelBinding.JsonBinder());

            // Model => JObject
            binders.Add(typeof(Newtonsoft.Json.Linq.JObject), new TH.WebUI.Base.ModelBinding.JsonBinder());

            // Model => FlexParam
            binders.Add(typeof(TH.WebUI.Base.ModelBinding.IDParam), new TH.WebUI.Base.ModelBinding.IDParamBinder());
        }
    }
}