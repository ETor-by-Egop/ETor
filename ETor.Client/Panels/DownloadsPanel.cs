using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Client.UiValues;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class DownloadsPanel : IImGuiPanel
{
    private readonly ITorrentRegistry _registry;
    private readonly Application _application;
    private readonly ILogger<DownloadsPanel> _logger;

    private readonly DownloadsTable _table;

    public DownloadsPanel(ILogger<DownloadsPanel> logger, ITorrentRegistry registry, Application application)
    {
        _logger = logger;
        _registry = registry;
        _application = application;

        _table = new DownloadsTable(_registry.GetTorrents());
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Torrents"))
        {
            _table.UpdateIfNeeded();

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

                    if (selectedRow != _registry.GetTorrents().SelectedIndex)
                    {
                        if (selectedRow != -1)
                        {
                            _application.SetSelectedTorrent(selectedRow);
                        }
                    }

                    ImGui.EndTable();
                }
            }
        }

        ImGui.End();
    }
}