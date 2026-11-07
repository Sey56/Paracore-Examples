// Parameters for LevelAuditor.
// The engine always merges Params.cs — it does not need to be referenced.

public class Params
{
    #region Settings

    /// <summary>Flag levels closer than this (in millimetres)</summary>
    [Unit("mm")]
    public double MinSpacingMm { get; set; } = 1000;

    /// <summary>Only audit levels at or above this level (optional)</summary>
    public Level? BaseLevel { get; set; }

    #endregion
}
