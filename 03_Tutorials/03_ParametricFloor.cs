/*
DocumentType: Project
Categories: Tutorial, Geometry
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description: 
Tutorial 3: Parametric Floor.
Demonstrates creating a rectangular floor based on simple Width/Depth inputs.
Introduces CurveLoops, Lists, and simple Transactions.
*/

// 1. Get user inputs from an instance of Params
var p = new Params();

// 2. Get the active level
var level = new FilteredElementCollector(Doc)
    .OfClass(typeof(Level))
    .Cast<Level>()
    .First();

// 3. Get a default Floor Type
var floorType = new FilteredElementCollector(Doc)
    .OfClass(typeof(FloorType))
    .Cast<FloorType>()
    .First();

// 4. Define the geometry
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

// 5. Modify the model inside a Transaction
Transact("Create Floor", () => {
    // We will use this overload to create the floor.
    // Refer to Revit API documentation for other overloads.
    Floor.Create(Doc, new List<CurveLoop> { loop }, floorType.Id, level.Id);

    Println($"Created floor: {p.Width} x {p.Depth}");
});

// --- 2. USER-DEFINED TYPES ---

public class Params
{
    /// Set the floor width
    [Unit("mm")]
    public double Width { get; set; } = 3000;

    /// Set the floor depth
    [Unit("mm")]
    public double Depth { get; set; } = 5000;
}
