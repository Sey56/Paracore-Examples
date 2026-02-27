# Paracore Examples Library 🚀

This repository contains the official example scripts, demonstrations, and starter templates for [Paracore](https://sey56.github.io/paracore-help) (Release 4.0.0+). 

These examples showcase the "One Source of Truth" architecture of Paracore—demonstrating how you can build professional C# Revit automations with zero boilerplate and immediate execution.

## 📂 Structure

This library is organized into specialized **Script Sources**. In Paracore v4, you can load the root folder directly to populate your gallery with all categories at once.

### 🏁 01_Getting_Started
Quick-start scripts to verify your installation and learn the basics.
- **`TwistingTower.cs`**: The classic "Twisting House" verification script. Creates walls on all levels with a rotational offset.

### 🌟 02_Paracore_Fundamentals
Core concepts of the Paracore engine.
- **`Safety_Lock_Demo.cs`**: Demonstrates the "Safety Lock" pattern for destructive operations.
- **`Parameters_Reference.cs`**: A "Kitchen Sink" of all UI control types and attributes.

### 🏗️ 03_RevitAPI_Fundamentals
Practical examples of interacting with the Revit API inside the Paracore ecosystem.
- **`Element_Selection_Demo.cs`**: Various patterns for selecting and filtering Revit elements.
- **`Parametric_Wall_Creator.cs`**: Creating elements with dynamic parameter inputs.

### 🛠️ 04_Advanced
Complex algorithms and production-ready automation patterns.
- **`ParacoreTiler.cs`**: An advanced computational floor pattern generator with gap-filling and cost estimation.
- **`Furniture_Path_Placer.cs`**: Distributes families along a selected curve path.

### 📖 05_Reference
Cheat sheets and internal engine documentation.
- **`Globals_Cheat_Sheet.cs`**: Quick reference for all available global functions (Select, Table, Transact, etc.).

### 🎨 06_Showcase_Demos
Advanced integrations demonstrating what's possible with professional libraries and high-end UI patterns.
- **`ProjectDashboard.cs`**: Renders bar, pie, and line charts for a project overview.
- **`Excel_Level_Export.cs`**: High-performance export of project Levels to Excel.

### 🛡️ 07_SentinelSource (Starter Templates)
These are **Test Scripts** designed to demonstrate the Sentinel (Watchdog) background monitoring system.
- **`LengthChecker.cs`**: Generated with the Visual Query Builder; shows how to monitor and report on element properties.
- **`UnplacedRooms.cs`**: A pattern for monitoring model health and reporting warnings.
- **`CADImportGuard.cs`**: Professional guardrail template for monitoring model cleanliness.

## ⚡ How to Use

Paracore v4 introduces a "Zero-Manual-Management" workflow.

1.  **Clone or Download** this repository.
2.  **Add all Examples**: In the Paracore **Sidebar**, click the **Folder (Load)** icon and select this root repository folder. Paracore will recursively scan and load all categorized script sources at once.
3.  **Select Active Source**: Use the **Script Sources** dropdown in the Sidebar to switch between categories (e.g., `01_Getting_Started`).
4.  **Edit & Run**: Click any script in the Gallery. Click **"Edit Script"** to see the "One Source of Truth" project scaffolding in VS Code, or simply click **"Run"** to execute immediately.

## 📝 License

These scripts are provided under the MIT License. They are intended as a "factory" for your own tools—feel free to copy, modify, and use them as templates for your own commercial or private projects.