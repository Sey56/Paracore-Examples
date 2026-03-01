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

Params p = new();

if (p.TargetLevel == null || p.FloorType == null)
{
    Println("🚫 No Level or Floor Type selected.");
    return;
}

// 1. Define the geometry
XYZ p1 = new(0, 0, 0);
XYZ p2 = new(p.Width, 0, 0);
XYZ p3 = new(p.Width, p.Depth, 0);
XYZ p4 = new(0, p.Depth, 0);

List<Curve> curves =
[
    Line.CreateBound(p1, p2),
    Line.CreateBound(p2, p3),
    Line.CreateBound(p3, p4),
    Line.CreateBound(p4, p1)
];

CurveLoop loop = CurveLoop.Create(curves);

// 2. Modify the model
Transact("Create Floor", () =>
{
    _ = Floor.Create(Doc, [loop], p.FloorType.Id, p.TargetLevel.Id);

    // Convert back to meters for the console report
    double widthM = UnitUtils.ConvertFromInternalUnits(p.Width, UnitTypeId.Meters);
    double depthM = UnitUtils.ConvertFromInternalUnits(p.Depth, UnitTypeId.Meters);

    Println($"✅ Created {p.FloorType.Name} floor: {widthM:F2}m x {depthM:F2}m");
});

public class Params
{
    #region Standard Revit Selections
    /// <summary>Base level for the floor</summary>
    public Level? TargetLevel { get; set; }

    /// <summary>Blueprint for the floor</summary>
    public FloorType? FloorType { get; set; }
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