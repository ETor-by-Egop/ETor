using System.Numerics;
using ETor.App;
using ETor.App.Services;
using ETor.Client.Abstractions;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Panels;

public class GeneralInfoPanel : IImGuiPanel
{
    private readonly Application _application;

    private TorrentDownload? _lastDisplayedTorrent;
    private string _torrentPath;
    private string _length;
    private string _name;
    private string _pieceLength;
    private string _pieces;
    private string _fileCount;

    public GeneralInfoPanel(Application application)
    {
        _application = application;
    }

    public void OnImGuiRender()
    {
        if (ImGui.Begin("General##info-general"))
        {
            if (_application.SelectedTorrent is not null)
            {
                if (_lastDisplayedTorrent != _application.SelectedTorrent)
                {
                    _torrentPath = "File: " + _application.SelectedTorrent.FilePath;
                    _length = "Length: " + (_application.SelectedTorrent.Manifest.Info?.Length?.FormatBytes() ?? "0 B");
                    _name = "Name: " + _application.SelectedTorrent.Manifest.Info?.Name;
                    _pieceLength = "Piece Length: " + (_application.SelectedTorrent.Manifest.Info?.PieceLength?.FormatBytes() ?? "0 B");
                    _pieces = "Pieces Count: " + (_application.SelectedTorrent.Manifest.Info?.Pieces?.Length / 20 ?? 0);
                    _fileCount = "File Count: " + (_application.SelectedTorrent.Manifest.Info?.Files?.Count ?? 0);
                }

                ImGui.Text(_torrentPath);
                ImGui.Text(_length);
                ImGui.Text(_name);
                ImGui.Text(_pieceLength);
                ImGui.Text(_pieces);
                ImGui.Text(_fileCount);
            }
            else
            {
                _lastDisplayedTorrent = null;
            }

            ImGui.End();
        }
    }
}