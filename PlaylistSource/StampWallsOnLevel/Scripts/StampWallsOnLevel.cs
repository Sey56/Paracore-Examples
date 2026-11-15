/*
DocumentType: Project
Categories: Tutorial, Audit
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin
Description:
Stamps the Comments parameter of every wall on one chosen level.
*/
Params p = new();

if (p.TargetLevel == null)
{
    Println("Pick a Target Level in this step's parameters.");
    return;
}

string levelName = p.TargetLevel.Name;
var walls = GetElements<Wall>()
    .Where(w => w.GetStr("Level") == levelName)
    .ToList();

Transact("Stamp walls", () =>
{
    foreach (var wall in walls)
        wall.SetVal("Comments", p.Stamp);
});

Println($"{p.TargetLevel.Name}: stamped {walls.Count} wall(s) with \"{p.Stamp}\".");

public class Params
{
    /// <summary>Level whose walls get stamped</summary>
    public Level? TargetLevel { get; set; }

    /// <summary>Text written into Comments</summary>
    public string Stamp { get; set; } = "Checked";
}
