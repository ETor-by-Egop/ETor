using ETor.App.Services;
using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App;

public class Application
{
    private readonly IManifestLoader _manifestLoader;
    private readonly ITorrentRegistry _registry;
    private readonly ITrackerManager _trackerManager;

    public TorrentDownload? SelectedTorrent { get; private set; }

    private readonly ILogger<Application> _logger;

    public Application(IManifestLoader manifestLoader, ITrackerManager trackerManager, ILogger<Application> logger, ITorrentRegistry registry)
    {
        _manifestLoader = manifestLoader;
        _trackerManager = trackerManager;
        _logger = logger;
        _registry = registry;
    }

    public async Task AddDownload(string manifestPath)
    {
        var torrent = await _manifestLoader.Load(manifestPath);

        _registry.Add(torrent);

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
        _registry.SetSelectedTorrent(index);
        var torrent = _registry.GetTorrents()[index];
        _logger.LogInformation("Selected torrent {name}", torrent.Name);
        SelectedTorrent = torrent;
    }
}