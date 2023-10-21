using System.Numerics;
using ETor.Client.Abstractions;
using ETor.Client.Gl;
using ETor.Configuration;
using ETor.Shared;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETor.Client.Popups;

public abstract class FilePickerBase
{
    private const string ImguiId = "File Picker##file-picker";

    protected DirectoryInfo CurrentDirectory;

    protected IEnumerable<FileSystemInfo> Content;

    private readonly ILogger<FilePickerBase> _logger;

    private bool _isCellMode;

    private string? _selectedFilePath;

    private bool _opened;
    private bool _shouldBeOpened;

    private readonly FilePickerData _data;

    public FilePickerBase(IOptions<FilePickerConfig> options, FilePickerData data, ILogger<FilePickerBase> logger)
    {
        _data = data;
        _logger = logger;

        GoToDirectory(new DirectoryInfo(options.Value.DefaultPath));
    }

    public void Open()
    {
        _shouldBeOpened = true;
    }

    public virtual bool ShouldShowFile(FileSystemInfo file)
    {
        return true;
    }

    private void GoToDirectory(DirectoryInfo directory)
    {
        CurrentDirectory = directory;
        Content = CurrentDirectory.EnumerateFileSystemInfos().Where(ShouldShowFile);
    }

    public bool Show(out string path)
    {
        bool result = false;

        if (!_opened && _shouldBeOpened)
        {
            ImGui.OpenPopup(ImguiId);
        }

        path = "";

        ImGui.SetNextWindowSize(new Vector2(600, 400));
        // doesn't affect anything, it's just a dummy variable for ImGui.Net
        bool dummyOpened = true;
        if (ImGui.BeginPopupModal(ImguiId, ref dummyOpened, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.MenuBar))
        {
            DrawMenuBar();

            if (_isCellMode)
            {
                result = DrawCells();
            }
            else
            {
                result = DrawTree();
            }

            path = _selectedFilePath;

            if (result)
            {
                ImGui.CloseCurrentPopup();
                _shouldBeOpened = false;
                _opened = false;
            }

            ImGui.EndPopup();
        }
        else
        {
            _opened = false;
            _shouldBeOpened = false;
        }

        return result;
    }

    private bool DrawTree()
    {
        bool selected = false;
        if (CurrentDirectory.Parent is not null)
        {
            if (ImGui.Button("<-"))
            {
                GoToDirectory(CurrentDirectory.Parent);
            }

            ImGui.SameLine();
        }

        ImGui.Text(CurrentDirectory.FullName);

        if (ImGui.BeginChild("FilePickerTree", ImGui.GetContentRegionAvail()))
        {
            foreach (var entry in Content)
            {
                var path = entry.FullName;
                var name = entry.Name;
                if (entry.IsDirectory())
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                }

                if (ImGui.TreeNodeEx(name, ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.Leaf))
                {
                    if (ImGui.IsItemHovered())
                    {
                        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        {
                            if (!entry.IsDirectory())
                            {
                                _selectedFilePath = entry.FullName;
                                selected = true;
                            }
                            else
                            {
                                GoToDirectory(new DirectoryInfo(path));
                            }
                        }
                    }

                    ImGui.TreePop();
                }

                if (entry.IsDirectory())
                {
                    ImGui.PopStyleColor();
                }
            }

            ImGui.EndChild();
        }

        return selected;
    }

    private bool DrawCells()
    {
        bool selected = false;
        if (CurrentDirectory.Parent is not null)
        {
            if (ImGui.Button("<-"))
            {
                GoToDirectory(CurrentDirectory.Parent);
            }

            ImGui.SameLine();
        }

        ImGui.Text(CurrentDirectory.FullName);

        if (ImGui.BeginChild("FilePickerCells", ImGui.GetContentRegionAvail()))
        {
            int columnCount = 6;
            ImGui.Columns(columnCount, "file_picker_columns", false);

            // magic 20, don't ask, it just works
            var columnWidth = ImGui.GetColumnWidth() - 20;

            foreach (var entry in Content)
            {
                var path = entry.FullName;

                string filename = entry.Name;

                ImGui.PushID(filename);

                var icon = entry.IsDirectory()
                    ? _data.DirIconTexture
                    : _data.FileIconTexture;
                // ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));

                ImGui.ImageButton(
                    (nint) icon.GlTexture,
                    new Vector2(columnWidth, columnWidth),
                    new Vector2(0, 1),
                    new Vector2(1, 0)
                );

                ImGui.PopID();
                if (ImGui.BeginDragDropSource())
                {
                    ImGui.Text(filename);
                    // const wchar_t* itemPath = relativePath.c_str();
                    // ImGui.SetDragDropPayload("CONTENT_BROWSER_ITEM", itemPath, (wcslen(itemPath) + 1) * sizeof(wchar_t));
                    ImGui.EndDragDropSource();
                }

                // ImGui.PopStyleColor();
                if (ImGui.IsItemHovered())
                {
                    if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        if (!entry.IsDirectory())
                        {
                            _selectedFilePath = entry.FullName;
                            selected = true;
                        }
                        else
                        {
                            GoToDirectory(new DirectoryInfo(path));
                        }
                    }
                }

                ImGui.TextWrapped(filename);

                ImGui.NextColumn();
            }

            ImGui.Columns(1);
            ImGui.EndChild();
        }

        return selected;
    }

    private void DrawMenuBar()
    {
        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu("Mode"))
            {
                if (ImGui.MenuItem("Tree", _isCellMode))
                {
                    _isCellMode = !_isCellMode;
                }

                if (ImGui.MenuItem("Cells", !_isCellMode))
                {
                    _isCellMode = !_isCellMode;
                }

                ImGui.EndMenu();
            }

            ImGui.EndMenuBar();
        }
    }
}