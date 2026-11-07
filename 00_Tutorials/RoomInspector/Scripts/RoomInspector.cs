/*
DocumentType: Project
Categories: Tutorial, Data
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Inspects rooms — shows a table of all rooms or details one room,
with optional area filtering and boundary information.
*/

Params p = new();

List<Room> rooms;

if (p.Scope == "All Rooms")
{
    rooms = GetElements<Room>().Where(r => r.Area > 0).ToList();
}
else if (p.SelectedRoom != null)
{
    rooms = [p.SelectedRoom];
}
else
{
    Println("Select a room, or set Scope to All Rooms.");
    return;
}

if (p.FilterByArea)
{
    rooms = rooms.Where(r => r.Area < p.MaxArea).ToList();
    Println($"Filtered to rooms under {OutputUnit(p.MaxArea, "m2"):F1} m²");
}

Println($"Showing {rooms.Count} room(s).");

var report = rooms.Select(r => new
{
    r.Id,
    r.Number,
    r.Name,
    Area_m2 = OutputUnit(r.Area, "m2").ToString("F2"),
    Level = Doc.GetElement(r.LevelId)?.Name ?? "—"
}).OrderBy(r => r.Number).ToList();

Table(report);

public class Params
{
    #region Scope

    /// <summary>Choose what to inspect</summary>
    [Segmented]
    public string Scope { get; set; } = "All Rooms";
    public List<string> Scope_Options => ["All Rooms", "One Room"];

    /// <summary>Pick a specific room (only when Scope is One Room)</summary>
    public Room? SelectedRoom { get; set; }
    public bool SelectedRoom_Visible => Scope == "One Room";

    #endregion

    #region Filter

    /// <summary>Only show rooms below a certain area</summary>
    public bool FilterByArea { get; set; } = false;

    /// <summary>Maximum area in square metres</summary>
    [Unit("m2")]
    [Range(1, 500, 1)]
    public double MaxArea { get; set; } = 20.0;

    public bool MaxArea_Visible => FilterByArea;

    #endregion
}
