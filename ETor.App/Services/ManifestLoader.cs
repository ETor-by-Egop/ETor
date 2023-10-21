using ETor.BEncoding;
using ETor.Manifest;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface IManifestLoader
{
    Task<TorrentManifest> Load(string manifestPath);
}

public class ManifestLoader : IManifestLoader
{
    private readonly ILogger<ManifestLoader> _logger;

    public ManifestLoader(ILogger<ManifestLoader> logger)
    {
        _logger = logger;
    }

    public async Task<TorrentManifest> Load(string manifestPath)
    {
        var torrentName = Path.GetFileName(manifestPath);

        var content = await File.ReadAllBytesAsync(manifestPath);

        _logger.LogInformation("Read .torrent file {file} of size {size}", torrentName, content.Length);

        var encodedContent = new BEncodeParser(content);

        var dict = encodedContent.ReadDictionary();

        var torrentManifest = new TorrentManifest(dict);

        _logger.LogInformation("Parsed .torrent file. Name: {name}. Single-file: {mode}", torrentManifest.Info.Name, torrentManifest.Info.IsSingleFile);

        return torrentManifest;
    }
}