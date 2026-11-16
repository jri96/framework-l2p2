using System.Diagnostics;
using RPG_API.Database;
using RPG_API.Entities;

namespace RPG_API.Middleware;

// Iedere request gaat door deze middleware. Na afloop wordt methode, route,
// statuscode en duur opgeslagen in de tabel ApiRequestLogs.
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();
        string? exceptionMessage = null;

        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            exceptionMessage = exception.Message;
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            exceptionMessage ??= httpContext.Items[ExceptionHandlingMiddleware.ExceptionMessageKey] as string;

            var requestLog = new ApiRequestLog
            {
                Method = httpContext.Request.Method,
                Path = Shorten(httpContext.Request.Path.ToString(), 300),
                StatusCode = httpContext.Response.StatusCode,
                DurationMilliseconds = stopwatch.ElapsedMilliseconds,
                ExceptionMessage = exceptionMessage is null ? null : Shorten(exceptionMessage, 1000)
            };

            _logger.LogInformation("{Method} {Path} -> {StatusCode} ({Duration} ms)",
                requestLog.Method, requestLog.Path, requestLog.StatusCode, requestLog.DurationMilliseconds);

            await SaveLogAsync(requestLog);
        }
    }

    // Een eigen DbContext (nieuwe scope), zodat een mislukte spelactie het
    // opslaan van de log niet meeneemt, en andersom.
    private async Task SaveLogAsync(ApiRequestLog requestLog)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            context.ApiRequestLogs.Add(requestLog);
            await context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            _logger.LogWarning("Request-log kon niet worden opgeslagen: {Message}", exception.Message);
        }
    }

    private static string Shorten(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
