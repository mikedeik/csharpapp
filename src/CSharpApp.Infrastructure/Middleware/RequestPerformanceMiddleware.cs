using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Infrastructure.Middleware {
    

    namespace App.Api.Middleware {
        public class RequestPerformanceMiddleware {
            private readonly RequestDelegate _next;
            private readonly ILogger<RequestPerformanceMiddleware> _logger;

            public RequestPerformanceMiddleware(RequestDelegate next, ILogger<RequestPerformanceMiddleware> logger) {
                _next = next;
                _logger = logger;
            }

            public async Task Invoke(HttpContext context) {
                var stopwatch = Stopwatch.StartNew();

                await _next(context); 

                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                if (elapsedMilliseconds > 500) 
                {
                    _logger.LogWarning(
                        "Slow request detected: {Method} {Path} took {ElapsedMilliseconds}ms",
                        context.Request.Method,
                        context.Request.Path,
                        elapsedMilliseconds
                    );
                } else {
                    _logger.LogInformation(
                        "Request: {Method} {Path} completed in {ElapsedMilliseconds}ms",
                        context.Request.Method,
                        context.Request.Path,
                        elapsedMilliseconds
                    );
                }
            }
        }
    }

}
