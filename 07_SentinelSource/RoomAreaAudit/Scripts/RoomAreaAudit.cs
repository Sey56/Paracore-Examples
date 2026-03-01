/*
DocumentType: Project
Categories: Architectural
Author: Paracore VQB
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Lists all rooms whose area is smaller than a given threshold area in square meters on a selected level all levels if the Level parameter is null.
*/

// Watchdog: Generated Sentinel
// Generated from Visual Query Builder
Watchdog(() =>
{
    Params p = new();

    // __PARACORE_QUERY_DATA__{"category": "OST_Rooms", "rootGroup": {"type": "group", "combinator": "AND", "children": [{"type": "rule", "name": "Area", "storage_type": "Double", "operator": "<", "value": "10", "unit": "m2", "is_builtin": true, "builtin_id": -1006902, "builtin_name": "ROOM_AREA", "revit_element_type": "", "spec_type_id": "autodesk.spec.aec:area-2.0.0"}, {"type": "rule", "name": "Level", "storage_type": "String", "operator": "==", "value": "", "unit": null, "is_builtin": true, "builtin_id": -1007101, "builtin_name": "LEVEL_NAME", "revit_element_type": "", "spec_type_id": "autodesk.spec:spec.string-2.0.0"}]}, "selectedColumns": [], "scope": "project"}

    // 1. Filtering Logic (High-Performance Native Filter)
    FilteredElementCollector collector = new(Doc);
    _ = collector.OfCategory(BuiltInCategory.OST_Rooms);
    _ = collector.WhereElementIsNotElementType();
    List<ElementFilter> roomsFilters = [];
    if (p.Area != 0)
    {
        roomsFilters.Add(new ElementParameterFilter(new FilterDoubleRule(new ParameterValueProvider(new ElementId(BuiltInParameter.ROOM_AREA)), new FilterNumericLess(), p.Area, 1e-6)));
    }
    if (!string.IsNullOrEmpty(p.Level))
    {
        roomsFilters.Add(new ElementParameterFilter(new FilterStringRule(new ParameterValueProvider(new ElementId(BuiltInParameter.LEVEL_NAME)), new FilterStringEquals(), p.Level)));
    }
    ElementFilter? finalRoomsFilter = roomsFilters.Count > 0
        ? (roomsFilters.Count == 1 ? roomsFilters[0] : new LogicalAndFilter(roomsFilters))
        : null;
    if (finalRoomsFilter != null)
    {
        _ = collector.WherePasses(finalRoomsFilter);
    }
    List<Room> elements = [.. collector.Cast<Room>()];

    // --- Actions & Reporting ---
    string action = ExecutionGlobals.Get<string>("__sentinel_action__")?.ToLowerInvariant() ?? string.Empty;

    if (action == "select")
    {
        Select(elements);
    }
    else if (action == "isolate")
    {
        Transact("Isolate Sentinel Results", () => Isolate(elements));
    }
    else if (action == "table")
    {
        // 2. Output Results
        Println($"Query complete. Found {elements.Count} elements in category 'Rooms'.");
        if (elements.Count > 0)
        {
            List<object> results = [.. elements.Select(el =>
            {
                object AreaValue = Math.Round(UnitUtils.ConvertFromInternalUnits(el.get_Parameter(BuiltInParameter.ROOM_AREA)?.AsDouble() ?? 0, UnitTypeId.SquareMeters), 4);
                object LevelValue = el.get_Parameter(BuiltInParameter.LEVEL_NAME)?.AsString() ?? "-";

                return (object)new
                {
                    Id = el.Id.Value,
                    el.Name,
                    Area = AreaValue,
                    Level = LevelValue,
                };
            })];
            Table(results);
        }

        // 3. Interactive Actions (Removed)
    }
    else
    {
        // Background Reporting (or Manual Gallery Run)
        if (elements.Count > 0)
        {
            WatchdogReport($"Found {elements.Count} elements matching 'RoomAreaAudit'", "warning", elements.Select(el => el.Id).ToList());
        }
        else
        {
            WatchdogReport("No elements match 'RoomAreaAudit'", "success");
        }

        // If running manually in Gallery (no action), also show results
        if (string.IsNullOrEmpty(action))
        {
            // 2. Output Results
            Println($"Query complete. Found {elements.Count} elements in category 'Rooms'.");
            if (elements.Count > 0)
            {
                List<object> results = [.. elements.Select(el =>
                {
                    object AreaValue = Math.Round(UnitUtils.ConvertFromInternalUnits(el.get_Parameter(BuiltInParameter.ROOM_AREA)?.AsDouble() ?? 0, UnitTypeId.SquareMeters), 4);
                    object LevelValue = el.get_Parameter(BuiltInParameter.LEVEL_NAME)?.AsString() ?? "-";

                    return (object)new
                    {
                        Id = el.Id.Value,
                        el.Name,
                        Area = AreaValue,
                        Level = LevelValue,
                    };
                })];
                Table(results);
            }

            // 3. Interactive Actions (Removed)
        }
    }
});



public class Params
{
    #region Generated Parameters
    /// Filter value for Area
    [Unit("m2")]
    public double Area { get; set; } = 10;
    /// Filter value for Level
    public string Level { get; set; } = "";
    #endregion
}
