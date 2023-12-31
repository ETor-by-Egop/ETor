using System.Text;

namespace ETor.BEncoding;

public class BEncodeInteger : BEncodeNode
{
    public long Value { get; set; }

    public BEncodeInteger(long value) : base(BEncodeTokenType.Integer)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override int CalculateSize()
    {
        var valLength = Encoding.UTF8.GetByteCount(Value.ToString());

        return 1 + valLength + 1;
    }

    public override void Serialize(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new InvalidOperationException("Stream is not writable");
        }
        
        stream.WriteByte((byte) 'i');
        stream.Write(Encoding.UTF8.GetBytes(Value.ToString()));
        stream.WriteByte((byte) 'e');
    }
}