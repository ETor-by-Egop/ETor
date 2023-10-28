using ETor.App;
using ETor.App.Data;
using ETor.Shared;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class DownloadsTable
{
    public const int Columns = 6;

    private List<DownloadsTableRow> _rows = new();

    private IReadOnlyList<TorrentData> _source;

    private int? _lastSelectedIndex = null;

    public DownloadsTable(IReadOnlyList<TorrentData> source)
    {
        _source = source;
    }

    // ReSharper disable once RedundantExplicitArraySize
    private static readonly string[] Headers = new string[Columns]
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

    public void UpdateIfNeeded(int? selectedIndex)
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