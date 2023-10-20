namespace ETor.BEncoding;

public class BEncodeList : BEncodeNode
{
    public List<BEncodeNode> Items { get; set; }

    public BEncodeList() : base(BEncodeTokenType.List)
    {
        Items = new List<BEncodeNode>();
    }

    public BEncodeList(IEnumerable<BEncodeNode> nodes) : base(BEncodeTokenType.List)
    {
        Items = new List<BEncodeNode>(nodes);
    }

    public override string ToString()
    {
        return $"[\n{string.Join(",\n", Items)}\n]";
    }

    public override BEncodeNode this[int key] => Items[key];
    
    public override void Serialize(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new InvalidOperationException("Stream is not writable");
        }
        
        stream.WriteByte((byte) 'l');
        foreach (var item in Items)
        {
            item.Serialize(stream);
        }
        stream.WriteByte((byte) 'e');
    }
}