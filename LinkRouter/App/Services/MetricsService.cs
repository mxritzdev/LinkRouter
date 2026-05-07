using Prometheus;

namespace LinkRouter.App.Services;


public class MetricsService
{
    private readonly Counter RouteCounter = Metrics.CreateCounter(
        "linkrouter_requests",
        "Counts the number of requests to the link router",
        new CounterConfiguration
        {
            LabelNames = new[] { "route" }
        }
    );


    private readonly Counter NotFoundCounter = Metrics.CreateCounter(
        "linkrouter_404_requests",
        "Counts the number of not found requests to the link router",
        new CounterConfiguration
        {
            LabelNames = new[] { "route" }
        }
    );

    public Task IncrementNotFound(string path)
    {
        NotFoundCounter
            .WithLabels(path)
            .Inc();

        return Task.CompletedTask;
    }

    public Task IncrementFound(string path)
    {
        RouteCounter
            .WithLabels(path)
            .Inc();

        return Task.CompletedTask;
    }
}