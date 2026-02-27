using System.Globalization;

/*
DocumentType: Project
Categories: Advanced, Documentation, Management
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Standardizes naming cases for Rooms, Views, Sheets, and Text Notes.
Supports UPPERCASE, lowercase, and Title Case conversions.
Demonstrates V3 hydration for sophisticated multi-select element analysis.
*/

var p = new Params();

// 1. Resolve Targets
var targetCats = p.SelectedCategories;
var collector = new FilteredElementCollector(Doc).WhereElementIsNotElementType();

List<Element> elements = collector.ToElements()
    .Where(e => e.Category != null && targetCats.Contains(e.Category.Name))
    .ToList();

if (elements.Count == 0)
{
    Println("ℹ️ No elements found matching the criteria.");
    return;
}

// 2. Transformation Logic
string Transform(string input) 
{
    if (string.IsNullOrEmpty(input)) return input;
    return p.CaseType switch 
    {
        "UPPERCASE" => input.ToUpper(),
        "lowercase" => input.ToLower(),
        "Title Case" => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower()),
        _ => input
    };
}

Autodesk.Revit.DB.Parameter GetTargetParameter(Element e) 
{
    if (e is TextNote) return e.get_Parameter(BuiltInParameter.TEXT_TEXT);
    if (e is Room) return e.get_Parameter(BuiltInParameter.ROOM_NAME);
    if (e is View) return e.get_Parameter(BuiltInParameter.VIEW_NAME);
    if (e is ViewSheet) return e.get_Parameter(BuiltInParameter.SHEET_NAME);
    if (e is Level) return e.get_Parameter(BuiltInParameter.DATUM_TEXT);
    return e.LookupParameter("Name") ?? e.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME);
}

// 3. Execution
int changedCount = 0;
var changes = new List<object>();

Transact("Standardize Case", () => 
{
    foreach (var e in elements)
    {
        var param = GetTargetParameter(e);
        if (param == null || param.IsReadOnly) continue;

        string originalValue = param.AsString();
        if (string.IsNullOrEmpty(originalValue)) continue;

        string newValue = Transform(originalValue);

        if (originalValue != newValue)
        {
            try {
                param.Set(newValue);
                changedCount++;
                changes.Add(new { Id = e.Id, Old = originalValue, New = newValue });
            } catch { }
        }
    }
});

// 4. Report
if (changedCount > 0)
{
    Println($"✅ Successfully standardized {changedCount} elements to {p.CaseType}.");
    Table(changes);
}
else
{
    Println($"✅ No changes needed. All {elements.Count} elements are already in {p.CaseType}.");
}

public class Params
{
    #region Settings
    /// <summary>Choose the text case format</summary>
    [Segmented]
    public string CaseType { get; set; } = "UPPERCASE";
    public List<string> CaseType_Options => ["UPPERCASE", "lowercase", "Title Case"];

    /// <summary>Select categories to process</summary>
    public List<string> SelectedCategories { get; set; } = ["Rooms", "Views", "Sheets"];
    public List<string> SelectedCategories_Options => ["Rooms", "Views", "Sheets", "Text Notes", "Levels"];

    /// Type "APPLY" to apply project wide case change
    [Confirm("APPLY")]
    public string? Confirmation {get; set;}
    #endregion
}