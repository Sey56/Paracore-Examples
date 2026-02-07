using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Tutorial, Geometry
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description: 
Tutorial 3: Parametric Floor (V3 Hydration).
Demonstrates creating a rectangular floor using real Revit objects 
hydrated directly into the Params class.
*/

var p = new Params();

if (p.TargetLevel == null || p.FloorType == null)
{
    Println("🚫 No Level or Floor Type selected.");
    return;
}

// 1. Define the geometry
var p1 = new XYZ(0, 0, 0);
var p2 = new XYZ(p.Width, 0, 0);
var p3 = new XYZ(p.Width, p.Depth, 0);
var p4 = new XYZ(0, p.Depth, 0);

var curves = new List<Curve> {
    Line.CreateBound(p1, p2),
    Line.CreateBound(p2, p3),
    Line.CreateBound(p3, p4),
    Line.CreateBound(p4, p1)
};
var loop = CurveLoop.Create(curves);

// 2. Modify the model
Transact("Create Floor", () => {
    Floor.Create(Doc, new List<CurveLoop> { loop }, p.FloorType.Id, p.TargetLevel.Id);
    
    // Convert back to meters for the console report
    double widthM = UnitUtils.ConvertFromInternalUnits(p.Width, UnitTypeId.Meters);
    double depthM = UnitUtils.ConvertFromInternalUnits(p.Depth, UnitTypeId.Meters);
    
    Println($"✅ Created {p.FloorType.Name} floor: {widthM:F2}m x {depthM:F2}m");
});

public class Params
{
    #region Standard Revit Selections
    /// <summary>Base level for the floor</summary>
    public Level TargetLevel { get; set; }

    /// <summary>Blueprint for the floor</summary>
    public FloorType FloorType { get; set; }
    #endregion

    #region Dimensions
    /// <summary>Width in meters</summary>
    [Unit("m")]
    public double Width { get; set; } = 3.0;

    /// <summary>Depth in meters</summary>
    [Unit("m")]
    public double Depth { get; set; } = 5.0;
    #endregion
}