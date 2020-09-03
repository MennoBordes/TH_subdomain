using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TH2.Server.Modules.Base
{
    /// <summary>
    /// The default controller to inherit from.
    /// Adds fault handling.
    /// </summary>
    public class BaseController : ControllerBase
    {
        protected HttpResponseMessage BadRequest(string message)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (message != null)
            {
                response.Content = new StringContent(message, Encoding.UTF8);
            }

            return response;
        }
    }
}
