using System.Web.Mvc;

namespace TH.WebUI.Base.HtmlHelpers
{
    /// <summary> Html helper for rendering. </summary>
    public static class RenderHelper
    {
        /// <summary> Write html into the view. </summary>
        public static void Write(this HtmlHelper helper, string content)
        {
            helper.ViewContext.Writer.Write(content);
        }

        /// <summary> Write a tag into the view. </summary>
        public static void Write(this HtmlHelper helper, TagBuilder tag)
        {
            helper.ViewContext.Writer.Write(tag.ToString());
        }
    }
}