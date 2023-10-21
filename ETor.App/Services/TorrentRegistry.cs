using ETor.Manifest;

namespace ETor.App.Services;

public interface ITorrentRegistry
{
    IReadOnlyList<TorrentDownload> GetTorrents();

    void Add(TorrentDownload torrent);
}

public class TorrentRegistry : ITorrentRegistry
{
    private readonly List<TorrentDownload> _torrents;

    public TorrentRegistry()
    {
        _torrents = new List<TorrentDownload>();
    }

    public IReadOnlyList<TorrentDownload> GetTorrents()
    {
        return _torrents;
    }

    public void Add(TorrentDownload torrent)
    {
        _torrents.Add(torrent);
    }
}