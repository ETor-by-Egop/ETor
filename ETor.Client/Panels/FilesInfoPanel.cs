using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class FilesInfoPanel : IImGuiPanel
{
    private readonly Application _app;
    private readonly ITrackerManager _trackerManager;
    private readonly ILogger<DownloadsPanel> _logger;

    public FilesInfoPanel(ILogger<DownloadsPanel> logger, Application app, ITrackerManager trackerManager)
    {
        _logger = logger;
        _app = app;
        _trackerManager = trackerManager;
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Files##info-files"))
        {
            var torrent = _app.GetSelectedTorrent();
            if (torrent is not null)
            {
                foreach (var file in torrent.Files)
                {
                    ImGui.Text(file.Path);
                    ImGui.SameLine();
                    ImGui.Text(file.LengthBytes.FormatBytes());
                }
            }

            ImGui.End();
        }
    }
}