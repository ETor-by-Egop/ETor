using System.Text;

namespace ETor.BEncoding;

public class ByteString
{
    public byte[] Value { get; set; }

    public ByteString(byte[] src)
    {
        Value = src;
    }

    public ByteString(byte[] src, int offset, int len)
    {
        Value = new ArraySegment<byte>(src, offset, len).ToArray();
    }

    public ByteString(string src)
    {
        Value = Encoding.UTF8.GetBytes(src);
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(Value);
    }
}