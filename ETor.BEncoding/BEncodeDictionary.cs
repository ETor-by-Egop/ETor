namespace ETor.BEncoding;

public class BEncodeDictionary : BEncodeNode
{
    public Dictionary<string, BEncodeNode> Items { get; set; }

    public BEncodeDictionary() : base(BEncodeTokenType.Dictionary)
    {
        Items = new Dictionary<string, BEncodeNode>();
    }

    public override string ToString()
    {
        return $"{{\n{string.Join(",\n", Items.Select(x => $"\"{x.Key}\": {x.Value}"))}}}";
    }

    public override BEncodeNode this[string key] => Items[key];
}