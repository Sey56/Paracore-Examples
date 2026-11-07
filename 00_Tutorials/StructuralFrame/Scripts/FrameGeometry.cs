// Structural frame geometry — columns, beams, slabs.
// Called from StructuralFrame.cs. No top-level statements.

public static class FrameGeometry
{
    private static Level? GetTopLevel(Level baseLevel)
    {
        Level? prev = null;
        foreach (Level l in GetElements<Level>().OrderByParam("Elevation"))
        {
            if (prev != null && prev.Id == baseLevel.Id)
                return l;
            prev = l;
        }
        return null;
    }

    private static List<(XYZ point, Grid g1, Grid g2)> FindIntersections(List<Grid> grids)
    {
        var result = new List<(XYZ, Grid, Grid)>();
        foreach (Grid g1 in grids)
        foreach (Grid g2 in grids)
        {
            if (g1.Id.Value >= g2.Id.Value) continue;
            XYZ s1 = g1.Curve.GetEndPoint(0), e1 = g1.Curve.GetEndPoint(1);
            XYZ s2 = g2.Curve.GetEndPoint(0), e2 = g2.Curve.GetEndPoint(1);
            XYZ d1 = (e1 - s1).Normalize();
            XYZ d2 = (e2 - s2).Normalize();
            if (Math.Abs(d1.DotProduct(d2)) > 0.99) continue;

            double det = d1.X * d2.Y - d1.Y * d2.X;
            if (Math.Abs(det) < 0.001) continue;
            double t = ((s2.X - s1.X) * d2.Y - (s2.Y - s1.Y) * d2.X) / det;
            XYZ pt = s1 + d1 * t;
            result.Add((new XYZ(pt.X, pt.Y, s1.Z), g1, g2));
        }
        return result;
    }

    public static int PlaceColumns(List<Grid> grids, Level baseLevel, FamilySymbol columnType)
    {
        var topLevel = GetTopLevel(baseLevel);
        if (topLevel == null) return 0;

        var intersections = FindIntersections(grids);
        if (intersections.Count == 0) return 0;

        if (!columnType.IsActive) columnType.Activate();

        int created = 0;
        foreach (var (pt, _, _) in intersections)
        {
            var col = Doc.Create.NewFamilyInstance(
                pt, columnType, baseLevel, StructuralType.Column);
            col.SetVal("Top Level", topLevel.Id);
            created++;
        }
        return created;
    }

    public static int PlaceBeams(
        List<Grid> grids, Level baseLevel, FamilySymbol beamType, FamilySymbol columnType)
    {
        var topLevel = GetTopLevel(baseLevel);
        if (topLevel == null) return 0;

        var intersections = FindIntersections(grids);
        if (intersections.Count == 0) return 0;

        if (!beamType.IsActive) beamType.Activate();

        int created = 0;
        foreach (Grid grid in grids)
        {
            var onGrid = intersections
                .Where(p => p.g1.Id == grid.Id || p.g2.Id == grid.Id)
                .Select(p => p.point)
                .ToList();
            if (onGrid.Count < 2) continue;

            XYZ gDir = (grid.Curve.GetEndPoint(1) - grid.Curve.GetEndPoint(0)).Normalize();
            onGrid.Sort((a, b) => a.DotProduct(gDir).CompareTo(b.DotProduct(gDir)));

            for (int i = 0; i < onGrid.Count - 1; i++)
            {
                var line = Line.CreateBound(onGrid[i], onGrid[i + 1]);
                Doc.Create.NewFamilyInstance(line, beamType, topLevel, StructuralType.Beam);
                created++;
            }
        }
        return created;
    }

    public static void PlaceSlab(
        List<Grid> grids, Level baseLevel, FloorType slabType, FamilySymbol columnType)
    {
        var topLevel = GetTopLevel(baseLevel);
        if (topLevel == null) return;

        var intersections = FindIntersections(grids);
        if (intersections.Count == 0) return;

        double minX = double.MaxValue, minY = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue;

        foreach (var (pt, _, _) in intersections)
        {
            minX = Math.Min(minX, pt.X); maxX = Math.Max(maxX, pt.X);
            minY = Math.Min(minY, pt.Y); maxY = Math.Max(maxY, pt.Y);
        }

        double z = topLevel.Elevation;
        var curves = new List<Curve>
        {
            Line.CreateBound(new XYZ(minX, minY, z), new XYZ(maxX, minY, z)),
            Line.CreateBound(new XYZ(maxX, minY, z), new XYZ(maxX, maxY, z)),
            Line.CreateBound(new XYZ(maxX, maxY, z), new XYZ(minX, maxY, z)),
            Line.CreateBound(new XYZ(minX, maxY, z), new XYZ(minX, minY, z))
        };

        Floor.Create(Doc, [CurveLoop.Create(curves)], slabType.Id, topLevel.Id);
    }
}
