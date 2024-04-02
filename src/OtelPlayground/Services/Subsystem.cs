using System.Diagnostics;

namespace otel;

public class Subsystem
{
    private readonly ActivitySource activitySource;
    private readonly IHttpClientFactory httpClientFactory;

    public Subsystem(ActivitySource activitySource, IHttpClientFactory httpClientFactory)
    {
        this.activitySource = activitySource;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task DoSomething(CancellationToken cancellationToken)
    {
        using (Activity? activity = activitySource.StartActivity("Do something in a call to an API", ActivityKind.Client))
        {
            try
            {
                using(var httpClient = httpClientFactory.CreateClient())
                {
                    HttpRequestMessage request = new(HttpMethod.Get, "https://www.google.com");
                    var response = await httpClient.SendAsync(request, cancellationToken);
                }
                activity?.AddEvent(new ActivityEvent("Did something"));
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch(Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "some error description");
            }
        }
    }

    public async Task DoSomethingElse(CancellationToken cancellationToken)
    {
        using (Activity? activity = activitySource.StartActivity("Do Something Else"))
        {
            activity?.AddEvent(new ActivityEvent("Started doing something else"));
            await Task.Delay(200, cancellationToken);
            activity?.AddEvent(new ActivityEvent("Finished doing something else"));
        }
        
    }
}
