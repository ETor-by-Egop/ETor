using ETor.App.Data;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface ITransferManager
{
    void StartAll(TorrentData torrent);
    Dictionary<Guid, TorrentTransfer> Transfers { get; }
}

public class TransferManager : ITransferManager
{
    public Dictionary<Guid, TorrentTransfer> Transfers { get; }

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TransferManager> _logger;

    public TransferManager(ILoggerFactory loggerFactory, ILogger<TransferManager> logger)
    {
        _loggerFactory = loggerFactory;
        _logger = logger;
        Transfers = new Dictionary<Guid, TorrentTransfer>();
    }

    public void StartAll(TorrentData torrent)
    {
        if (Transfers.ContainsKey(torrent.InternalId))
        {
            throw new InvalidOperationException("This torrent is already started");
        }

        var transfer = new TorrentTransfer(torrent, _loggerFactory);
        Transfers[torrent.InternalId] = transfer;
        transfer.Start();
    }
}