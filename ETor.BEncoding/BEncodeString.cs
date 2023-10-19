namespace ETor.BEncoding;

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