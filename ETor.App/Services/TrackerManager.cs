using System.Diagnostics;
using ETor.App.Data;
using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface ITrackerManager
{
    Task Connect(TrackerData tracker);

    Task ConnectToAll(TorrentData torrent);
}

public class TrackerManager : ITrackerManager
{
    private readonly IUdpSender _udp;
    private readonly ILogger<TrackerManager> _logger;

    public TrackerManager(IUdpSender udp, ILogger<TrackerManager> logger)
    {
        _udp = udp;
        _logger = logger;
    }

    public async Task Connect(TrackerData tracker)
    {
        if (tracker.Protocol is not TrackerProtocol.Udp)
        {
            _logger.LogWarning("Non udp tracker is not supported. {tracker}", tracker.Url);
            tracker.SetStatus(TrackerStatus.Unsupported);
            return;
        }

        tracker.SetStatus(TrackerStatus.Connecting);
        var request = new UdpConnectRequest(Random.Shared.Next());

        var connectResponse = await _udp.SendReceive<UdpConnectResponse>(tracker.Host, tracker.Port, request);

        if (connectResponse is not null)
        {
            tracker.SetConnected(connectResponse.ConnectionId, Stopwatch.GetTimestamp());
        }
        else
        {
            tracker.SetStatus(TrackerStatus.Failed);
        }
    }

    public async Task ConnectToAll(TorrentData torrent)
    {
        var connectTasks = torrent.Trackers.Select(Connect);

        await Task.WhenAll(connectTasks);
    }
}