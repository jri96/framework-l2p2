namespace RPG_API.Exceptions;

// Eigen exceptions. In week 7 worden ze vertaald naar een HTTP-statuscode.

// 404 Not Found: het gevraagde spel bestaat niet.
public class GameNotFoundException : Exception
{
    public GameNotFoundException(int gameId)
        : base($"Spel {gameId} bestaat niet.") { }
}

// 400 Bad Request: de invoer zelf is ongeldig (bijvoorbeeld een lege naam).
public class InvalidInputException : Exception
{
    public InvalidInputException(string message) : base(message) { }
}

// 409 Conflict: de invoer klopt, maar de actie mag niet in de huidige toestand
// van het spel (bijvoorbeeld bewegen tijdens een gevecht).
public class GameRuleException : Exception
{
    public GameRuleException(string message) : base(message) { }
}
