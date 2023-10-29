using ETor.Manifest;

namespace ETor.App.Data;

public class TorrentData : IHashCoded
{
    private readonly List<TrackerData> _trackers;

    private readonly List<FileData> _files;

    private readonly List<PieceData> _pieces;

    public IReadOnlyList<TrackerData> Trackers => _trackers;
    public IReadOnlyList<FileData> Files => _files;
    public IReadOnlyList<PieceData> Pieces => _pieces;

    public string Name { get; private set; }

    public string FilePath { get; private set; }

    public long PieceLength { get; private set; }

    public long TotalLength { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTime? CreationDate { get; private set; }

    public string? Comment { get; private set; }
    public string? Encoding { get; private set; }

    public Memory<byte> InfoHash { get; set; }

    public long HashCode { get; private set; }

    public TorrentData(TorrentManifest manifest, string filePath)
    {
        if (manifest.Info is null)
        {
            throw new InvalidOperationException("Bad .torrent manifest. No info section.");
        }

        if (manifest.Info.Pieces is null)
        {
            throw new InvalidOperationException("Bad .torrent manifest. No pieces section.");
        }

        if (manifest.Info.PieceLength is null)
        {
            throw new InvalidOperationException("Bad .torrent manifest. No piece-length section.");
        }

        _trackers = new List<TrackerData>();
        _files = new List<FileData>();
        _pieces = new List<PieceData>(manifest.Info.Pieces.Length / 20); // each piece in manifest is 20 bytes SHA1 of it's data

        PieceLength = manifest.Info.PieceLength.Value;

        if (!string.IsNullOrEmpty(manifest.Announce))
        {
            _trackers.Add(new TrackerData(manifest.Announce));
        }

        if (manifest.AnnounceList is not null)
        {
            _trackers.AddRange(
                manifest.AnnounceList
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .Where(x => _trackers.All(y => y.Url != x))
                    .Select(x => new TrackerData(x))
            );
        }

        FilePath = filePath;
        Name = manifest.Info.Name ?? Path.GetFileName(filePath);

        if (manifest.Info.Files is null)
        {
            // single-file manifest
            if (manifest.Info.Name is null || manifest.Info.Length is null)
            {
                // bad manifest, single-file .torrent must have a file defined
                throw new InvalidOperationException($"Bad single-file .torrent manifest. FileName:{manifest.Info.Name}, FileLength: {manifest.Info.Length}");
            }

            _files.Add(new FileData(manifest.Info.Name, manifest.Info.Length.Value));
        }
        else
        {
            // multi-file manifest
            foreach (var file in manifest.Info.Files)
            {
                if (file.Path is null || file.Length is null)
                {
                    // bad file section, just skip it
                    continue;
                }

                _files.Add(new FileData(string.Join("\\", file.Path), file.Length.Value));
            }

            if (_files.Count == 0)
            {
                throw new InvalidOperationException("Bad multi-file .torrent manifest. No files could be added");
            }
        }

        TotalLength = _files.Sum(x => x.LengthBytes);

        var pieces = manifest.Info.Pieces.AsMemory();
        for (var i = 0; i < pieces.Length / 20; i++)
        {
            _pieces.Add(new PieceData(pieces.Slice(i * 20, 20)));
        }

        CreatedBy = manifest.CreatedBy;
        CreationDate = manifest.CreationDate;
        Comment = manifest.Comment;
        Encoding = manifest.Encoding;

        InfoHash = manifest.Info.ComputeSha1();
    }
}