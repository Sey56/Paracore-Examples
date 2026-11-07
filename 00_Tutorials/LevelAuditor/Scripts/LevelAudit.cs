// Helper class for LevelAuditor.
// This file contains NO top-level statements — only a class definition.

public static class LevelAudit
{
    public static List<LevelRow> BuildReport(
        List<Level> levels, double minSpacingMm, Level? baseLevel)
    {
        double minSpacingFt = minSpacingMm / 304.8;   // mm → ft

        var rows = new List<LevelRow>();
        for (int i = 0; i < levels.Count; i++)
        {
            var level = levels[i];
            string issue = "OK";

            // Check spacing to the previous level
            if (i > 0)
            {
                double gap = level.Elevation - levels[i - 1].Elevation;
                if (gap < minSpacingFt)
                    issue = $"Only {gap * 304.8:F0} mm above {levels[i - 1].Name}";
            }

            // Check for duplicate elevations
            if (i > 0 && Math.Abs(level.Elevation - levels[i - 1].Elevation) < 0.001)
                issue = $"Duplicate elevation with {levels[i - 1].Name}";

            // If a base level is selected, skip levels below it
            if (baseLevel != null && level.Elevation < baseLevel.Elevation)
                continue;

            rows.Add(new LevelRow
            {
                Id = level.Id.Value,
                Name = level.Name,
                Elevation_m = level.Elevation * 0.3048,  // ft → m
                Issue = issue
            });
        }

        return rows;
    }
}

public record LevelRow
{
    public long Id { get; init; }
    public string Name { get; init; } = "";
    public double Elevation_m { get; init; }
    public string Issue { get; init; } = "OK";
}
