using System.Diagnostics;
using System.Net.Sockets;
using ETor.App.Data;
using ETor.App.Services;
using ETor.Networking;
using Microsoft.Extensions.Logging;
using ThreadState = System.Threading.ThreadState;

namespace ETor.App;

public class TrackerMonitoringThread
{
    private readonly ILogger<TrackerMonitoringThread> _logger;
    private readonly TrackerData _tracker;
    private readonly TorrentData _torrent;
    private readonly UdpClient _udpClient;
    private Thread? _thread;

    public long LastConnectedAt { get; private set; } = -1;
    public long LastAnnouncedAt { get; private set; } = -1;

    public int LastConnectAttempts { get; private set; } = 0;
    public int LastAnnounceAttempts { get; private set; } = 0;

    private UdpAnnounceResponse? _lastAnnounceResponse;

    public TrackerMonitoringThread(TrackerData tracker, TorrentData torrent, ILogger<TrackerMonitoringThread> logger)
    {
        _tracker = tracker;
        _torrent = torrent;
        _logger = logger;
        _udpClient = new UdpClient();
    }

    public void Start()
    {
        if (_thread is not null)
        {
            throw new InvalidOperationException("This announce thread is already started");
        }

        _thread = new Thread(Routine);
        _thread.Start();
    }

    private async void Routine()
    {
        if (_tracker.Protocol is not TrackerProtocol.Udp)
        {
            _logger.LogWarning("Non udp tracker is not supported. {tracker}", _tracker.Url);
            _tracker.SetUnsupported();
            return;
        }

        while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
        {
            if (Stopwatch.GetTimestamp() - LastConnectedAt > 2 * 60 * Stopwatch.Frequency && LastConnectAttempts < 8)
            {
                // connected too long ago
                _logger.LogInformation("Connecting tracker {tracker}", _tracker.Url);
                _tracker.SetConnecting();

                // If a response is not received after 15 * 2 ^ n seconds, the client should retransmit the request,
                // where n starts at 0 and is increased up to 8 (3840 seconds) after every retransmission.
                var waitSeconds = 15 * (int) Math.Pow(2, LastConnectAttempts);

                using var source = new CancellationTokenSource(waitSeconds * 1000);

                var request = new UdpConnectRequest(Random.Shared.Next());
                byte[] buffer = new byte[request.SerializedSize];
                request.Serialize(buffer);

                var connectResponse = await _udpClient.SendReceive<UdpConnectResponse>(
                    _tracker.Host,
                    _tracker.Port,
                    buffer.AsMemory(),
                    source.Token
                );

                if (connectResponse is not null)
                {
                    _tracker.SetConnected(connectResponse.ConnectionId);
                    LastConnectAttempts = 0;
                    LastConnectedAt = Stopwatch.GetTimestamp();
                }
                else
                {
                    LastConnectAttempts++;
                    if (LastConnectAttempts < 8)
                    {
                        _logger.LogInformation("Tracker {tracker} failed to connect.", _tracker.Url);
                        _tracker.SetRetrying();
                        Thread.Sleep(waitSeconds * 1000);
                        continue;
                    }
                    else
                    {
                        _tracker.SetFailed();
                        _logger.LogInformation("Tracker {tracker} failed to connect. No retries will be attempted.", _tracker.Url);
                        break;
                    }
                }
            }

            if (_tracker.Status != TrackerStatus.Connected)
            {
                _logger.LogInformation("Announcing tracker aborted: tracker is not connected!");
                continue;
            }

            if (_lastAnnounceResponse is null ||
                LastAnnouncedAt + _lastAnnounceResponse.Interval * Stopwatch.Frequency >= Stopwatch.GetTimestamp() &&
                LastAnnounceAttempts < 8)
            {
                // if we weren't announced, or we announced too long ago
                _logger.LogInformation("Announcing tracker {tracker}", _tracker.Url);

                _tracker.SetAnnouncing();

                var request = new UdpAnnounceRequest()
                {
                    ConnectionId = _tracker.ConnectionId,
                    TransactionId = Random.Shared.Next(int.MaxValue),
                    InfoHash = _torrent.InfoHash,
                    PeerId = "ETor Alpha 0.0.1 Egop"u8.ToArray(),
                    Downloaded = 0,
                    Left = _torrent.Files[0]
                        .LengthBytes,
                    Uploaded = 0,
                    Event = UdpEvent.Started,
                    IpAddress = 0,
                    Key = Random.Shared.Next(int.MaxValue),
                    NumWant = -1,
                    Port = 6969
                };
                byte[] buffer = new byte[request.SerializedSize];
                request.Serialize(buffer);

                var waitSeconds = 15 * (int) Math.Pow(2, LastConnectAttempts);
                var tokenSource = new CancellationTokenSource(waitSeconds * 1000);
                var announceResponse = await _udpClient.SendReceive<UdpAnnounceResponse>(
                    _tracker.Host,
                    _tracker.Port,
                    buffer,
                    tokenSource.Token
                );

                if (announceResponse is not null)
                {
                    _logger.LogInformation("Tracker {url} announced: {@response}", _tracker.Url, announceResponse);
                    _lastAnnounceResponse = announceResponse;
                    _tracker.SetAnnounced();
                    LastAnnounceAttempts = 0;
                    LastAnnouncedAt = Stopwatch.GetTimestamp();
                }
                else
                {
                    LastAnnounceAttempts++;
                    if (LastConnectAttempts < 8)
                    {
                        _logger.LogInformation("Tracker {tracker} failed to announce", _tracker.Url);
                        _tracker.SetRetrying();
                        Thread.Sleep(waitSeconds * 1000);
                    }
                    else
                    {
                        _tracker.SetFailed();
                        _logger.LogInformation("Tracker {tracker} failed to announce. No retries will be attempted.", _tracker.Url);
                        break;
                    }
                }
            }
        }
    }
}