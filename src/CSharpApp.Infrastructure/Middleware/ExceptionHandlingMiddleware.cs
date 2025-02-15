using CSharpApp.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;



namespace CSharpApp.Infrastructure.Middleware {
    public class ExceptionHandlingMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch (NotFoundException ex) {
                _logger.LogWarning(ex.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json"; // Set the response content type

                var problemDetails = new ProblemDetails {
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                var jsonResponse = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(jsonResponse);
            } catch (Exception ex) {
                _logger.LogError(ex, "An unexpected error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json"; // Set the response content type

                var problemDetails = new ProblemDetails {
                    Title = "Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred.",
                    Instance = context.Request.Path
                };

                var jsonResponse = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }


}
