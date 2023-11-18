using ETor.App.Data;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface ITrackerManager
{
    void StartAll(TorrentData torrent);
    Dictionary<Guid, TrackerMonitoringThread> MonitoringThreads { get; }
}

public class TrackerManager : ITrackerManager
{
    public Dictionary<Guid, TrackerMonitoringThread> MonitoringThreads { get; }

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TrackerManager> _logger;

    public TrackerManager(ILoggerFactory loggerFactory, ILogger<TrackerManager> logger)
    {
        _loggerFactory = loggerFactory;
        _logger = logger;
        MonitoringThreads = new Dictionary<Guid, TrackerMonitoringThread>();
    }

    public void StartAll(TorrentData torrent)
    {
        foreach (var tracker in torrent.Trackers)
        {
            var thread = new TrackerMonitoringThread(tracker, torrent, _loggerFactory.CreateLogger<TrackerMonitoringThread>());
            MonitoringThreads[tracker.InternalId] = thread;
            thread.Start();
        }
    }
}