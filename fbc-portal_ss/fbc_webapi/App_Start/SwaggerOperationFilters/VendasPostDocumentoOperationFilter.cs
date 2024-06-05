using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace fbc_webapi
{
    public class VendasPostDocumentoOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.operationId == "Vendas_PostDocumento")
            {
                if (!operation.parameters.Any(p => p.name == "documento"))
                {
                    operation.parameters.Add(new Parameter()
                    {
                        name = "documento",
                        required = true,
                        @in = "formData",
                        schema = new Schema()
                        {
                            @ref = "#/definitions/DocumentoVenda"
                        },
                    });
                }

                if (!operation.parameters.Any(p => p.name == "ficheirosAnexos"))
                {
                    operation.parameters.Add(new Parameter()
                    {
                        name = "ficheirosAnexos",
                        required = false,
                        @in = "formData",
                        type = "file",
                    });
                }
            }
        }
    }
}