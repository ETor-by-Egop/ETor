namespace ETor.Shared;

public static class BytesExtensions
{
    public static string ToHexString(this IList<byte> bytes)
    {
        return string.Create(
            bytes.Count * 2,
            bytes,
            (span, segment) =>
            {
                for (var index = 0; index < segment.Count; index++)
                {
                    var c = segment[index];
                    if (!c.TryFormat(span[(index * 2)..], out var written, "X2") || written < 2)
                    {
                        throw new InvalidOperationException("Failed to format bytes as hex");
                    }
                }
            }
        );
    }

    public static string FormatBytes(this long? bytes)
    {
        if (bytes is null)
        {
            return "Unknown";
        }

        var kb = bytes / 1024;
        var mb = kb / 1024;
        var gb = mb / 1024;

        if (gb == 0)
        {
            if (mb == 0)
            {
                if (kb == 0)
                {
                    return $"{bytes} Kb";
                }

                return $"{kb % 1024} Kb, {bytes % 1024} B";
            }

            return $"{mb % 1024} Mb, {kb % 1024} Kb, {bytes % 1024} B";
        }

        return $"{gb % 1024} Gb, {mb % 1024} Mb, {kb % 1024} Kb, {bytes % 1024} B";
    }
}