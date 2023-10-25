using ETor.App.Data;
using ETor.App.Services;
using ETor.BEncoding;
using ETor.Manifest;
using ETor.Networking;
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

    public async Task CreateFiles(TorrentData torrentData)
    {
        await _fileManager.CreateFiles(torrentData);
    }
}