using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

/*
DocumentType: Project
Categories: Tutorial, Logic, Filters
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Tutorial 03: Conditional Logic & Filters (V3 Hydration).
Learn how to use C# conditional statements and LINQ to filter the model.
Finds all rooms smaller than a user-defined threshold and displays them in an interactive table.
*/

var p = new Params();

// 1. Collect all Rooms in the active document
var allRooms = new FilteredElementCollector(Doc)
    .OfCategory(BuiltInCategory.OST_Rooms)
    .WhereElementIsNotElementType()
    .Cast<Room>()
    .ToList();

if (allRooms.Count == 0)
{
    Println("ℹ️ No rooms found in the current project.");
    return;
}

// 2. Apply Conditional Logic (Filter for rooms LESS than threshold)
// Note: p.ThresholdArea is already converted to Internal Units (sqft) by the engine.
var smallRooms = allRooms.Where(r => r.Area < p.ThresholdArea).ToList();

// 3. Report Findings
Println($"📊 Analysis Complete.");
Println($"🔍 Total Rooms: {allRooms.Count}");
Println($"🔴 Rooms < {UnitUtils.ConvertFromInternalUnits(p.ThresholdArea, UnitTypeId.SquareMeters):F2} m²: {smallRooms.Count}");

// 4. Display Interactive Table
if (smallRooms.Count > 0)
{
    var tableData = smallRooms.Select(r => new {
        Id = r.Id, // Enables "Select in Revit" on click
        Number = r.Number,
        Name = r.Name,
        Area_Internal = r.Area.ToString("F2") + " sqft",
        Area_Metric = UnitUtils.ConvertFromInternalUnits(r.Area, UnitTypeId.SquareMeters).ToString("F2") + " m²"
    }).OrderBy(x => x.Number).ToList();

    Table(tableData);
}
else
{
    Println("✅ No rooms found matching the small area criteria.");
}

public class Params
{
    #region Filters
    /// <summary>
    /// Area threshold for "Small Rooms". 
    /// Value entered in m² will be auto-converted to Revit internal units (sqft).
    /// </summary>
    [Unit("m2")]
    public double ThresholdArea { get; set; } = 10.0;
    #endregion
}