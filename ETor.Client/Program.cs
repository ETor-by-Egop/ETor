// Create a Silk.NET window as usual

using System.Drawing;
using ETor.Client;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using Color = System.Drawing.Color;
// ReSharper disable AccessToDisposedClosure

// this file is mostly a copy of Silk.Net ImGui example + my adaptation from SCVE

IWindow window = Window.Create(WindowOptions.Default);

// Declare some variables
ImGuiController controller = null!;
GL gl = null!;
IInputContext inputContext = null!;

var etorClient = new ETorClient(window);

// Our loading function
window.Load += () =>
{
    controller = new ImGuiController(
        gl = etorClient.GL = window.CreateOpenGL(), // load OpenGL
        window, // pass in our window
        inputContext = window.CreateInput(), // create an input context
        () =>
        {
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            etorClient.OpenSansFont = io.Fonts.AddFontFromFileTTF(
                "assets/Font/OpenSans-Regular.ttf",
                18,
                null,
                io.Fonts.GetGlyphRangesCyrillic()
            );
        }
    );
    etorClient.Init();

    // inputContext.Keyboards[0]
    //     .KeyDown += (keyboard, key, scancode) => { etorClient.OnKeyDown(key); };
    // inputContext.Keyboards[0]
    //     .KeyUp += (keyboard, key, scancode) => { etorClient.OnKeyPressed(key); };
    // inputContext.Keyboards[0]
    //     .KeyUp += (keyboard, key, scancode) => { etorClient.OnKeyReleased(key); };
};

// Handle resizes
window.FramebufferResize += s =>
{
    // Adjust the viewport to the new window size
    gl.Viewport(s);
};

// Handles the dile drop and receives the array of paths to the files.
// window.FileDrop += paths => { etorClient.OnFileDrop(paths); };

window.Update += delta =>
{
    // Make sure ImGui is up-to-date
    controller.Update((float) delta);

    etorClient.Update(delta);
};

// The render function
window.Render += delta =>
{
    // This is where you'll do any rendering beneath the ImGui context
    // Here, we just have a blank screen.
    gl.ClearColor(
        Color.FromArgb(
            255,
            (int) (.45f * 255),
            (int) (.55f * 255),
            (int) (.60f * 255)
        )
    );
    gl.Clear((uint) ClearBufferMask.ColorBufferBit);

    etorClient.OnImGuiRender();

    // Make sure ImGui renders too!
    controller.Render();
};

// The closing function
window.Closing += () =>
{
    // etorClient.Exit();

    ImGui.SaveIniSettingsToDisk("imgui.ini");
    // Dispose our controller first
    controller?.Dispose();

    // Dispose the input context
    inputContext?.Dispose();

    // Unload OpenGL
    gl?.Dispose();
};

// Now that everything's defined, let's run this bad boy!
window.Run();

window.Dispose();