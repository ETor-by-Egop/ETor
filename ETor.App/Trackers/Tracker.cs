using System.Diagnostics;
using System.Timers;
using ETor.App.Data;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace ETor.App.Trackers;

public abstract class Tracker
{
    private Timer _timer;
    private readonly ILogger<Tracker> _logger;

    public Tracker(TrackerData trackerData, Memory<byte> peerId, Memory<byte> torrentInfoHash, short listeningPort, ILogger<Tracker> logger)
    {
        UpdateInterval = TimeSpan.FromMinutes(10);
        TrackerData = trackerData;
        TorrentInfoHash = torrentInfoHash;
        ListeningPort = listeningPort;
        _logger = logger;
        PeerId = peerId;
    }

    public Memory<byte> PeerId { get; set; }

    public short ListeningPort { get; set; }

    public Memory<byte> TorrentInfoHash { get; set; }

    public TrackerData TrackerData { get; set; }

    public TimeSpan UpdateInterval { get; set; }
    public int WantedPeerCount { get; set; }
    public long Left { get; set; }
    public long Downloaded { get; set; }
    public long Uploaded { get; set; }

    public event AnnouncedEventHandler? Announced;

    public event AnnouncingEventHandler? Announcing;

    public event AnnounceFailedEventHandler? AnnounceFailed;

    public void StartTracking()
    {
        _logger.LogInformation("starting tracking {uri} for torrent {torrent}", this.TrackerData, this.TorrentInfoHash);

        OnStart();

        _timer = new System.Timers.Timer();
        _timer.Interval = TimeSpan.FromSeconds(1)
            .TotalMilliseconds;
        _timer.Elapsed += Timer_Elapsed;
        _timer.Enabled = true;
        _timer.Start();
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _timer.Interval = TimeSpan.FromDays(1)
            .TotalMilliseconds;

        OnAnnounce();

        _timer.Interval = UpdateInterval.TotalMilliseconds;
    }

    protected abstract Task OnStart();
    protected abstract Task OnAnnounce();

    protected void OnAnnounced(TrackerData tracker, AnnouncedEventArgs eventArgs)
    {
        Announced?.Invoke(tracker, eventArgs);
    }

    protected void OnAnnouncing(TrackerData tracker)
    {
        Announcing?.Invoke(tracker);
    }

    protected void OnAnnounceFailed(TrackerData tracker)
    {
        AnnounceFailed?.Invoke(tracker);
        _timer.Stop();
    }
}