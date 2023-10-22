using ETor.App;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class DownloadsTableRow : ComputedTableRow<TorrentDownload>
{
    private int _index;

    private IAutoComputedValueOf<TorrentDownload>[] _columns;

    private bool _isActive;

    public DownloadsTableRow(int index)
    {
        _index = index;
        _columns = new IAutoComputedValueOf<TorrentDownload>[]
        {
            new IndexColumnOf<TorrentDownload>(_index),
            AutoComputedValue<TorrentDownload>.Of(x => x.Name, x => x),
            AutoComputedValue<TorrentDownload>.Of(
                x => x.Manifest.Info?.Length ?? x.Manifest.Info?.Files?
                    .Where(y => y.Length is not null)
                    .Select(y => y.Length!.Value)
                    .Sum() ?? 0,
                x => x.FormatBytes()
            ),
            AutoComputedValue<TorrentDownload>.Of(x => "Added", x => x),
            AutoComputedValue<TorrentDownload>.Of(x => 0L, x => x.FormatBytes() + " / s"),
            AutoComputedValue<TorrentDownload>.Of(x => 0L, x => x.FormatBytes() + " / s")
        };
    }

    public void SetActive(bool active)
    {
        _isActive = active;
    }

    public bool Draw()
    {
        if (IsDirty)
        {
            Recalculate();
        }

        ImGui.TableNextRow();
        
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