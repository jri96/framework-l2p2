using System.Text.Json.Serialization;
using RPG_API.Services;

namespace RPG_API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        Configure(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Dependency Injection: een controller vraagt in zijn constructor om een
        // interface, de DI-container geeft het object dat hier is geregistreerd.
        // Scoped: één object per HTTP-request.
        builder.Services.AddScoped<IStatusService, StatusService>();
        builder.Services.AddScoped<IGameInfoService, GameInfoService>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddOpenApi();
    }

    private static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.MapOpenApi(); // JSON-beschrijving op /openapi/v1.json

        // Week 3: de endpoints staan nu in controllers (map Controllers).
        app.MapControllers();
    }
}
