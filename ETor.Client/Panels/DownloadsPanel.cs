using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Client.UiValues;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class DownloadsPanel : IImGuiPanel
{
    private readonly Application _app;
    private readonly ILogger<DownloadsPanel> _logger;

    private readonly DownloadsTable _table;

    public DownloadsPanel(ILogger<DownloadsPanel> logger, Application app)
    {
        _logger = logger;
        _app = app;

        _table = new DownloadsTable(_app.Torrents);
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Torrents"))
        {
            _table.UpdateIfNeeded(_app.SelectedTorrentIndex);

            if (!_table.HasRows)
            {
                ImGui.Text("No torrents");
            }
            else
            {
                if (ImGui.BeginTable("##torrents-table", DownloadsTable.Columns, ImGuiTableFlags.RowBg))
                {
                    _table.DrawHeaders();

                    var selectedRow = _table.DrawData();

                    if (selectedRow != _app.SelectedTorrentIndex)
                    {
                        if (selectedRow != null)
                        {
                            _app.SetSelectedTorrent(selectedRow.Value);
                        }
                    }

                    ImGui.EndTable();
                }
            }
        }

        ImGui.End();
    }
}