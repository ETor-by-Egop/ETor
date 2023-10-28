using System.Security.Cryptography;

namespace ETor.Shared;

public static class HashingExtensions
{
    public static byte[] Sha1(this Memory<byte> input)
    {
        return SHA1.HashData(input.Span);
    }

    public static void Sha1(this Memory<byte> input, Memory<byte> dest)
    {
        SHA1.HashData(input.Span, dest.Span);
    }
}