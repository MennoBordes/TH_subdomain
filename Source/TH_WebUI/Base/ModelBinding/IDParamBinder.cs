using System;
using System.Web.Mvc;

namespace TH.WebUI.Base.ModelBinding
{
    /// <summary> Model Binder for : ID Param. </summary>
    public class IDParamBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Extract value
            ValueProviderResult vpr = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            // Analyze
            IDParam output = new IDParam();

            if (vpr == null)
            {
                output.IsNull = true;
            }
            else
            {
                output.Raw = vpr.RawValue;

                if (vpr.RawValue is int)
                {
                    // Number
                    output.Number = Convert.ToInt32(vpr.RawValue);
                }
                else if (vpr.RawValue is string)
                {
                    string s = Convert.ToString(vpr.RawValue);

                    // Number
                    int id = 0;
                    if (int.TryParse(s, out id))
                    {
                        output.Number = id;
                    }

                    // Custom
                    else
                    {
                        output.Custom = s;

                        if (output.Custom != null && output.Custom.StartsWith("guid-"))
                        {
                            output.IsGuid = true;
                        }

                        if (string.IsNullOrWhiteSpace(output.Custom))
                        {
                            output.IsNull = true;
                            output.Custom = null;
                        }
                    }
                }
            }

            return output;
        }
    }
}