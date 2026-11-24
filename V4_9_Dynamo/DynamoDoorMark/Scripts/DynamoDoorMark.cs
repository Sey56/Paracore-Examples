/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 1 - Door mark generator. Pure .NET: no Revit API, no Paracore DSL.
Builds a mark by joining plain text inputs: L + 02 + - + R + 105 + - + A.
The text pieces are ordinary StringInput nodes in the exported graph, so the
CodeBlock only concatenates values - no formatting rules and no string methods.

With the declared defaults the result is:
  LevelPrefix "L", LevelNumber "02", RoomPrefix "R", RoomNumber "105",
  Separator "-", DoorSuffix "A"  ->  L02-R105-A

Exportable because:
- public class Params with public auto-properties (all string)
- logic in top-level statements, pure .NET string concatenation
- CodeBlock-safe syntax: no if statements, no casts, no method calls

The released 4.9.0 exporter copies these statements into a Dynamo CodeBlock.
Keep the statements as plain assignments that read each input once.
*/

string levelCode = LevelPrefix + LevelNumber;
string roomCode = RoomPrefix + RoomNumber;
string doorMark = levelCode + Separator + roomCode + Separator + DoorSuffix;

public class Params
{
    /// Suffix letter for the door, e.g. A.
    public string DoorSuffix { get; set; } = "A";

    /// Text placed between the level and room parts.
    public string Separator { get; set; } = "-";

    /// Room number as text, e.g. 105.
    public string RoomNumber { get; set; } = "105";

    /// Prefix for the room part, e.g. R.
    public string RoomPrefix { get; set; } = "R";

    /// Level number as text, e.g. 02.
    public string LevelNumber { get; set; } = "02";

    /// Prefix for the level part, e.g. L.
    public string LevelPrefix { get; set; } = "L";
}
