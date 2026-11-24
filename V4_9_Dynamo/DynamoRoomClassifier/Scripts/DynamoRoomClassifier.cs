/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 3 - Room review classifier. Pure .NET: no Revit API, no Paracore DSL.
A multi-branch C# rule that produces a label and a boolean flag from a room's
area and name. The kind of conditional logic Dynamo CodeBlocks struggle with.

Use the gallery's Export Dynamo button, choose Graph, and get a standalone .dyn
whose two outputs appear on Watch nodes. In Dynamo, feed the outputs into native
Revit nodes: write the label to the room's Comments parameter, and use the flag
to drive a second action (e.g. highlight rooms that need review).

Exportable because:
- public class Params with public auto-properties (double Area, string RoomName)
- logic in top-level statements, pure .NET
- no Revit tokens, no Paracore helpers
*/

Params p = new();

string label;
bool needsReview;

string name = p.RoomName.Trim().ToUpperInvariant();
double area = p.Area;

if (area < 5.0)
{
    label = "Too small - check";
    needsReview = true;
}
else if (name.Contains("BATH") || name.Contains("WC") || name.Contains("TOILET"))
{
    label = "Wet area";
    needsReview = area > 12.0;
}
else if (name.Contains("CORRIDOR") || name.Contains("HALL") || name.Contains("PASSAGE"))
{
    label = "Circulation";
    needsReview = area > 50.0;
}
else if (name.Contains("STAIR") || name.Contains("LIFT") || name.Contains("SHAFT"))
{
    label = "Vertical circulation";
    needsReview = true;
}
else if (area > 200.0)
{
    label = "Large space - verify fire code";
    needsReview = true;
}
else
{
    label = "Standard room";
    needsReview = false;
}

public class Params
{
    /// Room area in square metres.
    public double Area { get; set; } = 18.5;

    /// Room name as it appears in the Revit model (e.g. "Office", "Corridor 01").
    public string RoomName { get; set; } = "Office";
}
