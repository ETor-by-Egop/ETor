using System.Numerics;
using ETor.App;
using ETor.App.Data;
using ETor.App.Trackers;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class PeersTable
{
    public const int Columns = 3;

    private int? _lastSelectedIndex = null;

    private IReadOnlyList<PeerData> _source = ArraySegment<PeerData>.Empty;
    private List<PeersTableRow> _rows = new();
    public bool HasRows => _rows.Count > 0;

    // ReSharper disable once RedundantExplicitArraySize
    private static readonly string[] Headers = new string[Columns]
    {
        "#",
        "IP",
        "Port"
    };

    private TorrentTransfer? _transfer;
    private IReadOnlyList<PeerData> _peers;

    public void DrawHeaders()
    {
        var padding = ImGui.GetStyle()
            .CellPadding;
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, padding with {X = 20});
        ImGui.TableSetupColumn(Headers[0], ImGuiTableColumnFlags.WidthFixed, 15f);

        for (var index = 1; index < Headers.Length; index++)
        {
            var column = Headers[index];
            ImGui.TableSetupColumn(column);
        }

        ImGui.TableHeadersRow();
        ImGui.PopStyleVar();
    }

    public void UpdateIfNeeded(IReadOnlyList<PeerData> source, TorrentTransfer? transfer, int? selectedIndex)
    {
        _transfer = transfer;
        _source = source;

        if (_transfer is null)
        {
            return;
        }
        
        while (_transfer.Peers.Count > _rows.Count)
        {
            _rows.Add(new PeersTableRow(_rows.Count));
        }

        while (_rows.Count > _transfer.Peers.Count)
        {
            _rows.RemoveAt(_rows.Count - 1);
        }

        for (var i = 0; i < _source.Count; i++)
        {
            _transfer.Peers.TryGetValue(
                _source[i]
                    .Address,
                out var peer
            );
            _rows[i]
                .UpdateIfNeeded(_source[i], peer);
        }

        if (_lastSelectedIndex != selectedIndex)
        {
            if (_lastSelectedIndex != null)
            {
                _rows[_lastSelectedIndex.Value]
                    .SetActive(false);
            }

            if (selectedIndex != null)
            {
                _rows[selectedIndex.Value]
                    .SetActive(true);
            }

            _lastSelectedIndex = selectedIndex;
        }
    }

    public int? DrawData()
    {
        int? selectedIndex = null;
        for (var index = 0; index < _rows.Count; index++)
        {
            var row = _rows[index];
            if (row.ImguiDraw())
            {
                selectedIndex = index;
            }
        }

        return selectedIndex;
    }
}