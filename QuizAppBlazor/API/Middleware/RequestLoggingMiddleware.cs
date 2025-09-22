using System.Diagnostics;
using System.Text;

namespace QuizAppBlazor.API.Middleware
{
    /// <summary>
    /// Middleware to log HTTP requests and responses
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            // Log request
            await LogRequestAsync(context, requestId);

            // Capture response
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {RequestId} failed with exception", requestId);
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // Log response
                await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

                // Copy response back to original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogRequestAsync(HttpContext context, string requestId)
        {
            try
            {
                var request = context.Request;
                var logData = new
                {
                    RequestId = requestId,
                    Method = request.Method,
                    Path = request.Path.Value,
                    QueryString = request.QueryString.Value,
                    UserAgent = request.Headers.UserAgent.ToString(),
                    IpAddress = GetClientIpAddress(context),
                    UserId = context.User?.Identity?.Name,
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("HTTP Request {RequestId}: {Method} {Path}{QueryString} from {IpAddress}", 
                    requestId, request.Method, request.Path, request.QueryString, GetClientIpAddress(context));

                // Log request body for POST/PUT requests (excluding sensitive endpoints)
                if ((request.Method == "POST" || request.Method == "PUT") && 
                    !request.Path.Value?.Contains("login", StringComparison.OrdinalIgnoreCase) == true &&
                    !request.Path.Value?.Contains("password", StringComparison.OrdinalIgnoreCase) == true)
                {
                    request.EnableBuffering();
                    var requestBody = await ReadRequestBodyAsync(request);
                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        _logger.LogDebug("Request {RequestId} Body: {RequestBody}", requestId, requestBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log request for {RequestId}", requestId);
            }
        }

        private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs)
        {
            try
            {
                var response = context.Response;
                var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

                _logger.Log(logLevel, "HTTP Response {RequestId}: {StatusCode} in {ElapsedMs}ms", 
                    requestId, response.StatusCode, elapsedMs);

                // Log response body for errors
                if (response.StatusCode >= 400)
                {
                    response.Body.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
                    response.Body.Seek(0, SeekOrigin.Begin);

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        _logger.LogWarning("Error Response {RequestId}: {ResponseBody}", requestId, responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log response for {RequestId}", requestId);
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
                return body;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (in case of reverse proxy)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
