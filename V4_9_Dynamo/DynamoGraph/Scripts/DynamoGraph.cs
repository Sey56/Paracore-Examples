/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
NEW IN 4.9.0 - Graph export example. Pure .NET: no Revit API, no Paracore DSL.
Use the gallery's Export Dynamo button, choose Graph, and get a standalone .dyn
with inputs already wired to a CodeBlock and outputs on Watch nodes. The graph
runs in Dynamo without Paracore installed.
*/

Params p = new();

double cols = Math.Ceiling(p.FacadeLen / (p.PanelW + p.Joint));
double rows = Math.Ceiling(p.FacadeHt / (p.PanelH + p.Joint));
double panels = cols * rows;
double netAreaM2 = panels * (p.PanelW - p.Joint) * (p.PanelH - p.Joint) / 1000000;

public class Params
{
    /// <summary>Facade length in mm.</summary>
    public double FacadeLen { get; set; } = 12000;

    /// <summary>Facade height in mm.</summary>
    public double FacadeHt { get; set; } = 3200;

    /// <summary>Panel width in mm.</summary>
    public double PanelW { get; set; } = 600;

    /// <summary>Panel height in mm.</summary>
    public double PanelH { get; set; } = 1200;

    /// <summary>Joint or gap in mm.</summary>
    public double Joint { get; set; } = 10;
}
