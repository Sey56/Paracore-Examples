/*
DocumentType: Project
Categories: Tutorial, Geometry
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Adjusts wall base offsets or heights based on user input in millimetres
and metres. Demonstrates correct unit handling with [Unit] and OutputUnit.
*/

Params p = new();

if (p.TargetWallType == null)
{
    Println("Select a wall type.");
    return;
}

var walls = GetElements<Wall>()
    .Where(w => w.WallType.Id == p.TargetWallType.Id)
    .ToList();

if (walls.Count == 0)
{
    Println($"No walls of type '{p.TargetWallType.Name}' found.");
    return;
}

Println($"Found {walls.Count} walls of type '{p.TargetWallType.Name}'.");

Transact("Adjust Walls", () =>
{
    foreach (var wall in walls)
    {
        if (p.Mode == "Offset")
        {
            // p.OffsetMm is already in feet — pass it directly
            wall.SetNum("Base Offset", p.OffsetMm);
        }
        else if (p.Mode == "Set Height")
        {
            // p.TargetHeightM is already in feet too
            _ = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(p.TargetHeightM);
        }
    }
});

// Report back in the units the user thinks in
Println($"Mode: {p.Mode}");
if (p.Mode == "Offset")
    Println($"Offset applied: {OutputUnit(p.OffsetMm, "mm"):F0} mm");
else
    Println($"Height set to: {OutputUnit(p.TargetHeightM, "m"):F2} m");

public class Params
{
    #region Selection

    /// <summary>Select the wall type to modify</summary>
    public WallType? TargetWallType { get; set; }

    #endregion

    #region Operation

    /// <summary>Choose what to adjust</summary>
    [Segmented]
    public string Mode { get; set; } = "Offset";
    public List<string> Mode_Options => ["Offset", "Set Height"];

    #endregion

    #region Values

    /// <summary>Base offset in millimetres</summary>
    [Unit("mm")]
    [Range(-3000, 3000, 50)]
    public double OffsetMm { get; set; } = 0;

    /// <summary>Wall height in metres</summary>
    [Unit("m")]
    public double TargetHeightM { get; set; } = 3.0;

    public bool OffsetMm_Visible => Mode == "Offset";
    public bool TargetHeightM_Visible => Mode == "Set Height";

    #endregion
}
