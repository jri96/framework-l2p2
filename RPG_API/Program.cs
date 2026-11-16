using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPG_API.Database;
using RPG_API.Middleware;
using RPG_API.Models;
using RPG_API.Services;

namespace RPG_API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ApplyMigrations(app);
        Configure(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection ontbreekt in appsettings.json.");

        builder.Services.AddDbContext<GameDbContext>(options =>
        {
            options.UseLazyLoadingProxies();
            // Vaste MySQL-versie: zo kun je ook een migration maken als MySQL even niet draait.
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
        });

        // Dependency Injection
        // Singleton: één object voor de hele applicatie.
        builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();

        // Scoped: één object per HTTP-request (samen met de DbContext).
        builder.Services.AddScoped<IStatusService, StatusService>();
        builder.Services.AddScoped<IGameInfoService, GameInfoService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<GameStartService>();
        builder.Services.AddScoped<GameQueryService>();
        builder.Services.AddScoped<GameStateService>();
        builder.Services.AddScoped<GameResponseMapper>();
        builder.Services.AddScoped<MovementService>();
        builder.Services.AddScoped<CombatService>();
        builder.Services.AddScoped<CombatRoundService>();
        builder.Services.AddScoped<CombatGameService>();
        builder.Services.AddScoped<PotionService>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Validatiefouten ([Required], [StringLength], ongeldige JSON) krijgen
                // dezelfde vorm als onze andere foutmeldingen.
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = new List<string>();

                    foreach (var (key, entry) in context.ModelState)
                    {
                        if (entry.Errors.Count == 0)
                            continue;

                        // Een lege sleutel, "request" of een sleutel met "$" komt van een JSON-fout:
                        // de body kon niet worden gelezen. Die standaardmelding is Engels en technisch.
                        if (key.Length == 0 || key.StartsWith('$') || key == "request")
                            errors.Add("De verstuurde gegevens zijn geen geldige JSON of bevatten een ongeldige waarde.");
                        else
                            errors.AddRange(entry.Errors.Select(error => error.ErrorMessage));
                    }

                    return new BadRequestObjectResult(new ErrorResponse
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Error = string.Join(" ", errors.Distinct()),
                        Path = context.HttpContext.Request.Path
                    });
                };
            });

        builder.Services.AddOpenApi();
    }

    private static void Configure(WebApplication app)
    {
        // Volgorde is belangrijk: logging buitenom, zodat ook de statuscode
        // van een afgevangen fout (404, 409, ...) wordt gelogd.
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
            app.MapOpenApi(); // JSON-beschrijving op /openapi/v1.json

        app.MapControllers();
    }

    // Voert bij het starten alle migrations uit die nog niet in de database staan.
    // Bestaat de database nog niet, dan maakt EF Core hem aan.
    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        if (!context.Database.GetMigrations().Any())
        {
            app.Logger.LogWarning("Er zijn nog geen migrations. Voer eerst uit: " +
                "dotnet ef migrations add InitialGame --project RPG_API --startup-project RPG_API");
            return;
        }

        try
        {
            context.Database.Migrate();
        }
        catch (Exception exception)
        {
            app.Logger.LogCritical("Kan de database niet bijwerken. Staat MySQL (WAMP) aan en klopt de connection string? {Message}",
                exception.Message);
            throw;
        }
    }
}
