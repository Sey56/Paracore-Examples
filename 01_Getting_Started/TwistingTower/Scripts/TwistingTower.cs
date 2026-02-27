using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;





/*
DocumentType: Project
Categories: Getting Started, Geometry
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Creates a "Twisting House" by generating walls on multiple levels with a rotational offset.
Demonstrates V3 dynamic hydration for Levels and WallTypes.
Note: Uses "Smart Height" logic—automatically calculates wall heights based on level elevations to prevent overlaps, falling back to WallHeight only for the top floor.
*/

Params p = new();

// 1. Validation
if (p.BaseLevel == null || p.WallType == null)
{
    Println("🚫 Please select a Base Level and a Wall Type in the UI.");
    return;
}

// 2. Resolve Levels
List<Level> allLevels = [.. new FilteredElementCollector(Doc)
    .OfClass(typeof(Level))
    .Cast<Level>()
    .OrderBy(l => l.Elevation)];

List<Level> upperLevels = [.. allLevels.Where(l => l.Elevation >= p.BaseLevel.Elevation)];

// 3. Geometry Prep
double halfWidth = p.Width / 2.0;
double halfDepth = p.Depth / 2.0;

// 4. Execution
Transact("Create Twisting House", () =>
{
    for (int i = 0; i < upperLevels.Count; i++)
    {
        Level level = upperLevels[i];
        Level? nextLevel = (i + 1 < upperLevels.Count) ? upperLevels[i + 1] : null;

        double angle = i * p.RotationStep * (Math.PI / 180.0);
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        List<XYZ> pts = [
            new(-halfWidth, -halfDepth, level.Elevation),
            new(-halfWidth, halfDepth, level.Elevation),
            new(halfWidth, halfDepth, level.Elevation),
            new(halfWidth, -halfDepth, level.Elevation)
        ];

        List<XYZ> rotatedPts = [.. pts.Select(q => new XYZ((q.X * cos) - (q.Y * sin), (q.X * sin) + (q.Y * cos), q.Z))];

        for (int j = 0; j < 4; j++)
        {
            Line line = Line.CreateBound(rotatedPts[j], rotatedPts[(j + 1) % 4]);
            Wall wall = Wall.Create(Doc, line, level.Id, false);
            wall.WallType = p.WallType;

            double height = (nextLevel != null) ? (nextLevel.Elevation - level.Elevation) : p.WallHeight;
            _ = (wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(height));
        }
    }
});

Println($"✅ Created twisting house starting from {p.BaseLevel.Name}.");

public class Params
{
    #region Geometry
    /// Rotation per floor (degrees)
    public double RotationStep { get; set; } = 5.0;

    /// Width in meters
    [Unit("m")]
    public double Width { get; set; } = 15.0;

    /// Depth in meters
    [Unit("m")]
    public double Depth { get; set; } = 10.0;

    /// Height if no next level exists (meters)
    [Unit("m")]
    public double WallHeight { get; set; } = 3.0;
    #endregion

    #region Revit Settings
    /// Starting level for construction
    [Required]
    public Level? BaseLevel { get; set; }

    /// Wall Type to use
    [Required]
    public WallType? WallType { get; set; }
    #endregion
}