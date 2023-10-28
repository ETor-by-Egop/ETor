using ETor.App.Data;
using ETor.App.Services;
using ETor.BEncoding;
using ETor.Manifest;
using ETor.Networking;
using ETor.Shared;
using Microsoft.Extensions.Logging;

namespace ETor.App;

public class Application
{
    private readonly ITrackerManager _trackerManager;
    private readonly IFileManager _fileManager;

    private readonly List<TorrentData> _torrents;

    public IReadOnlyList<TorrentData> Torrents => _torrents;

    public int? SelectedTorrentIndex { get; private set; }

    private readonly ILogger<Application> _logger;

    public Application(ITrackerManager trackerManager, ILogger<Application> logger, IFileManager fileManager)
    {
        _trackerManager = trackerManager;
        _logger = logger;
        _fileManager = fileManager;

        _torrents = new List<TorrentData>();
    }

    public async Task AddDownload(string manifestPath)
    {
        var content = await File.ReadAllBytesAsync(manifestPath);

        _logger.LogInformation("Read .torrent file {path} of size {size}", manifestPath, content.Length);

        var encodedContent = new BEncodeParser(content);

        var dict = encodedContent.ReadDictionary();

        var torrentManifest = new TorrentManifest(dict);

        var torrentData = new TorrentData(torrentManifest, manifestPath);
        _torrents.Add(torrentData);

        // var trackerUrls = _trackerManager.GetTrackerUrlsFromManifest(torrent.Manifest)
        //     .Distinct()
        //     .ToArray();
        //
        // foreach (var trackerUrl in trackerUrls)
        // {
        //     var tracker = new Tracker(trackerUrl);
        //
        //     _logger.LogInformation("Attempting to connect to {trackerUrl}", trackerUrl);
        //     await _trackerManager.Connect(tracker);
        //     _logger.LogInformation("Connection to {trackerUrl} finished", trackerUrl);
        // }
    }

    public void SetSelectedTorrent(int index)
    {
        SelectedTorrentIndex = index;
    }

    public TorrentData? GetSelectedTorrent()
    {
        return SelectedTorrentIndex is not null
            ? Torrents[SelectedTorrentIndex.Value]
            : null;
    }

    public async Task StartDownload(TorrentData torrent)
    {
        if (torrent.Files.Count == 0)
        {
            return;
        }

        await _fileManager.EnsureFileExistence(torrent);

        if (torrent.Files.Count > 1)
        {
            _logger.LogWarning("Failed to start download, because multi-file torrents aren't supported yet");
        }
        

        Memory<byte> buffer = new byte[torrent.PieceLength];
        Memory<byte> hashBuffer = new byte[64];

        var file = torrent.Files[0];
        var stream = _fileManager.GetStream(torrent, file);
        var pieces = torrent.Pieces;
        
        for (var i = 0; i < pieces.Count; i++)
        {
            var piece = pieces[i];
        
            var readBytes = await stream.ReadAsync(buffer);

            if (readBytes != torrent.PieceLength)
            {
                _logger.LogWarning("Failed to read {pieceLength} bytes from filestream, only read {actual}", torrent.PieceLength, readBytes);
            }

            buffer.Sha1(hashBuffer);

            if (hashBuffer.Span.SequenceEqual(piece.Hash.Span))
            {
                piece.Status = PieceStatus.Good;
                _logger.LogInformation("Piece {number} is ok at {position}", i, stream.Position);
            }
            else
            {
                piece.Status = PieceStatus.Bad;
                _logger.LogInformation("Piece {number} is bad at {position}", i, stream.Position);
            }
        }

        // find pieces, that are already downloaded (cache it?)
        // connect to trackers
        // announce
        // connect to peer
        // download piece
        // write data to a file
        // when piece is downloaded, begin next piece
    }
}