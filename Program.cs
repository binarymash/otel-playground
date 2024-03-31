using otel;

var builder = WebApplication.CreateBuilder(args)
    .RegisterServices();

var app = builder.Build();
app.RegisterMiddleware();

app.MapGet("/rolldice/{player?}", async (DiceService service, string? player, CancellationToken ct) => await service.RollDice(player, ct));

app.Run();
