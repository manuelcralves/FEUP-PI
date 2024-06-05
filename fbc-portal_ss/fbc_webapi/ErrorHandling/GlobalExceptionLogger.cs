using System.Web.Http.ExceptionHandling;
using SerilogLog = Serilog.Log;

namespace fbc_webapi.ErrorHandling
{
    public class GlobalExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            SerilogLog.Error(context.Exception, "Exceção não tratada em pedido a '{@UrlPedido}'", context.Request?.RequestUri?.PathAndQuery);
        }
    }
}