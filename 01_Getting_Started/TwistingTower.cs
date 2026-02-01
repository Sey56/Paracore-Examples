using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;

/*
DocumentType: Project
DisplayName: Twisting Tower
Categories: Getting Started, Geometry
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Demonstrates how to create a "Twisting House" by generating walls on multiple levels with a rotational offset.        
A perfect introduction to working with Levels, Geometry, and Transactions in Paracore.

UsageExamples:
- "Create a twisting tower on all levels"
- "Generate a concept mass"
*/

// Initialize Parameters
var p = new Params();

// 1. Data Retrieval & Auto-Level Creation
var allLevels = new FilteredElementCollector(Doc)
    .OfClass(typeof(Level))
    .Cast<Level>()
    .OrderBy(l => l.Elevation)
    .ToList();

var baseLevel = allLevels.FirstOrDefault(l => l.Name == p.BaseLevelName) ?? allLevels.FirstOrDefault();

if (baseLevel == null)
{
    Println("🚫 No levels found in document.");
    return;
}

// Check if we need to create more levels for a good demo
int targetLevelCount = 10;
int currentCount = allLevels.Count(l => l.Elevation >= baseLevel.Elevation);

if (p.AutoCreateLevels && currentCount < targetLevelCount)
{
    Transact("Create Demo Levels", () => 
    {
        double currentElev = allLevels.Last().Elevation;
        for (int i = 0; i < (targetLevelCount - currentCount); i++)
        {
            currentElev += (p.WallHeightMeters / 0.3048); // Convert meters to feet
            Level.Create(Doc, currentElev);
        }
    });

    // Refresh collector
    allLevels = new FilteredElementCollector(Doc)
        .OfClass(typeof(Level))
        .Cast<Level>()
        .OrderBy(l => l.Elevation)
        .ToList();
}

// Filter levels from base upwards
var upperLevels = allLevels.Where(l => l.Elevation >= baseLevel.Elevation).ToList();
if (upperLevels.Count == 0)
{
    Println("🚫 No upper levels found.");
    return;
}

// Find Wall Type
var wallType = new FilteredElementCollector(Doc)
    .OfClass(typeof(WallType))
    .Cast<WallType>()
    .FirstOrDefault(wt => wt.Name == p.WallTypeName);

if (wallType == null)
{
    Println($"⚠️ Wall type '{p.WallTypeName}' not found.");
    return;
}

// 2. Geometry Preparation
double halfWidth = p.WidthMeters / 2.0;
double halfDepth = p.DepthMeters / 2.0;

// 3. Execution
Transact("Create Twisting House", () =>
{
    for (int i = 0; i < upperLevels.Count; i++)
    {
        var level = upperLevels[i];
        Level nextLevel = (i + 1 < upperLevels.Count) ? upperLevels[i + 1] : null;

        double angleDeg = i * p.RotationStepDegrees;
        double angle = angleDeg * (Math.PI / 180.0);
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        // Rectangle points centered at origin (relative to level elevation)
        var pts = new List<XYZ> {
            new(-halfWidth, -halfDepth, level.Elevation), // Bottom-Left
            new(-halfWidth, halfDepth, level.Elevation),  // Top-Left
            new(halfWidth, halfDepth, level.Elevation),   // Top-Right
            new(halfWidth, -halfDepth, level.Elevation)   // Bottom-Right
        };

        // Rotate points around origin in XY plane
        var rotatedPts = new List<XYZ>();
        foreach (var q in pts)
        {
            double x = q.X * cos - q.Y * sin;
            double y = q.X * sin + q.Y * cos;
            rotatedPts.Add(new XYZ(x, y, q.Z));
        }

        // Create 4 walls
        for (int j = 0; j < 4; j++)
        {
            var a = rotatedPts[j];
            var b = rotatedPts[(j + 1) % 4];

            var line = Line.CreateBound(a, b);
            if (line.Length <= 0.0026) continue; // Revit minimum line length check

            var wall = Wall.Create(Doc, line, level.Id, false);
            if (wall == null) continue;

            // Set Wall Type
            wall.WallType = wallType;

            // Set Height
            double heightFeet = (nextLevel != null)
                ? (nextLevel.Elevation - level.Elevation)
                : p.WallHeightMeters;

            var hParam = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
            if (hParam != null && !hParam.IsReadOnly)
            {
                hParam.Set(heightFeet);
            }
        }
    }
});

Println($"✅ Created twisting house on {upperLevels.Count} levels starting from: {baseLevel.Name}.");

public class Params
{
    #region Block Geometry
    /// <summary>Rotation step per level, in degrees.</summary>
    public double RotationStepDegrees { get; set; } = 5.0;

    /// <summary>Wall height for the top level (meters) if no next level exists.</summary>
    [Unit("m")]
    public double WallHeightMeters { get; set; } = 3.0;

    /// <summary>House width (meters, X axis).</summary>
    [Unit("m")]
    public double WidthMeters { get; set; } = 20.0;

    /// <summary>House depth (meters, Y axis).</summary>
    [Unit("m")]
    public double DepthMeters { get; set; } = 10.0;

    /// <summary>If true, automatically creates extra levels to ensure at least 10 exist.</summary>
    public bool AutoCreateLevels { get; set; } = true;
    #endregion

    #region Revit Settings
    /// <summary>Select the wall type to use for construction.</summary>
    [Mandatory]
    public string WallTypeName { get; set; }
    public List<string> WallTypeName_Options => new FilteredElementCollector(Doc)
        .OfClass(typeof(WallType))
        .Cast<WallType>()
        .Select(w => w.Name)
        .OrderBy(n => n)
        .ToList();

    /// <summary>Starting level for the first floor.</summary>
    [Mandatory]
    public string BaseLevelName { get; set; } = "Level 1";
    public List<string> BaseLevelName_Options => new FilteredElementCollector(Doc)
        .OfClass(typeof(Level))
        .Cast<Level>()
        .Select(l => l.Name)
        .OrderBy(n => n)
        .ToList();
    #endregion
}
