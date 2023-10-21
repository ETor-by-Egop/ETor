using ETor.Manifest;

namespace ETor.App;

public class TorrentDownload
{
    public TorrentManifest Manifest { get; }

    public string FilePath { get; set; }

    public string Name { get; }
    
    public TorrentDownload(TorrentManifest manifest, string path)
    {
        Manifest = manifest;
        FilePath = path;
        Name = Path.GetFileName(path);
    }
}