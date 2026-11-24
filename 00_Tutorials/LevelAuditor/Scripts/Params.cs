// Parameters for LevelAuditor.
// The engine always merges Params.cs — it does not need to be referenced.

public class Params
{
    #region Settings

    /// Flag levels closer than this (in millimetres; converted to feet before the body runs)
    [Unit("mm")]
    public double MinSpacing { get; set; } = 1000;

    /// Only audit levels at or above this level (optional)
    public Level? BaseLevel { get; set; }

    #endregion
}
