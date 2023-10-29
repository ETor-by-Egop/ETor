namespace ETor.Networking;

public interface ICanDeserialize
{
    void Deserialize(Memory<byte> buffer);
}