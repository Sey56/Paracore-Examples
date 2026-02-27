# Reference: Magic Element Hydration (v3.0.2)

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
    If the value is a standard string (like "Level 01"), the engine automatically searches the document for an element of the **target class** with that **name**. This allows for flexible workflows where you can simply set a parameter by name.

### 💎 The "Ultimate" Filter: The _Options Provider

While simple hydration is "magic," the `{ParameterName}_Options` provider is the **ultimate, high-level filtering mechanism** in Paracore. Without it, hydration simply lists all elements of the target type. With it, you have total control over what appears in the UI.

This works for both native Revit types (Hydration) and standard types (Strings).

```csharp
public class Params
{
    /// <summary>Advanced Filtered Hydration</summary>
    public WallType MyWallType { get; set; }

    /// <summary>The Ultimate Filter</summary>
    // We only want Wall Types that contain the word "Generic"
    public List<WallType> MyWallType_Options => new FilteredElementCollector(Doc)
        .OfClass(typeof(WallType))
        .Cast<WallType>()
        .Where(wt => wt.Name.Contains("Generic"))
        .ToList();
}
```
*The `_Options` provider is your tool for professional-grade, contextual filtering. It ensures the user only sees exactly what is relevant to your specific automation script.*

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
- **Breaking Change Warning**: Legacy scripts using `string` parameters for Revit elements are no longer supported and must be updated to use the target Revit Class directly.

## 🧱 System Families vs. Loadable Components

Magic Hydration handles different elements based on their Revit architecture:

### 1. Automatic Hydration (System Families)
For **System Families** and unique Revit types (e.g., `Wall`, `WallType`, `Level`, `Material`, `ViewSheet`), no attribute is needed. The type itself is specific enough for Paracore to know what to list or pick.

### 2. Category Restriction (Loadable Components)
For **Loadable Components** (`FamilyInstance` or `FamilySymbol`), the type is generic—a `FamilyInstance` could be a Door, a Desk, or a Column. To narrow the scope, you **must** use the `[RevitElements]` attribute to define the target category.

**Legacy Way (NO LONGER SUPPORTED):**
```csharp
// Error: String-based Revit element extraction is removed in v3.0.2
[RevitElements(TargetType = "FamilyInstance", Category = "Doors")]
public string MyDoor { get; set; }
```

**Modern v3.0.2 Way (Mandatory for Loadable Components):**
```csharp
// Use the native Revit type + Category restriction
[RevitElements(Category = "Doors")]
public FamilyInstance MyDoor { get; set; }

[RevitElements(Category = "Windows")]
public FamilySymbol WindowType { get; set; }
```

## ⚡ Reactive String Options (The Hybrid Way)

While "Magic" attribute-driven string extraction is gone, **Custom Reactive Options for strings** are still fully supported. This is useful when you want to pick a Name or a custom value from Revit but store it as a string in your logic.

```csharp
public class Params
{
    /// <summary>Step 1: Define a standard string</summary>
    public string WallName { get; set; }

    /// <summary>Step 2: Provide a custom string list</summary>
    public List<string> WallName_Options => new FilteredElementCollector(Doc)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .Select(w => w.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToList();
}
```
*In this scenario, Paracore generates a Searchable Dropdown. The user picks a name, and your logic receives it as a simple string. This remains an extraordinary tool for data-driven workflows.*

---
*Introduced in Paracore v3.0.2 - "The Hydration Update"*
