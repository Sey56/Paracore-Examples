using Autodesk.Revit.DB;
using System;
using System.Linq;

// 1. Instantiate Params
var p = new Params();

// 2. Validate Inputs
if (p.TargetLevel == null)
{
    Println("Please select a valid Level.");
    return;
}

// 3. Query Walls on Selected Level
var walls = new FilteredElementCollector(Doc)
    .OfClass(typeof(Wall))
    .WhereElementIsNotElementType()
    .Cast<Wall>()
    .Where(w => w.LevelId == p.TargetLevel.Id)
    .ToList();

// 4. Process and Filter Data
// Note: p.Threshold_mm is auto-converted to internal units (feet) by [Unit("mm")]
var shortWallsData = walls
    .Select(w =>
    {
        // Get length in internal units (feet)
        double lengthFt = w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH)?.AsDouble() ?? 0;
        // Convert to millimeters for display
        double lengthMm = UnitUtils.ConvertFromInternalUnits(lengthFt, UnitTypeId.Millimeters);
        
        return new
        {
            Id = w.Id.Value, // Use .Value for Revit 2024+ compatibility
            Type = w.Name,
            Length_mm = Math.Round(lengthMm, 2),
            LengthInternal = lengthFt, // Keep internal units for comparison
            Element = w // Keep reference for potential selection
        };
    })
    .Where(x => x.LengthInternal < p.Threshold_mm) // Compare in internal units (both in feet)
    .OrderBy(x => x.Length_mm)
    .ToList();

// Convert threshold back to mm for display
double thresholdDisplayMm = Math.Round(UnitUtils.ConvertFromInternalUnits(p.Threshold_mm, UnitTypeId.Millimeters), 2);

// 5. Output Results
if (shortWallsData.Any())
{
    Println($"Found {shortWallsData.Count} walls shorter than {thresholdDisplayMm} mm on level '{p.TargetLevel.Name}'.");
    
    // Render as a sortable table
    Table(shortWallsData);
}
else
{
    Println($"No walls found shorter than {thresholdDisplayMm} mm on level '{p.TargetLevel.Name}'.");
}

// ---------------------------------------------------------
// PARAMS (MUST BE LAST)
// ---------------------------------------------------------
public class Params
{
    #region Filters

    /// <summary>
    /// Select the level to audit for short walls.
    /// </summary>
    [Required]
    public Level TargetLevel { get; set; }

    /// <summary>
    /// Define the maximum length threshold in millimeters.
    /// Walls shorter than this value will be listed.
    /// </summary>
    [Unit("mm")] 
    public double Threshold_mm { get; set; } = 1000.0;

    #endregion
}
