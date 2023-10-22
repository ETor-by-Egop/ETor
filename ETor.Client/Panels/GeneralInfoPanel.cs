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

    private int? _lastDisplayedTorrentIndex;
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
            var torrent = _application.GetSelectedTorrent();
            if (torrent is not null)
            {
                if (_lastDisplayedTorrentIndex != _application.SelectedTorrentIndex)
                {
                    _torrentPath = "File: " + torrent.FilePath;
                    _length = "Length: " + torrent.TotalLength.FormatBytes();
                    _name = "Name: " + torrent.Name;
                    _pieceLength = "Piece Length: " + torrent.PieceLength.FormatBytes();
                    _pieces = "Pieces Count: " + torrent.Pieces.Count;
                    _fileCount = "File Count: " + torrent.Files.Count;
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
                _lastDisplayedTorrentIndex = null;
            }

            ImGui.End();
        }
    }
}