using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Any
Categories: Reference, Parameters, UI
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
The definitive V3 reference for the Paracore "Hydration-First" Engine.
Demonstrates all UI controls including Professional IO, Color, and native Revit types.
*/

var p = new Params();

// 1. Use the real objects directly!
if (p.TargetLevel != null) Println($"✅ Level Selected: {p.TargetLevel.Name}");
if (p.FavoriteWall != null) Println($"✅ Wall Selected: {p.FavoriteWall.Id}");

// ---------------------------------------------------------
// PARAMS CLASS DEFINITION
// ---------------------------------------------------------
public class Params
{
    // -----------------------------------------------------
    #region 01. Automatic Hydration
    /// <summary>Returns the REAL Level object</summary>
    public Level TargetLevel { get; set; }

    /// <summary>Select multiple real Sheets (Multi-select inferred)</summary>
    public List<ViewSheet> TargetSheets { get; set; }
    #endregion

    // -----------------------------------------------------
    #region 02. High-End UI Controls
    /// <summary>Modern horizontal selection group</summary>
    [Segmented]
    public string OperationMode { get; set; } = "Preview";
    public List<string> OperationMode_Options => ["Preview", "Commit", "Audit"];

    /// <summary>Visual hex color swatch</summary>
    [Color]
    public string UIHighlightColor { get; set; } = "#3B82F6";

    /// <summary>+/- Buttons for precise numeric control</summary>
    [Stepper]
    public int Iterations { get; set; } = 10;
    #endregion

    // -----------------------------------------------------
    #region 03. File & Folder IO
    /// <summary>Opens native 'Open File' dialog (filters by .csv, .xlsx)</summary>
    [InputFile("csv, xlsx")]
    public string DataSource { get; set; }

    /// <summary>Opens native 'Save File' dialog</summary>
    [OutputFile("xlsx")]
    public string ExportPath { get; set; }

    /// <summary>Opens native 'Folder Browser' dialog</summary>
    [FolderPath]
    public string ProjectBackupFolder { get; set; }
    #endregion

    // -----------------------------------------------------
    #region 04. Professional Filtering
    /// <summary>Pick a specific wall instance</summary>
    [Select(SelectionType.Element)]
    public Wall FavoriteWall { get; set; }

    /// <summary>Filtered: Only Generic Wall Types</summary>
    public WallType CustomWallType { get; set; }
    public List<WallType> CustomWallType_Options => new FilteredElementCollector(Doc)
            .OfClass(typeof(WallType))
            .Cast<WallType>()
            .Where(wt => wt.Name.Contains("Generic"))
            .ToList();
    #endregion

    // -----------------------------------------------------
    #region 05. Conditional UI
    /// <summary>Toggle visibility for advanced settings</summary>
    public bool ShowAdvanced { get; set; } = false;

    /// <summary>Disabled unless ShowAdvanced is true</summary>
    [EnabledWhen(nameof(ShowAdvanced), "true")]
    public string ConfigKey { get; set; } = "Default-Key";

    /// <summary>Hidden unless ShowAdvanced is true</summary>
    public bool UnlockExperimental { get; set; } = false;
    public bool UnlockExperimental_Visible => ShowAdvanced;
    #endregion

    // -----------------------------------------------------
    #region 06. Units
    /// <summary>Auto-converted to Internal Units (Feet)</summary>
    [Unit("mm")]
    public double WallOffset { get; set; } = 250;
    #endregion
}