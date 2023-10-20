using System.Diagnostics.CodeAnalysis;

namespace ETor.BEncoding;

public abstract class BEncodeNode
{
    public BEncodeTokenType Type { get; set; }

    protected BEncodeNode(BEncodeTokenType type)
    {
        Type = type;
    }

    public abstract int CalculateSize();

    public virtual BEncodeNode this[string key] => throw new InvalidOperationException("index access on base type");

    public virtual BEncodeNode this[int key] => throw new InvalidOperationException("index access on base type");

    public virtual bool TryGetValue<T>(string key, [MaybeNullWhen(false)] out T node)
        where T : BEncodeNode
        => throw new InvalidOperationException("index access on base type");

    public abstract void Serialize(Stream stream);
}