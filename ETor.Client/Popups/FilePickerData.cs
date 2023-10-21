using ETor.Client.Abstractions;
using ETor.Client.Gl;
using Microsoft.Extensions.Logging;

namespace ETor.Client.Popups;

public class FilePickerData : IRequireInit
{
    public Texture? FileIconTexture;
    public Texture? DirIconTexture;

    private readonly ILogger<FilePickerData> _logger;

    public FilePickerData(ILogger<FilePickerData> logger)
    {
        _logger = logger;
    }

    public void Init()
    {
        var fileIconData = ImageSharpTextureLoader.Load("assets/FileIcon.png");
        FileIconTexture = new Texture(
            ETorClient.Instance.GL,
            fileIconData.Width,
            fileIconData.Height,
            fileIconData.RgbaPixels
        );
        var dirIconData = ImageSharpTextureLoader.Load("assets/DirectoryIcon.png");
        DirIconTexture = new Texture(
            ETorClient.Instance.GL,
            dirIconData.Width,
            dirIconData.Height,
            dirIconData.RgbaPixels
        );
        _logger.LogInformation("Initialized FilePickerData");
    }
}