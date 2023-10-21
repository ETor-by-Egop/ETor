using ETor.BEncoding;
using ETor.Manifest;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface IManifestLoader
{
    Task<TorrentDownload> Load(string manifestPath);
}

public class ManifestLoader : IManifestLoader
{
    private readonly ILogger<ManifestLoader> _logger;

    public ManifestLoader(ILogger<ManifestLoader> logger)
    {
        _logger = logger;
    }

    public async Task<TorrentDownload> Load(string manifestPath)
    {
        var content = await File.ReadAllBytesAsync(manifestPath);

        _logger.LogInformation("Read .torrent file {path} of size {size}", manifestPath, content.Length);

        var encodedContent = new BEncodeParser(content);

        var dict = encodedContent.ReadDictionary();

        var torrentManifest = new TorrentManifest(dict);

        _logger.LogInformation("Parsed .torrent file. {@manifest}", torrentManifest);

        return new TorrentDownload(torrentManifest, manifestPath);
    }
}