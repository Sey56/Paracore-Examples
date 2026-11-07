/*
DocumentType: Project
Categories: Tutorial, Annotation
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Renumbers door marks on selected walls with a prefix and sequential numbers.
Pick walls in the Revit viewport, choose a prefix, and run.
*/

Params p = new();

if (p.TargetWalls == null || p.TargetWalls.Count == 0)
{
    Println("Pick at least one wall in the parameters panel.");
    return;
}

// Collect doors hosted by the selected walls
var doors = GetElements<FamilyInstance>("Doors")
    .Where(d => d.Host?.Id != null && p.TargetWalls.Any(w => w.Id == d.Host.Id))
    .ToList();

if (doors.Count == 0)
{
    Println("No doors found on the selected walls.");
    return;
}

if (p.SkipDoorsWithMarks)
{
    doors = doors.Where(d => string.IsNullOrWhiteSpace(d.GetStr("Mark"))).ToList();
    Println($"Skipping doors that already have marks — {doors.Count} left.");
}

Transact("Renumber Doors", () =>
{
    int counter = p.StartNumber;
    foreach (var door in doors)
    {
        string newMark = $"{p.Prefix}-{counter:D3}";
        door.SetVal("Mark", newMark);
        counter++;
    }

    Println($"Renumbered {doors.Count} doors ({p.Prefix}-{p.StartNumber:D3} to {p.Prefix}-{counter - 1:D3}).");
});

public class Params
{
    #region Selection

    /// <summary>Pick walls in the Revit viewport</summary>
    [Select(SelectionType.Element)]
    [Required]
    public List<Wall>? TargetWalls { get; set; }

    #endregion

    #region Settings

    /// <summary>Prefix for the door marks (e.g. "A", "L1", "N")</summary>
    [Required]
    public string Prefix { get; set; } = "D";

    /// <summary>Starting number for the sequence</summary>
    public int StartNumber { get; set; } = 1;

    /// <summary>Skip doors that already have a Mark value</summary>
    public bool SkipDoorsWithMarks { get; set; } = false;

    #endregion
}
