/*
DocumentType: Project
Categories: Tutorial, Rooms
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Audits rooms with optional area filtering and a confirmation gate.
Read-only — reports only, writes nothing.
*/

Params p = new();

// Resolve which rooms to audit
IEnumerable<Room> rooms = GetElements<Room>().Where(r => r.Area > 0);

Println($"Found {rooms.Count()} placed room(s).");

// Apply area filter if enabled
if (p.FilterByArea)
{
    rooms = rooms.Where(r => r.GetNum("Area", "m2") < p.MaxArea);
    Println($"After area filter (< {p.MaxArea:F1} m2): {rooms.Count()} room(s).");
}

// Optionally narrow to selected rooms
if (p.UseScope && p.Rooms != null && p.Rooms.Count > 0)
{
    var ids = new HashSet<long>(p.Rooms.Select(r => r.Id.Value));
    rooms = rooms.Where(r => ids.Contains(r.Id.Value));
    Println($"Narrowed to {rooms.Count()} selected room(s).");
}

if (!rooms.Any())
{
    Println("No rooms match the criteria.");
    return;
}

// Report
Table(rooms.Select(r => new
{
    r.Id,
    Number = r.GetStr("Number"),
    Name   = r.Name,
    Level  = r.GetStr("Level"),
    Area   = r.GetVal("Area", "m2", 1)
}));

Println($"Reported {rooms.Count()} room(s).");

public class Params
{
    #region Filter

    /// <summary>Only show rooms under a certain area</summary>
    public bool FilterByArea { get; set; } = false;

    /// <summary>Maximum area in square metres</summary>
    [Unit("m2")]
    public double MaxArea { get; set; } = 15.0;

    public bool MaxArea_Visible => FilterByArea;

    #endregion

    #region Scope

    /// <summary>Narrow the audit to specific rooms</summary>
    public bool UseScope { get; set; } = false;

    /// <summary>Pick rooms to include (enable the toggle first)</summary>
    [EnabledWhen(nameof(UseScope), "true")]
    public List<Room>? Rooms { get; set; }

    #endregion

    #region Confirmation

    /// <summary>Type REPORT to confirm</summary>
    [Confirm("REPORT")]
    public string? Confirm { get; set; }

    #endregion
}
