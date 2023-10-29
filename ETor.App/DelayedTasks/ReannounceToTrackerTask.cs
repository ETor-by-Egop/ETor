using ETor.App.Data;
using ETor.App.Services;

namespace ETor.App.DelayedTasks;

public class ReannounceToTrackerTask : IDelayedTask
{
    private readonly TorrentData _torrent;
    private readonly TrackerData _tracker;

    private readonly string _asString;

    public ReannounceToTrackerTask(TorrentData torrent, TrackerData tracker)
    {
        _torrent = torrent;
        _tracker = tracker;
        _asString = $"ReannounceToTrackerTask: {torrent.Name} - {_tracker.Url}";
    }

    public async Task ExecuteAsync(IDelayer delayer)
    {
        await delayer.ReannounceToTrackerTask(_torrent, _tracker);
    }

    public override string ToString()
    {
        return _asString;
    }
}