using System.Diagnostics;
using ETor.App.Data;
using ETor.App.DelayedTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface IDelayer
{
    void BeginMonitor(IDelayedTask task, long executeAt);
    Task ReconnectToTracker(TrackerData tracker);
    void Update();
}

public class Delayer : IDelayer
{
    private readonly IServiceProvider _serviceProvider;
    
    private readonly PriorityQueue<IDelayedTask, long> _tasks;
    private readonly ILogger<Delayer> _logger;

    public Delayer(IServiceProvider serviceProvider, ILogger<Delayer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _tasks = new PriorityQueue<IDelayedTask, long>(1000);
    }

    public void BeginMonitor(IDelayedTask task, long executeAt)
    {
        _tasks.Enqueue(task, executeAt);
        _logger.LogInformation("Delayer begin monitoring {task}", task);
    }

    public async Task ReconnectToTracker(TrackerData tracker)
    {
        _logger.LogInformation("Delayer reconnecting tracker {url}", tracker.Url);
        await _serviceProvider.GetRequiredService<ITrackerManager>().BeginConnect(tracker);
    }

    public void Update()
    {
        if (_tasks.TryPeek(out var task, out var executeAt))
        {
            if (executeAt <= Stopwatch.GetTimestamp())
            {
                _logger.LogInformation("Delayer processing {task}", task);
                var _ = _tasks.Dequeue();
                task.ExecuteAsync(this);
            }
        }
    }
}