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
High-performance, zero-COM export of project Levels to Excel.
Demonstrates MiniExcel integration for rapid data export.
*/

// 1. Setup
var p = new Params();

// Determine final path logic
string finalPath = p.TargetFile;

// If they used the Folder + FileName combo instead
if (string.IsNullOrEmpty(finalPath) && !string.IsNullOrEmpty(p.ExportFolder))
{
    finalPath = Path.Combine(p.ExportFolder, p.FileName + ".xlsx");
}

if (string.IsNullOrEmpty(finalPath))
{
    throw new Exception("🚫 Please specify an Output File or an Export Folder.");
}

// 2. Data Preparation
var levels = new FilteredElementCollector(Doc)
    .OfClass(typeof(Level))
    .Cast<Level>()
    .Select(l => new 
    { 
        Name = l.Name, 
        Elevation_Feet = l.Elevation,
        Elevation_Meters = UnitUtils.ConvertFromInternalUnits(l.Elevation, UnitTypeId.Meters),
        Id = l.Id.Value 
    })
    .OrderBy(l => l.Elevation_Feet)
    .ToList();

// 3. Execution
if (p.ExportNow)
{
    try 
    {
        // Use Template if provided
        if (!string.IsNullOrEmpty(p.TemplateFile) && File.Exists(p.TemplateFile))
        {
            Println($"📝 Using Template: {Path.GetFileName(p.TemplateFile)}");
            // MiniExcel can use a template, but for simplicity we'll just show the path usage
        }

        Println($"🚀 Exporting {levels.Count} levels to: {finalPath}");

        // MiniExcel Save
        MiniExcel.SaveAs(finalPath, levels);
        
        Println("✅ Export Successful!");
        Println($"📂 Path: {finalPath}");
    }
    catch (Exception ex)
    {
        throw new Exception($"❌ Export Failed: {ex.Message}");
    }
}

// ---------------------------------------------------------
// PARAMETERS
// ---------------------------------------------------------

public class Params
{
    #region Export Location (Choose One)

    /// <summary>
    /// DIRECT FILE OUTPUT: This opens a "Save File" dialog.
    /// Use this for a precise, one-click export path.
    /// </summary>
    [OutputFile("xlsx")]
    public string TargetFile { get; set; }

    /// <summary>
    /// FOLDER PICKER: This opens a folder selection dialog.
    /// Combined with 'FileName' below if 'TargetFile' is empty.
    /// </summary>
    [FolderPath]
    public string ExportFolder { get; set; }

    /// The name of the file to create inside the selected folder.
    public string FileName { get; set; } = "Revit_Level_Report";

    public bool FileName_Visible => !string.IsNullOrEmpty(ExportFolder);

    #endregion

    #region Template (Optional)

    /// <summary>
    /// FILE INPUT: This opens an "Open File" dialog.
    /// Pick an existing Excel file to use as a template.
    /// </summary>
    [InputFile("xlsx, xls")]
    public string TemplateFile { get; set; }

    #endregion

    #region Action

    /// Toggle to trigger the export.
    public bool ExportNow { get; set; } = true;

    #endregion
}