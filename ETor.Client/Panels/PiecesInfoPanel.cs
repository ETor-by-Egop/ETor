﻿using System.Numerics;
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
    private readonly ITransferManager _transferManager;
    private readonly PiecesTable _table;
    private readonly ILogger<PiecesInfoPanel> _logger;

    public PiecesInfoPanel(ILogger<PiecesInfoPanel> logger, Application app, ITransferManager transferManager)
    {
        _logger = logger;
        _app = app;
        _transferManager = transferManager;
        _table = new PiecesTable();
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("Pieces##info-files"))
        {
            var torrent = _app.GetSelectedTorrent();

            if (ImGui.BeginTable("##pieces-table", PiecesTable.Columns, ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoBordersInBodyUntilResize))
            {
                if (torrent is not null)
                {
                    _table.UpdateIfNeeded(torrent.Pieces, null);
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
                        _logger.LogInformation("User selected piece {index}", selectedFile);
                    }

                    ImGui.EndTable();
                }
            }

            ImGui.End();
        }
    }
}