﻿using ETor.App;
using ETor.App.Data;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class PiecesTableRow : ComputedTableRow<PieceData>
{
    private readonly IAutoComputedValueOf<PieceData>[] _columns;

    private bool _isActive;

    public PiecesTableRow(int index)
    {
        _columns = new IAutoComputedValueOf<PieceData>[]
        {
            new IndexColumnOf<PieceData>(index),
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
}