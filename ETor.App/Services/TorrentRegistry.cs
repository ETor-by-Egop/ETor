using ETor.Manifest;
using ETor.Shared;

namespace ETor.App.Services;

public interface ITorrentRegistry
{
    SelectableReadOnlyList<TorrentDownload> GetTorrents();

    void Add(TorrentDownload torrent);
    void SetSelectedTorrent(int index);
}

public class TorrentRegistry : ITorrentRegistry
{
    private readonly List<TorrentDownload> _torrents;
    private readonly SelectableReadOnlyList<TorrentDownload> _selectableList;

    public TorrentRegistry()
    {
        _torrents = new List<TorrentDownload>();
        _selectableList = new SelectableReadOnlyList<TorrentDownload>(_torrents);
    }

    public SelectableReadOnlyList<TorrentDownload> GetTorrents()
    {
        return _selectableList;
    }

    public void Add(TorrentDownload torrent)
    {
        _torrents.Add(torrent);
    }

    public void SetSelectedTorrent(int index)
    {
        _selectableList.SelectedIndex = index;
    }
}