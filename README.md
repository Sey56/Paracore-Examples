# Paracore Examples Library 🚀

This repository contains the official example scripts, demonstrations, and tutorials for [Paracore](https://sey56.github.io/paracore-help) (Release 3.0+). These examples are designed to showcase the power of C# automation in Revit with zero boilerplate.

## 📂 Structure

This library is organized into specialized **Script Sources**. Each folder represents a category that can be loaded into Paracore.

### 🏁 01_Getting_Started
Quick-start scripts to verify your installation and learn the basics.
- **`TwistingTower.cs`**: The classic "Twisting House" verification script. Creates walls on all levels with a rotational offset.

### 🌟 02_Showcase_Demos
Advanced integrations demonstrating what's possible with professional libraries.
- **`ProjectDashboard.cs`**: Renders bar, pie, and line charts for a project overview.
- **`Api_Weather_RestSharp.cs`**: Connects to a live weather API to fetch site data using RestSharp.
- **`Excel_Level_Export.cs`**: High-performance, zero-COM export of project Levels to Excel.
- **`Image_Asset_Processor.cs`**: Production-ready image processing (Resizing, Grayscale) via ImageSharp.
- **`Math_Structural_Audit.cs`**: Statistical analysis of element dimensions using MathNet.Numerics.
- **`Safety_Lock_Demo.cs`**: Demonstrates the "Safety Lock" pattern for destructive operations.
- **`DeleteAllWalls.cs`**: A simple cleanup utility with confirmation logic.

### 🎓 03_Tutorials
Scripts that match the step-by-step guides in the [Official Documentation](https://sey56.github.io/paracore-help/docs/tutorials).
- **`01_HelloRevit.cs`**: Your first script greeting.
- **`02_HelloWall.cs`**: Creating a basic linear wall.
- **`03_ParametricFloor.cs`**: Driving geometry with user-defined Width/Depth.
- **`04_ElementSelection.cs`**: Learning the `[Select]` attribute for model interaction.
- **`05_MultiFile_HelloRevit/`**: Organizing complex logic across multiple files.

### 🛠️ 04_Advanced
Complex algorithms and production-ready automation tools.
- **`ParacoreTiler.cs`**: The v3.0 Flagship. An advanced computational floor pattern generator with gap-filling and cost estimation.
- **`Furniture_Path_Placer.cs`**: Distributes families along a selected curve path.
- **`Wall_Geometry_Editor.cs`**: Bulk adding Sweeps and Reveals to walls.
- **`RoomFinishUpdater.cs`**: Syncing Revit room data from external CSV files.
- **`CaseStandardizer.cs`**: Standardizing naming cases for annotations and views.

### 📖 05_Reference
Interactive "Cheat Sheets" for the Paracore engine.
- **`Parameters_Reference.cs`**: A "Kitchen Sink" of all UI control types and attributes.
- **`Paracore_Parameter_Engine.md`**: Detailed guide on how UI is inferred from C# code.
- **`Magic_Hydration.md`**: Explaining the v3.0.2 system for automatic Revit type resolution and category restriction.

## ⚡ How to Use

Paracore populates its Gallery from one source at a time. To use this library:

1.  **Clone or Download** this repository to your computer.
2.  **Add Sources**: In the Paracore **Sidebar**, click the **"+"** button and add the sub-folders (e.g., `01_Getting_Started`, `03_Tutorials`) one by one.
3.  **Switching Folders**: Once added, these folders will appear in the **Local Folders** dropdown in the Sidebar.
4.  **View Scripts**: Select a folder from the dropdown to instantly populate the Gallery with its scripts. 

> 💡 **Note**: The Gallery only shows scripts from the **currently selected** folder. To see a different category, simply change the selection in the dropdown.

## 📝 License

These scripts are provided under the MIT License. Feel free to copy, modify, and use them in your own commercial or private projects.