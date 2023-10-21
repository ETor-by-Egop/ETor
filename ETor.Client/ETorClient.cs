using System.Numerics;
using ETor.App;
using ETor.Client.Abstractions;
using ETor.Client.Popups;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ETor.Client;

public class ETorClient
{
    private readonly IWindow _window;
    public GL GL { get; set; }
    public IInputContext Input { get; set; }

    public static ETorClient Instance;

    private static bool _dockspaceOpen = true;
    private static bool _optFullscreenPersistant = true;
    private static bool _optFullscreen = _optFullscreenPersistant;

    private static ImGuiDockNodeFlags _dockspaceFlags = ImGuiDockNodeFlags.None;

    public ImFontPtr OpenSansFont;


    private List<IImGuiPanel> _imGuiPanels = null!;

    public ETorClient(IWindow window)
    {
        _window = window;
        Instance = this;
    }

    public void Init()
    {
        ImGui.StyleColorsLight();

        IServiceCollection services = new ServiceCollection();

        var panels = AssemblyUtils.GetAssignableTypes<IImGuiPanel>();
        foreach (var type in panels)
        {
            services.AddSingleton(type);
        }
        foreach (var type in AssemblyUtils.GetAssignableTypes<FilePickerBase>())
        {
            services.AddSingleton(type);
        }

        services.RegisterApplication();

        services.AddSingleton<FilePickerData>();

        var serviceProvider = services.BuildServiceProvider();
        
        foreach (var requireInit in AssemblyUtils.GetAssignableTypes<IRequireInit>().Select(t => serviceProvider.GetRequiredService(t) as IRequireInit))
        {
            requireInit!.Init();
        }

        _imGuiPanels = panels
            .Select(t => serviceProvider.GetRequiredService(t) as IImGuiPanel)
            .ToList()!;
    }

    public void OnImGuiRender()
    {
        ImGui.PushFont(OpenSansFont);

        // We are using the ImGuiWindowFlags_NoDocking flag to make the parent window not dockable into,
        // because it would be confusing to have two docking targets within each others.
        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        if (_optFullscreen)
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize |
                           ImGuiWindowFlags.NoMove;
            windowFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        }

        // When using ImGuiDockNodeFlags_PassthruCentralNode, DockSpace() will render our background and handle the pass-thru hole, so we ask Begin() to not render a background.
        if ((_dockspaceFlags & ImGuiDockNodeFlags.PassthruCentralNode) != 0)
            windowFlags |= ImGuiWindowFlags.NoBackground;

        // Important: note that we proceed even if Begin() returns false (aka window is collapsed).
        // This is because we want to keep our DockSpace() active. If a DockSpace() is inactive, 
        // all active windows docked into it will lose their parent and become undocked.
        // We cannot preserve the docking relationship between an active window and an inactive docking, otherwise 
        // any change of dockspace/settings would lead to windows being stuck in limbo and never being visible.
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));

        ImGui.Begin("DockSpace Demo", ref _dockspaceOpen, windowFlags);
        {
            ImGui.PopStyleVar();

            if (_optFullscreen)
                ImGui.PopStyleVar(2);

            // DockSpace
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiStylePtr style = ImGui.GetStyle();
            float minWinSizeX = style.WindowMinSize.X;
            style.WindowMinSize.X = 370.0f;
            if ((io.ConfigFlags & ImGuiConfigFlags.DockingEnable) != 0)
            {
                uint dockspace_id = ImGui.GetID("MyDockSpace");
                ImGui.DockSpace(dockspace_id, new Vector2(0.0f, 0.0f), _dockspaceFlags);
            }

            style.WindowMinSize.X = minWinSizeX;

            foreach (var imGuiPanel in _imGuiPanels)
            {
                imGuiPanel.OnImGuiRender();
            }

            // ImGui.ShowMetricsWindow();
        }
        
        ImGui.ShowDemoWindow();

        ImGui.End();

        ImGui.PopFont();
    }
}