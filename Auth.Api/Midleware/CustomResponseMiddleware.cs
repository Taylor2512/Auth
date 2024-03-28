
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

                // Modificar la respuesta solo si el código de estado es 200 (OK)
                if (context.Response.StatusCode == StatusCodes.Status200OK)
                {
                    // Almacenar el cuerpo original de la respuesta
                    var originalBody = context.Response.Body;

                    // Crear un nuevo MemoryStream para almacenar la respuesta modificada
                    using (var memoryStream = new MemoryStream())
                    {
                        // Establecer el cuerpo de la respuesta como el nuevo MemoryStream
                        context.Response.Body = memoryStream;

                        // Copiar el contenido del MemoryStream original al nuevo MemoryStream
                        await originalBody.CopyToAsync(memoryStream);

                        // Restablecer la posición del MemoryStream a 0 para leer desde el principio
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        // Leer el contenido del MemoryStream como una cadena
                        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                        // Crear una estructura de respuesta personalizada
                        var responseObject = new ApiResponse<string>
                        {
                            StatusCode = context.Response.StatusCode,
                            Success = true,
                            Data = responseBody
                        };

                        // Serializar la respuesta personalizada
                        var jsonResponse = JsonSerializer.Serialize(responseObject);

                        // Restablecer el cuerpo original de la respuesta
                        context.Response.Body = originalBody;

                        // Escribir la respuesta modificada en el cuerpo de la respuesta HTTP
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(jsonResponse);
                    }
                }
            
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
