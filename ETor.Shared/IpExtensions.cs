namespace ETor.Shared;

public static class IpExtensions
{
    public static string FormatIp(this int ip)
    {
        var part1 = ip >> 24 & 0xFF;
        var part2 = ip >> 16 & 0xFF;
        var part3 = ip >> 8 & 0xFF;
        var part4 = ip >> 0 & 0xFF;

        return $"{part1}.{part2}.{part3}.{part4}";
    }
}