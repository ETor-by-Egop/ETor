using System.Text;

namespace ETor.BEncoding;

public class BEncodeString : BEncodeNode
{
    public ByteString Value { get; set; }

    public BEncodeString() : base(BEncodeTokenType.String)
    {
        Value = new ByteString("");
    }

    public BEncodeString(string value) : base(BEncodeTokenType.String)
    {
        Value = new ByteString(value);
    }

    public BEncodeString(byte[] value) : base(BEncodeTokenType.String)
    {
        Value = new ByteString(value, 0, value.Length);
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

        var len = Value.Value.Count.ToString();
        
        stream.Write(Encoding.UTF8.GetBytes(len));
        
        stream.WriteByte((byte) ':');
        stream.Write(Value.Value);
    }
}