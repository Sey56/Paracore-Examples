using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

/*
DocumentType: Project
Categories: Advanced, Placement
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Places furniture instances along a selected model line path at regular intervals.
Demonstrates Reference selection, FamilySymbol lookup, and instance placement.
*/

var p = new Params();

// 1. Validation (Direct Hydration!)
if (p.PathLine == null || p.FurnitureName == null)
{
    Println("🚫 Please select a Path Line and a Furniture Type in the UI.");
    return;
}

Curve pathCurve = p.PathLine.GeometryCurve;
if (pathCurve == null)
{
    Println("🚫 Selected element does not have a valid geometry curve.");
    return;
}

// 2. Resolve Placement Distances based on Mode
double curveLength = pathCurve.Length;
var targetDistances = new List<double>();

if (p.DistributionMode == "Divide by Number")
{
    if (p.ItemCount < 2) targetDistances.Add(0);
    else 
    {
        double step = curveLength / (p.ItemCount - 1);
        for (int i = 0; i < p.ItemCount; i++) targetDistances.Add(i * step);
    }
}
else if (p.DistributionMode == "Centered")
{
    int count = (int)Math.Floor(curveLength / p.Spacing);
    if (count < 1) targetDistances.Add(curveLength / 2.0);
    else 
    {
        double usedLength = (count - 1) * p.Spacing;
        double startOffset = (curveLength - usedLength) / 2.0;
        for (int i = 0; i < count; i++) targetDistances.Add(startOffset + (i * p.Spacing));
    }
}
else // "Fixed Spacing"
{
    int count = (int)Math.Floor(curveLength / p.Spacing) + 1;
    if ((count - 1) * p.Spacing > curveLength + 0.001) count--; 
    for (int i = 0; i < count; i++) targetDistances.Add(i * p.Spacing);
}

// 3. Precise Sampling (Equidistant on Splines)
int sampleCount = 1000;
var samples = new List<(double dist, double param)>();
double cumDist = 0;
bool isBound = !pathCurve.IsBound;
double startParam = isBound ? 0 : pathCurve.GetEndParameter(0);
double endParam = isBound ? (pathCurve.IsCyclic ? Math.PI * 2 : 1) : pathCurve.GetEndParameter(1);

XYZ prevPt = pathCurve.Evaluate(startParam, false);
samples.Add((0, startParam));

for (int i = 1; i <= sampleCount; i++)
{
    double t = (double)i / sampleCount;
    double pVal = startParam + t * (endParam - startParam);
    XYZ currPt = pathCurve.Evaluate(pVal, false);
    cumDist += currPt.DistanceTo(prevPt);
    samples.Add((cumDist, pVal));
    prevPt = currPt;
}

// 4. Execution
Transact("Place Furniture on Path", () =>
{
    if (!p.FurnitureName.IsActive) p.FurnitureName.Activate();

    int placed = 0;
    foreach (double d in targetDistances)
    {
        double targetDist = p.FlipDirection ? (curveLength - d) : d;
        
        // Interpolated Parameter Lookup
        double param = samples.Last().param;
        if (targetDist <= 0) param = samples[0].param;
        else if (targetDist >= cumDist) param = samples.Last().param;
        else {
            for (int j = 0; j < samples.Count - 1; j++) {
                if (targetDist <= samples[j+1].dist) {
                    double ratio = (targetDist - samples[j].dist) / (samples[j+1].dist - samples[j].dist);
                    param = samples[j].param + ratio * (samples[j+1].param - samples[j].param);
                    break;
                }
            }
        }

        XYZ point = pathCurve.Evaluate(param, false); 
        Transform curveTransform = pathCurve.ComputeDerivatives(param, false);
        XYZ tangent = curveTransform.BasisX.Normalize();
        if (p.FlipDirection) tangent = -tangent;
        
        double angle = Math.Atan2(tangent.Y, tangent.X);

        // Create Instance
        FamilyInstance inst = Doc.Create.NewFamilyInstance(point, p.FurnitureName, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

        // Align to Path
        if (Math.Abs(angle) > 0.001)
        {
            Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
            ElementTransformUtils.RotateElement(Doc, inst.Id, axis, angle);
        }
        placed++;
    }

    Println($"✅ Successfully placed {placed} '{p.FurnitureName.Name}' instances along the selected path.");
});

public class Params
{
    #region 01. Placement Path
    /// <summary>Click a Model Line or Detail Line to use as the path</summary>
    [Select(SelectionType.Element), Required]
    public CurveElement PathLine { get; set; }

    /// <summary>Choose the furniture type to place</summary>
    [RevitElements(Category = "Furniture"), Required]
    public FamilySymbol FurnitureName { get; set; }
    #endregion

    #region 02. Distribution Logic
    /// <summary>Choose how items are spaced along the path</summary>
    public string DistributionMode { get; set; } = "Fixed Spacing";
    public List<string> DistributionMode_Options => ["Fixed Spacing", "Divide by Number", "Centered"];

    /// <summary>Distance between instances (feet)</summary>
    [Range(0.5, 100.0)]
    [Unit("ft")]
    public double Spacing { get; set; } = 5.0;
    public bool Spacing_Visible => DistributionMode != "Divide by Number";

    /// <summary>Total number of items to distribute from start to end</summary>
    [Range(2, 200)]
    public int ItemCount { get; set; } = 5;
    public bool ItemCount_Visible => DistributionMode == "Divide by Number";

    /// <summary>Toggle to start placement from the opposite end of the curve</summary>
    public bool FlipDirection { get; set; } = false;
    #endregion
}
