// Parameters for StructuralFrame.
// The engine always merges this file by its exact name.

public class Params
{
    #region Grids & Levels

    /// <summary>Select structural grids — at least one in each direction</summary>
    [Required]
    public List<Grid> Grids { get; set; } = new();

    /// <summary>Columns span from this level to the next level above</summary>
    [Required]
    public Level BaseLevel { get; set; }

    #endregion

    #region Element Types

    /// <summary>Column family type to place at grid intersections</summary>
    [RevitElements(Category = "Structural Columns"), Required]
    public FamilySymbol ColumnType { get; set; }

    /// <summary>Beam family type to place between columns</summary>
    [RevitElements(Category = "Structural Framing")]
    public FamilySymbol BeamType { get; set; }

    /// <summary>Floor type for the slab on top</summary>
    [RevitElements(Category = "Floors")]
    public FloorType SlabType { get; set; }

    #endregion
}
