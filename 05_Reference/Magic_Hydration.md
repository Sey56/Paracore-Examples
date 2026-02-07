# Reference: Magic Element Hydration (V3)

The **Magic Element Hydration** system is a core feature introduced in Paracore v3.0.2. It eliminates the need for manual element collection and filtering in C# scripts by automatically resolving Revit objects directly from UI parameters.

## Overview
In **previous versions of Paracore scripting**, developers often had to accept element names or IDs as strings and then write repetitive "plumbing" code (like `FilteredElementCollector`) to find the actual element. Even in recent versions, you often had to use the `[RevitElements]` attribute to help the engine understand what you wanted.

With **Magic Hydration**, you can now use the Revit API type name (like `Level`, `WallType`, `Material`) **directly** as your property type. No attributes, no manual searching, and no boilerplate are required. The engine automatically populates the UI and hydrates the object before your script even runs.

## How it Works: The Three Pillars
When a script parameter is defined as a Revit type (e.g., `Level`, `WallType`, `Material`), the Paracore engine follows a robust three-tier resolution strategy:

1.  **Pillar 1: Precision (UniqueId)**
    If the incoming value is a valid Revit `UniqueId`, the engine performs a direct look-up. This is the most stable method and is used by the Paracore UI for internal selections.
2.  **Pillar 2: Compatibility (ElementId)**
    If the value is an integer or can be parsed as a long, it is treated as a classic Revit `ElementId`.
3.  **Pillar 3: Intuition (Name-Based Search)**
    If the value is a standard string (like "Level 01"), the engine automatically searches the document for an element of the **target class** with that **name**. This allows for "Agentic" workflows where you can simply set a parameter by name via chat or text.

### Custom Filtering (e.g. Placed Rooms)
If you need to filter elements (e.g. only rooms that are placed and have area > 10 sqm), you use the standard `{ParameterName}_Options` convention. In V3, your provider can return `List<Element>` directly!

```csharp
// 1. Script Logic at the Top
var p = new Params();
if (p.TargetRoom != null) 
    Println($"Operating on Room: {p.TargetRoom.Name} (Area: {p.TargetRoom.Area})");

// 2. User-Defined Types at the Bottom
public class Params
{
    /// <summary>Filtered Room Picker</summary>
    public Room TargetRoom { get; set; }

    // V3 Professional Pattern: 
    // We use OST_Rooms + WhereElementIsNotElementType for absolute reliability with spatial elements.
    public List<Room> TargetRoom_Options => 
        new FilteredElementCollector(Doc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .Where(r => r.Area > 107.6) // Filter for > 10 square meters
            .ToList();
}
```

## Usage Examples

### Single Element Resolution
Simply use the Revit Class in your `Params` block (defined at the bottom):

```csharp
// 1. Script Logic at the Top
var p = new Params();
Println($"Placing elements on: {p.TargetLevel.Name}");

// 2. User-Defined Types at the Bottom
public class Params
{
    public Level TargetLevel { get; set; }
    public WallType TargetType { get; set; }
}
```

### Multi-Select (Collections)
You can also hydrate lists of elements automatically:

```csharp
// 1. Script Logic at the Top
var p = new Params();
foreach (var mat in p.SelectedMaterials)
{
    Println($"Material found: {mat.Name}");
}

// 2. User-Defined Types at the Bottom
public class Params
{
    public List<Material> SelectedMaterials { get; set; }
}
```

## Key Benefits
- **Zero Boilerplate**: No more `FilteredElementCollector` calls just to find a Level or Type.
- **Strong Typing**: Enjoy full Intellisense and type safety inside your script logic.
- **Agent Friendly**: Allows the Paracore AI Agent to interact with scripts using natural names rather than cryptic IDs.
- **Backward Compatible**: Existing scripts using `string` parameters continue to work as usual.

## Simplified Attributes (New in V3)
For cases where you need to filter by Category (like Doors or Windows) but still want strong typing, you can now use the `[RevitElements]` attribute without specifying the redundant `TargetType`.

**Old Way:**
```csharp
[RevitElements(TargetType = "FamilyInstance", Category = "Doors")]
public FamilyInstance MyDoor { get; set; }
```

**New V3 Way:**
```csharp
[RevitElements(Category = "Doors")]
public FamilyInstance MyDoor { get; set; }
```

---
*Introduced in Paracore v3.0.2 - "The Hydration Update"*
