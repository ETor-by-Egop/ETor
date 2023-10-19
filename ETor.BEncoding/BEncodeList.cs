namespace ETor.BEncoding;

public class BEncodeList : BEncodeNode
{
    public List<BEncodeNode> Items { get; set; }

    public BEncodeList() : base(BEncodeTokenType.List)
    {
        Items = new List<BEncodeNode>();
    }

    public override string ToString()
    {
        return $"[\n{string.Join(",\n", Items)}\n]";
    }

    public override BEncodeNode this[int key] => Items[key];
}