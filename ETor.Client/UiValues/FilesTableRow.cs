using ETor.App;
using ETor.App.Data;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class FilesTableRow : ComputedTableRow<FileData>
{
    private readonly IAutoComputedValueOf<FileData>[] _columns;

    private bool _isActive;
    private long _pieceLength;

    public FilesTableRow(int index, long pieceLength)
    {
        _pieceLength = pieceLength;
        _columns = new IAutoComputedValueOf<FileData>[]
        {
            new IndexColumnOf<FileData>(index),
            AutoComputedValue<FileData>.Of(x => x.Path, x => x),
            AutoComputedValue<FileData>.Of(x => x.LengthBytes, x => x.FormatBytes()),
            AutoComputedValue<FileData>.Of(x => x.Status, x => x.ToString("G")),
            AutoComputedValue<FileData>.Of(x => (int)MathF.Ceiling(x.LengthBytes / (float)_pieceLength), x => x.ToString())
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

    public void UpdateIfNeeded(FileData? value, long pieceLength)
    {
        base.UpdateIfNeeded(value);
        _pieceLength = pieceLength;
    }
}