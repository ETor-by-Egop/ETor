using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Client.UiValues;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class FilesInfoPanel : IImGuiPanel
{
    private readonly Application _app;
    private readonly ITrackerManager _trackerManager;
    private readonly FilesTable _table;
    private readonly ILogger<DownloadsPanel> _logger;

    public FilesInfoPanel(ILogger<DownloadsPanel> logger, Application app, ITrackerManager trackerManager)
    {
        _logger = logger;
        _app = app;
        _trackerManager = trackerManager;
        _table = new FilesTable();
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Files##info-files"))
        {
            var torrent = _app.GetSelectedTorrent();
            if (torrent is not null)
            {
                _table.UpdateIfNeeded(torrent.PieceLength, torrent.Files, null);
                
                if (!_table.HasRows)
                {
                    ImGui.Text("No torrents");
                }
                else
                {
                    if (ImGui.BeginTable("##files-table", DownloadsTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
                    {
                        _table.DrawHeaders();

                        var selectedFile = _table.DrawData();

                        if (selectedFile is not null)
                        {
                            _logger.LogInformation("User selected file {index}", selectedFile);
                        }

                        ImGui.EndTable();
                    }
                }
            }

            ImGui.End();
        }
    }
}