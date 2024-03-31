using System.Diagnostics;

namespace otel;

public class Subsystem
{
    private readonly ActivitySource activitySource;

    public Subsystem(ActivitySource activitySource)
    {
        this.activitySource = activitySource;
    }

    public async Task DoSomething(CancellationToken cancellationToken)
    {
        using (Activity? activity = activitySource.StartActivity("Do something in a call to an API", ActivityKind.Client))
        {
            try
            {
                await Task.Delay(1000, cancellationToken);
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
