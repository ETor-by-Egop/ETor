using System.Globalization;
using System.Net;
using ETor.App;
using ETor.App.Data;
using ETor.App.Trackers;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class PeersTableRow : ComputedTableRow<PeerData>
{
    private readonly IAutoComputedValueOf<PeerData>[] _columns;

    private bool _isActive;
    private Peer? _peer;

    public PeersTableRow(int index)
    {
        _columns = new IAutoComputedValueOf<PeerData>[]
        {
            new IndexColumnOf<PeerData>(index),
            AutoComputedValue<PeerData>.Of(x => x?.Ip, x => x?.FormatIp() ?? "unknown"),
            AutoComputedValue<PeerData>.Of(x => x?.Port, x => x?.ToString() ?? "unknown")
        };
    }

    public void SetActive(bool active)
    {
        _isActive = active;
    }

    public bool ImguiDraw()
    {
        if (IsDirty)
        {
            Recalculate();
            IsDirty = false;
        }

        ImGui.TableNextColumn();

        if (_isActive)
        {
            ImGui.PushStyleColor(ImGuiCol.Header, 0xFFAAAAAA);
        }

        // first column is specific, we need to add a full-span selectable, to allow selection of a row
        // pass in current activeness and save the result, which tells us, if a row was selected now
        var selectedNow = ImGui.Selectable(
            _columns[0]
                .Get(),
            _isActive,
            ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick
        );

        if (_isActive)
        {
            ImGui.PopStyleColor();
        }

        for (var i = 1; i < _columns.Length; i++)
        {
            ImGui.TableNextColumn();
            ImGui.Text(
                _columns[i]
                    .Get()
            );
        }

        return selectedNow;
    }

    private void Recalculate()
    {
        foreach (var column in _columns)
        {
            column.Fetch(Value);
        }
    }

    public void UpdateIfNeeded(PeerData peerData, Peer? peer)
    {
        base.UpdateIfNeeded(peerData);
        _peer = peer;
    }
}