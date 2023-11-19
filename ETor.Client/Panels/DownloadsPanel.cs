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
            if (!_app.SelectedTorrentIndex.HasValue)
            {
                ImGui.BeginDisabled();
            }

            if (ImGui.Button("Start##create-files"))
            {
                Task.Run(() => _app.StartDownload(_app.Torrents[_app.SelectedTorrentIndex!.Value]))
                    .ContinueWith(
                        faultedTask => {
                            _logger.LogError(faultedTask.Exception, "Start failed with exception"); 
                        }, 
                        TaskContinuationOptions.OnlyOnFaulted);
            }

            if (!_app.SelectedTorrentIndex.HasValue)
            {
                ImGui.EndDisabled();
            }

            if (ImGui.BeginTable("##torrents-table", DownloadsTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
            {
                _table.UpdateIfNeeded(_app.SelectedTorrentIndex);

                _table.DrawHeaders();

                if (!_table.HasRows)
                {
                    ImGui.EndTable();
                    ImGui.Text("No torrents");
                }
                else
                {
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