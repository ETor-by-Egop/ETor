# GUI for ETor

This project contains GUI for ETor app.

### Borrowed items

* Icons from `Hazel` game engine
* Startup logic from my video editor - SCVE

### Worth noting

#### UiValues.ComputedValue and UiValues.ComputedTableRow

UI value wrappers.

Main goal of them is not to compute values every frame (it's somewhat expensive).

So these classes solve the problem by caching their computed string-values ready for UI only when underlying object changes.

Greatly reduces string interpolation and UI maths.

#### FilePickers.OpenFilePicker

Custom-made "C# native" file picker in ImGui.

Also solves an issue with ImGui stack, which prevents popup opening from menu bar items.