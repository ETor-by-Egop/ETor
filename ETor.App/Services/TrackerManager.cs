﻿using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using ETor.App.Data;
using ETor.App.DelayedTasks;
using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface ITrackerManager
{
    Task BeginConnect(TrackerData tracker);

    Task BeginConnectToAll(TorrentData torrent);
    Task BeginAnnounceToAll(TorrentData torrent);
    Task BeginAnnounce(TorrentData torrent, TrackerData tracker);
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


    public async Task BeginAnnounce(TorrentData torrent, TrackerData tracker)
    {
        if (!_udpClients.TryGetValue(tracker.InternalId, out var udpClient))
        {
            _logger.LogWarning("Tracker {tracker} can't be announced because it's not connected", tracker.Url);
            return;
        }

        while (!tracker.IsStillConnected() && tracker.Status != TrackerStatus.Failed)
        {
            _logger.LogInformation("Reconnecting tracker, while announcing!");
            await BeginConnect(tracker);
        }

        if (tracker.Status == TrackerStatus.Failed)
        {
            _logger.LogInformation("Reconnecting tracker aborted: tracker is in failed status!");
            return;
        }
        
        tracker.SetAnnouncing();
        
        var request = new UdpAnnounceRequest()
        {
            ConnectionId = tracker.ConnectionId,
            TransactionId = Random.Shared.Next(int.MaxValue),
            InfoHash = torrent.InfoHash,
            PeerId = "ETor Alpha 0.0.1 Egop"u8.ToArray(),
            Downloaded = 0,
            Left = torrent.Files[0].LengthBytes,
            Uploaded = 0,
            Event = UdpEvent.Started,
            IpAddress = 0,
            Key = Random.Shared.Next(int.MaxValue),
            NumWant = -1,
            Port = 6969
        };
        byte[] buffer = new byte[request.SerializedSize];
        request.Serialize(buffer);

        var tokenSource = new CancellationTokenSource();

        var announceResponse = await udpClient.SendReceive<UdpAnnounceResponse>(
            tracker.Host,
            tracker.Port,
            buffer,
            tokenSource.Token
        );

        if (announceResponse is not null)
        {
            _logger.LogInformation("Tracker {url} announced: {@response}", tracker.Url, announceResponse);
            tracker.SetAnnounced();
        }
        else
        {
            var currentTimestamp = Stopwatch.GetTimestamp();
            var newWaitSeconds = 15 * (int) Math.Pow(2, tracker.MadeAttempts);
            var waitTimestamp = currentTimestamp + newWaitSeconds * Stopwatch.Frequency;

            _logger.LogInformation("Tracker {tracker} failed to announce. An attempt will be made after {waitSeconds}s.", tracker.Url, newWaitSeconds);
            tracker.SetRetrying();
            _delayer.BeginMonitor(new ReannounceToTrackerTask(torrent, tracker), waitTimestamp);

            tracker.SetRetrying();
        }
    }

    public async Task BeginAnnounceToAll(TorrentData torrent)
    {
        var announceTasks = torrent.Trackers.Select(x => BeginAnnounce(torrent, x));

        await Task.WhenAll(announceTasks);
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