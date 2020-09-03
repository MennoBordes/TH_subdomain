using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TH.WebUI.Base.Models;

namespace TH.WebUI.Base.Controllers
{
    public class CoreController : Controller
    {
        protected const string ERROR_OCCURED = "An error occured while processing your request.";
        //=== Overrides

        /// <summary> Override: Before Controller Action. </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        /// <summary> Override: After Controller Action. </summary>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        /// <summary> Override: Unhandled Exception. </summary>
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        //=== Response

        /// <summary> Returns an 'OK' (200) response. </summary>
        protected ActionResult Ok()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary> Returns an 'OK' (200) response. </summary>
        protected ActionResult OK(string message)
        {
            if (message != null)
                message = message.Replace(System.Environment.NewLine, " ");

            return new HttpStatusCodeResult(HttpStatusCode.OK, message);
        }

        /// <summary> Returns a 'BadRequest' (400) response. </summary>
        protected ActionResult BadRequest()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //return new PlainTextResult(null, (int)HttpStatusCode.BadRequest);
        }

        /// <summary> Returns a 'BadRequest' (400) response. </summary>
        protected ActionResult BadRequest(string message)
        {
            if (message != null)
                message = message.Replace(System.Environment.NewLine, " ");

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);

            //return new PlainTextResult(message, (int)HttpStatusCode.BadRequest);
        }

        /// <summary> Returns a 'Unauthorized' (401) response. </summary>
        protected HttpStatusCodeResult Unauthorized()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }

        /// <summary> Returns a 'Unauthorized' (401) response. </summary>
        protected HttpStatusCodeResult Unauthorized(string message)
        {
            if (message != null)
                message = message.Replace(System.Environment.NewLine, " ");

            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, message);
        }

        /// <summary> Returns a 'Forbidden' (403) response. </summary>
        protected HttpStatusCodeResult Forbidden()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        /// <summary> Returns a 'Forbidden' (403) response. </summary>
        protected HttpStatusCodeResult Forbidden(string message)
        {
            if (message != null)
                message = message.Replace(System.Environment.NewLine, " ");

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, message);
        }

        /// <summary> Returns an 'InternalServerError' (500) response. </summary>
        protected HttpStatusCodeResult InternalServerError()
        {
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        /// <summary> Returns an 'InternalServerError' (500) response. </summary>
        protected HttpStatusCodeResult InternalServerError(string message)
        {
            if (message != null)
                message = message.Replace(System.Environment.NewLine, " ");

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message);
        }

        //=== Response: Content

        /// <summary> Returns a 'Json' response. </summary>
        protected ContentResult Json(JToken content)
        {
            return Content(content.ToString(), MimeType.JSON);
        }

        /// <summary> Returns a 'Html' response. </summary>
        protected ContentResult Html(string content)
        {
            return Content(content, MimeType.HTML, System.Text.Encoding.UTF8);
        }

        /// <summary> Returns a 'File' response. </summary>
        //protected ActionResult File(FileReference reference)
        //{
        //    // Check
        //    if (reference == null)
        //        return HttpNotFound();

        //    // Bytes
        //    byte[] bytes = reference.Bytes ?? reference.GetBytes();
        //    if (bytes == null)
        //        return HttpNotFound();

        //    // ContentType
        //    string contentType = reference.FileExtension.GetMimeType();

        //    // Name
        //    string name = null;
        //    if (!string.IsNullOrWhiteSpace(reference.OriginalName))
        //    { name = IORepository.AdjustFileName(reference.OriginalName); }
        //    else if (!string.IsNullOrWhiteSpace(reference.TempName))
        //    { name = IORepository.AdjustFileName(reference.TempName); }
        //    else
        //    { name = "file"; }

        //    // File Name
        //    string downloadName = name + "." + reference.FileExtension;

        //    // Response
        //    return File(bytes, contentType, downloadName);
        //}

        /// <summary> Action Result : Plain Text. </summary>
        private class PlainTextResult : ActionResult
        {
            /// <summary> Text content. </summary>
            public string Text { get; set; }

            /// <summary> Http Status Code (defaults to 200). </summary>
            public int? StatusCode { get; set; }

            /// <summary> Content Encoding (defaults to UTF-8). </summary>
            public Encoding Encoding { get; set; }

            /// <summary> Construct. </summary>
            public PlainTextResult() { }

            /// <summary> Construct. </summary>
            public PlainTextResult(string text)
            {
                this.Text = text;
            }

            /// <summary> Construct. </summary>
            public PlainTextResult(string text, int statusCode)
            {
                this.Text = text;
                this.StatusCode = statusCode;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                HttpContextBase httpContextBase = context.HttpContext;
                httpContextBase.Response.Buffer = true;
                httpContextBase.Response.Clear();

                // Content Type
                httpContextBase.Response.ContentType = MimeType.TEXT;

                // Encoding
                httpContextBase.Response.ContentEncoding = this.Encoding ?? Encoding.UTF8;

                // Status Code
                httpContextBase.Response.StatusCode = this.StatusCode ?? (int)HttpStatusCode.OK;

                // Text
                if (this.Text != null)
                { httpContextBase.Response.Write(this.Text); }
            }
        }
    }
}