# V4_9_Dynamo — export gallery scripts to Dynamo

> **NEW IN 4.9.0.** These scripts use the **Export Dynamo** button, which does not exist in
> earlier versions. Folder names in this repository carry a `V4_9_` prefix from this release
> onward, so you can tell at a glance which examples need a newer Paracore.

This is a **Paracore script source**: five scripts that show the two Dynamo export kinds and
three wiring examples. The exported logic is pure .NET, so the node or graph runs in Dynamo
without Paracore installed. `DynamoMethod` uses `Println` only when you Run it in Paracore; that
call is not part of the exported node. The other four scripts use no Revit API or Paracore helpers
at all.

## Load it

1. In the desktop app, open **hamburger menu → Sidebar → Local Sources → Load or Initialize Source**.
2. Select the **`V4_9_Dynamo`** folder (the one containing `.paracore`), not the repository root.
3. Open **Gallery**. You will see `DynamoMethod`, `DynamoGraph`, `DynamoDoorMark`,
   `DynamoFacadeLayout` and `DynamoRoomClassifier`.

## 1. `DynamoMethod` — export a Method node

1. Select `DynamoMethod` in the gallery and click **Export Dynamo**.
2. Choose **Methods**. Run a dry run first, then turn Write Files on and export.
3. In Dynamo 3.3, the node appears in the library as `ParacoreMath.AddTwoNums(x, y)`.
4. Feed it `10` and `6`; the result is `16`.

The source has a `public static` class (`ParacoreMath`) with a static method that takes scalar
`int` parameters. That is the shape Methods export needs.

## 2. `DynamoGraph` — export a Graph

1. Select `DynamoGraph` in the gallery and click **Export Dynamo**.
2. Choose **Graph**. Dry run first, then write the `.dyn`.
3. Open the generated `.dyn` in Dynamo. Its inputs come from the `Params` class, feed one
   CodeBlock, and the computed values appear on Watch nodes.

The source is pure .NET top-level statements plus a `public class Params`. A graph cannot call
Revit or Paracore helpers; those scripts stay in Paracore. If a script cannot be exported, the
error names the exact token and what to change.

:::note
**Export Dynamo** writes only Dynamo nodes today: Method nodes or a `.dyn` with input nodes,
one CodeBlock and Watch nodes. There are no Revit nodes in the export and no Paracore runtime
inside Dynamo. The exported input nodes carry short hints about which Revit parameter read to
wire into them. Add native Revit nodes in Dynamo if you want the exported logic to read or write
model data.
:::

## 3. `DynamoDoorMark` — string composition (Graph)

Joins six text inputs into a door mark `L02-R105-A`: level prefix, level number, room prefix,
room number, separator and suffix. Each piece of text is an ordinary StringInput node, so the
CodeBlock only concatenates values. Export as Graph, then add native Revit nodes yourself to
read door parameters into the inputs and write `doorMark` to each door's `Mark` parameter.

## 4. `DynamoFacadeLayout` — multi-output calculation (Graph)

Computes a simple panel grid from wall dimensions and a target module. Four outputs: columns,
rows, panel count and actual panel width. Shows one Paracore block producing several values.
Export as Graph, read wall `Length` and `Unconnected Height` with native Dynamo nodes, and build
a short summary for `Comments` with a Dynamo String Concat node.

## 5. `DynamoRoomClassifier` — conditional branching (Graph)

Compares a room area with a threshold and produces a label and a boolean flag. The threshold
and both labels are input nodes, so the CodeBlock contains only one comparison and one inline
conditional. Export as Graph, write `label` to `Comments`, and use `needsReview` to filter rooms
that need further review.

## Try the 4.9.0 IntelliSense in the REPL playground

Both snippets below get completion while you type. They work with Revit closed; execution still
needs Revit and the add-in. The first prints to the History tab, the second renders a table in
the Analytics tab.

```csharp
// Raw Revit API - walls shorter than 2 m, printed in the History tab
FilteredElementCollector col = new(Doc);
col.OfCategory(BuiltInCategory.OST_Walls);
col.WhereElementIsNotElementType();
List<Wall> walls = col.Cast<Wall>().ToList();
foreach (Wall wl in walls)
{
    double wallLength = wl.GetNum("Length", "mm");
    if (wallLength < 2000)
    {
        Println($"{wl.Id} {wl.Name} {wl.GetNum("Length", "mm", 0)}");
    }
}
```

```csharp
// Paracore DSL - walls shorter than 2 m, table in the Analytics tab
GetElements("Walls")
.WhereParam("Unconnected Height", "<", 2000, "mm")
.Select(sw => new {
    sw.Id,
    sw.Name,
    Unconnected_Height = sw.GetNum("Unconnected Height", "mm", 0),
    Comments = sw.GetVal("Comments")
})
.Table()
```

Notice the two completions introduced in 4.9.0: the category inside `GetElements("Walls")`, and
the category's real Revit parameter name inside `.WhereParam("Unconnected Height", …)`.
