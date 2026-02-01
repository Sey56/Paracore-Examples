// 1. Setup
var p = new Params();
var revitUserName = Doc.Application.Username;

Println($"{p.Message} {revitUserName}");

// 3. Parameters (MUST BE LAST)
public class Params {
    #region Settings
    
    /// <summary>
    /// The name to greet.
    /// </summary>
    public string Message { get; set; } = "Welcome to Paracore";
    
    #endregion
}
