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
    private readonly Application _application;
    private readonly ITrackerManager _trackerManager;
    private readonly ILogger<DownloadsPanel> _logger;

    public FilesInfoPanel(ILogger<DownloadsPanel> logger, Application application, ITrackerManager trackerManager)
    {
        _logger = logger;
        _application = application;
        _trackerManager = trackerManager;
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Files##info-files"))
        {
            if (_application.SelectedTorrent is not null)
            {
                foreach (var file in _application.SelectedTorrent.Manifest.Info.Files)
                {
                    ImGui.Text(file.ComputeFilePath());
                }
            }

            ImGui.End();
        }
    }
}