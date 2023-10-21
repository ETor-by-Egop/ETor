using ETor.App;
using ETor.Client.Abstractions;
using ETor.Client.Popups;
using ETor.Configuration;
using ETor.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETor.Client.FilePickers;

public class OpenFilePicker : FilePickerBase, IImGuiPanel
{
    private readonly Application _app;
    private readonly ILogger<OpenFilePicker> _logger;

    public OpenFilePicker(FilePickerData data, ILogger<FilePickerBase> logger, ILogger<OpenFilePicker> logger1, Application app, IOptions<FilePickerConfig> options) : base(options, data, logger)
    {
        _logger = logger1;
        _app = app;
    }

    public void OnImGuiRender()
    {
        if (Show(out var path))
        {
            _logger.LogInformation("Opening file {path}", path);

            if (Path.GetExtension(path) is not ".torrent")
            {
                _logger.LogWarning("Opened file is not a .torrent file");
            }
            else
            {
                _app.AddDownload(path);
            }
        }
    }

    public override bool ShouldShowFile(FileSystemInfo file)
    {
        if (file.IsDirectory())
        {
            return true;
        }

        if (file.Extension == ".torrent")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}