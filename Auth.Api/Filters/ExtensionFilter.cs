

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Auth.Api.Filters
{

    public static class ExtensionFilter
    {
        public static BadRequestObjectResult GenerateBadRequest(this ActionContext context)
        {
            BadRequestObjectResult badRequestObjectResult;
            object errorResponse;
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value!=null&& x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp =>  kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray());

                string error = "";
                foreach (var errores in errorsInModelState)
                {
                    var mensajeError = $"{errores.Key}:  {string.Join("\r\n", errores.Value?? errores.Value)}";
                    error = string.IsNullOrEmpty(error) ? mensajeError : $"{error} {mensajeError}";
                }

                errorResponse = new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = error,
                    traceId = context.HttpContext.TraceIdentifier
                };

                badRequestObjectResult = new BadRequestObjectResult(errorResponse)
                {
                    ContentTypes =
                        {
                            System.Net.Mime.MediaTypeNames.Application.Json,
                            System.Net.Mime.MediaTypeNames.Application.Xml
                        }
                };
                return badRequestObjectResult;
            }

             errorResponse = new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                title = "One or more validation errors occurred.",
                status = 400,
                traceId = context.HttpContext.TraceIdentifier
            };

             badRequestObjectResult = new BadRequestObjectResult(errorResponse)
            {
                ContentTypes =
                    {
                        System.Net.Mime.MediaTypeNames.Application.Json,
                        System.Net.Mime.MediaTypeNames.Application.Xml
                    }
            };
            return badRequestObjectResult;
        }
    }
}
