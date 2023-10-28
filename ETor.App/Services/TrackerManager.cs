using ETor.Networking;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface ITrackerManager
{
    Task Connect(Tracker tracker);
}

public class TrackerManager : ITrackerManager
{
    private readonly IUdpConnector _udpConnector;
    private readonly ILogger<TrackerManager> _logger;

    public TrackerManager(IUdpConnector udpConnector, ILogger<TrackerManager> logger)
    {
        _udpConnector = udpConnector;
        _logger = logger;
    }

    public async Task Connect(Tracker tracker)
    {
        if (!tracker.IsUdp)
        {
            _logger.LogWarning("Non udp tracker is not supported. {tracker}", tracker.Host);
            return;
        }

        await _udpConnector.ConnectTo(tracker.Host, tracker.Port);
    }
}