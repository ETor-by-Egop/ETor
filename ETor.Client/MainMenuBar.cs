using ETor.Client.Abstractions;
using ETor.Client.FilePickers;
using ETor.Client.Popups;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace ETor.Client;

public class MainMenuBar : IImGuiPanel
{
    private readonly OpenFilePicker _openFilePicker;
    private readonly ILogger<MainMenuBar> _logger;

    public MainMenuBar(OpenFilePicker openFilePicker, ILogger<MainMenuBar> logger)
    {
        _openFilePicker = openFilePicker;
        _logger = logger;
    }

    public void OnImGuiRender()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New"))
                {
                    _logger.LogInformation("Creating torrents is not supported yet (");
                }

                if (ImGui.MenuItem("Open"))
                {
                    _openFilePicker.Open();
                    _logger.LogInformation("Opening file picker");
                }
            }

            ImGui.EndMenu();
            ImGui.EndMenuBar();
        }
    }
}