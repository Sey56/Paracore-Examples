/*
DocumentType: Project
Categories: Tutorial, Dynamo
Author: Paracore Team
Dependencies: .NET only (no Revit, no Paracore DSL) - export requires Paracore 4.9.0+

Description:
NEW IN 4.9.0 - Method export example. This is a pure .NET script with a public
static class and one scalar method. Use the gallery's Export Dynamo button,
choose Methods, and the method appears as a node in Dynamo's library. Wire the
two inputs yourself and read the result.
*/

Params p = new();

int total = ParacoreMath.AddTwoNums(p.Num_1, p.Num_2);
Println($"Sum = {total}");

public class Params
{
    /// <summary>First number.</summary>
    public int Num_1 { get; set; } = 10;

    /// <summary>Second number.</summary>
    public int Num_2 { get; set; } = 6;
}

/// <summary>Adds two integers and returns the sum.</summary>
public static class ParacoreMath
{
    public static int AddTwoNums(int x, int y)
    {
        return x + y;
    }
}
