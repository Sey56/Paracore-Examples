/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 1 - Door mark generator. Pure .NET: no Revit API, no Paracore DSL.
Composes a standard door mark from a level code, room number, and door suffix.
Shows full C# string rules that Dynamo CodeBlocks cannot express cleanly.

The mark format is: {level}-{room}-{suffix}
  level   = "L" + zero-padded 2-digit level number
  room    = "R" + zero-padded 3-digit room number
  suffix  = single uppercase letter, or "X" if empty/invalid

Examples:
  Level 2, Room 105, suffix "A"  ->  "L02-R105-A"
  Level 1, Room 3,   suffix ""   ->  "L01-R003-X"

Use the gallery's Export Dynamo button, choose Graph, and get a standalone .dyn
whose output appears on a Watch node. In Dynamo, feed the mark string into a
native Element.SetParameterByName node to write it to each door's Mark parameter.

Exportable because:
- public class Params with public auto-properties (int, int, string)
- logic in top-level statements, pure .NET string manipulation
- no Revit tokens, no Paracore helpers
*/

Params p = new();

string levelCode = "L" + p.LevelNumber.ToString("D2");
string roomCode = "R" + p.RoomNumber.ToString("D3");

string suffix = p.DoorSuffix.Trim().ToUpperInvariant();
if (suffix.Length > 1) suffix = suffix.Substring(0, 1);
if (string.IsNullOrEmpty(suffix)) suffix = "X";

string doorMark = levelCode + "-" + roomCode + "-" + suffix;

public class Params
{
    /// Level number (1 = ground floor, 2 = first floor, etc.).
    public int LevelNumber { get; set; } = 2;

    /// Room number as an integer (e.g. 105).
    public int RoomNumber { get; set; } = 105;

    /// Door suffix letter — A, B, C ... Use empty for the only door in a room.
    public string DoorSuffix { get; set; } = "A";
}
