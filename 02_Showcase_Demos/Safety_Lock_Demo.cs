/*
DocumentType: Any
Categories: Showcase, Safety, Demo
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, Paracore.Addin

Description:
Demonstrates the "Safety Lock" pattern.
The 'Run' button remains disabled until you deliberately fill the required safety parameters.
Use this for destructive operations like Delete or Mass-Rename.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

var p = new Params();

// 1. Double-check the safety confirmation in code as well
if (p.ConfirmDeletion != "DELETE")
{
    throw new Exception("🚫 Deletion aborted. Safety code mismatch.");
}

// 2. Execution
var elementsToDelete = new FilteredElementCollector(Doc)
    .WhereElementIsNotElementType()
    .Where(x => x.Name.Contains(p.SearchTerm))
    .ToList();

if (elementsToDelete.Count == 0)
{
    Println($"No elements found matching '{p.SearchTerm}'.");
}
else
{
    Transact("Safety Lock Delete", () => {
        foreach (var el in elementsToDelete)
        {
            Doc.Delete(el.Id);
        }
    });

    Println($"✅ Successfully deleted {elementsToDelete.Count} elements.");
}

// ---------------------------------------------------------
// PARAMETERS
// ---------------------------------------------------------

public class Params
{
    #region 1. Target

    /// <summary>Search term for elements to be deleted.</summary>
    [Mandatory]
    public string SearchTerm { get; set; }

    #endregion

    #region 2. Safety Lock (CRITICAL)

    /// <summary>
    /// SAFETY PROMPT: You must TYPE the word 'DELETE' here to unlock the Run button.
    /// This prevents accidental execution of destructive operations.
    /// </summary>
    [Mandatory]
    [Confirm("DELETE")] 
    public string ConfirmDeletion { get; set; }

    /// <summary>
    /// MANDATORY CHECK: This must be checked to enable execution.
    /// </summary>
    [Mandatory]
    public bool ProceedWithCaution { get; set; }

    #endregion
}
