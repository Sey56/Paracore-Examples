using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Showcase, Math, Structural, Audit
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, MathNet.Numerics

Description:
Performs a statistical audit of wall lengths in the project using MathNet.Numerics.
Calculates Mean, Median, and Standard Deviation to identify geometric variations.
*/

// 1. Setup
var p = new Params();

// 2. Data Extraction
var walls = new FilteredElementCollector(Doc)
    .OfClass(typeof(Wall))
    .Cast<Wall>()
    .ToList();

if (!walls.Any())
{
    Println("No walls found in the document.");
}
else
{
    // Extract Lengths (Internal Units: Feet)
    var lengths = walls.Select(w => w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble()).ToList();

    // 3. Statistical Analysis (MathNet.Numerics) - Calculations stay in Internal Units
    double meanFeet = lengths.Mean();
    double medianFeet = lengths.Median();
    double stdDevFeet = lengths.StandardDeviation();
    double maxFeet = lengths.Maximum();

    // 4. Output Findings (Convert to Meters for display)
    double ftToM = 0.3048;
    Println($"--- Wall Length Audit (Total: {walls.Count}) ---");
    Println($"Average Length: {meanFeet * ftToM:F2} m");
    Println($"Median Length:  {medianFeet * ftToM:F2} m");
    Println($"Standard Dev:   {stdDevFeet * ftToM:F2} m");
    Println($"Longest Wall:   {maxFeet * ftToM:F2} m");
    
    // Compare in Internal Units (Both are Feet now)
    if (stdDevFeet > p.Tolerance_m)
    {
        Println("⚠️ Warning: Significant variation in wall lengths detected.");
    }
}

// ---------------------------------------------------------
// PARAMETERS
// ---------------------------------------------------------

public class Params
{
    #region Settings

    /// <summary>
    /// The variation threshold in meters.
    /// Triggers a warning if Standard Deviation exceeds this value.
    /// </summary>
    [Unit("m")]
    public double Tolerance_m { get; set; } = 2.0;

    #endregion
}