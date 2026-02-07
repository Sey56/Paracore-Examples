using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

/*
DocumentType: Project
Categories: Tutorial, Getting Started
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description: 
Tutorial 2: Hello Wall (V3 Hydration).
Demonstrates creating a linear wall using real Revit Types 
automatically hydrated by the engine.
*/

var p = new Params();

// 1. Setup the geometry
XYZ pt1 = new XYZ(0, 0, 0);
XYZ pt2 = new XYZ(p.WallLength, 0, 0);
Line wallLine = Line.CreateBound(pt1, pt2);

// 2. Execution (Direct use of real objects!)
if (p.TargetLevel == null || p.WallType == null)
{
    Println("🚫 Please ensure both a Level and a Wall Type are selected in the UI.");
    return;
}

Transact("Create Tutorial Wall", () =>
{
    Wall wall = Wall.Create(Doc, wallLine, p.TargetLevel.Id, false);
    wall.WallType = p.WallType;
    
    // Set the height (auto-converted from meters in UI)
    wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(p.WallHeight);

    Println($"✅ Wall created: {wall.Id} on {p.TargetLevel.Name}");
});

public class Params {
    /// <summary>Select the base level</summary>
    public Level TargetLevel { get; set; }

    /// <summary>Select the wall type</summary>
    public WallType WallType { get; set; }

    /// <summary>Length in meters</summary>
    [Unit("m")]
    public double WallLength { get; set; } = 5.0;

    /// <summary>Height in meters</summary>
    [Unit("m")]
    public double WallHeight { get; set; } = 3.0;
}