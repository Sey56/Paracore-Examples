/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 3 - Room review classifier. Pure .NET: no Revit API, no Paracore DSL.
One area threshold and two plain text labels produce a boolean flag and a
label. Both labels are StringInput nodes in the exported graph, so the
CodeBlock contains only a comparison and one inline conditional.

Outputs:
  needsReview - true when Area is below ReviewThreshold
  label       - ReviewLabel or StandardLabel

Exportable because:
- public class Params with public auto-properties (double Area, double threshold, two strings)
- logic in top-level statements, pure .NET
- CodeBlock-safe syntax: no if statements, no casts, no string literals

The released 4.9.0 exporter copies these statements into a Dynamo CodeBlock.
Keep the statements as plain assignments that read each input once.
*/

bool needsReview = Area < ReviewThreshold;
string label = needsReview ? ReviewLabel : StandardLabel;

public class Params
{
    /// Label used when the room does not need review.
    public string StandardLabel { get; set; } = "Standard room";

    /// Label used when the room is below the review threshold.
    public string ReviewLabel { get; set; } = "Too small - check";

    /// Area in square metres below which the room is flagged.
    public double ReviewThreshold { get; set; } = 5.0;

    /// Room area in square metres.
    public double Area { get; set; } = 18.5;
}
