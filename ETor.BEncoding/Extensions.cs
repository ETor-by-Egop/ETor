using System.Text;

namespace ETor.BEncoding;

public static class Extensions
{
    public static void WriteAsBEncodedString(this string str, Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new InvalidOperationException("Stream is not writable");
        }

        var len = str.Length.ToString();
        
        stream.Write(Encoding.UTF8.GetBytes(len));
        
        stream.WriteByte((byte) ':');
        
        stream.Write(Encoding.UTF8.GetBytes(str));
    }
}