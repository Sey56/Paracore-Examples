# Paracore Examples

Official example scripts and finished tutorial scripts for [Paracore](https://sey56.github.io/paracore-help) — dynamic C# automation for Revit, written in VS Code and run from the desktop app.

**New here? Start with `00_Tutorials`.** It contains the finished script for every lesson in the
[Gallery tutorials](https://sey56.github.io/paracore-help/docs/tutorials/gallery-00-start-here),
using the exact script names the lessons teach. Follow a lesson and build the script yourself, or
load this source and compare your result with the finished version.

## Load a script source

Each numbered folder is a **script source** — a folder containing a `.paracore` file and one
sub-folder per script, each with a `Scripts\` folder of `.cs` files.

1. Clone or download this repository.
2. In the desktop app open the **hamburger menu → Sidebar → Local Sources → Load or Initialize Source**.
3. Select the numbered folder you want — for example **`00_Tutorials`** — not the repository root.
4. That folder becomes the active source. Open **Gallery** to see its scripts, then **Configure** and **Run**.

To switch to another category, load that folder the same way. **Edit Script** opens the script in
VS Code and adds the IntelliSense helper files if they are missing; Paracore itself runs only the
`.cs` files in `Scripts\`.

Full walkthrough: [Before You Start: Script Sources and VS Code](https://sey56.github.io/paracore-help/docs/tutorials/gallery-00-start-here).

## 📂 What's here

### 🎓 00_Tutorials — the current Gallery lessons

| Script | Lesson |
|---|---|
| `MyScript` | [Before You Start](https://sey56.github.io/paracore-help/docs/tutorials/gallery-00-start-here) — your first generated script |
| `HelloWall` | [1 · Hello Wall](https://sey56.github.io/paracore-help/docs/tutorials/gallery-01-hello-wall) — `Params` and `[Unit]` |
| `RoomInspector` | [2 · Parameter Palette](https://sey56.github.io/paracore-help/docs/tutorials/gallery-02-parameter-palette) — the full control palette |
| `DoorRenumberer` | [3 · Pick and Place](https://sey56.github.io/paracore-help/docs/tutorials/gallery-03-pick-and-place) — viewport selection |
| `LevelAuditor` | [4 · Modular Scripts](https://sey56.github.io/paracore-help/docs/tutorials/gallery-04-modular-scripts) — three files |
| `SmartOffset` | [5 · Units and Conversions](https://sey56.github.io/paracore-help/docs/tutorials/gallery-05-units-and-conversions) |
| `RoomFinishUpdater` | [6 · Dynamic UI](https://sey56.github.io/paracore-help/docs/tutorials/gallery-06-dynamic-ui) |
| `CsvRoomSync` | [7 · File I/O](https://sey56.github.io/paracore-help/docs/tutorials/gallery-07-file-io) — includes `sample-rooms.csv` |
| `StructuralFrame` | [8 · Multi-File Architecture](https://sey56.github.io/paracore-help/docs/tutorials/gallery-08-multi-file) — four files |

### 🗂 01–07 — the 4.0.0-era example library

These are older examples and starter templates. They still load and run, but they are **not** the
current tutorials. Start with `00_Tutorials` above.

| Source | Scripts |
|---|---|
| `01_Getting_Started` | `TwistingTower` |
| `02_Paracore_Fundamentals` | `01_HelloRevit`, `02_HelloWall`, `03_ParametricFloor`, `04_ElementSelection` |
| `03_RevitAPI_Fundamentals` | `01_ReadingTheModel`, `02_ParametersDeepDive`, `03_ConditionalLogic`, `04_BulkUpdates`, `05_TypesVsInstances`, `WallLengthAuditor` |
| `04_Advanced` | `CaseStandardizer`, `Furniture_Path_Placer`, `ParacoreTiler`, `RoomFinishUpdater`, `Wall_Geometry_Editor` |
| `05_Reference` | `MagicHydrationTest`, `Parameters_Reference` — plus `Magic_Hydration.md` and `Paracore_Parameter_Engine.md` |
| `06_Showcase_Demos` | `Api_Weather_RestSharp`, `DeleteAllWalls`, `Excel_Level_Export`, `Image_Asset_Processor`, `Math_Structural_Audit`, `ProjectDashboard`, `Safety_Lock_Demo` |
| `07_SentinelSource` | `CADImportGuard`, `LengthChecker`, `RoomAreaAudit`, `UnplacedRooms` — Sentinel (watchdog) starter templates |

## 📝 License

These scripts are provided under the MIT License. Use them as a factory for your own tools — copy,
modify and adapt them for commercial or private projects.
