/*
DocumentType: Project
Categories: Tutorial, Audit
Author: Paracore Team
Dependencies: RevitAPI 2025+, Paracore.Addin
Description:
Counts walls by level. Read-only.
*/
Println($"Walls in the model: {GetElements<Wall>().Count()}");
GetElements<Wall>().GroupByParam("Level").Table();
