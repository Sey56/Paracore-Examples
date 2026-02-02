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
    // Extract Lengths (converted to meters for demo)
    var lengths = walls.Select(w => UnitUtils.ConvertFromInternalUnits(
        w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble(), 
        UnitTypeId.Meters)).ToList();

    // 3. Statistical Analysis (MathNet.Numerics)
    double mean = lengths.Mean();
    double median = lengths.Median();
    double stdDev = lengths.StandardDeviation();
    double max = lengths.Maximum();

    // 4. Output Findings
    Println($"--- Wall Length Audit (Total: {walls.Count}) ---");
    Println($"Average Length: {mean:F2} m");
    Println($"Median Length:  {median:  F2} m");
    Println($"Standard Dev:   {stdDev:F2} m");
    Println($"Longest Wall:   {max:F2} m");
    
    if (stdDev > p.Tolerance_m)
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