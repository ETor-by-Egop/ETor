# ETor

(ETor) is Egop's Torrent

### Cross-platform Bittorrent reimplementation written in C# for .NET7

Protocol reimplementation completely in C#.

No protocol-related logic is borrowed.

## Current state

* Working UI
* Bencoding processing - `.torrent` file parsing
* Single-file and multi-file `.torrent` file support
* File on-disk allocation
* Pieces check from disk
* UDP Trackers connection (currently no announcing)
* Auto reconnection to tracker on failure

## Uses

* `Silk.Net` for window management
* `OpenGL` for graphics backend
* `ImGui` for UI
* `Serilog` for logging
* `Newtonsoft.Json` for json processing
* `SixLabors.ImageSharp` - only for icons loading

Additional usages:

* `Microsoft.DependencyInjection` for easier app wiring
* `Microsoft.Configuration` for easier configuration
* `Microsoft.Logging` for easier logging