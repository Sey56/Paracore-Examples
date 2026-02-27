using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

/*
DocumentType: Project
Categories: Advanced, Architectural, Structural
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Edit wall geometry by adding sweeps or reveals to selected walls.
Uses V3 Hydration with a robust type-safe architecture.
*/

var p = new Params();

// 1. Resolve Walls
List<Wall> walls = new List<Wall>();
if (p.UseSelection)
{
    walls = p.SelectedWalls ?? new List<Wall>();
    if (walls.Count == 0)
    {
        Println("🚫 No walls selected. Please pick walls in the UI.");
        return;
    }
}
else
{
    walls = new FilteredElementCollector(Doc).OfClass(typeof(Wall)).Cast<Wall>().ToList();
}

// 2. Resolve Profile
FamilySymbol selectedProfile = p.ProfileName;
if (selectedProfile == null)
{
    Println("🚫 Please select a Profile.");
    return;
}

// 3. Execution using Automatic Type Management
int successCount = 0;
Autodesk.Revit.DB.BuiltInCategory typeCategory = p.Mode == "AddSweep" 
    ? BuiltInCategory.OST_Cornices 
    : BuiltInCategory.OST_Reveals;

Autodesk.Revit.DB.WallSweepType operationType = p.Mode == "AddSweep" 
    ? Autodesk.Revit.DB.WallSweepType.Sweep 
    : Autodesk.Revit.DB.WallSweepType.Reveal;

// Find a template type to duplicate
var templateType = new FilteredElementCollector(Doc)
    .WhereElementIsElementType()
    .OfCategory(typeCategory)
    .Cast<ElementType>()
    .FirstOrDefault();

if (templateType == null)
{
    Println($"🚫 No {p.Mode} types found in project to use as a template.");
    return;
}

Transact($"Wall Geometry - {p.Mode}", () =>
{
    // Resolve/Create the specific Type for this Profile
    ElementType typeToUse = null;
    string targetTypeName = $"{p.Mode}_{selectedProfile.Name}"; 

    // check existance
    var existing = new FilteredElementCollector(Doc)
        .WhereElementIsElementType()
        .OfCategory(typeCategory)
        .Cast<ElementType>()
        .FirstOrDefault(x => x.Name.Equals(targetTypeName, StringComparison.OrdinalIgnoreCase));

    if (existing != null)
    {
        typeToUse = existing;
        UpdateProfileParam(typeToUse, selectedProfile.Id);
    }
    else
    {
        try 
        {
            typeToUse = templateType.Duplicate(targetTypeName);
            UpdateProfileParam(typeToUse, selectedProfile.Id);
        }
        catch (Exception ex)
        {
            Println($"Error creating type '{targetTypeName}': {ex.Message}");
            return; // Skip if we can't make the type
        }
    }

    // Force regeneration to ensure type properties are committed before instance creation
    Doc.Regenerate();

    foreach (var wall in walls)
    {
        try
        {
            WallSweepInfo info = new WallSweepInfo(operationType, p.Vertical);
            info.WallSide = p.WallSide == "Exterior" ? WallSide.Exterior : WallSide.Interior;

            if (p.Vertical)
            {
                info.Distance = p.Offset;
            }
            else
            {
                double height = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                if (height < 0.001) height = wall.get_BoundingBox(null).Max.Z - wall.get_BoundingBox(null).Min.Z;
                
                info.Distance = height * p.Offset;
                info.DistanceMeasuredFrom = DistanceMeasuredFrom.Base;
            }

            WallSweep.Create(wall, typeToUse.Id, info);
            successCount++;
        }
        catch { }
    }
});

if (successCount > 0)
    Println($"✅ Successfully created {successCount} {p.Mode}s using profile '{selectedProfile.Name}'.");
else
    Println("⚠️ No geometry created. Ensure walls are standard walls.");

// Helper function to robustly set the Profile
void UpdateProfileParam(ElementType type, ElementId profileId)
{
    Parameter param = type.get_Parameter(BuiltInParameter.WALL_SWEEP_PROFILE_PARAM);
    
    // Fallback: Look by name "Profile" if BuiltIn fails
    if (param == null) param = type.LookupParameter("Profile");

    if (param != null && !param.IsReadOnly)
    {
        param.Set(profileId);
    }
}

public class Params
{
    #region Settings
    [Segmented]
    public string Mode { get; set; } = "AddSweep";
    public List<string> Mode_Options => ["AddSweep", "AddReveal"];

    public string WallSide { get; set; } = "Exterior";
    public List<string> WallSide_Options => ["Exterior", "Interior"];

    public bool Vertical { get; set; } = false;
    #endregion

    #region Geometry
    [Range(0.0, 1.0, 0.05)]
    public double Offset { get; set; } = 0.5;

    /// <summary>Select the Profile</summary>
    [RevitElements(Category = "Profiles")]
    public FamilySymbol ProfileName { get; set; }
    #endregion

    #region Selection
    public bool UseSelection { get; set; } = true;

    [EnabledWhen(nameof(UseSelection), "true")]
    public List<Wall> SelectedWalls { get; set; }
    #endregion
}
