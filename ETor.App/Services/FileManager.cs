using System.Security.AccessControl;
using ETor.App.Data;
using ETor.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETor.App.Services;

public interface IFileManager
{
    Task EnsureFileExistence(TorrentData torrent);

    FileStream GetStream(TorrentData torrent, FileData file);
}

public class FileManager : IFileManager, IDisposable
{
    private readonly Dictionary<Guid, FileStream> _openStreams;

    private readonly ILogger<FileManager> _logger;
    private readonly IOptions<FileManagerConfig> _options;

    public FileManager(ILogger<FileManager> logger, IOptions<FileManagerConfig> options)
    {
        _logger = logger;
        _options = options;
        _openStreams = new Dictionary<Guid, FileStream>();
    }

    public async Task EnsureFileExistence(TorrentData torrent)
    {
        var downloadsDirectory = new DirectoryInfo(_options.Value.DownloadPath);

        if (!downloadsDirectory.Exists)
        {
            downloadsDirectory.Create();
        }

        if (torrent.Files.Count == 1)
        {
            var file = torrent.Files[0];
            var fileLength = file.LengthBytes;

            var path = Path.Combine(
                downloadsDirectory.FullName,
                file.Path
            );

            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                var stream = fileInfo.Create();
                stream.SetLength(fileLength);
                _openStreams[file.InternalId] = stream;
                _logger.LogInformation("File created: {path}", path);
            }
            else
            {
                _openStreams[file.InternalId] = fileInfo.Open(FileMode.Open);
                _logger.LogInformation("File {path} already exists", path);
            }
        }
        else
        {
            var torrentDirectory = new DirectoryInfo(Path.Combine(downloadsDirectory.FullName, torrent.Name));

            if (!torrentDirectory.Exists)
            {
                torrentDirectory.Create();
            }

            foreach (var file in torrent.Files)
            {
                var fileLength = file.LengthBytes;

                var path = Path.Combine(torrentDirectory.FullName, file.Path);

                var fileInfo = new FileInfo(path);

                if (!fileInfo.Exists)
                {
                    var stream = fileInfo.Create();
                    stream.SetLength(fileLength);
                    _openStreams[file.InternalId] = stream;
                    _logger.LogInformation("File created: {path}", path);
                }
                else
                {
                    _openStreams[file.InternalId] = fileInfo.Open(FileMode.Open);
                    _logger.LogInformation("File {path} already exists", path);
                }
            }
        }
    }

    public FileStream GetStream(TorrentData torrent, FileData file)
    {
        if (_openStreams.TryGetValue(file.InternalId, out var stream))
        {
            return stream;
        }

        throw new InvalidOperationException($"Failed to open stream to torrent file of {torrent.Name}. File {file.Path}.");
    }

    public void Dispose()
    {
        foreach (var key in _openStreams.Keys)
        {
            _openStreams[key].Dispose();
        }
    }
}