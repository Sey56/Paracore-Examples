using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Tutorial, Parameters, Data
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Tutorial 02: Parameters Deep Dive (V3 Hydration).
Provides a comprehensive audit of all parameters for a selected Wall.
Demonstrates how to access Parameter definitions, values, and storage types.
*/

var p = new Params();

if (p.TargetWall == null) {
    Println("🚫 Please select a Wall in the UI to begin the deep dive.");
    return;
}

// 1. Log the identity of the hydrated object
Println($"🔍 Inspecting Wall: {p.TargetWall.Name} (Id: {p.TargetWall.Id})");

// 2. Collect ALL parameters from the selected wall
var paramData = new List<object>();

foreach (Parameter param in p.TargetWall.Parameters)
{
    // Capture the name, value (formatted or raw), and the storage type
    string paramName = param.Definition.Name;
    string paramValue = param.AsValueString() ?? param.AsString() ?? param.AsDouble().ToString() ?? param.AsInteger().ToString() ?? "(null)";
    string paramType = param.StorageType.ToString();

    paramData.Add(new
    {
        Id = p.TargetWall.Id,
        Name = paramName,
        Value = paramValue,
        Type = paramType
    });
}

// 3. Build a clean summary table of the schema
Println($"📊 Found {paramData.Count} parameters on the selected element.");
Table(paramData.OrderBy(x => x.GetType().GetProperty("Name").GetValue(x)).ToList());

public class Params
{
    #region Targets
    /// <summary>Pick a wall instance to dive into its parameters</summary>
    public Wall TargetWall { get; set; }
    #endregion
}