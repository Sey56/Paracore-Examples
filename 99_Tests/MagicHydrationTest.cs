/*
DocumentType: Project
Categories: Stress Test, Hydration
Description: 
The ultra stress test for the Paracore V3 Engine. 
Tests dynamic hydration, MEP discovery, 
FamilyInstances vs FamilySymbols, Curves, and Filtered Providers.
*/

var p = new Params();

Println("--- 01. SPATIAL ELEMENTS (Hydration) ---");
if (p.TargetRoom != null) Println($"✅ Room: {p.TargetRoom.Name} [{p.TargetRoom.Number}]");
if (p.TargetArea != null) Println($"✅ Area: {p.TargetArea.Name} (Area: {p.TargetArea.Area:F2} sqft)");

Println("\n--- 02. MEP & COMPONENTS (IntelliSense Check) ---");
if (p.TargetDuct != null) Println($"✅ Duct Instance: {p.TargetDuct.Id}");
if (p.TargetDoor != null) Println($"✅ Door Instance: {p.TargetDoor.Name} (Mark: {p.TargetDoor.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString()})");
if (p.DoorType != null) Println($"✅ Door Type: {p.DoorType.FamilyName} - {p.DoorType.Name}");

Println("\n--- 03. INSTANCE VS TYPE ---");
if (p.WallInstance != null) Println($"✅ Wall Instance: {p.WallInstance.Id} (Type: {p.WallInstance.WallType.Name})");
if (p.WallType != null) Println($"✅ Wall Type: {p.WallType.Name}");

Println("\n--- 04. GEOMETRY & CURVES ---");
if (p.ModelLine != null) Println($"✅ Curve Element ID: {p.ModelLine.Id}");

Println("\n--- 05. SHEET & VIEWS ---");
if (p.MySheet != null) Println($"✅ Hydrated Sheet: {p.MySheet.SheetNumber} - {p.MySheet.Name}");

Println("\n--- 06. MULTI-SELECT HYDRATION ---");
if (p.Materials != null && p.Materials.Count > 0) {
    Println($"✅ Hydrated {p.Materials.Count} Materials:");
    foreach(var m in p.Materials) Println($"   - {m.Name}");
}

Println("\n--- 07. FILTERED PROVIDERS ---");
if (p.SmallRoom != null) 
    Println($"✅ Hydrated Filtered Room: {p.SmallRoom.Name} (Area: {p.SmallRoom.Area:F2} sqft)");

Println("\n--- 08. UNITS & VALIDATION ---");
Println($"Threshold: {p.AreaThreshold} internal units");

public class Params {
    #region Spatial & MEP
    /// <summary>Tests SpatialElement resilience</summary>
    public Room TargetRoom { get; set; }

    /// <summary>Tests Area scheme hydration</summary>
    public Area TargetArea { get; set; }

    /// <summary>Tests MEP instances (No flagging in VSCode!)</summary>
    public Duct TargetDuct { get; set; }
    #endregion

    #region Components (Category Filtering)
    /// <summary>Should list ONLY Doors</summary>
    [RevitElements(Category = "Doors")]
    public FamilyInstance TargetDoor { get; set; }

    /// <summary>Should list ONLY Door Types (Symbols)</summary>
    [RevitElements(Category = "Doors")]
    public FamilySymbol DoorType { get; set; }

    /// <summary>Tests CurveElement discovery</summary>
    public CurveElement ModelLine { get; set; }
    #endregion

    #region Types & Instances
    /// <summary>Should list actual Wall instances</summary>
    public Wall WallInstance { get; set; }

    /// <summary>Should list Wall types</summary>
    public WallType WallType { get; set; }

    /// <summary>Select a Sheet directly</summary>
    public ViewSheet MySheet { get; set; }
    #endregion

    #region Advanced & Filtered
    /// <summary>Multi-select hydration test</summary>
    public List<Material> Materials { get; set; }
    
    /// <summary>Filtered Room Picker (Only shows rooms with Area < Threshold)</summary>
    public Room SmallRoom { get; set; }
    // V3 Professional Filtered Provider
    public List<Room> SmallRoom_Options =>
        new FilteredElementCollector(Doc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .Where(r => r.Area < AreaThreshold) // Filter: Area < ThresholdRoomArea
            .ToList();
    #endregion

    #region Units
    /// <summary>Tests unit conversion logic</summary>
    [Unit("m2")]
    public double AreaThreshold { get; set; } = 10.0;
    #endregion
}
