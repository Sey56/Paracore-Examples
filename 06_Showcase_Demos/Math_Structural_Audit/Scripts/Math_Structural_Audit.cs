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
Enhanced with V3 Hydration for target selection.
*/

// 1. Setup
var p = new Params();

// 2. Data Extraction (Hydration!)
List<Wall> targetWalls;
if (p.AuditSelectedOnly && p.SelectedWalls != null && p.SelectedWalls.Count > 0)
{
    targetWalls = p.SelectedWalls;
}
else
{
    targetWalls = new FilteredElementCollector(Doc)
        .OfClass(typeof(Wall))
        .WhereElementIsNotElementType()
        .Cast<Wall>()
        .ToList();
}

if (!targetWalls.Any())
{
    Println("ℹ️ No walls found matching the audit criteria.");
}
else
{
    // Extract Lengths (Internal Units: Feet)
    var lengths = targetWalls.Select(w => w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble()).ToList();

    // 3. Statistical Analysis
    double meanFeet = lengths.Mean();
    double medianFeet = lengths.Median();
    double stdDevFeet = lengths.StandardDeviation();
    double maxFeet = lengths.Maximum();

    // 4. Output Findings (Convert to Meters for display)
    Println($"📊 --- Wall Length Audit (Total: {targetWalls.Count}) ---");
    Println($"Average Length: {UnitUtils.ConvertFromInternalUnits(meanFeet, UnitTypeId.Meters):F2} m");
    Println($"Median Length:  {UnitUtils.ConvertFromInternalUnits(medianFeet, UnitTypeId.Meters):F2} m");
    Println($"Standard Dev:   {UnitUtils.ConvertFromInternalUnits(stdDevFeet, UnitTypeId.Meters):F2} m");
    Println($"Longest Wall:   {UnitUtils.ConvertFromInternalUnits(maxFeet, UnitTypeId.Meters):F2} m");
    
    // Check against threshold (Already in internal units)
    if (stdDevFeet > p.Tolerance_m)
    {
        Println("\n⚠️ Warning: Significant variation in wall lengths detected.");
    }
}

public class Params
{
    #region Audit Scope
    /// Toggle: Audit ONLY selection or ALL walls in project
    public bool AuditSelectedOnly { get; set; } = false;

    /// V3 MAGIC: Select specific walls to audit
    [EnabledWhen(nameof(AuditSelectedOnly), "true")]
    public List<Wall> SelectedWalls { get; set; }
    #endregion

    #region Thresholds
    /// Variation threshold in meters
    [Unit("m")]
    public double Tolerance_m { get; set; } = 2.0;
    #endregion
}
