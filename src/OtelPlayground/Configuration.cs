using System.Diagnostics;
using Amazon.DynamoDBv2;
using LocalStack.Client.Extensions;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using otel.Telemetry;

namespace otel;

public static class Configuration
{
    const string serviceName = "BinaryMash.OtelPlayground.DiceRoller";

    const string serviceVersion = "2.12.3.1";

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
                //.AddConsoleExporter()
                .AddOtlpExporter(oltpOptions => oltpOptions.Endpoint = otelCollectorUri)
        );

        // domain
        appBuilder.Services
            .AddScoped<DiceService>()
            .AddScoped<Subsystem>();

        // infrastructure
        appBuilder.Services
            .AddHttpClient()
            .AddLocalStack(appBuilder.Configuration)
            .AddDefaultAWSOptions(appBuilder.Configuration.GetAWSOptions())
            .AddAwsService<IAmazonDynamoDB>()
            .AddSingleton<DynamoRepo>();

        // telemetry
        appBuilder.Services            
            .AddSingleton(activitySource)
            .AddSingleton<DomainMetrics>()
            .AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName, serviceVersion: serviceVersion))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAWSInstrumentation()                    
//                    .AddSource(nameof(activitySource)) // seems not to be needed, as trace is being written out without it
                    //.AddConsoleExporter()
                    .AddOtlpExporter(oltpOptions => oltpOptions.Endpoint = otelCollectorUri)
                )
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(DomainMetrics.Name)
                    //.AddConsoleExporter()
                    .AddOtlpExporter(oltpOptions => oltpOptions.Endpoint = otelCollectorUri)
                );

        return appBuilder;
    }

    public static WebApplication RegisterMiddleware(this WebApplication app)
    {
        return app;
    }
}
