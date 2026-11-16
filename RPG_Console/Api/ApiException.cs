namespace RPG_Console.Api;

// Een fout die de API netjes heeft teruggestuurd (400, 404, 409, 500).
public class ApiException : Exception
{
    public ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
