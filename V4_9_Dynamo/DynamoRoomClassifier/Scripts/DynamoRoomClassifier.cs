/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 3 - Room review classifier. Pure .NET: no Revit API, no Paracore DSL.
A multi-branch rule that produces a label and a boolean flag from a room's
area and name. Shows conditional logic built from inline conditionals and
Contains checks — the form a Dynamo CodeBlock can actually run.

Use the gallery's Export Dynamo button, choose Graph, and get a standalone .dyn
whose two outputs appear on Watch nodes. In Dynamo, feed the outputs into native
Revit nodes: write the label to the room's Comments parameter, and use the flag
to drive a second action (e.g. highlight rooms that need review).

Exportable because:
- public class Params with public auto-properties (double Area, string RoomName)
- logic in top-level statements, pure .NET
- CodeBlock-safe syntax: no if statements, no casts, no bare declarations

The released 4.9.0 exporter copies these statements into a Dynamo CodeBlock.
DesignScript accepts assignments, inline conditionals and string methods, but
rejects C# 'if' statements and bare 'string label;' declarations. Each branch is
therefore one chained inline conditional, and every name gets its value at
declaration.
*/

Params p = new();

string name = p.RoomName.Trim().ToUpper();
double area = p.Area;

bool wet = name.Contains("BATH") || name.Contains("WC") || name.Contains("TOILET");
bool circ = name.Contains("CORRIDOR") || name.Contains("HALL") || name.Contains("PASSAGE");
bool vert = name.Contains("STAIR") || name.Contains("LIFT") || name.Contains("SHAFT");

string label = area < 5.0 ? "Too small - check" : wet ? "Wet area" : circ ? "Circulation" : vert ? "Vertical circulation" : area > 200.0 ? "Large space - verify fire code" : "Standard room";
bool needsReview = area < 5.0 ? true : wet ? area > 12.0 : circ ? area > 50.0 : vert ? true : area > 200.0 ? true : false;

public class Params
{
    /// Room area in square metres.
    public double Area { get; set; } = 18.5;

    /// Room name as it appears in the Revit model (e.g. "Office", "Corridor 01").
    public string RoomName { get; set; } = "Office";
}
