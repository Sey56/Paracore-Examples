# 00_Tutorials — the Gallery tutorial scripts

This is a **Paracore script source**: one `.paracore` folder containing the finished scripts
from the [Gallery tutorials](https://sey56.github.io/paracore-help/docs/tutorials/gallery-00-start-here).
Load it once in the desktop app and every lesson's script is already there, ready to Run.
It is the answer key — build a script by hand while you follow a lesson, or load this source and
compare your result.

> The help site is the source of truth. If a lesson and a file here ever disagree, the lesson wins.

## Load it

1. Clone or download this repository.
2. In the desktop app, open the **hamburger menu → Sidebar → Local Sources → Load or Initialize Source**.
3. Select **`00_Tutorials`** (the folder that contains the `.paracore` file), not the repository root.
4. It becomes the active source. Open **Gallery** to see the nine scripts, then **Configure** and **Run**.

**Edit Script** on any card opens VS Code and adds the IntelliSense helper files (`.csproj`, `.sln`,
`Globals.cs`, `global.json`, `bin`, `obj`) if they are missing. Those files are for autocomplete only —
Paracore runs the `.cs` files in `Scripts\`. They are deliberately not committed here, so the source
looks exactly like the one you create yourself in [Before You Start](https://sey56.github.io/paracore-help/docs/tutorials/gallery-00-start-here).

## Lessons and scripts

| Lesson | Script | What it teaches |
|---|---|---|
| [Before You Start](https://sey56.github.io/paracore-help/docs/tutorials/gallery-00-start-here) | `MyScript` | Creating a source, NEW → Archetype, the generated starter |
| [1 · Hello Wall](https://sey56.github.io/paracore-help/docs/tutorials/gallery-01-hello-wall) | `HelloWall` | The `Params` class and `[Unit]` |
| [2 · Parameter Palette](https://sey56.github.io/paracore-help/docs/tutorials/gallery-02-parameter-palette) | `RoomInspector` | `[Segmented]`, `[Range]`, conditional visibility |
| [3 · Pick and Place](https://sey56.github.io/paracore-help/docs/tutorials/gallery-03-pick-and-place) | `DoorRenumberer` | `[Select]`, `[Required]` |
| [4 · Modular Scripts](https://sey56.github.io/paracore-help/docs/tutorials/gallery-04-modular-scripts) | `LevelAuditor` | Three files: entry, helper, `Params.cs` |
| [5 · Units and Conversions](https://sey56.github.io/paracore-help/docs/tutorials/gallery-05-units-and-conversions) | `SmartOffset` | `[Unit]` on input, `OutputUnit` on output |
| [6 · Dynamic UI](https://sey56.github.io/paracore-help/docs/tutorials/gallery-06-dynamic-ui) | `RoomFinishUpdater` | `[EnabledWhen]`, `[Confirm]` |
| [7 · File I/O](https://sey56.github.io/paracore-help/docs/tutorials/gallery-07-file-io) | `CsvRoomSync` | `[InputFile]`, `[OutputFile]`, `[FolderPath]` |
| [8 · Multi-File Architecture](https://sey56.github.io/paracore-help/docs/tutorials/gallery-08-multi-file) | `StructuralFrame` | Four files and real geometry |

`CsvRoomSync/sample-rooms.csv` is a ready-made input for Lesson 7 — pick it in the **InputCsvPath**
parameter and run.

## The other folders

`01_Getting_Started` through `07_SentinelSource` are the **4.0.0-era example library**. They still
load and run, but they are older examples, not the current tutorials. Start with `00_Tutorials`.
