using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Tutorial, Collection, Data
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Tutorial 01: Reading the Model (V3 Standards).
Learn how to collect and analyze elements project-wide.
Demonstrates using a Segmented UI to switch between categories.
*/

var p = new Params();

// 1. Efficiently collect all instances of the selected category
var collector = new FilteredElementCollector(Doc)
    .OfCategory(p.TargetCategory)
    .WhereElementIsNotElementType();

var allInstances = collector.ToList();

Println($"📊 Analyzing Category: {p.TargetCategory}");
Println($"🔍 Total instances in project: {allInstances.Count}");

// 2. Filter for instances in the active view
var inViewCount = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
    .OfCategory(p.TargetCategory)
    .WhereElementIsNotElementType()
    .GetElementCount();

Println($"👁️ Instances in active view: {inViewCount}");

// 3. Build a clean summary table of the first 100 items
if (allInstances.Count > 0)
{
    var summary = allInstances.Take(100).Select(e => new {
        e.Id,
        e.Name,
        Level = e.LevelId != ElementId.InvalidElementId ? Doc.GetElement(e.LevelId)?.Name : "N/A"
    }).ToList();

    Table(summary);
}
else
{
    Println("ℹ️ No elements found for this category in the current project.");
}

public class Params
{
    #region Settings
    /// <summary>Choose a category to analyze</summary>
    [Segmented]
    public BuiltInCategory TargetCategory { get; set; } = BuiltInCategory.OST_Walls;

    public List<BuiltInCategory> TargetCategory_Options => new List<BuiltInCategory>
    {
        BuiltInCategory.OST_Walls,
        BuiltInCategory.OST_Doors,
        BuiltInCategory.OST_Windows,
        BuiltInCategory.OST_Rooms,
        BuiltInCategory.OST_Floors,
        BuiltInCategory.OST_Columns
    };
    #endregion
}
