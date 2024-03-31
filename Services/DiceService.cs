using System.Globalization;
using otel.Telemetry;

namespace otel;

public class DiceService
{
    private readonly ILogger<DiceService> logger;
    private readonly Subsystem subsystem;
    private readonly DomainMetrics metrics;

    public DiceService(ILogger<DiceService> logger, Subsystem subsystem, DomainMetrics metrics)
    {
        this.logger = logger;
        this.subsystem = subsystem;
        this.metrics = metrics;
    }

    public async Task<IResult> RollDice(string? player, CancellationToken cancellationToken)
    {
        await subsystem.DoSomething(cancellationToken);
        await subsystem.DoSomethingElse(cancellationToken);

        var result = Random.Shared.Next(1, 7);

        metrics.DiceRolled(result);

        if (string.IsNullOrEmpty(player))
        {
            logger.LogInformation("Anonymous player is rolling the dice: {result}", result);
        }
        else
        {
            logger.LogInformation("{player} is rolling the dice: {result}", player, result);
        }

        return Results.Ok(result.ToString(CultureInfo.InvariantCulture));       
    }
}
