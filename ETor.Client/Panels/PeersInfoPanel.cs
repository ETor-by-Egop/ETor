using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Client.UiValues;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class PeersInfoPanel : IImGuiPanel
{
    private readonly Application _app;
    private readonly ITransferManager _transferManager;
    private readonly PeersTable _table;
    private readonly ILogger<PeersInfoPanel> _logger;

    public PeersInfoPanel(ILogger<PeersInfoPanel> logger, Application app, ITransferManager transferManager)
    {
        _logger = logger;
        _app = app;
        _transferManager = transferManager;
        _table = new PeersTable();
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Peers##info-peers"))
        {
            var torrent = _app.GetSelectedTorrent();

            if (ImGui.BeginTable("##peers-table", PeersTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
            {
                if (torrent is not null)
                {
                    _transferManager.Transfers.TryGetValue(torrent.InternalId, out var transfer);
                    _table.UpdateIfNeeded(torrent.Peers, transfer, null);
                }

                _table.DrawHeaders();

                if (!_table.HasRows)
                {
                    ImGui.EndTable();
                    ImGui.Text("No torrent");
                }
                else
                {
                    var selectedPeer = _table.DrawData();

                    if (selectedPeer is not null)
                    {
                        _logger.LogInformation("User selected peer {index}", selectedPeer);
                    }

                    ImGui.EndTable();
                }
            }

            ImGui.End();
        }
    }
}