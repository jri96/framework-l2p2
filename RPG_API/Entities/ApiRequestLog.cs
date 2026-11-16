using System.ComponentModel.DataAnnotations;

namespace RPG_API.Entities;

// Eén regel per HTTP-request, gevuld door RequestLoggingMiddleware.
public class ApiRequestLog
{
    [Key]
    public long Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(10)]
    public required string Method { get; set; }

    [MaxLength(300)]
    public required string Path { get; set; }

    public int StatusCode { get; set; }
    public long DurationMilliseconds { get; set; }

    [MaxLength(1000)]
    public string? ExceptionMessage { get; set; }
}
