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
    private readonly IPieceManager _pieceManager;

    private readonly List<TorrentData> _torrents;

    public IReadOnlyList<TorrentData> Torrents => _torrents;

    public int? SelectedTorrentIndex { get; private set; }

    private readonly ILogger<Application> _logger;

    public Application(ITrackerManager trackerManager, ILogger<Application> logger, IFileManager fileManager, IPieceManager pieceManager)
    {
        _trackerManager = trackerManager;
        _logger = logger;
        _fileManager = fileManager;
        _pieceManager = pieceManager;

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

        // ensure that all files exist
        await _fileManager.EnsureFileExistence(torrent);

        // find pieces, that are already downloaded (cache it?)
        var checkPiecesTask = _pieceManager.CheckPieces(torrent);

        // connect to trackers
        var connectToTrackersTask = _trackerManager.BeginConnectToAll(torrent);

        await Task.WhenAll(checkPiecesTask, connectToTrackersTask);

        // announce
        // connect to peer
        // download piece
        // write data to a file
        // when piece is downloaded, begin next piece
    }
}