/*
DocumentType: Project
Categories: Tutorials
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin

Description:
Paracore HelloRevit
*/

// 1. Setup
Params p = new();
string revitUserName = Doc.Application.Username;

Println($"{p.Message} {revitUserName}");

// 3. Parameters (MUST BE LAST)
public class Params
{
    #region Settings

    /// <summary>
    /// The name to greet.
    /// </summary>
    public string Message { get; set; } = "Welcome to Paracore";

    #endregion
}
