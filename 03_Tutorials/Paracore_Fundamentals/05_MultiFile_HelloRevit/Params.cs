// This file only contains the Parameter definitions.
// Paracore automatically detects public classes in all files in the folder.

public class Params 
{
    #region Settings
    /// <summary>Message to print</summary>
    public string Message { get; set; } = "Real Type Hydration!";

    /// <summary>Choose a level to associate with the greeting</summary>
    public Level AssociatedLevel { get; set; }
    #endregion
}