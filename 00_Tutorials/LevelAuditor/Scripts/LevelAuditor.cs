/*
DocumentType: Project
Categories: Tutorial, Data
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Audits level spacing — finds levels that are too close together,
reports duplicate elevations, and shows a table of all levels.
*/

Params p = new();

var allLevels = GetElements<Level>().OrderBy(l => l.Elevation).ToList();

if (allLevels.Count == 0)
{
    Println("No levels in the project.");
    return;
}

var rows = LevelAudit.BuildReport(allLevels, p.MinSpacingMm, p.BaseLevel);
var issues = rows.Where(r => r.Issue != "OK").ToList();

Println($"Audited {allLevels.Count} levels. {issues.Count} issue(s) found.");
Table(rows);

if (issues.Count > 0)
{
    Println("\nIssues:");
    foreach (var issue in issues)
        Println($"  {issue.Name}: {issue.Issue}");
}
