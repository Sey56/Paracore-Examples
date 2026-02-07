using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

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

var p = new Params();

if (p.TargetWall == null)
{
    Println("🚫 No wall selected in the UI.");
    return;
}

Transact("Update Wall Comment", () => {
    // We already have the real Wall object!
    var param = p.TargetWall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
    param?.Set(p.Comment);

    Println($"✅ Updated Wall {p.TargetWall.Id} with comment: {p.Comment}");
});

public class Params
{
    /// <summary>Select a Wall instance from the dropdown or by picking</summary>
    [Select(SelectionType.Element), Mandatory]
    public Wall TargetWall { get; set; }

    /// <summary>New comment text</summary>
    public string Comment { get; set; } = "Updated by Paracore Hydration";
}