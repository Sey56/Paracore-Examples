using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

/*
DocumentType: Any
Categories: Reference, Parameters, UI
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
A comprehensive "Kitchen Sink" reference for the Paracore Parameter Engine.
Demonstrates all supported C# types, attributes, and UI generation conventions.
*/

// 1. Instantiate Params at the top of your script
var p = new Params();

// 2. Use the inputs in your script logic
if (p.IsActive)
{
    // ... logic ...
}

// ---------------------------------------------------------
// PARAMS CLASS DEFINITION
// ---------------------------------------------------------
public class Params
{
    // -----------------------------------------------------
    #region 01. Basic Inputs (Inferred UI)
    /// <summary>
    /// Renders as an empty text box.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Renders as a text box pre-filled with "Default User".
    /// </summary>
    public string UserName { get; set; } = "Default User";

    /// <summary>
    /// Renders as an empty number input.
    /// </summary>
    public int NumberOfWalls { get; set; }

    /// <summary>Renders as a Toggle Switch.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Renders as a Slider + Number Input.
    /// Range: 1 to 10, Step: 1. Default: 5.
    /// </summary>
    [Range(1, 10, 1)]
    public int Counter { get; set; } = 5;

    #endregion

    // -----------------------------------------------------
    #region 02. Dropdowns & Multi-Select (Static Lists)
    /// <summary>
    /// Renders as a Dropdown because the type is 'string'.
    /// "UPPERCASE" is selected by default.
    /// Options are defined by CaseOption_Options.
    /// </summary>
    public string CaseOption { get; set; } = "UPPERCASE";
    public List<string> CaseOption_Options => ["UPPERCASE", "lowercase", "Camel Case"];

    /// <summary>
    /// Renders as Checkboxes because the type is 'List<string>'.
    /// "Revit" is checked by default.
    /// Options are defined by AppNames_Options.
    /// </summary>
    public List<string> AppNames { get; set; } = ["Revit"];
    public List<string> AppNames_Options => ["Revit", "Paracore", "Other"];

    /// <summary>
    /// Checkboxes with multiple items pre-selected.
    /// </summary>
    public List<string> PreferredTools { get; set; } = ["Revit", "Paracore"];
    public List<string> PreferredTools_Options => ["Revit", "Paracore", "Rhino", "Blender"];

    #endregion

    // -----------------------------------------------------
    #region 03. Custom Filtering (Revit API)
    /// <summary>
    /// Custom Filter: Only Wall Types named "Generic...".
    /// </summary>
    public string SpecialWallType { get; set; }
    public List<string> SpecialWallType_Options => new FilteredElementCollector(Doc)
            .OfClass(typeof(WallType))
            .Cast<WallType>()
            .Select(wt => wt.Name)
            .Where(name => name.Contains("Generic"))
            .ToList();

    /// <summary>
    /// Custom Filter: Distinct names of Wall Instances.
    /// </summary>
    public string WallInstanceName { get; set; }
    public List<string> WallInstanceName_Options => new FilteredElementCollector(Doc)
            .OfClass(typeof(Wall))
            .Cast<Wall>()
            .Select(w => w.Name)
            .Distinct()
            .ToList();

    #endregion

    // -----------------------------------------------------
    #region 04. Magic Extraction ([RevitElements])
    /// <summary>
    /// Automatically Populated Dropdown of all Wall Types.
    /// (Single selection because type is string)
    /// </summary>
    [RevitElements(TargetType = "WallType")]
    public string SelectOneWallType { get; set; }

    /// <summary>
    /// Automatically Populated Checkboxes of all Wall Types.
    /// (Multi-selection inferred because type is List<string>)
    /// </summary>
    [RevitElements(TargetType = "WallType")]
    public List<string> SelectMultipleWallTypes { get; set; }

    /// <summary>
    /// Dropdown of Door Types (FamilySymbols) filtered by Category.
    /// </summary>
    [RevitElements(TargetType = "FamilySymbol", Category = "Doors")]
    public string StandardDoor { get; set; }

    #endregion

    // -----------------------------------------------------
    #region 05. Native Revit Selection
    /// <summary>
    /// UI: "Pick Point" Button.
    /// Returns XYZ coordinate.
    /// </summary>
    [Select(SelectionType.Point)]
    public XYZ OriginPoint { get; set; }

    /// <summary>
    /// UI: "Select" Button.
    /// Returns Element ID (long).
    /// </summary>
    [Select(SelectionType.Element)]
    public long TargetElementId { get; set; }

    /// <summary>
    /// UI: "Pick Face" Button.
    /// Returns Reference object.
    /// </summary>
    [Select(SelectionType.Face)]
    public Reference SelectedFace { get; set; }

    #endregion

    // -----------------------------------------------------
    #region 06. File System
    /// <summary>
    /// File Input: Opens a native "Open File" dialog filtering for csv and xlsx files.
    /// </summary>
    [InputFile("csv,xlsx"), Required] 
    public string SourceFile { get; set; }

    /// <summary>
    /// Folder Input: Opens a native "Select Folder" dialog.
    /// </summary>
    [FolderPath]           
    public string ExportDir { get; set; }

    /// <summary>
    /// File Output: Opens a native "Save File" dialog.
    /// </summary>
    [OutputFile("json")]      
    public string OutputPath { get; set; }

    #endregion

    // -----------------------------------------------------
    #region 07. Validation & Logic
    /// <summary>
    /// Required Field. User cannot submit form if empty.
    /// Note: [Mandatory] is a built-in alias for [Required]. 
    /// Use [Mandatory] if [Required] conflicts with other library namespaces.
    /// </summary>
    [Mandatory]
    public string MandatoryField { get; set; }

    /// <summary>
    /// Use [Required] if you prefer the standard DataAnnotations naming.
    /// Functionally identical to [Mandatory].
    /// </summary>
    [Required]
    public string RequiredField { get; set; }

    /// <summary>
    /// Confirmation: The 'Run' button remains disabled until the user 
    /// types the exact string "DELETE" into this field. Best for destructive tasks.
    /// </summary>
    [Confirm("DELETE")] [Required]
    public string ConfirmText { get; set; }

    #endregion

    // -----------------------------------------------------
    #region 08. Conditional UI & Visibility
    /// <summary>Toggle visibility for advanced settings.</summary>
    public bool UseAdvancedSettings { get; set; } = false;

    /// <summary>
    /// This input is disabled (grayed out) unless UseAdvancedSettings is true.
    /// </summary>
    [EnabledWhen(nameof(UseAdvancedSettings), "true")]
    public string AdvancedConfig { get; set; }

    /// <summary>
    /// This input is completely hidden unless UseAdvancedSettings is true.
    /// </summary>
    public string SecretKey { get; set; }
    public bool SecretKey_Visible => UseAdvancedSettings;
 
    #endregion

    // -----------------------------------------------------
    #region 09. High-End UI Controls
    /// <summary>
    /// Stepper: +/- Buttons for precise numeric control.
    /// Good for iterations, counts, or small steps.
    /// </summary>
    [Stepper]
    public int Iterations { get; set; } = 10;

    /// <summary>
    /// Color Picker: Visual hex color swatch.
    /// Useful for element color overrides or material settings.
    /// </summary>
    [Color]
    public string WallColor { get; set; } = "#3B82F6";

    /// <summary>
    /// Segmented Toggle: Modern horizontal button group.
    /// Replaces dropdowns for parameters with 2-4 options.
    /// </summary>
    [Segmented]
    public string ViewOrientation { get; set; } = "Horizontal";
    public List<string> ViewOrientation_Options => ["Horizontal", "Vertical", "Axonometric"];

    #endregion

    // -----------------------------------------------------
    #region 10. Compute Action Button (Static vs Dynamic)
    /// <summary>
    /// CASE A: Static Options (No Compute Button).
    /// The dropdown is populated immediately because the list is hardcoded.
    /// </summary>
    public string StaticName { get; set; }
    public List<string> StaticName_Options => ["John", "Jack", "Jasper"];

    /// <summary>
    /// CASE B: Logic-Based Options (Triggers Compute Button).
    /// Even if the list is "static" code-wise, the presence of transformation 
    /// logic (like .OrderBy) triggers the 'Compute' button in the UI. 
    /// This ensures complex logic only runs in Revit when requested.
    /// </summary>
    public string DynamicName { get; set; }
    public List<string> DynamicName_Options 
    {
        get 
        {
            List<string> names = ["Paracore", "Dynamo", "Rhino"];
            return names.OrderBy(n => n).ToList(); 
        }
    }

    /// <summary>
    /// CASE C: [RevitElements] (Always Triggers Compute Button).
    /// Any parameter asking for live Revit data automatically shows the 
    /// Compute button to sync with the current document state.
    /// </summary>
    [RevitElements(TargetType = "Wall")]
    public string SelectedWall { get; set; }

    #endregion
}
