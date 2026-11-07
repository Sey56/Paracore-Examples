/*
DocumentType: Project
Categories: Tutorial, Structural
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Creates a complete structural frame — columns at grid intersections,
beams between them, and a slab on top. All elements are joined with
correct cut priority so concrete volumes match construction quantities.
*/

Params p = new();

if (p.Grids == null || p.Grids.Count < 2)
{
    Println("Select at least 2 grids (one in each direction).");
    return;
}
if (p.BaseLevel == null)
{
    Println("Select a Base Level.");
    return;
}
if (p.ColumnType == null)
{
    Println("Select a Column type.");
    return;
}

int colCount = 0, beamCount = 0;

Transact("Create Structural Frame", () =>
{
    colCount = FrameGeometry.PlaceColumns(p.Grids, p.BaseLevel, p.ColumnType);

    if (p.BeamType != null)
        beamCount = FrameGeometry.PlaceBeams(p.Grids, p.BaseLevel, p.BeamType, p.ColumnType);

    if (p.SlabType != null)
        FrameGeometry.PlaceSlab(p.Grids, p.BaseLevel, p.SlabType, p.ColumnType);
});

int joinCount = JoinManager.SetJoinOrder(Doc, p.BaseLevel);

Println($"Created {colCount} columns, {beamCount} beams, 1 slab ({joinCount} joins set).");
