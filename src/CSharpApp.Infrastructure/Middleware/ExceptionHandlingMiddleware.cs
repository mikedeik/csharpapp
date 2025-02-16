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
                // Log the full error response for debugging/monitoring
                _logger.LogWarning($"Error response from API: {ex.FullErrorResponse}");

                // Return only a simplified message to the client
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
            } catch (BadRequestException ex) {
                _logger.LogWarning($"Error response from API: {ex.FullErrorResponse}");
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
            } catch (Exception ex) {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string title, string detail) {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails {
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };

            var jsonResponse = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(jsonResponse);
        }
    }


}
