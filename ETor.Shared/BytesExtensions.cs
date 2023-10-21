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
}