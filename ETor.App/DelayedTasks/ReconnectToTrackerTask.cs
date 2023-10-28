using ETor.App.Data;
using ETor.App.Services;

namespace ETor.App.DelayedTasks;

public class ReconnectToTrackerTask : IDelayedTask
{
    private readonly TrackerData _tracker;

    private readonly string _asString;

    public ReconnectToTrackerTask(TrackerData tracker)
    {
        _tracker = tracker;
        _asString = $"ReconnectToTrackerTask: {_tracker.Url}";
    }

    public async Task ExecuteAsync(IDelayer delayer)
    {
        await delayer.ReconnectToTracker(_tracker);
    }

    public override string ToString()
    {
        return _asString;
    }
}