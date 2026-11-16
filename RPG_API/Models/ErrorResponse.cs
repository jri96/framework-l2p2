namespace RPG_API.Models;

// Vaste vorm voor iedere foutmelding van de API, zodat de console hem altijd kan lezen.
public class ErrorResponse
{
    public int Status { get; set; }
    public string Error { get; set; } = "";
    public string Path { get; set; } = "";
}
