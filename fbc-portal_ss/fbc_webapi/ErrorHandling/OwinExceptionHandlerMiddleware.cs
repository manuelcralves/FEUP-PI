using Microsoft.Owin;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using fbc_webapi.ErrorHandling;

namespace fbc_webapi
{
    public class OwinExceptionHandlerMiddleware : OwinMiddleware
    {
        public OwinExceptionHandlerMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleException(ex, context);
            }
        }

        private async Task HandleException(Exception ex, IOwinContext context)
        {
            Log.Error(ex, "Exceção não tratada em pedido a '{@UrlPedido}', em principio originado por um OWIN Middleware", context?.Request?.Uri?.PathAndQuery);

            if (context == null)
                throw new ArgumentNullException("context");

            ErrorResponse errorResponse = ErrorResponse.ConstruirRespostaDeExcecao(ex);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            if (errorResponse.StatusCode != null)
                context.Response.StatusCode = (int)errorResponse.StatusCode.Value;

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}