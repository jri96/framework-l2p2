using RPG_API.Models;
using RPG_API.Services;

namespace RPG_API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        // Week 1: werkt de API?
        app.MapGet("/api/status", () => new StatusResponse
        {
            Message = "API werkt",
            ServerTime = DateTime.Now
        });

        // Week 2: tweede GET-endpoint met spelinformatie.
        // De gegevens blijven in de API; de console toont ze alleen.
        app.MapGet("/api/info", () => new GameInfoResponse
        {
            Title = GameRules.Title,
            Description = GameRules.Description,
            MaxLevel = GameRules.MaxLevel
        });

        app.Run();
    }
}
