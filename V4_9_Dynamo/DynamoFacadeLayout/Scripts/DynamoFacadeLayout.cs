/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 2 - Facade panel layout. Pure .NET: no Revit API, no Paracore DSL.
Computes a simple panel grid from a wall's length and height plus a target
panel module. Four values appear on Watch nodes in the exported .dyn:
  cols        - number of columns along the wall
  rows        - number of rows up the wall
  panelCount  - total panels in the grid
  panelW      - actual panel width after fitting

In Dynamo, feed wall Length and Unconnected Height into the wall inputs, then
use the Watch outputs to write a summary to the wall's Comments parameter.

Exportable because:
- public class Params with public auto-properties (all double)
- logic in top-level statements, pure .NET arithmetic
- CodeBlock-safe syntax: no if statements, no casts, no string literals

The released 4.9.0 exporter copies these statements into a Dynamo CodeBlock.
Math.Floor and Math.Max are native Dynamo functions, so no C# cast is needed.
*/

double cols = Math.Max(1, Math.Floor(WallLength / TargetPanelW));
double rows = Math.Max(1, Math.Floor(WallHeight / TargetPanelH));
double panelCount = cols * rows;
double panelW = WallLength / cols;

public class Params
{
    /// Target panel height in mm.
    public double TargetPanelH { get; set; } = 900;

    /// Wall height in mm.
    public double WallHeight { get; set; } = 3600;

    /// Target panel width in mm.
    public double TargetPanelW { get; set; } = 1200;

    /// Wall length in mm.
    public double WallLength { get; set; } = 8000;
}
