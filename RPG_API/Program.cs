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

        // Week 1: eerste endpoint. Open http://localhost:5265/api/status in de browser.
        app.MapGet("/api/status", () => new { message = "API werkt" });

        app.Run();
    }
}
