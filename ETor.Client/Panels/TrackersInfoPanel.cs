using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class TrackersInfoPanel : IImGuiPanel
{
    private readonly Application _application;
    private readonly ITrackerManager _trackerManager;
    private readonly ILogger<DownloadsPanel> _logger;

    public TrackersInfoPanel(ILogger<DownloadsPanel> logger, Application application, ITrackerManager trackerManager)
    {
        _logger = logger;
        _application = application;
        _trackerManager = trackerManager;
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Trackers##info-trackers"))
        {
            var selectedTorrent = _application.GetSelectedTorrent();
            if (selectedTorrent is not null)
            {
                foreach (var tracker in selectedTorrent.Trackers)
                {
                    ImGui.Text(tracker.Url);
                }
            }

            ImGui.End();
        }
    }
}