﻿using System.Security.Cryptography;
using ETor.App.Data;
using ETor.Shared;
using Microsoft.Extensions.Logging;

namespace ETor.App.Services;

public interface IPersistenceManager
{
    Task CheckPieces(TorrentData torrent);
}

public class PersistenceManager : IPersistenceManager
{
    private readonly ILogger<PersistenceManager> _logger;
    private readonly IFileManager _fileManager;

    public PersistenceManager(ILogger<PersistenceManager> logger, IFileManager fileManager)
    {
        _logger = logger;
        _fileManager = fileManager;
    }
    
    public async Task CheckPieces(TorrentData torrent)
    {
        if (torrent.Files.Count > 1)
        {
            _logger.LogWarning("Failed to start download, because multi-file torrents aren't supported yet");
        }

        Memory<byte> buffer = new byte[torrent.PieceLength];
        Memory<byte> hashBuffer = new byte[SHA1.HashSizeInBytes];

        var file = torrent.Files[0];
        var stream = _fileManager.GetStream(torrent, file);
        var pieces = torrent.Pieces;
        
        for (var i = 0; i < pieces.Count; i++)
        {
            var piece = pieces[i];
        
            var readBytes = await stream.ReadAsync(buffer);

            if (readBytes != torrent.PieceLength)
            {
                _logger.LogWarning("Failed to read {pieceLength} bytes from filestream, only read {actual}", torrent.PieceLength, readBytes);
            }

            buffer.Sha1(hashBuffer);

            if (hashBuffer.Span.SequenceEqual(piece.Hash.Span))
            {
                piece.SetStatus(PieceStatus.Good);
                _logger.LogInformation("Piece {number} is ok at {position}", i, stream.Position);
            }
            else
            {
                piece.SetStatus(PieceStatus.Bad);
                // _logger.LogInformation("Piece {number} is bad at {position}", i, stream.Position);
            }
        }
        
        _logger.LogInformation("Done checking pieces");
    }
}