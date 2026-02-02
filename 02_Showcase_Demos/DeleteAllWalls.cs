using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

/*
DocumentType: Project
Categories: Architectural, Cleanup
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, Paracore.Addin

Description:
Deletes all wall elements in the active document. Useful for prototyping resets,
batch cleanup, or preparing a fresh layout canvas. Requires confirmation.
*/

var p = new Params();

// Filter Walls
FilteredElementCollector wallCollector = new FilteredElementCollector(Doc)
    .OfClass(typeof(Wall))
    .WhereElementIsNotElementType();

ICollection<ElementId> wallIds = [.. wallCollector.Select(w => w.Id)];
int wallCount = wallIds.Count;

if (wallCount == 0)
{
    Println("ℹ️ No walls found to delete.");
    return;
}

if (!p.ConfirmDeletion)
{
    Println("⚠️ Deletion skipped due to 'ConfirmDeletion = false'.");
    Println($"Found {wallCount} wall(s) that could be deleted.");
    return;
}

// Write operations inside a transaction
Transact("Delete All Walls", () =>
{
    Doc.Delete(wallIds);
});

// Print result
Println($"✅ Deleted {wallCount} wall(s).");

// V3 Simplified Parameters
public class Params
{
    /// <summary>Check this to delete all walls in the current document</summary>
    [Mandatory]
    public bool ConfirmDeletion { get; set; }

    /// <summary>
    ///  Type 'DELETE' to confirm deletion
    /// </summary>
    [Confirm("DELETE")] [Required]
    public string ConfirmText { get; set; }
}
