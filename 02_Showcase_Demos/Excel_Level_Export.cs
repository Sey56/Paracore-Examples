using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MiniExcelLibs;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Showcase, Excel, Export
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, MiniExcel

Description:
High-performance export of Revit Levels to Excel.
Demonstrates V3 dynamic hydration for level selection.
*/

var p = new Params();

// 1. Resolve Output Path
string finalPath = p.TargetFile;
if (string.IsNullOrEmpty(finalPath)) throw new Exception("🚫 Please specify a target Excel file.");

// 2. Resolve Targets (Hydration)
List<Level> exportTargets;
if (p.ExportAllLevels)
{
    exportTargets = new FilteredElementCollector(Doc).OfClass(typeof(Level)).Cast<Level>().ToList();
}
else
{
    exportTargets = p.LevelsToExport ?? new List<Level>();
}

if (exportTargets.Count == 0)
{
    Println("ℹ️ No levels selected for export.");
    return;
}

// 3. Prepare Data
var data = exportTargets
    .Select(l => new 
    { 
        Name = l.Name, 
        Elevation_Internal = l.Elevation,
        Elevation_Meters = UnitUtils.ConvertFromInternalUnits(l.Elevation, UnitTypeId.Meters),
        UniqueId = l.UniqueId
    })
    .OrderBy(l => l.Elevation_Internal)
    .ToList();

// 4. Export
if (p.RunExport)
{
    try 
    {
        Println($"🚀 Exporting {data.Count} levels to: {finalPath}");
        MiniExcel.SaveAs(finalPath, data);
        Println("✅ Export Successful!");
    }
    catch (Exception ex) { throw new Exception($"❌ Export Failed: {ex.Message}"); }
}

public class Params
{
    #region Settings
    /// <summary>Toggle: Export EVERYTHING or just selection below</summary>
    public bool ExportAllLevels { get; set; } = true;

    /// <summary>V3 MAGIC: Pick specific levels to export</summary>
    [EnabledWhen(nameof(ExportAllLevels), "false")]
    public List<Level> LevelsToExport { get; set; }
    #endregion

    #region Output
    /// <summary>Select path for the Excel report</summary>
    [OutputFile("xlsx")]
    public string TargetFile { get; set; }

    /// <summary>Ready to export?</summary>
    public bool RunExport { get; set; } = true;
    #endregion
}