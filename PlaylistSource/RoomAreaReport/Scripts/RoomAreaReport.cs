/*
DocumentType: Project
Categories: Tutorial, Audit
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin
Description:
Reports room area by level, in square metres. Read-only.
*/
Println($"Rooms in the model: {GetElements("Rooms").Count()}");
GetElements("Rooms").GroupByParam("Level", "Area", "m2").Table();
