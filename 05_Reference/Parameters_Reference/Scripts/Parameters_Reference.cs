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
    /// Returns the REAL Level object
    public Level TargetLevel { get; set; }

    /// Select multiple real Sheets (Multi-select inferred)
    public List<ViewSheet> TargetSheets { get; set; }
    #endregion

    // -----------------------------------------------------
    #region 02. High-End UI Controls
    /// Modern horizontal selection group
    [Segmented]
    public string OperationMode { get; set; } = "Preview";
    public List<string> OperationMode_Options => ["Preview", "Commit", "Audit"];

    /// Visual hex color swatch
    [Color]
    public string UIHighlightColor { get; set; } = "#3B82F6";

    /// +/- Buttons for precise numeric control
    [Stepper]
    public int Iterations { get; set; } = 10;
    #endregion

    // -----------------------------------------------------
    #region 03. File & Folder IO
    /// Opens native 'Open File' dialog (filters by .csv, .xlsx)
    [InputFile("csv, xlsx")]
    public string DataSource { get; set; }

    /// Opens native 'Save File' dialog
    [OutputFile("xlsx")]
    public string ExportPath { get; set; }

    /// Opens native 'Folder Browser' dialog
    [FolderPath]
    public string ProjectBackupFolder { get; set; }
    #endregion

    // -----------------------------------------------------
    #region 04. Professional Filtering
    /// Pick a specific wall instance
    [Select(SelectionType.Element)]
    public Wall FavoriteWall { get; set; }

    /// Filtered: Only Generic Wall Types
    public WallType CustomWallType { get; set; }
    public List<WallType> CustomWallType_Options => new FilteredElementCollector(Doc)
            .OfClass(typeof(WallType))
            .Cast<WallType>()
            .Where(wt => wt.Name.Contains("Generic"))
            .ToList();
    #endregion

    // -----------------------------------------------------
    #region 05. Conditional UI
    /// Toggle visibility for advanced settings
    public bool ShowAdvanced { get; set; } = false;

    /// Disabled unless ShowAdvanced is true
    [EnabledWhen(nameof(ShowAdvanced), "true")]
    public string ConfigKey { get; set; } = "Default-Key";

    /// Hidden unless ShowAdvanced is true
    public bool UnlockExperimental { get; set; } = false;
    public bool UnlockExperimental_Visible => ShowAdvanced;
    #endregion

    // -----------------------------------------------------
    #region 06. Units
    /// Auto-converted to Internal Units (Feet)
    [Unit("mm")]
    public double WallOffset { get; set; } = 250;
    #endregion


    #region 07. Enums

    public BuiltInCategory MyBuiltInCategory { get; set; }
    public BuiltInParameter MyBuiltInParameter { get; set; }

    #endregion

}