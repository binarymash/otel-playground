using System.Diagnostics;
using System.Globalization;
using otel.Telemetry;

namespace otel;

public class DiceService
{
    private readonly ILogger<DiceService> logger;
    private readonly Subsystem subsystem;
    private readonly DynamoRepo repo;
    private readonly DomainMetrics metrics;

    public DiceService(ILogger<DiceService> logger, Subsystem subsystem, DynamoRepo repo, DomainMetrics metrics)
    {
        this.logger = logger;
        this.subsystem = subsystem;
        this.repo = repo;
        this.metrics = metrics;
    }

    public async Task<IResult> RollDice(string? player, CancellationToken cancellationToken)
    {
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

        await repo.StoreDiceRoll(player, result, cancellationToken);
        Activity.Current?.AddEvent(new ActivityEvent("Stored dice roll"));

        await Task.WhenAll(
            subsystem.DoSomething(cancellationToken),
            subsystem.DoSomethingElse(cancellationToken)
        );

        return Results.Ok(result.ToString(CultureInfo.InvariantCulture));       
    }
}
