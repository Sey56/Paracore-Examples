/*
DocumentType: Project
Categories: Tutorial, Audit
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin
Description:
Lists doors ordered by width. Read-only.
*/
Println($"Doors in the model: {GetElements<FamilyInstance>("Doors").Count()}");
GetElements<FamilyInstance>("Doors").OrderByParam("Width").Table();
