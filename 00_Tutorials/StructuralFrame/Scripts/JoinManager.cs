// Join order management for structural elements.
// Columns cut beams, beams cut slabs, columns cut slabs.

public static class JoinManager
{
    public static int SetJoinOrder(Document doc, Level baseLevel)
    {
        var levels = new[] { baseLevel.Name };

        var columns = GetElements<FamilyInstance>("Structural Columns")
            .Where(c => levels.Contains(c.GetStr("Base Level"))).ToList();
        var beams = GetElements<FamilyInstance>("Structural Framing")
            .Where(b => levels.Contains(b.GetStr("Reference Level"))).ToList();
        var slabs = GetElements("Floors")
            .Where(s => levels.Contains(s.GetStr("Level"))).ToList();

        int joins = 0;
        Transact("Set Join Order", () =>
        {
            // Columns cut beams
            foreach (var col in columns)
            foreach (var beam in beams)
            {
                if (!BoxesOverlap(col, beam)) continue;
                try
                {
                    if (!JoinGeometryUtils.AreElementsJoined(doc, col, beam))
                    {
                        JoinGeometryUtils.JoinGeometry(doc, col, beam);
                        JoinGeometryUtils.SwitchJoinOrder(doc, col, beam);
                        joins++;
                    }
                } catch { }
            }

            // Beams cut slabs
            foreach (var beam in beams)
            foreach (var slab in slabs)
            {
                if (!BoxesOverlap(beam, slab)) continue;
                try
                {
                    if (!JoinGeometryUtils.AreElementsJoined(doc, beam, slab))
                    {
                        JoinGeometryUtils.JoinGeometry(doc, beam, slab);
                        JoinGeometryUtils.SwitchJoinOrder(doc, beam, slab);
                        joins++;
                    }
                } catch { }
            }
        });

        return joins;
    }

    private static bool BoxesOverlap(Element a, Element b)
    {
        var bb1 = a.get_BoundingBox(null);
        var bb2 = b.get_BoundingBox(null);
        if (bb1 == null || bb2 == null) return false;
        return bb1.Min.X <= bb2.Max.X && bb2.Min.X <= bb1.Max.X
            && bb1.Min.Y <= bb2.Max.Y && bb2.Min.Y <= bb1.Max.Y;
    }
}
