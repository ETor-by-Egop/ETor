﻿using System.Numerics;
using ETor.App.Data;
using ImGuiNET;

namespace ETor.Client.UiValues;

public class TrackersTable
{
    public const int Columns = 4;

    private List<TrackersTableRow> _rows = new();

    private IReadOnlyList<TrackerData> _source = ArraySegment<TrackerData>.Empty;

    private int? _lastSelectedIndex = null;

    public bool HasRows => _rows.Count > 0;

    private static readonly string[] Headers =
    {
        "#",
        "Url",
        "Protocol",
        "Status"
    };

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

    public void UpdateIfNeeded(IReadOnlyList<TrackerData> source, int? selectedIndex)
    {
        _source = source;

        while (_source.Count > _rows.Count)
        {
            _rows.Add(new TrackersTableRow(_rows.Count));
        }

        while (_rows.Count > _source.Count)
        {
            _rows.RemoveAt(_rows.Count - 1);
        }

        for (var i = 0; i < _source.Count; i++)
        {
            _rows[i]
                .UpdateIfNeeded(_source[i]);
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