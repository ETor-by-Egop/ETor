using ETor.App;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class DownloadsTable
{
    public const int Columns = 6;

    private List<DownloadsTableRow> _rows = new();

    private SelectableReadOnlyList<TorrentDownload> _source;

    private int _lastSelectedIndex = -1;

    public DownloadsTable(SelectableReadOnlyList<TorrentDownload> source)
    {
        _source = source;
    }

    private static readonly string[] Headers =
    {
        "#",
        "Name",
        "Size",
        "Status",
        "Download",
        "Upload",
    };

    public bool HasRows => _rows.Count > 0;

    public void DrawHeaders()
    {
        ImGui.TableSetupColumn(Headers[0], ImGuiTableColumnFlags.None, 0.1f);

        for (var index = 1; index < Headers.Length; index++)
        {
            var column = Headers[index];
            ImGui.TableSetupColumn(column);
        }

        ImGui.TableHeadersRow();
    }

    public void UpdateIfNeeded()
    {
        while (_source.Count > _rows.Count)
        {
            _rows.Add(new DownloadsTableRow(_rows.Count));
        }

        while (_rows.Count > _source.Count)
        {
            _rows.RemoveAt(_rows.Count - 1);
        }
        
        for (var i = 0; i < _source.Count; i++)
        {
            _rows[i].UpdateIfNeeded(_source[i]);
        }

        if (_lastSelectedIndex != _source.SelectedIndex)
        {
            if (_lastSelectedIndex != -1)
            {
                _rows[_lastSelectedIndex]
                    .SetActive(false);
            }

            if (_source.SelectedIndex != -1)
            {
                _rows[_source.SelectedIndex]
                    .SetActive(true);
            }

            _lastSelectedIndex = _source.SelectedIndex;
        }
    }

    public int DrawData()
    {
        int selectedIndex = -1;
        for (var index = 0; index < _rows.Count; index++)
        {
            var row = _rows[index];
            if (row.Draw())
            {
                selectedIndex = index;
            }
        }
        
        return selectedIndex;
    }
}