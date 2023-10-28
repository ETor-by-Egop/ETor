using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Client.UiValues;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class PiecesInfoPanel : IImGuiPanel
{
    private readonly Application _app;
    private readonly ITrackerManager _trackerManager;
    private readonly PiecesTable _table;
    private readonly ILogger<DownloadsPanel> _logger;

    public PiecesInfoPanel(ILogger<DownloadsPanel> logger, Application app, ITrackerManager trackerManager)
    {
        _logger = logger;
        _app = app;
        _trackerManager = trackerManager;
        _table = new PiecesTable();
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Pieces##info-files"))
        {
            var torrent = _app.GetSelectedTorrent();
            if (torrent is not null)
            {
                _table.UpdateIfNeeded(torrent.Pieces, null);
                
                if (!_table.HasRows)
                {
                    ImGui.Text("No torrents");
                }
                else
                {
                    if (ImGui.BeginTable("##pieces-table", PiecesTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
                    {
                        _table.DrawHeaders();

                        var selectedFile = _table.DrawData();

                        if (selectedFile is not null)
                        {
                            _logger.LogInformation("User selected piece {index}", selectedFile);
                        }

                        ImGui.EndTable();
                    }
                }
            }

            ImGui.End();
        }
    }
}