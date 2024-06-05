using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace fbc_webapi.ErrorHandling
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Request == null)
                throw new ArgumentNullException("context.Request");

            HttpResponseMessage httpErrorResponse = ErrorResponse.ConstruirHttpResponseMessageDeExcecao(context.Exception, context.Request);

            context.Result = new ResponseMessageResult(httpErrorResponse);
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }
}