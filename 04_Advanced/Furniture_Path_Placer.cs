using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Advanced, Placement
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, Paracore.Addin

Description:
Places furniture instances along a selected model line path at regular intervals.
Demonstrates Reference selection, FamilySymbol lookup, and instance placement.
*/

var p = new Params();

// 1. Resolve the selected path line to a Curve
Element lineElement = Doc.GetElement(p.PathLine);
if (lineElement == null)
{
    Println("🚫 No line selected. Please pick a Model Line or Detail Line.");
    return;
}

Curve pathCurve = null;
if (lineElement is CurveElement curveEl)
{
    pathCurve = curveEl.GeometryCurve;
}
else
{
    Println($"🚫 Selected element ({lineElement.Category?.Name}) is not a valid line.");
    return;
}

// 2. Resolve the FamilySymbol
FamilySymbol furnitureSymbol = new FilteredElementCollector(Doc)
    .OfClass(typeof(FamilySymbol))
    .OfCategory(BuiltInCategory.OST_Furniture)
    .Cast<FamilySymbol>()
    .FirstOrDefault(fs => $"{fs.FamilyName}: {fs.Name}" == p.FurnitureName);

if (furnitureSymbol == null)
{
    Println($"🚫 Furniture type '{p.FurnitureName}' not found.");
    return;
}

// 3. Divide the curve into segments using equidistant sampling
double curveLength = pathCurve.Length;

// Calculate placement points based on selected mode
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
else // "Fixed Spacing" or Default
{
    int count = (int)Math.Floor(curveLength / p.Spacing) + 1;
    // Check if we overlap the end
    if ((count - 1) * p.Spacing > curveLength + 0.001) count--; 
    for (int i = 0; i < count; i++) targetDistances.Add(i * p.Spacing);
}

// Handle unbound/cyclic curve range
bool isUnbound = !pathCurve.IsBound;
double startParam = isUnbound ? 0 : pathCurve.GetEndParameter(0);
double endParam = isUnbound ? (pathCurve.IsCyclic ? Math.PI * 2 : 1) : pathCurve.GetEndParameter(1);

// BUILD SAMPLE TABLE (Distance -> Parameter)
// This ensures that even on Splines, objects are physically equidistant.
int sampleCount = 1000;
var samples = new List<(double dist, double param)>();
double cumDist = 0;
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

// 4. Place furniture at each point
Transact("Place Furniture on Path", () =>
{
    if (!furnitureSymbol.IsActive)
    {
        furnitureSymbol.Activate();
        Doc.Regenerate();
    }

    int placed = 0;
    foreach (double d in targetDistances)
    {
        // 1. Calculate final distance (handle Wrap/Flip)
        double targetDist = p.FlipDirection ? (curveLength - d) : d;
        
        // 2. Lookup Parameter by Distance (Interpolated)
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
        if (p.FlipDirection) tangent = -tangent; // Flip rotation too
        
        double angle = Math.Atan2(tangent.Y, tangent.X);

        // Create the instance
        FamilyInstance inst = Doc.Create.NewFamilyInstance(point, furnitureSymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

        // Rotate to align
        if (Math.Abs(angle) > 0.001)
        {
            Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
            ElementTransformUtils.RotateElement(Doc, inst.Id, axis, angle);
        }

        placed++;
    }

    Println($"✅ Placed {placed} '{furnitureSymbol.FamilyName}' instances using '{p.DistributionMode}' mode.");
});

// --- Parameters ---
public class Params
{
    #region 01. Placement Path
    /// <summary>Click a Model Line or Detail Line to use as the path</summary>
    [Select(SelectionType.Element)]
    [Required]
    public Reference PathLine { get; set; }

    /// <summary>Choose the furniture type to place</summary>
    [RevitElements(TargetType = "FamilySymbol", Category = "Furniture")]
    [Required]
    public string FurnitureName { get; set; }
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