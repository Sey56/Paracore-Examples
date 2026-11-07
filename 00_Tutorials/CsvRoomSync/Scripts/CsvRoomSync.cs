/*
DocumentType: Project
Categories: Tutorial, Import/Export
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Imports room finish data from a CSV file, applies it to Revit rooms,
and exports a summary of changes. Reads: RoomName,FloorFinish
*/

Params p = new();

// 1. Read the input CSV
if (string.IsNullOrWhiteSpace(p.InputCsvPath) || !File.Exists(p.InputCsvPath))
{
    Println("Select an input CSV file.");
    return;
}

var updates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
var lines = File.ReadAllLines(p.InputCsvPath);

int startRow = p.HasHeaderRow ? 1 : 0;
for (int i = startRow; i < lines.Length; i++)
{
    var parts = lines[i].Split(',');
    if (parts.Length < 2) continue;

    string name = parts[0].Trim().Trim('"');
    string finish = parts[1].Trim().Trim('"');
    if (!string.IsNullOrWhiteSpace(name))
        updates[name] = finish;
}

Println($"Parsed {updates.Count} room updates from CSV.");

// 2. Apply to Revit rooms
var rooms = GetElements<Room>().Where(r => r.Area > 0).ToList();
int matched = 0, notFound = 0;
var log = new List<object>();

Transact("Sync Room Finishes from CSV", () =>
{
    foreach (var kvp in updates)
    {
        var room = rooms.FirstOrDefault(r =>
            string.Equals(r.Name?.Trim(), kvp.Key, StringComparison.OrdinalIgnoreCase));

        if (room == null)
        {
            notFound++;
            log.Add(new { Room = kvp.Key, Status = "Not Found", New = kvp.Value });
            continue;
        }

        var param = room.LookupParameter("Floor Finish");
        if (param == null || param.IsReadOnly)
        {
            log.Add(new { Room = room.Name, Status = "Read Only", New = kvp.Value });
            continue;
        }

        string old = param.AsString() ?? "(empty)";
        param.Set(kvp.Value);
        matched++;

        log.Add(new { Room = room.Name, Status = "Updated", Old = old, New = kvp.Value });
    }
});

Println($"Matched {matched}, not found {notFound}.");
Table(log);

// 3. Export summary (optional)
if (!string.IsNullOrWhiteSpace(p.OutputCsvPath))
{
    var csv = new List<string> { "Room,Status,OldFinish,NewFinish" };
    foreach (var entry in log)
    {
        var type = entry.GetType();
        string room = type.GetProperty("Room")?.GetValue(entry)?.ToString() ?? "";
        string status = type.GetProperty("Status")?.GetValue(entry)?.ToString() ?? "";
        string oldVal = type.GetProperty("Old")?.GetValue(entry)?.ToString() ?? "";
        string newVal = type.GetProperty("New")?.GetValue(entry)?.ToString() ?? "";
        csv.Add($"\"{room}\",\"{status}\",\"{oldVal}\",\"{newVal}\"");
    }

    File.WriteAllLines(p.OutputCsvPath, csv);
    Println($"Summary exported to: {p.OutputCsvPath}");
}

public class Params
{
    #region Input

    /// <summary>CSV file with columns: RoomName,FloorFinish</summary>
    [InputFile("csv")]
    [Required]
    public string InputCsvPath { get; set; } = "";

    /// <summary>Skip the first row (header)</summary>
    public bool HasHeaderRow { get; set; } = true;

    #endregion

    #region Output

    /// <summary>Optional: export a summary of changes</summary>
    [OutputFile("csv")]
    public string OutputCsvPath { get; set; } = "";

    /// <summary>Optional: folder to back up the Revit project first</summary>
    [FolderPath]
    public string BackupFolder { get; set; } = "";

    #endregion
}
