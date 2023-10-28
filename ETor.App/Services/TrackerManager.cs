using System.Diagnostics;
using System.Net.Sockets;
using ETor.App.Data;
using ETor.App.DelayedTasks;
using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface ITrackerManager
{
    Task BeginConnect(TrackerData tracker);

    Task BeginConnectToAll(TorrentData torrent);
}

public class TrackerManager : ITrackerManager, IDisposable
{
    private readonly IDelayer _delayer;

    private readonly Dictionary<Guid, UdpClient> _udpClients;

    private readonly ILogger<TrackerManager> _logger;

    public TrackerManager(IDelayer delayer, ILogger<TrackerManager> logger)
    {
        _delayer = delayer;
        _logger = logger;
        _udpClients = new Dictionary<Guid, UdpClient>();
    }

    public async Task BeginConnect(TrackerData tracker)
    {
        if (tracker.Protocol is not TrackerProtocol.Udp)
        {
            _logger.LogWarning("Non udp tracker is not supported. {tracker}", tracker.Url);
            tracker.SetUnsupported();
            return;
        }

        tracker.SetConnecting();

        if (!_udpClients.TryGetValue(tracker.InternalId, out var udpClient))
        {
            udpClient = new UdpClient();
            _udpClients[tracker.InternalId] = udpClient;
        }

        // If a response is not received after 15 * 2 ^ n seconds, the client should retransmit the request,
        // where n starts at 0 and is increased up to 8 (3840 seconds) after every retransmission.
        var waitSeconds = 15 * (int) Math.Pow(2, tracker.MadeAttempts);

        using var source = new CancellationTokenSource(waitSeconds * 1000);

        var request = new UdpConnectRequest(Random.Shared.Next());
        byte[] buffer = new byte[request.SerializedSize];
        request.Serialize(buffer);

        var connectResponse = await udpClient.SendReceive<UdpConnectResponse>(
            tracker.Host,
            tracker.Port,
            buffer.AsMemory(),
            source.Token
        );

        if (connectResponse is not null)
        {
            tracker.SetConnected(connectResponse.ConnectionId, Stopwatch.GetTimestamp());
        }
        else
        {
            if (tracker.MadeAttempts < 8)
            {
                var currentTimestamp = Stopwatch.GetTimestamp();
                var newWaitSeconds = 15 * (int) Math.Pow(2, tracker.MadeAttempts);
                var waitTimestamp = currentTimestamp + newWaitSeconds * Stopwatch.Frequency;

                _logger.LogInformation("Tracker {tracker} failed to connect. An attempt will be made after {waitSeconds}s.", tracker.Url, newWaitSeconds);
                tracker.SetRetrying();
                _delayer.BeginMonitor(new ReconnectToTrackerTask(tracker), waitTimestamp);
            }
            else
            {
                tracker.SetFailed();
                _logger.LogInformation("Tracker {tracker} failed to connect. No retries will be attempted.", tracker.Url);
            }
        }
    }

    public async Task BeginConnectToAll(TorrentData torrent)
    {
        var connectTasks = torrent.Trackers.Select(BeginConnect);

        await Task.WhenAll(connectTasks);
    }

    public void Dispose()
    {
        foreach (var key in _udpClients.Keys)
        {
            _udpClients[key]
                .Dispose();
        }

        _udpClients.Clear();
    }
}