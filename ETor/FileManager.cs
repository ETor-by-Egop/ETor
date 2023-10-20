namespace ETor;

public class FileManager
{
    public static void CreateFiles(Torrent torrent, string torrentName)
    {
        var downloadsDirectory = new DirectoryInfo("C:\\Users\\Admin\\Downloads\\ETorDownloads");

        if (!downloadsDirectory.Exists)
        {
            downloadsDirectory.Create();
        }

        var torrentDirectory = new DirectoryInfo(Path.Combine(downloadsDirectory.FullName, torrentName));

        if (!torrentDirectory.Exists)
        {
            torrentDirectory.Create();
        }

        if (torrent.Info.IsSingleFile)
        {
            if (torrent.Info.Length is not null && torrent.Info.Name is not null)
            {
                var fileLength = torrent.Info.Length.Value;

                var advanced = new FileStreamOptions
                {
                    Mode = FileMode.CreateNew,
                    Access = FileAccess.Write,
                    Share = FileShare.None,
                    PreallocationSize = fileLength
                };

                using var fileStream = new FileStream(Path.Combine(torrentDirectory.FullName, torrent.Info.Name), advanced);

                fileStream.SetLength(fileLength);

                fileStream.Flush(true);
            }
        }
        else
        {
            if (torrent.Info?.Files is not null)
            {
                foreach (var fileSection in torrent.Info.Files)
                {
                    var fileLength = fileSection.Length!.Value;

                    var advanced = new FileStreamOptions
                    {
                        Mode = FileMode.CreateNew,
                        Access = FileAccess.Write,
                        Share = FileShare.None,
                        PreallocationSize = fileLength
                    };

                    using var fileStream = new FileStream(Path.Combine(torrentDirectory.FullName, fileSection.ComputeFilePath()), advanced);

                    fileStream.SetLength(fileLength);

                    fileStream.Flush(true);
                }
            }
        }
    }
}