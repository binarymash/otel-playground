using System.Diagnostics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using otel.Telemetry;

namespace otel;

public static class Configuration
{
    const string serviceName = "BinaryMash.OtelPlayground.DiceRoller";

    const string serviceVersion = "2.9.3.1";

    static Uri otelCollectorUri = new Uri("http://localhost:4317");

    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder appBuilder)
    {        
        ActivitySource activitySource = new (serviceName, serviceVersion);
        
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, "some.namespace", serviceVersion)
            .AddAttributes(new Dictionary<string, object>
            { 
                ["team"] = "team-phil"
            });

        appBuilder.Logging
            .AddOpenTelemetry(options => options
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(oltpOptions => oltpOptions.Endpoint = otelCollectorUri)
        );

        appBuilder.Services
            
            .AddSingleton(activitySource)
            .AddSingleton<DomainMetrics>()

            .AddScoped<DiceService>()
            .AddScoped<Subsystem>()
            
            .AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName, serviceVersion: serviceVersion))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
//                    .AddSource(nameof(activitySource)) // seems not to be needed, as trace is being written out without it
                    .AddOtlpExporter(oltpOptions => oltpOptions.Endpoint = otelCollectorUri)
                )
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddMeter(DomainMetrics.Name)
                    .AddOtlpExporter(oltpOptions => oltpOptions.Endpoint = otelCollectorUri)
                );

        return appBuilder;
    }

    public static WebApplication RegisterMiddleware(this WebApplication app)
    {
        return app;
    }
}
