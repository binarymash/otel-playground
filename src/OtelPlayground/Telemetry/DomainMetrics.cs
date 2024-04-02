using System.Diagnostics.Metrics;

namespace otel.Telemetry;

public class DomainMetrics
{
    public const string Name = "DomainMetrics";

    private Meter _meter;
    
    private Counter<int> _rollCounter;
    
    private Histogram<int> _rollDistribution;

    public DomainMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(Name);

        _rollDistribution = _meter.CreateHistogram<int>("roll.distribution");
        _rollCounter = _meter.CreateCounter<int>("roll.counter");
    }

    public void DiceRolled(int result)
    {
        _rollCounter.Add(1);
        _rollDistribution.Record(result);
    }
}
