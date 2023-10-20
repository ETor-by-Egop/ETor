using System.Text;

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