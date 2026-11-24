// Parameters for StructuralFrame.
// The engine always merges this file by its exact name.

public class Params
{
    #region Grids & Levels

    /// Select structural grids — at least one in each direction
    [Required]
    public List<Grid> Grids { get; set; } = new();

    /// Columns span from this level to the next level above
    [Required]
    public Level BaseLevel { get; set; }

    #endregion

    #region Element Types

    /// Column family type to place at grid intersections
    [RevitElements(Category = "Structural Columns"), Required]
    public FamilySymbol ColumnType { get; set; }

    /// Beam family type to place between columns
    [RevitElements(Category = "Structural Framing")]
    public FamilySymbol BeamType { get; set; }

    /// Floor type for the slab on top
    [RevitElements(Category = "Floors")]
    public FloorType SlabType { get; set; }

    #endregion
}
