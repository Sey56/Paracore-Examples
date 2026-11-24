/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
Tutorial 1 - Door mark generator. Pure .NET: no Revit API, no Paracore DSL.
Composes a standard door mark from a level number, room number, and suffix letter.
Shows multi-step string composition that stays inside the Dynamo CodeBlock subset.

The mark format is: {level}-{room}-{suffix}
  level   = "L" + zero-padded 2-digit level number
  room    = "R" + zero-padded 3-digit room number
  suffix  = single uppercase letter, or "X" if empty/invalid

Examples:
  Level 2, Room 105, suffix "A"  ->  "L02-R105-A"
  Level 1, Room 3,   suffix ""   ->  "L01-R003-X"

Exportable because:
- public class Params with public auto-properties (int, int, string)
- logic in top-level statements, pure .NET
- CodeBlock-safe syntax: no if statements, no casts, no bare declarations

The released 4.9.0 exporter copies these statements into a Dynamo CodeBlock.
DesignScript accepts assignments, inline conditionals, string concat and simple
string methods - but not C# 'if', '(int)' casts or bare 'string x;' declarations.
*/

Params p = new();

string levelCode = p.LevelNumber < 10 ? "L0" + p.LevelNumber : "L" + p.LevelNumber;
string roomCode = p.RoomNumber < 10 ? "R00" + p.RoomNumber : (p.RoomNumber < 100 ? "R0" + p.RoomNumber : "R" + p.RoomNumber);

string suffix = p.DoorSuffix.Trim().ToUpper();
string suffixOne = suffix.Length > 1 ? suffix.Substring(0, 1) : suffix;
string suffixFinal = suffixOne.Length == 0 ? "X" : suffixOne;

string doorMark = levelCode + "-" + roomCode + "-" + suffixFinal;

public class Params
{
    /// Level number (1 = ground floor, 2 = first floor, etc.).
    public int LevelNumber { get; set; } = 2;

    /// Room number as an integer (e.g. 105).
    public int RoomNumber { get; set; } = 105;

    /// Door suffix letter — A, B, C ... Use empty for the only door in a room.
    public string DoorSuffix { get; set; } = "A";
}
