/*
DocumentType: Any
Categories: Basics, Tutorials, Multi-File
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine

Description:
Tutorial 5:
A multi-file version of the Hello Revit tutorial.
This file (Main.cs) contains the execution logic, while Params.cs contains the settings.
*/

// 1. Setup
// Parameters are defined in Params.cs, but we use them here just the same.
var p = new Params();

var revitUserName = Doc.Application.Username;
var docTitle = Doc.Title;

Println($"👋 Hello, {p.Message}!");
Println($"👤 Revit User: {revitUserName}");
Println($"📂 Active Document: {docTitle}");
Println();

// 2. Logic & Execution
Transact("Example Transaction", () =>
{
    Println("--- Output from inside Transaction ---");
    Println("This logic is in Main.cs");
    Println("But the parameters are in Params.cs");
    Println("Paracore combines them automatically!");
    Println("--------------------------------------");
});

Println("✅ Multi-file script completed successfully.");
