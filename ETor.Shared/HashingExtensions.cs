using System.Security.Cryptography;

namespace ETor.Shared;

public static class HashingExtensions
{
    public static byte[] Sha1(this byte[] input)
    {
        return SHA1.HashData(input);
    }
}