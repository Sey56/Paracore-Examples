using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Tutorial, Types, Instances
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Tutorial 05: Types vs Instances (V3 Hydration).
Understand the Revit data model by modifying all Instances of a selected Type.
Demonstrates how changing a "Blueprint" affects the entire model.
*/

var p = new Params();

// 1. Validation
if (p.TargetType == null) {
    Println("🚫 Please select a Wall Type in the UI.");
    return;
}

// 2. Find all instances of this specific type
var instances = new FilteredElementCollector(Doc)
    .OfClass(typeof(Wall))
    .WhereElementIsNotElementType()
    .Cast<Wall>()
    .Where(w => w.WallType.Id == p.TargetType.Id)
    .ToList();

Println($"📊 Selected Type: {p.TargetType.Name}");
Println($"🔍 Found {instances.Count} instances in the project.");

// 3. Perform a Bulk Update on all instances
if (instances.Count > 0) {
    Transact("Batch Update Type Instances", () => {
        foreach (var wall in instances) {
            wall.LookupParameter("Comments")?.Set(p.NewComment);
        }
    });
    
    Println($"✅ Successfully updated {instances.Count} walls with comment: '{p.NewComment}'");

    // 4. Show an interactive table of the instances
    var tableData = instances.Select(w => new {
        Id = w.Id, // Enables "Select in Revit"
        w.Name,
        Level = Doc.GetElement(w.LevelId)?.Name ?? "N/A"
    }).ToList();

    Table(tableData);
}
else {
    Println("ℹ️ No instances of this type currently exist in the model.");
}

public class Params
{
    #region Type Selection
    /// <summary>Choose the Wall Type (The Blueprint)</summary>
    public WallType TargetType { get; set; }
    #endregion

    #region Instance Data
    /// <summary>Comment to apply to all instances</summary>
    public string NewComment { get; set; } = "Updated by Type Batch";
    #endregion
}