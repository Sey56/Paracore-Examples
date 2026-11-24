/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paritacore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 2 - Facade panel layout. Pure .NET: no Revit API, no Paracore DSL.
Given a wall's length and height plus a target panel module, computes a panel
layout grid and a waste ratio. Shows a multi-output calculation that Dynamo
native nodes can then write back to the wall's Comments parameter.

Outputs (all appear on Watch nodes in the exported .dyn):
  panelCount   - total panels in the grid
  cols         - number of columns along the wall length
  rows         - number of rows along the wall height
  panelW       - actual panel width after fitting (mm)
  panelH       - actual panel height after fitting (mm)
  wastePct     - percentage of wall area not covered by panels

In Dynamo, wire wall.Length and wall.get_Parameter("Unconnected Height") into
the length and height inputs. Write the summary to Comments with
Element.SetParameterByName.

Exportable because:
- public class Params with public auto-properties (all double)
- logic in top-level statements, pure .NET arithmetic
- no Revit tokens, no Paracore helpers
*/

Params p = new();

double usableLen = p.WallLength - 2 * p.EdgeOffset;
double usableHt = p.WallHeight - p.BottomOffset - p.TopOffset;

int cols = (int)Math.Floor(usableLen / p.TargetPanelW);
int rows = (int)Math.Floor(usableHt / p.TargetPanelH);

if (cols < 1) cols = 1;
if (rows < 1) rows = 1;

double panelW = usableLen / cols;
double panelH = usableHt / rows;

int panelCount = cols * rows;

double wallArea = p.WallLength * p.WallHeight;
double panelArea = panelCount * panelW * panelH;
double wastePct = wallArea > 0 ? (1.0 - panelArea / wallArea) * 100.0 : 0.0;

public class Params
{
    /// Wall length in mm.
    public double WallLength { get; set; } = 8000;

    /// Wall height in mm.
    public double WallHeight { get; set; } = 3600;

    /// Target panel width in mm (actual width is adjusted to fit).
    public double TargetPanelW { get; set; } = 1200;

    /// Target panel height in mm (actual height is adjusted to fit).
    public double TargetPanelH { get; set; } = 900;

    /// Edge offset left and right in mm.
    public double EdgeOffset { get; set; } = 50;

    /// Bottom offset (skirting / floor line) in mm.
    public double BottomOffset { get; set; } = 100;

    /// Top offset (soffit / ceiling line) in mm.
    public double TopOffset { get; set; } = 100;
}
