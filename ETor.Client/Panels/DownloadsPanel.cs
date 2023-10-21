using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class DownloadsPanel : IImGuiPanel
{
    private readonly ITorrentRegistry _registry;
    private readonly Application _application;
    private readonly ILogger<DownloadsPanel> _logger;

    public DownloadsPanel(ILogger<DownloadsPanel> logger, ITorrentRegistry registry, Application application)
    {
        _logger = logger;
        _registry = registry;
        _application = application;
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Torrents"))
        {
            var torrents = _registry.GetTorrents();

            if (torrents.Count == 0)
            {
                ImGui.Text("No torrents");
            }
            else
            {
                const int columns = 6;

                if (ImGui.BeginTable("##torrents-table", columns, ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("#", ImGuiTableColumnFlags.None, 0.1f);
                    ImGui.TableSetupColumn("Name");
                    ImGui.TableSetupColumn("Size");
                    ImGui.TableSetupColumn("Status");
                    ImGui.TableSetupColumn("Download");
                    ImGui.TableSetupColumn("Upload");
                    
                    ImGui.TableHeadersRow();

                    for (var index = 0; index < torrents.Count; index++)
                    {
                        var torrent = torrents[index];
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();

                        // first column is specific, we need to add a full-span selectable, to allow selection of a row
                        var selected = false;
                        if (ImGui.Selectable(index.ToString(), ref selected, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                        {
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, 0xFF0000FF);
                            }
                        }

                        if (selected)
                        {
                            _application.SetSelectedTorrent(torrent);
                        }

                        ImGui.TableNextColumn();
                        ImGui.Text(torrent.Name);
                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                    }

                    ImGui.EndTable();
                }
            }
        }

        ImGui.End();
    }
}