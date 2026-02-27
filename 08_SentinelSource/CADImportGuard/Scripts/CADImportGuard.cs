using Autodesk.Revit.DB;
using System.Linq;
using System.Collections.Generic;

// Watchdog: Monitor for excessive Links or Imports
Watchdog(() => 
{
    var imports = new FilteredElementCollector(Doc)
        .OfClass(typeof(ImportInstance))
        .Cast<ImportInstance>()
        .Where(i => !i.IsLinked) // True imports (CAD)
        .ToList();

    string action = ExecutionGlobals.Get<string>("__sentinel_action__")?.ToLowerInvariant() ?? string.Empty;

    if (action == "select")
    {
        Select(imports);
    }
    else if (action == "isolate")
    {
        Transact("Isolate CAD Imports", () => Isolate(imports));
    }
    else if (action == "table" || string.IsNullOrEmpty(action))
    {
        // Output Results to Summary Tab
        if (imports.Count > 0)
        {
            var results = imports.Select(i => (object)new
            {
                Id = i.Id.Value,
                Name = i.Name,
                Category = i.Category?.Name ?? "CAD Import",
                Pinned = i.Pinned
            }).ToList();
            
            Table(results);
            Println($"Project contains {imports.Count} imported CAD files.");
        }

        // Background Reporting
        if (string.IsNullOrEmpty(action))
        {
            if (imports.Any())
            {
                WatchdogReport($"Project contains {imports.Count} imported CAD files.", "error", imports.Select(i => i.Id).ToList());
            }
            else
            {
                WatchdogReport("No CAD imports detected. Clean model!", "success");
            }
        }
    }
});
