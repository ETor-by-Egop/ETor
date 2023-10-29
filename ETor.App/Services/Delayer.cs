using System.Collections.Concurrent;
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
    Task ReannounceToTrackerTask(TorrentData torrent, TrackerData tracker);
}

public class Delayer : IDelayer
{
    private readonly IServiceProvider _serviceProvider;
    
    private readonly ConcurrentPriorityQueue<long, IDelayedTask> _tasks;
    private readonly ILogger<Delayer> _logger;

    public Delayer(IServiceProvider serviceProvider, ILogger<Delayer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _tasks = new ConcurrentPriorityQueue<long, IDelayedTask>();
    }

    public void BeginMonitor(IDelayedTask task, long executeAt)
    {
        _tasks.Enqueue(executeAt, task);
        _logger.LogInformation("Delayer begin monitoring {task}", task);
    }

    public async Task ReconnectToTracker(TrackerData tracker)
    {
        _logger.LogInformation("Delayer reconnecting tracker {url}", tracker.Url);
        await _serviceProvider.GetRequiredService<ITrackerManager>().BeginConnect(tracker);
    }

    public async Task ReannounceToTrackerTask(TorrentData torrent, TrackerData tracker)
    {
        _logger.LogInformation("Delayer reannouncing tracker {torrent} - {url}", torrent.Name, tracker.Url);
        await _serviceProvider.GetRequiredService<ITrackerManager>().BeginAnnounce(torrent, tracker);
    }

    public void Update()
    {
        if (_tasks.TryPeek(out var executeAt, out var task))
        {
            if (executeAt <= Stopwatch.GetTimestamp())
            {
                _logger.LogInformation("Delayer processing {task}", task);
                var _ = _tasks.TryDequeue(out var _);
                task.ExecuteAsync(this);
            }
        }
    }
}