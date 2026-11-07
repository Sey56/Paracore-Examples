/*
DocumentType: Project
Categories: Tutorial, Geometry
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Creates a single wall from user-selected parameters.
*/

Params p = new();

if (p.TargetLevel == null || p.WallType == null)
{
    Println("Select a Level and a Wall Type in the parameters panel.");
    return;
}

XYZ pt1 = new(0, 0, 0);
XYZ pt2 = new(p.WallLength, 0, 0);
Line wallLine = Line.CreateBound(pt1, pt2);

Transact("Create Wall", () =>
{
    Wall wall = Wall.Create(Doc, wallLine, p.TargetLevel.Id, false);
    wall.WallType = p.WallType;
    _ = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(p.WallHeight);

    Println($"Wall created: {wall.Id} on {p.TargetLevel.Name}");
});

public class Params
{
    /// <summary>Select the base level</summary>
    public Level? TargetLevel { get; set; }

    /// <summary>Select the wall type</summary>
    public WallType? WallType { get; set; }

    /// <summary>Length in metres</summary>
    [Unit("m")]
    public double WallLength { get; set; } = 5.0;

    /// <summary>Height in metres</summary>
    [Unit("m")]
    public double WallHeight { get; set; } = 3.0;
}
