/*
DocumentType: Project
DisplayName: HelloWall
Categories: Tutorial, Getting Started
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description: 
Tutorial 1: Hello Wall.
This is the simple script referenced in the "Hello Revit" tutorial.
It creates a single linear wall based on coordinates.
*/

// Instantiate Parameters
var p = new Params();

// 1. Setup the geometry
// Create two points and a line for the wall location
XYZ pt1 = new XYZ(0, 0, 0);
XYZ pt2 = new XYZ(p.WallLength, 0, 0);
Line wallLine = Line.CreateBound(pt1, pt2);

// 2. Select the elements from Revit using the parameter names
Level level = new FilteredElementCollector(Doc)
    .OfClass(typeof(Level))
    .Cast<Level>()
    .FirstOrDefault(l => l.Name == p.LevelName);

WallType wallType = new FilteredElementCollector(Doc)
    .OfClass(typeof(WallType))
    .Cast<WallType>()
    .FirstOrDefault(w => w.Name == p.WallTypeName);

// 3. Create the wall inside a transaction
Transact("Create Tutorial Wall", () =>
{
    Wall wall = Wall.Create(Doc, wallLine, level.Id, false);
    wall.WallType = wallType;
    
    // Set the height parameter
    wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(p.WallHeight);

    Println($"✅ Wall created: {wall.Id}");
});

// --- 2. USER-DEFINED TYPES ---

public class Params {
    [RevitElements(TargetType = "Level")]
    public string LevelName { get; set; } = "Level 1";

    [RevitElements(TargetType = "WallType")]
    public string WallTypeName { get; set; } = "Generic - 200mm";

    /// <summary>Length in feet</summary>
    public double WallLength { get; set; } = 10.0;

    /// <summary>Height in feet</summary>
    public double WallHeight { get; set; } = 10.0;
}
