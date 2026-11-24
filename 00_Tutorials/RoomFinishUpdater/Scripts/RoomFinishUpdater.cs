/*
DocumentType: Project
Categories: Tutorial, Rooms
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Updates room floor finish parameters from a text input,
with optional area filtering and a confirmation gate.
*/

Params p = new();

// Resolve which rooms to update
List<Room> rooms;

if (p.UseCustomScope && p.CustomRooms != null && p.CustomRooms.Count > 0)
{
    rooms = p.CustomRooms;
    Println($"Using {rooms.Count} selected room(s).");
}
else
{
    rooms = GetElements<Room>().Where(r => r.Area > 0).ToList();
    Println($"Updating all {rooms.Count} placed room(s).");
}

// Apply area filter if enabled
if (p.FilterByArea)
{
    rooms = rooms.Where(r => r.Area < p.MaxArea).ToList();
    Println($"After area filter: {rooms.Count} room(s).");
}

if (rooms.Count == 0)
{
    Println("No rooms to update.");
    return;
}

// Apply the finish
int updated = 0;
var log = new List<object>();

Transact("Update Room Finishes", () =>
{
    foreach (var room in rooms)
    {
        var param = room.LookupParameter("Floor Finish");
        if (param == null || param.IsReadOnly) continue;

        string old = param.AsString() ?? "(empty)";
        param.Set(p.FloorFinish);
        updated++;

        log.Add(new { room.Id, room.Name, Old = old, New = p.FloorFinish });
    }
});

Println($"Updated {updated} of {rooms.Count} rooms.");
if (log.Count > 0) Table(log);

public class Params
{
    #region Scope

    /// Update only specific rooms instead of all
    public bool UseCustomScope { get; set; } = false;

    /// Pick rooms to include (enable the toggle first)
    [EnabledWhen(nameof(UseCustomScope), "true")]
    public List<Room>? CustomRooms { get; set; }

    #endregion

    #region Filter

    /// Only update rooms under a certain area
    public bool FilterByArea { get; set; } = false;

    /// Maximum area in square metres
    [Unit("m2")]
    public double MaxArea { get; set; } = 15.0;

    public bool MaxArea_Visible => FilterByArea;

    #endregion

    #region Value

    /// The floor finish to apply (e.g. "Tile", "Carpet", "Polished Concrete")
    public string FloorFinish { get; set; } = "Tile";

    #endregion

    #region Confirmation

    /// Type UPDATE to confirm — this overwrites existing values
    [Confirm("UPDATE")]
    public string? Confirm { get; set; }

    #endregion
}
