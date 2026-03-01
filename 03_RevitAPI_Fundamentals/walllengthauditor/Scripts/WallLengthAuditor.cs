
// 1. Instantiate Params
Params p = new();

// 2. Validate Inputs
if (p.TargetLevel == null)
{
    Println("Please select a valid Level.");
    return;
}

// 3. Query Walls on Selected Level
List<Wall> walls = [.. new FilteredElementCollector(Doc)
    .OfClass(typeof(Wall))
    .WhereElementIsNotElementType()
    .Cast<Wall>()
    .Where(w => w.LevelId == p.TargetLevel.Id)];

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
    .Where(x => x.LengthInternal < p.Threshold) // Compare in internal units (both in feet)
    .OrderBy(x => x.Length_mm)
    .ToList();

// Convert threshold back to mm for display
double thresholdDisplayMm = Math.Round(UnitUtils.ConvertFromInternalUnits(p.Threshold, UnitTypeId.Millimeters), 2);

// 5. Output Results
if (shortWallsData.Count != 0)
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

    /// Select the level to audit for short walls.
    [Required]
    public Level? TargetLevel { get; set; }

    /// <summary>
    /// Define the maximum length threshold in millimeters.
    /// Walls shorter than this value will be listed.
    /// </summary>
    [Unit("mm")]
    public double Threshold { get; set; } = 1000.0;

    #endregion
}
