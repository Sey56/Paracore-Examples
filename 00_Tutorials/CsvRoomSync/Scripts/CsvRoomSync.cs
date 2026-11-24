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

// Keep the exact input file so a dry run or a later edit can be traced back.
if (!string.IsNullOrWhiteSpace(p.BackupFolder))
{
    Directory.CreateDirectory(p.BackupFolder);
    string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
    string backup = Path.Combine(p.BackupFolder, $"RoomFinishes-{stamp}.csv");
    File.Copy(p.InputCsvPath, backup, overwrite: false);
    Println($"Input CSV backed up to: {backup}");
}

// 2. Apply to Revit rooms
var rooms = GetElements<Room>().Where(r => r.Area > 0);
int matched = 0, notFound = 0;
var log = new List<FinishLogRow>();

Transact("Sync Room Finishes from CSV", () =>
{
    foreach (var kvp in updates)
    {
        var room = rooms.FirstOrDefault(r =>
            string.Equals(r.Name?.Trim(), kvp.Key, StringComparison.OrdinalIgnoreCase));

        if (room == null)
        {
            notFound++;
            log.Add(new FinishLogRow { Id = 0, Room = kvp.Key, Status = "Not Found", New = kvp.Value });
            continue;
        }

        var param = room.LookupParameter("Floor Finish");
        if (param == null || param.IsReadOnly)
        {
            log.Add(new FinishLogRow { Id = room.Id.Value, Room = room.Name, Status = "Read Only", New = kvp.Value });
            continue;
        }

        string old = param.AsString() ?? "(empty)";
        param.Set(kvp.Value);
        matched++;

        log.Add(new FinishLogRow { Id = room.Id.Value, Room = room.Name, Status = "Updated", Old = old, New = kvp.Value });
    }
});

Println($"Matched {matched}, not found {notFound}.");
Table(log);

// 3. Export summary (optional)
if (!string.IsNullOrWhiteSpace(p.OutputCsvPath))
{
    var csv = new List<string> { "Id,Room,Status,OldFinish,NewFinish" };
    foreach (var entry in log)
    {
        csv.Add($"\"{entry.Id}\",\"{entry.Room}\",\"{entry.Status}\",\"{entry.Old}\",\"{entry.New}\"");
    }

    File.WriteAllLines(p.OutputCsvPath, csv);
    Println($"Summary exported to: {p.OutputCsvPath}");
}

public class Params
{
    #region Input

    /// CSV file with columns: RoomName,FloorFinish
    [InputFile("csv")]
    [Required]
    public string InputCsvPath { get; set; } = "";

    /// Skip the first row (header)
    public bool HasHeaderRow { get; set; } = true;

    #endregion

    #region Output

    /// Optional: export a summary of changes
    [OutputFile("csv")]
    public string OutputCsvPath { get; set; } = "";

    /// Optional: folder to copy the input CSV into before applying it
    [FolderPath]
    public string BackupFolder { get; set; } = "";

    #endregion
}

public class FinishLogRow
{
    public long Id { get; set; }
    public string Room { get; set; } = "";
    public string Status { get; set; } = "";
    public string Old { get; set; } = "";
    public string New { get; set; } = "";
}
