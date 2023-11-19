using System.Diagnostics;
using System.Net.Sockets;
using ETor.App.Data;
using ETor.App.Services;
using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App.Trackers;

public class UdpTracker : Tracker
{
    private readonly ILogger<Tracker> _logger;
    private long _connectionId;
    private int _transactionId;

    public UdpTracker(TrackerData trackerData, Memory<byte> peerId, Memory<byte> torrentInfoHash, short listeningPort, ILogger<Tracker> logger) : base(
        trackerData,
        peerId,
        torrentInfoHash,
        listeningPort,
        logger
    )
    {
        _logger = logger;
        _transactionId = Random.Shared.Next();
    }

    protected override async Task OnStart()
    {
        var request = new UdpConnectRequest(_transactionId);
        byte[] buffer = new byte[request.SerializedSize];
        request.Serialize(buffer);

        using var udpClient = new UdpClient();
        udpClient.Client.SendTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
        udpClient.Client.ReceiveTimeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;
        
        var message = await udpClient.SendReceive<UdpConnectResponse>(
            TrackerData.Uri.Host,
            TrackerData.Uri.Port,
            buffer.AsMemory()
        );

        if (message is not null)
        {
            if (message.TransactionId == this._transactionId)
            {
                this._connectionId = message.ConnectionId;
            }
            else
            {
                // connect failed -> drop tracker
                _logger.LogWarning("connecting to tracker {uri} failed", this.TrackerData.Uri);
            }
        }
        else
        {
            _logger.LogWarning("Didn't receive anything while connecting from {uri}", this.TrackerData.Uri);
        }
    }

    protected override async Task OnAnnounce()
    {
        OnAnnouncing(TrackerData);

        var reqMessage = new UdpAnnounceRequest()
        {
            ConnectionId = _connectionId,
            TransactionId = _transactionId,
            InfoHash = TorrentInfoHash,
            PeerId = PeerId,
            Downloaded = Downloaded,
            Left = Left,
            Uploaded = Uploaded,
            Event = UdpEvent.Started,
            IpAddress = 0,
            Key = Random.Shared.Next(int.MaxValue),
            NumWant = WantedPeerCount,
            Port = ListeningPort
        };
        byte[] buffer = new byte[reqMessage.SerializedSize];
        reqMessage.Serialize(buffer);

        _logger.LogWarning("Sending announce to {tracker}", TrackerData.Uri);

        using var udpClient = new UdpClient();
        udpClient.Client.SendTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
        udpClient.Client.ReceiveTimeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;

        var message = await udpClient.SendReceive<UdpAnnounceResponse>(
            TrackerData.Uri.Host,
            TrackerData.Uri.Port,
            buffer.AsMemory()
        );
        
        if (message is not null)
        {
            OnAnnounced(
                TrackerData,
                new AnnouncedEventArgs(
                    message
                        .Interval,
                    message
                        .Leechers,
                    message
                        .Seeders,
                    message
                        .IpPortPairs
                )
            );
        }
        else
        {
            _logger.LogWarning("Didn't receive anything while announcing {uri}", this.TrackerData.Uri);
            OnAnnounceFailed(TrackerData);
        }
    }
}