using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Tutorial, Modification, Parameters
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Tutorial 04: Bulk Parameter Updates (V3 Hydration).
Learn how to modify multiple elements efficiently using hydrated objects.
Demonstrates batch parameter updates for selected Walls.
*/

var p = new Params();

// 1. Resolve Targets (List<Wall> is automatically populated!)
if (p.TargetWalls == null || p.TargetWalls.Count == 0) {
    Println("ℹ️ No walls selected. Please select walls in the UI.");
    return;
}

int successCount = 0;

// 2. Perform Batch Update
Transact("Bulk Update Walls", () => {
    foreach (var wall in p.TargetWalls) {
        var param = wall.LookupParameter(p.ParameterName);
        if (param != null && !param.IsReadOnly) {
            param.Set(p.NewValue);
            successCount++;
        }
    }
});

Println($"✅ Successfully updated {successCount} walls with {p.ParameterName} = '{p.NewValue}'");

public class Params
{
    #region Targets
    /// <summary>Choose which walls to modify</summary>
    public List<Wall> TargetWalls { get; set; }
    #endregion

    #region Values
    /// <summary>Name of the parameter to change</summary>
    public string ParameterName { get; set; } = "Comments";

    /// <summary>New value to apply</summary>
    public string NewValue { get; set; } = "Updated via Paracore Batch";
    #endregion
}
