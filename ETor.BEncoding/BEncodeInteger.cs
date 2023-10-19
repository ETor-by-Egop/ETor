namespace ETor.BEncoding;

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