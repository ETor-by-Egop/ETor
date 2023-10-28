using ETor.App;
using ETor.App.Data;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class PiecesTableRow : ComputedTableRow<PieceData>
{
    private int _index;

    private IAutoComputedValueOf<PieceData>[] _columns;

    private bool _isActive;
    private long _pieceLength;

    public PiecesTableRow(int index, long pieceLength)
    {
        _index = index;
        _pieceLength = pieceLength;
        _columns = new IAutoComputedValueOf<PieceData>[]
        {
            new IndexColumnOf<PieceData>(_index),
            AutoComputedValue<PieceData>.Of(x => x.Status, x => x.ToString("G"))
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

    public void UpdateIfNeeded(PieceData? value, long pieceLength)
    {
        base.UpdateIfNeeded(value);
        _pieceLength = pieceLength;
    }
}