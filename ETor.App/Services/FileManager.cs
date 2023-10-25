using ETor.App.Data;
using ETor.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETor.App.Services;

public interface IFileManager
{
    Task CreateFiles(TorrentData torrent);
}

public class FileManager : IFileManager
{
    private readonly ILogger<FileManager> _logger;
    private readonly IOptions<FileManagerConfig> _options;

    public FileManager(ILogger<FileManager> logger, IOptions<FileManagerConfig> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task CreateFiles(TorrentData torrent)
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

            var advanced = new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                Share = FileShare.None,
                PreallocationSize = fileLength
            };

            var path = Path.Combine(
                downloadsDirectory.FullName,
                file.Path
            );
            await using var fileStream = new FileStream(path, advanced);

            fileStream.SetLength(fileLength);

            await fileStream.FlushAsync();

            file.Status = FileStatus.Created;
            _logger.LogInformation("Created file at {path} of length {length}", path, fileLength);
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

                var advanced = new FileStreamOptions
                {
                    Mode = FileMode.CreateNew,
                    Access = FileAccess.Write,
                    Share = FileShare.None,
                    PreallocationSize = fileLength
                };

                var path = Path.Combine(torrentDirectory.FullName, file.Path);
                await using var fileStream = new FileStream(path, advanced);

                fileStream.SetLength(fileLength);

                await fileStream.FlushAsync();

                file.Status = FileStatus.Created;
                _logger.LogInformation("Created file at {path} of length {length}", path, fileLength);
            }
        }
    }
}