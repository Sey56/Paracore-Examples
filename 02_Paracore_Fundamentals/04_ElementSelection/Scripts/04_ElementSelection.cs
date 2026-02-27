/*
DocumentType: Project
Categories: Tutorial, Selection, Data
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description: 
Tutorial 4: Element Selection (V3 Hydration).
Teaches how to pick a Wall directly using its real Revit class.
No manual GetElement() or casting required!
*/

Params p = new();

if (p.TargetWall == null)
{
    Println("🚫 No wall selected in the UI.");
    return;
}

Transact("Update Wall Comment", () =>
{
    // We already have the real Wall object!
    Autodesk.Revit.DB.Parameter param = p.TargetWall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
    _ = (param?.Set(p.Comment));

    Println($"✅ Updated Wall {p.TargetWall.Id} with comment: {p.Comment}");
});

public class Params
{
    /// Select a Wall instance by picking
    [Select(SelectionType.Element), Mandatory]
    public Wall? TargetWall { get; set; }

    /// New comment text
    public string Comment { get; set; } = "Updated by Paracore Hydration";
}