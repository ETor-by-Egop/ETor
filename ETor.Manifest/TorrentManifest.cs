using ETor.BEncoding;

namespace ETor.Manifest;

public class TorrentManifest
{
    public string? Announce { get; set; }

    public List<string>? AnnounceList { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Comment { get; set; }

    public string? Encoding { get; set; }

    public InfoSection? Info { get; set; }

    public bool HasAnyTrackers => Announce is not null ||
                                  AnnounceList is not null && AnnounceList.Count > 0;

    public TorrentManifest(BEncodeDictionary torrentDictionary)
    {
        if (torrentDictionary.TryGetValue<BEncodeString>("announce", out var announce))
        {
            Announce = announce.Value.ToString();
        }

        if (torrentDictionary.TryGetValue<BEncodeList>("announce-list", out var announceList))
        {
            AnnounceList = new List<string>();
            ExpandAnnounceList(announceList);
        }

        if (torrentDictionary.TryGetValue<BEncodeString>("created by", out var createdBy))
        {
            CreatedBy = createdBy.Value.ToString();
        }

        if (torrentDictionary.TryGetValue<BEncodeInteger>("creation date", out var creationDate))
        {
            CreationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.UnixEpoch.AddSeconds(creationDate.Value));
        }

        if (torrentDictionary.TryGetValue<BEncodeString>("comment", out var comment))
        {
            Comment = comment.Value.ToString();
        }

        if (torrentDictionary.TryGetValue<BEncodeString>("encoding", out var encoding))
        {
            Encoding = encoding.Value.ToString();
        }

        if (torrentDictionary.TryGetValue<BEncodeDictionary>("info", out var info))
        {
            Info = new InfoSection(info);
        }
    }

    private void ExpandAnnounceList(BEncodeList announceList)
    {
        foreach (var announceListItem in announceList.Items)
        {
            if (announceListItem is BEncodeString str)
            {
                AnnounceList.Add(str.Value.ToString());
            }
            else if (announceListItem is BEncodeList list)
            {
                ExpandAnnounceList(list);
            }
        }
    }
}