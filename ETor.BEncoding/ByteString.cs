using System.Text;

namespace ETor.BEncoding;

public class ByteString
{
    public ArraySegment<byte> Value { get; set; }

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