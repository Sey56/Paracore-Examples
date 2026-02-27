using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

/*
DocumentType: Project
Categories: Advanced, Data, Import/Export
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, Paracore.Addin

Description:
Demonstrates File Picker usage for both INPUT and OUTPUT to sync room finishes.
Reads data from CSV and applies updates to Revit room parameters.
Now enhanced with V3 Hydration for custom scope selection.
*/

var p = new Params();

// =================================================================================
// STEP 1: READ INPUT CSV FILE
// =================================================================================
if (string.IsNullOrWhiteSpace(p.InputCsvPath))
{
    Println("⚠️ No input CSV file selected. Please select a file and run again.");
    return;
}

if (!File.Exists(p.InputCsvPath))
{
    Println($"❌ File not found: {p.InputCsvPath}");
    return;
}

Println($"📂 Reading CSV file: {p.InputCsvPath}");

// Parse CSV
var roomUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
try
{
    var lines = File.ReadAllLines(p.InputCsvPath);
    if (lines.Length < 2)
    {
        Println("❌ CSV file is empty or has no data rows.");
        return;
    }

    // Skip header row (line 0)
    for (int i = 1; i < lines.Length; i++)
    {
        var line = lines[i].Trim();
        if (string.IsNullOrWhiteSpace(line)) continue;

        var parts = line.Split(',');
        if (parts.Length < 2) continue;

        string roomName = parts[0].Trim().Trim('"');
        string floorFinish = parts[1].Trim().Trim('"');

        if (!string.IsNullOrWhiteSpace(roomName) && !string.IsNullOrWhiteSpace(floorFinish))
        {
            roomUpdates[roomName] = floorFinish;
        }
    }

    Println($"✅ Parsed {roomUpdates.Count} room updates from CSV.");
}
catch (Exception ex)
{
    Println($"❌ Failed to read CSV: {ex.Message}");
    return;
}

// =================================================================================
// STEP 2: APPLY UPDATES TO REVIT ROOMS
// =================================================================================
Println("\n🔄 Applying updates to Revit rooms...");

// RESOLVE SCOPE: Either custom selection (Hydration) or entire project
List<Room> rooms;
if (p.UseCustomScope && p.CustomRooms != null && p.CustomRooms.Count > 0)
{
    rooms = p.CustomRooms;
}
else
{
    rooms = new FilteredElementCollector(Doc)
        .OfCategory(BuiltInCategory.OST_Rooms)
        .WhereElementIsNotElementType()
        .Cast<Room>()
        .Where(r => r.Area > 0)
        .ToList();
}

var updateLog = new List<Dictionary<string, object>>();
int successCount = 0;
int notFoundCount = 0;

Transact("Update Room Finishes", () =>
{
    foreach (var kvp in roomUpdates)
    {
        string targetRoomName = kvp.Key;
        string newFloorFinish = kvp.Value;

        // Find room (case-insensitive) from the resolved scope
        var room = rooms.FirstOrDefault(r => 
            string.Equals((r.Name ?? "").Trim(), targetRoomName, StringComparison.OrdinalIgnoreCase));

        if (room == null)
        {
            // Only log as "Not Found" if we are processing the entire project
            if (!p.UseCustomScope) {
                updateLog.Add(new Dictionary<string, object> {
                    { "RoomName", targetRoomName },
                    { "Status", "Not Found" },
                    { "OldFinish", "" },
                    { "NewFinish", newFloorFinish }
                });
                notFoundCount++;
            }
            continue;
        }

        // Get the Floor Finish parameter
        Autodesk.Revit.DB.Parameter floorFinishParam = room.LookupParameter("Floor Finish");
        if (floorFinishParam == null || floorFinishParam.IsReadOnly)
        {
            updateLog.Add(new Dictionary<string, object> {
                { "RoomName", room.Name },
                { "Status", "Parameter Error" },
                { "OldFinish", "" },
                { "NewFinish", newFloorFinish }
            });
            continue;
        }

        // Store old value
        string oldValue = floorFinishParam.AsString() ?? "(empty)";

        // Set new value
        floorFinishParam.Set(newFloorFinish);
        
        updateLog.Add(new Dictionary<string, object> {
            { "RoomName", room.Name },
            { "Status", "Updated" },
            { "OldFinish", oldValue },
            { "NewFinish", newFloorFinish }
        });
        successCount++;
    }
});

Println($"\n📊 Summary: {successCount} updated.");

// =================================================================================
// STEP 3: EXPORT SUMMARY (OPTIONAL)
// =================================================================================
if (!string.IsNullOrWhiteSpace(p.OutputCsvPath))
{
    try
    {
        var csvLines = new List<string> { "RoomName,Status,OldFinish,NewFinish" };
        
        foreach (var log in updateLog)
        {
            string rName = $"\"{log["RoomName"]}\"";
            string rStatus = $"\"{log["Status"]}\"";
            string rOld = $"\"{log["OldFinish"]}\"";
            string rNew = $"\"{log["NewFinish"]}\"";
            csvLines.Add($"{rName},{rStatus},{rOld},{rNew}");
        }

        File.WriteAllLines(p.OutputCsvPath, csvLines);
        Println($"💾 Exported summary to: {p.OutputCsvPath}");
    }
    catch (Exception ex)
    {
        Println($"❌ Failed to export summary: {ex.Message}");
    }
}

// Show results in table
Show("table", updateLog);
Println($"\n✅ Script completed. Updated {successCount} rooms.");


// =================================================================================
// PARAMETERS CLASS
// =================================================================================
public class Params
{
    #region Input
    /// <summary>CSV file with room names (RoomName,FloorFinish)</summary>
    [InputFile("csv"), Required]
    public string InputCsvPath { get; set; } = "";
    #endregion

    #region Scope Selection
    /// <summary>Toggle: Update only specifically selected rooms below</summary>
    public bool UseCustomScope { get; set; } = false;

    /// <summary>V3 MAGIC: Pick specific rooms to include in the update</summary>
    [EnabledWhen(nameof(UseCustomScope), "true")]
    public List<Room> CustomRooms { get; set; }
    #endregion

    #region Output
    /// <summary>Optional: Export summary of changes</summary>
    [OutputFile("csv")]
    public string OutputCsvPath { get; set; } = "";
    #endregion
}