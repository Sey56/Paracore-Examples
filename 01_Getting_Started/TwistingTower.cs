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

var p = new Params();

// 1. Validation
if (p.BaseLevel == null || p.WallType == null)
{
    Println("🚫 Please select a Base Level and a Wall Type in the UI.");
    return;
}

// 2. Resolve Levels
var allLevels = new FilteredElementCollector(Doc)
    .OfClass(typeof(Level))
    .Cast<Level>()
    .OrderBy(l => l.Elevation)
    .ToList();

var upperLevels = allLevels.Where(l => l.Elevation >= p.BaseLevel.Elevation).ToList();

// 3. Geometry Prep
double halfWidth = p.Width / 2.0;
double halfDepth = p.Depth / 2.0;

// 4. Execution
Transact("Create Twisting House", () =>
{
    for (int i = 0; i < upperLevels.Count; i++)
    {
        var level = upperLevels[i];
        Level nextLevel = (i + 1 < upperLevels.Count) ? upperLevels[i + 1] : null;

        double angle = (i * p.RotationStep) * (Math.PI / 180.0);
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        var pts = new List<XYZ> {
            new(-halfWidth, -halfDepth, level.Elevation),
            new(-halfWidth, halfDepth, level.Elevation),
            new(halfWidth, halfDepth, level.Elevation),
            new(halfWidth, -halfDepth, level.Elevation)
        };

        var rotatedPts = pts.Select(q => new XYZ(q.X * cos - q.Y * sin, q.X * sin + q.Y * cos, q.Z)).ToList();

        for (int j = 0; j < 4; j++)
        {
            var line = Line.CreateBound(rotatedPts[j], rotatedPts[(j + 1) % 4]);
            var wall = Wall.Create(Doc, line, level.Id, false);
            wall.WallType = p.WallType;

            double height = (nextLevel != null) ? (nextLevel.Elevation - level.Elevation) : p.WallHeight;
            wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(height);
        }
    }
});

Println($"✅ Created twisting house starting from {p.BaseLevel.Name}.");

public class Params
{
    #region Geometry
    /// <summary>Rotation per floor (degrees)</summary>
    public double RotationStep { get; set; } = 5.0;

    /// <summary>Width in meters</summary>
    [Unit("m")]
    public double Width { get; set; } = 15.0;

    /// <summary>Depth in meters</summary>
    [Unit("m")]
    public double Depth { get; set; } = 10.0;

    /// <summary>Height if no next level exists (meters)</summary>
    [Unit("m")]
    public double WallHeight { get; set; } = 3.0;
    #endregion

    #region Revit Settings
    /// <summary>Starting level for construction</summary>
    [Required]
    public Level BaseLevel { get; set; }

    /// <summary>Wall Type to use</summary>
    [Required]
    public WallType WallType { get; set; }
    #endregion
}