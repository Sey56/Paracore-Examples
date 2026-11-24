using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

/*
DocumentType: Project
Categories: Architectural, Cleanup
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Deletes walls in the active document with professional filtering options. 
Demonstrates V3 hydration for sophisticated multi-selection.
*/

var p = new Params();

// 1. Determine targets based on selection
IEnumerable<Wall> targetWalls;

if (p.DeleteSelectionOnly && p.WallsToDelete != null && p.WallsToDelete.Count > 0)
{
    targetWalls = p.WallsToDelete;
}
else
{
    targetWalls = new FilteredElementCollector(Doc)
        .OfClass(typeof(Wall))
        .WhereElementIsNotElementType()
        .Cast<Wall>();
}

// 2. Perform Deletion
var targetIds = targetWalls.Select(w => w.Id).ToList();
int wallCount = targetIds.Count;

if (wallCount == 0)
{
    Println("ℹ️ No target walls found to delete.");
    return;
}

if (!p.ConfirmDeletion)
{
    Println($"⚠️ Deletion skipped. Found {wallCount} wall(s) that could be deleted.");
    return;
}

Transact("Delete Walls", () =>
{
    Doc.Delete(targetIds);
});

Println($"✅ Successfully deleted {wallCount} wall(s).");

public class Params
{
    #region Configuration
    /// Check this to authorize the deletion
    public bool ConfirmDeletion { get; set; } = false;

    /// Type 'DELETE' to confirm destructive action
    [Confirm("DELETE"), Required]
    public string ConfirmText { get; set; }
    #endregion

    #region Targets
    /// Toggle: Delete ONLY selected walls below, or ALL walls in project
    public bool DeleteSelectionOnly { get; set; } = false;

    /// V3 MAGIC: Select multiple walls to delete
    [EnabledWhen(nameof(DeleteSelectionOnly), "true")]
    public List<Wall> WallsToDelete { get; set; }
    #endregion
}
