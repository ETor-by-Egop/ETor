using ETor.App.Data;

namespace ETor.App.Services;

public interface IPeerManager
{
}

public class PeerManager
{
    private readonly Dictionary<Guid, List<PeerData>> _peers;

    public PeerManager(Dictionary<Guid, List<PeerData>> peers)
    {
        _peers = peers;
    }
}