using RPG_API.Exceptions;
using RPG_API.Models;

namespace RPG_API.Middleware;

// Vangt alle exceptions uit controllers en services af en maakt er een
// begrijpelijke JSON-foutmelding met de juiste HTTP-statuscode van.
public class ExceptionHandlingMiddleware
{
    public const string ExceptionMessageKey = "ExceptionMessage";

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var message = exception.Message;

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                // Onverwachte fout: details alleen in de API-terminal, niet naar de console sturen.
                _logger.LogError(exception, "Onverwachte fout bij {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
                message = "Er ging iets mis in de API. Bekijk de API-terminal voor details.";
            }

            // Zodat RequestLoggingMiddleware de foutmelding ook kan opslaan.
            httpContext.Items[ExceptionMessageKey] = exception.Message;

            if (httpContext.Response.HasStarted)
                throw;

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Status = statusCode,
                Error = message,
                Path = httpContext.Request.Path
            });
        }
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            GameNotFoundException => StatusCodes.Status404NotFound,
            InvalidInputException => StatusCodes.Status400BadRequest,
            GameRuleException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
