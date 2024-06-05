using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace fbc_webapi.Controllers
{
    public class InfoController : ApiController
    {
        /// <summary>
        /// Versao
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        public IHttpActionResult Versao()
        {
            return Ok($"Versão {Assembly.GetExecutingAssembly()?.GetName().Version}");
        }
    }
}
