using System.Buffers.Binary;
using System.Collections;
using ETor.App.Data;
using ETor.App.Trackers;
using Microsoft.Extensions.Logging;

namespace ETor.App;

/// <summary>
/// Represents active torrent downloading, encapsulates trackers and peers, related to an instance of a torrent
/// </summary>
public class TorrentTransfer
{
    private readonly TorrentData _torrent;
    private readonly ILogger<TorrentTransfer> _logger;

    public Memory<byte> PeerId { get; } = "ETor Alpha 0.0.1 Egop"u8.ToArray();

    /// <summary>
    /// The trackers.
    /// </summary>
    public IDictionary<Guid, Tracker> Trackers { get; } = new Dictionary<Guid, Tracker>();

    public IDictionary<long, Peer> Peers { get; } = new Dictionary<long, Peer>();

    public long Downloaded { get; set; }

    public DateTime StartTime { get; private set; }

    public TorrentTransfer(TorrentData torrent, ILoggerFactory loggerFactory)
    {
        _torrent = torrent;
        _logger = loggerFactory.CreateLogger<TorrentTransfer>();

        foreach (var trackerData in torrent.Trackers)
        {
            Tracker? tracker = null;
            if (trackerData.Uri.Scheme is "http" or "https")
            {
                // tracker = new HttpTracker(trackerUri, this.PeerId, torrentInfo.InfoHash, listeningPort);
            }
            else if (trackerData.Uri.Scheme == "udp")
            {
                tracker = new UdpTracker(trackerData, PeerId, torrent.InfoHash, 6969, loggerFactory.CreateLogger<Tracker>());
            }

            if (tracker != null)
            {
                // tracker.TrackingEvent = TrackingEvent.Started;
                tracker.Announcing += this.Tracker_Announcing;
                tracker.Announced += this.Tracker_Announced;
                tracker.AnnounceFailed += this.Tracker_AnnounceFailed;
                // tracker.TrackingFailed += this.Tracker_TrackingFailed;
                tracker.Left = torrent.TotalLength - this.Downloaded;
                tracker.WantedPeerCount = 30; // we can handle 30 peers at a time

                Trackers[trackerData.InternalId] = tracker;
            }
            else
            {
                // unsupported tracker protocol
                _logger.LogInformation("unsupported tracker protocol {scheme}", trackerData.Uri.Scheme);
            }
        }
    }

    public void Start()
    {
        StartTime = DateTime.UtcNow;

        _logger.LogInformation("starting torrent manager for torrent {name}", _torrent.Name);

        // this.OnTorrentHashing(this, EventArgs.Empty);

        // initialize piece manager
        // this.pieceManager = new PieceManager(this.TorrentInfo.InfoHash, this.TorrentInfo.Length, this.TorrentInfo.PieceHashes, this.TorrentInfo.PieceLength, this.TorrentInfo.BlockLength, this.persistenceManager.Verify());
        // this.pieceManager.PieceCompleted += this.PieceManager_PieceCompleted;
        // this.pieceManager.PieceRequested += this.PieceManager_PieceRequested;

        // start tracking
        lock (((IDictionary)Trackers).SyncRoot)
        {
            foreach (var tracker in Trackers.Values)
            {
                tracker.StartTracking();
            }
        }

        // this.OnTorrentStarted(this, EventArgs.Empty);

        // if (this.pieceManager.IsComplete)
        // {
            // this.OnTorrentSeeding(this, EventArgs.Empty);
        // }
    }

    private void Tracker_Announced(TrackerData tracker, AnnouncedEventArgs e)
    {
        _logger.LogInformation("Tracker announced {uri}, {@data}", tracker.Uri, e);
        tracker.SetAnnounced();
        tracker.SetUpdateInterval(e.Interval);
        
        // 6 = 4 bytes ip + 2 bytes port
        for (int i = 0; i < e.IpPortPairs.Length; i += 6)
        {
            var ip = BinaryPrimitives.ReadInt32BigEndian(
                e.IpPortPairs.Slice(i, 4)
                    .Span
            );
            var port = BinaryPrimitives.ReadInt16BigEndian(
                e.IpPortPairs.Slice(i + 4, 2)
                    .Span
            );

            var id = (long)ip << 16 | (ushort) port;
            if (Peers.ContainsKey(id))
            {
                _logger.LogInformation("Peer {ip}:{port} is already known", ip, port);
            }
            else
            {
                var peerData = new PeerData(
                    e.IpPortPairs.Slice(i, 6)
                );
                var peer = new Peer(peerData);
                Peers[peerData.Address] = peer;
                _torrent.AddPeer(peerData);
            }
        }
    }

    private void Tracker_Announcing(TrackerData tracker)
    {
        _logger.LogInformation("Tracker announcing {uri}", tracker.Uri);
        tracker.SetAnnouncing();
    }

    private void Tracker_AnnounceFailed(TrackerData tracker)
    {
        _logger.LogInformation("Tracker announce failed {uri}", tracker.Uri);
        tracker.SetFailed();
    }
}