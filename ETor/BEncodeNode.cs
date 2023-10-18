using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace ETor;

public class BEncodeNode
{
    public BEncodeTokenType Type { get; set; }

    public BEncodeNode(BEncodeTokenType type)
    {
        Type = type;
    }

    public virtual BEncodeNode this[string key]
    {
        get { throw new InvalidOperationException("index access on base type"); }
    }

    public virtual BEncodeNode this[int key]
    {
        get { throw new InvalidOperationException("index access on base type"); }
    }
}

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

    public override BEncodeNode this[string key]
    {
        get { return Items[key]; }
    }
}

public class BEncodeString : BEncodeNode
{
    public ByteString Value { get; set; }

    public BEncodeString() : base(BEncodeTokenType.String)
    {
        Value = new ByteString("");
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public class ByteString
{
    public ArraySegment<byte> Value { get; set; }

    public bool IsRaw { get; set; }

    public ByteString(byte[] src, int offset, int len)
    {
        Value = new ArraySegment<byte>(src, offset, len);
    }

    public ByteString(string src)
    {
        Value = new ArraySegment<byte>(Encoding.UTF8.GetBytes(src));
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(Value);
    }
}

public class BEncodeInteger : BEncodeNode
{
    public long Value { get; set; }

    public BEncodeInteger() : base(BEncodeTokenType.Integer)
    {
        Value = 0;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}