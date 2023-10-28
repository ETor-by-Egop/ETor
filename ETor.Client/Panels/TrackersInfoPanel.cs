using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Client.UiValues;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class TrackersInfoPanel : IImGuiPanel
{
    private readonly Application _app;
    private readonly ITrackerManager _trackerManager;
    private readonly TrackersTable _table;
    private readonly ILogger<DownloadsPanel> _logger;

    public TrackersInfoPanel(ILogger<DownloadsPanel> logger, Application app, ITrackerManager trackerManager)
    {
        _logger = logger;
        _app = app;
        _trackerManager = trackerManager;
        _table = new TrackersTable();
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Trackers##info-trackers"))
        {
            var torrent = _app.GetSelectedTorrent();

            if (ImGui.BeginTable("##trackers-table", TrackersTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
            {
                if (torrent is not null)
                {
                    _table.UpdateIfNeeded(torrent.Trackers, null);
                }

                _table.DrawHeaders();

                if (!_table.HasRows)
                {
                    ImGui.EndTable();
                    ImGui.Text("No torrent");
                }
                else
                {
                    var selectedFile = _table.DrawData();

                    if (selectedFile is not null)
                    {
                        _logger.LogInformation("User selected tracker {index}", selectedFile);
                    }

                    ImGui.EndTable();
                }
            }

            ImGui.End();
        }
    }
}