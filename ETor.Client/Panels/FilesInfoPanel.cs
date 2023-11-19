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
    private readonly ITransferManager _transferManager;
    private readonly FilesTable _table;
    private readonly ILogger<DownloadsPanel> _logger;

    public FilesInfoPanel(ILogger<DownloadsPanel> logger, Application app, ITransferManager transferManager)
    {
        _logger = logger;
        _app = app;
        _transferManager = transferManager;
        _table = new FilesTable();
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Files##info-files"))
        {
            var torrent = _app.GetSelectedTorrent();

            if (ImGui.BeginTable("##files-table", FilesTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
            {
                if (torrent is not null)
                {
                    _table.UpdateIfNeeded(torrent.PieceLength, torrent.Files, null);
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
                        _logger.LogInformation("User selected file {index}", selectedFile);
                    }

                    ImGui.EndTable();
                }
            }

            ImGui.End();
        }
    }
}