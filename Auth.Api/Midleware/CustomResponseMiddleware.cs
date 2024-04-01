
using Auth.Api.Model;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Auth.Api.Midleware
{

    public class CustomErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            
            }
            catch (Exception ex)
            {
                if (context.Response.StatusCode == 400 && context.Response.ContentType?.Contains("application/json") == true)
                {
                    var response = new
                    {
                        type = "https://your-custom-error-url",
                        title = "One or more validation errors occurred.",
                        status = 400,
                        errors = new { message = "Your custom validation error message" },
                        traceId = context.TraceIdentifier
                    };

                    var jsonResponse = JsonSerializer.Serialize(response);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonResponse);
                }
            }
        }
    }

    public static class CustomErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomErrorHandlingMiddleware>();
        }
    }

}
