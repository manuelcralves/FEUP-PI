using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace fbc_webapi
{
    public class OAuth2OperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            bool isAuthorized = apiDescription.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>(true).Any()
                || (
                    apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizeAttribute>(true).Any()
                    && !apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any()
                   );

            if (!isAuthorized) return;

            operation.responses.Add("401", new Response { description = "Unauthorized" });
            operation.responses.Add("403", new Response { description = "Forbidden" });

            if (operation.security == null)
                operation.security = new List<IDictionary<string, IEnumerable<string>>>();

            Dictionary<string, IEnumerable<string>> oAuthRequirements = new Dictionary<string, IEnumerable<string>>
            {
                { "oauth2", Enumerable.Empty<string>() }
            };

            operation.security.Add(oAuthRequirements);
        }
    }
}