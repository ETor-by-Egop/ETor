using ETor.App.Services;
using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App;

public class Application
{
    private readonly IManifestLoader _manifestLoader;
    private readonly ITrackerManager _trackerManager;

    private readonly ILogger<Application> _logger;
    
    
    private readonly TaskCompletionSource _taskCompletionSource = new();

    public Application(IManifestLoader manifestLoader, ITrackerManager trackerManager, ILogger<Application> logger)
    {
        _manifestLoader = manifestLoader;
        _trackerManager = trackerManager;
        _logger = logger;
    }

    public async Task AddDownload(string manifestPath)
    {
        var torrentManifest = await _manifestLoader.Load(manifestPath);

        var trackerUrls = _trackerManager.GetTrackerUrlsFromManifest(torrentManifest)
            .Distinct()
            .ToArray();
        
        foreach (var trackerUrl in trackerUrls)
        {
            var tracker = new Tracker(trackerUrl);

            _logger.LogInformation("Attempting to connect to {trackerUrl}", trackerUrl);
            await _trackerManager.Connect(tracker);
            _logger.LogInformation("Connection to {trackerUrl} finished", trackerUrl);
        }
    }

    public async Task Initialize()
    {
        await Task.Yield();
        _logger.LogInformation("Application started");
    }

    public Task WaitForExit()
    {
        return _taskCompletionSource.Task;
    }
}