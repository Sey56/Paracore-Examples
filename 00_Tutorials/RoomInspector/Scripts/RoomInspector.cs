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

IEnumerable<Room> rooms;

if (p.Scope == "All Rooms")
{
    rooms = GetElements<Room>().Where(r => r.Area > 0);
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
    rooms = rooms.Where(r => r.Area < p.MaxArea);
    Println($"Filtered to rooms under {p.MaxArea.OutputUnit("m2"):F1} m²");
}

Println($"Showing {rooms.Count()} room(s).");

var report = rooms.Select(r => new
{
    r.Id,
    r.Number,
    r.Name,
    Area_m2 = r.Area.OutputUnit("m2").ToString("F2"),
    Level = Doc.GetElement(r.LevelId)?.Name ?? "—"
}).OrderBy(r => r.Number);

Table(report);

public class Params
{
    #region Scope

    /// Choose what to inspect
    [Segmented]
    public string Scope { get; set; } = "All Rooms";
    public List<string> Scope_Options => ["All Rooms", "One Room"];

    /// Pick a specific room (only when Scope is One Room)
    public Room? SelectedRoom { get; set; }
    public bool SelectedRoom_Visible => Scope == "One Room";

    #endregion

    #region Filter

    /// Only show rooms below a certain area
    public bool FilterByArea { get; set; } = false;

    /// Maximum area in square metres
    [Unit("m2")]
    [Range(1, 500, 1)]
    public double MaxArea { get; set; } = 20.0;

    public bool MaxArea_Visible => FilterByArea;

    #endregion
}
