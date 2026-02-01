/*
DocumentType: Project
DisplayName: Element Selection
Categories: Tutorial, Selection, Data
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description: 
Tutorial 3: Element Selection.
Teaches how to use the [Select] attribute to pick an element from the model
and modify its parameters (in this case, the Comments field).
*/

// Instantiate params
var p = new Params();

Transact("Update Wall Comment", () => {
    // Get the element from the selection reference
    if (p.TargetWall == null)
    {
        Println("🚫 No element selected. Please pick a wall first.");
        return;
    }

    var wall = Doc.GetElement(p.TargetWall) as Wall;

    if (wall == null) {
        Println("The selected element is not a wall!");
        return;
    }

    // Update the built-in parameter
    var param = wall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
    param.Set(p.Comment);

    Println($"Updated wall {wall.Id} with comment: {p.Comment}");
});

// --- 2. USER-DEFINED TYPES ---

public class Params
{
    /// Select a Wall in Revit
    [Select(SelectionType.Element)]
    [Mandatory]
    public Reference TargetWall { get; set; }

    /// Enter the new comment text
    public string Comment { get; set; } = "Updated by Paracore";
}
