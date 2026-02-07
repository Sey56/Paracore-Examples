




var viewWalls = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
    .OfCategory(BuiltInCategory.OST_Walls)
    .WhereElementIsNotElementType()
    .GetElementCount();

Println($"Walls in active view: {viewWalls}");