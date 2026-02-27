using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Linq;
using System.Collections.Generic;
using System;

// Watchdog: Warns if there are unplaced rooms in the model
Watchdog(() => 
{
    var unplacedRooms = new FilteredElementCollector(Doc)
        .OfCategory(BuiltInCategory.OST_Rooms)
        .WhereElementIsNotElementType()
        .Cast<Room>()
        .Where(r => r.Location == null || r.Area == 0)
        .ToList();

    string action = ExecutionGlobals.Get<string>("__sentinel_action__")?.ToLowerInvariant() ?? string.Empty;

    if (action == "select")
    {
        Select(unplacedRooms);
    }
    else if (action == "isolate")
    {
        Transact("Isolate Unplaced Rooms", () => Isolate(unplacedRooms));
    }
    else if (action == "table" || string.IsNullOrEmpty(action))
    {
        // Output Results to Summary Tab
        if (unplacedRooms.Count > 0)
        {
            var results = unplacedRooms.Select(r => (object)new
            {
                Id = r.Id.Value,
                r.Name,
                r.Number,
                Level = r.Level?.Name ?? "-",
                Area = Math.Round(UnitUtils.ConvertFromInternalUnits(r.Area, UnitTypeId.SquareMillimeters), 2)
            }).ToList();
            
            Table(results);
            Println($"Found {unplacedRooms.Count} unplaced rooms.");
        }

        // Background Reporting
        if (string.IsNullOrEmpty(action))
        {
            if (unplacedRooms.Any())
            {
                WatchdogReport($"Found {unplacedRooms.Count} unplaced rooms.", "warning", unplacedRooms.Select(r => r.Id).ToList());
            }
            else
            {
                WatchdogReport("All rooms are placed correctly.", "success");
            }
        }
    }
});
