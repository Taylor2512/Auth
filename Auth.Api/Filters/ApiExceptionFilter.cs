using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using Auth.Api.Model;
using Auth.Shared.Extensions.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Api.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = new ApiResponse();
            var errorResponse = new ErrorResponse();
            switch (context.Exception)
            {
                case InvalidDataException invalidDataException:
                    HandleInvalidDataException(invalidDataException, response, errorResponse, context);
                    break;

                case NotFoundException notFoundException:
                    HandleNotFoundException(notFoundException, response, errorResponse, context);
                    break;

                case ArgumentException argumentException:
                    HandleArgumentException(argumentException, response, errorResponse, context);
                    break;

                default:
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        errorResponse.Message = context.Exception.InnerException?.InnerException?.Message?? context.Exception.InnerException?.Message?? context.Exception.Message;
                    }
                    else
                    {
                        errorResponse.Message = "Ocurrió un error interno en el servidor.";
                    }
                     context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            response.Error = errorResponse;
            context.Result = new ObjectResult(response);
            context.ExceptionHandled = true;
        }

        private void HandleInvalidDataException(InvalidDataException exception, ApiResponse response, ErrorResponse errorResponse, ExceptionContext context)
        {
            response.StatusCode = StatusCodes.Status422UnprocessableEntity;

            // Verificar si el mensaje es un array de strings
            if (IsValidArrayStrings(exception.Message))
            {
                // El mensaje ya es un array de strings, no es necesario modificarlo
                errorResponse.Message = exception.Message;
            }
            else
            {
                // El mensaje no es un array de strings, lo convertimos en un array de strings
                string[] errorMessages = { exception.Message };
                errorResponse.Message = JsonSerializer.Serialize(errorMessages);
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        }

        private void HandleNotFoundException(NotFoundException exception, ApiResponse response, ErrorResponse errorResponse, ExceptionContext context)
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            errorResponse.Message = exception.Message;
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleArgumentException(ArgumentException exception, ApiResponse response, ErrorResponse errorResponse, ExceptionContext context)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;

            // Verificar si el mensaje es un array de strings
            if (IsValidArrayStrings(exception.Message))
            {
                // El mensaje ya es un array de strings, no es necesario modificarlo
                errorResponse.Message = exception.Message;
            }
            else
            {
                // El mensaje no es un array de strings, lo convertimos en un array de strings
                string[] errorMessages = { exception.Message };
                errorResponse.Message = JsonSerializer.Serialize(errorMessages);
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private static bool IsValidArrayStrings(string Message)
        {
            return Message.StartsWith('[') && Message.EndsWith(']');
        }
    }
}
