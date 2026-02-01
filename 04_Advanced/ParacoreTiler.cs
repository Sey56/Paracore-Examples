using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
/*
DocumentType: Project
Categories: Architectural, Interiors, Analysis
Author: Seyoum Hagos
Dependencies: RevitAPI 2025, CoreScript.Engine, Paracore.Addin

Description:
An advanced floor pattern generator that creates precise tile layouts within room boundaries. 
Supports multiple tiling modes including Checkered, Alternating Rows, and Random distribution. 
Features include custom rotation angles, adjustable grout widths, and gap-filling logic. 
The script provides a detailed summary of tile counts, total area, and an estimated cost based on user-provided pricing.

UsageExamples:
- "Create a checkered floor pattern for the Lobby"
- "Generate 600x600 floor tiles rotated by 45 degrees"
- "Estimate the cost of tiling the kitchen with custom grout spacing"
- "Fill floor gaps with a secondary tile type"
*/
// 1. Instantiate Parameters
var p = new Params();

// 2. Data Preparation & Validation (Read-Only)
var room = new FilteredElementCollector(Doc)
    .OfCategory(BuiltInCategory.OST_Rooms)
    .WhereElementIsNotElementType()
    .Cast<Room>()
    .FirstOrDefault(r => r.Name == p.SelectedRoom);

if (room == null) throw new Exception("🚫 Target room not found.");

// Retrieve Floor Types
var ft1 = new FilteredElementCollector(Doc).OfClass(typeof(FloorType)).Cast<FloorType>().FirstOrDefault(x => x.Name == p.Tile1_Type);
var ft2 = new FilteredElementCollector(Doc).OfClass(typeof(FloorType)).Cast<FloorType>().FirstOrDefault(x => x.Name == p.Tile2_Type);

if (ft1 == null) throw new Exception($"🚫 Tile 1 Type '{p.Tile1_Type}' not found.");
if (p.PatternType != "Single Tile" && ft2 == null) throw new Exception($"🚫 Tile 2 Type '{p.Tile2_Type}' not found.");

// Retrieve Filler Type
var ft3 = new FilteredElementCollector(Doc).OfClass(typeof(FloorType)).Cast<FloorType>().FirstOrDefault(x => x.Name == p.Tile3_Type);
if (p.FillEmptySpaces && ft3 == null) throw new Exception($"🚫 Filler Tile Type '{p.Tile3_Type}' not found.");

// Get Boundary and Bounding Box
var opt = new SpatialElementBoundaryOptions { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish };
var boundarySegments = room.GetBoundarySegments(opt);
if (boundarySegments == null || boundarySegments.Count == 0) throw new Exception("🚫 Could not determine room boundaries.");

BoundingBoxXYZ bbox = room.get_BoundingBox(null);
XYZ roomCenter = (bbox.Min + bbox.Max) / 2.0;

List<ElementId> createdFloorIds = new List<ElementId>();
Random random = new Random();

// Prepare Room Solid for Clipping
List<CurveLoop> roomLoops = new List<CurveLoop>();
double zLevel = bbox.Min.Z; // Default Z

if (boundarySegments.Count > 0 && boundarySegments[0].Count > 0)
{
    // Use the Z level of the actual boundary curves to ensure intersection alignment
    zLevel = boundarySegments[0][0].GetCurve().GetEndPoint(0).Z;
    
    foreach (var segList in boundarySegments)
    {
        List<Curve> curves = new List<Curve>();
        foreach (var seg in segList) curves.Add(seg.GetCurve());
        roomLoops.Add(CurveLoop.Create(curves));
    }
}

// Create a solid representing the room volume (extruded up slightly)
Solid roomSolid = GeometryCreationUtilities.CreateExtrusionGeometry(roomLoops, XYZ.BasisZ, 1.0);

// Grid Calculation (Rotated Coverage)
double rotationRad = p.RotationAngle * (Math.PI / 180.0);
double cos = Math.Cos(rotationRad);
double sin = Math.Sin(rotationRad);

// Calculate a search radius large enough to cover the room at any rotation angle
double radius = bbox.Min.DistanceTo(bbox.Max); 
double minU = -radius;
double minV = -radius;
double maxU = radius;
double maxV = radius;

// Cost Accumulators (Internal Units: SqFt)
double areaT1 = 0;
double areaT2 = 0;
double areaT3 = 0;
int countT1 = 0;
int countT2 = 0;
int countT3 = 0;

// Helper Action to create a tile (avoids code duplication)
Action<double, double, double, double, FloorType, int> CreateTile = (u, v, w, h, fType, typeIdx) => 
{
    // Grout Logic: Shrink geometry, keep grid center
    double gw = p.GroutWidth;
    double drawW = w - gw;
    double drawH = h - gw;
    if (drawW <= 0.001 || drawH <= 0.001) return; // Skip if tile vanishes

    // Transform to World Coordinates (Rotate & Translate to Room Center)
    XYZ[] corners = new XYZ[4];
    corners[0] = new XYZ(u + gw/2, v + gw/2, 0);
    corners[1] = new XYZ(u + gw/2 + drawW, v + gw/2, 0);
    corners[2] = new XYZ(u + gw/2 + drawW, v + gw/2 + drawH, 0);
    corners[3] = new XYZ(u + gw/2, v + gw/2 + drawH, 0);

    List<Curve> profile = new List<Curve>();
    for (int i = 0; i < 4; i++)
    {
        // Rotate around (0,0)
        double rx = corners[i].X * cos - corners[i].Y * sin;
        double ry = corners[i].X * sin + corners[i].Y * cos;
        // Translate to Room Center + Z Level
        corners[i] = new XYZ(roomCenter.X + rx, roomCenter.Y + ry, zLevel);
    }

    profile.Add(Line.CreateBound(corners[0], corners[1]));
    profile.Add(Line.CreateBound(corners[1], corners[2]));
    profile.Add(Line.CreateBound(corners[2], corners[3]));
    profile.Add(Line.CreateBound(corners[3], corners[0]));

    CurveLoop tileLoop = CurveLoop.Create(profile);
    
    // Create Tile Solid & Intersect
    Solid tileSolid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { tileLoop }, XYZ.BasisZ, 1.0);

    Solid intersection = null;
    try { intersection = BooleanOperationsUtils.ExecuteBooleanOperation(roomSolid, tileSolid, BooleanOperationsType.Intersect); }
    catch { return; }

    if (intersection != null && intersection.Volume > 0.00001)
    {
        foreach (Face face in intersection.Faces)
        {
            if (face is PlanarFace pf && pf.FaceNormal.IsAlmostEqualTo(-XYZ.BasisZ))
            {
                var loops = pf.GetEdgesAsCurveLoops();
                if (loops.Count > 0)
                    try { 
                        createdFloorIds.Add(Floor.Create(Doc, loops, fType.Id, room.LevelId).Id); 
                        // Accumulate Area
                        if (typeIdx == 1) { areaT1 += pf.Area; countT1++; }
                        else if (typeIdx == 2) { areaT2 += pf.Area; countT2++; }
                        else if (typeIdx == 3) { areaT3 += pf.Area; countT3++; }
                    } catch { }
            }
        }
    }
};

// 3. Execution (Single Transaction)
Transact("Generate Smart Floor Pattern", () =>
{
    double currentV = minV;
    int rowIndex = 0;

    while (currentV < maxV)
    {
        double currentU = minU;
        int colIndex = 0;
        double rowHeight = 0;
        var rowBuffer = new List<(double u, double v, double w, double h)>();

        while (currentU < maxU)
        {
            // 1. Determine Tile Settings based on Pattern
            bool useTile1 = true;
            if (p.PatternType == "Checkered") useTile1 = (rowIndex + colIndex) % 2 == 0;
            else if (p.PatternType == "Alternating Rows") useTile1 = rowIndex % 2 == 0;
            else if (p.PatternType == "Random") useTile1 = random.Next(0, 100) < p.RandomBias;

            FloorType currentFt = useTile1 ? ft1 : ft2;
            double w = useTile1 ? p.Tile1_Width : p.Tile2_Width;
            double h = useTile1 ? p.Tile1_Height : p.Tile2_Height;

            // 2. Calculate Local Coordinates (u, v)
            double u = currentU;
            double v = currentV;

            // Create Main Tile
            CreateTile(u, v, w, h, currentFt, useTile1 ? 1 : 2);
            
            // Buffer for filler calculation
            rowBuffer.Add((u, v, w, h));

            // Advance Cursor
            currentU += w;
            rowHeight = Math.Max(rowHeight, h);
            colIndex++;
        }

        // Fill Gaps in Row
        if (p.FillEmptySpaces && ft3 != null)
        {
            foreach (var t in rowBuffer)
            {
                // 1. Fill Gap Below (if tile shifted up)
                double gapBelow = t.v - currentV;
                if (gapBelow > 0.001)
                {
                    CreateTile(t.u, currentV, t.w, gapBelow, ft3, 3);
                }

                // 2. Fill Gap Above (if tile shorter or shifted down)
                double tileTop = t.v + t.h;
                double rowTop = currentV + rowHeight;
                double gapAbove = rowTop - tileTop;
                if (gapAbove > 0.001)
                {
                    CreateTile(t.u, tileTop, t.w, gapAbove, ft3, 3);
                }
            }
        }

        if (rowHeight < 0.001) break; // Safety break
        currentV += rowHeight;
        rowIndex++;
    }
});

// 4. Output & Selection
if (createdFloorIds.Count > 0)
{
    UIDoc.Selection.SetElementIds(createdFloorIds);
    Println($"✅ Successfully generated {createdFloorIds.Count} tiles in room: {room.Name}");
    
    // Cost Calculation
    double sqFtToSqM = 0.092903;
    double c1 = (areaT1 * sqFtToSqM) * p.Tile1_Cost;
    double c2 = (areaT2 * sqFtToSqM) * p.Tile2_Cost;
    double c3 = (areaT3 * sqFtToSqM) * p.Tile3_Cost;
    double totalCost = c1 + c2 + c3;

    Println($"💰 Estimated Cost: {totalCost:F2} (T1: {c1:F2}, T2: {c2:F2}, Fill: {c3:F2})");

    // Visualization Table
    var summary = new List<object> {
        new { Type = p.Tile1_Type, Count = countT1, Price_m2 = p.Tile1_Cost.ToString("F2"), Total_Price = c1.ToString("F2"), Total_Area_m2 = (areaT1 * sqFtToSqM).ToString("F2") }
    };
    if (p.PatternType != "Single Tile")
        summary.Add(new { Type = p.Tile2_Type, Count = countT2, Price_m2 = p.Tile2_Cost.ToString("F2"), Total_Price = c2.ToString("F2"), Total_Area_m2 = (areaT2 * sqFtToSqM).ToString("F2") });
    if (p.FillEmptySpaces)
        summary.Add(new { Type = p.Tile3_Type, Count = countT3, Price_m2 = p.Tile3_Cost.ToString("F2"), Total_Price = c3.ToString("F2"), Total_Area_m2 = (areaT3 * sqFtToSqM).ToString("F2") });
    
    summary.Add(new { Type = "TOTAL", Count = countT1 + countT2 + countT3, Price_m2 = "-", Total_Price = totalCost.ToString("F2"), Total_Area_m2 = ((areaT1+areaT2+areaT3) * sqFtToSqM).ToString("F2") });
    Table(summary);
}
else
{
    throw new Exception("⚠️ No tiles could be generated. Check room size and spacing.");
}

// 5. Parameter Definitions
public class Params
{
    #region 01. General Settings
    /// <summary>Select the target room where the floor tiles will be generated.</summary>
    [RevitElements(TargetType = "Room"), Required]
    public string SelectedRoom { get; set; }

    /// <summary>Choose the tiling pattern layout.</summary>
    public string PatternType { get; set; } = "Checkered";
    public List<string> PatternType_Options => ["Checkered", "Alternating Rows", "Random", "Single Tile"];

    /// <summary>Rotate the entire grid by a specific angle (in degrees).</summary>
    [Range(0, 360, 15)]
    public double RotationAngle { get; set; } = 0;

    /// <summary>Define the spacing between tiles (grout lines). Set to 0 for no gaps.</summary>
    [Unit("mm")]
    [Range(0, 50)]
    public double GroutWidth { get; set; } = 0;

    #endregion

    #region 02. Primary Tile (Tile 1)
    /// <summary>The Floor Type used for the primary tile.</summary>
    [RevitElements(TargetType = "FloorType"), Required]
    public string Tile1_Type { get; set; }

    /// <summary>Width of the primary tile.</summary>
    [Unit("m")]
    public double Tile1_Width { get; set; } = 0.6;

    /// <summary>Height of the primary tile.</summary>
    [Unit("m")]
    public double Tile1_Height { get; set; } = 0.6;

    /// <summary>Cost per square meter for Tile 1 (used for estimation).</summary>
    public double Tile1_Cost { get; set; } = 0;

    #endregion

    #region 03. Secondary Tile (Tile 2)
    /// <summary>The Floor Type used for the secondary tile (if pattern requires it).</summary>
    [RevitElements(TargetType = "FloorType"), Required]
    public string Tile2_Type { get; set; }

    /// <summary>Width of the secondary tile.</summary>
    [Unit("m")]
    public double Tile2_Width { get; set; } = 0.6;

    /// <summary>Height of the secondary tile.</summary>
    [Unit("m")]
    public double Tile2_Height { get; set; } = 0.6;

    /// <summary>Cost per square meter for Tile 2.</summary>
    public double Tile2_Cost { get; set; } = 0;

    public bool Tile2_Type_Visible => PatternType != "Single Tile";
    public bool Tile2_Width_Visible => PatternType != "Single Tile";
    public bool Tile2_Height_Visible => PatternType != "Single Tile";
    public bool Tile2_Cost_Visible => PatternType != "Single Tile";
    
    #endregion

    #region 04. Gap Filler (Tile 3)
    /// <summary>Automatically detect and fill empty spaces caused by unequal tile sizes or offsets.</summary>
    public bool FillEmptySpaces { get; set; } = false;

    /// <summary>The Floor Type used to fill the detected gaps.</summary>
    [RevitElements(TargetType = "FloorType")]
    public string Tile3_Type { get; set; }
    
    /// <summary>Cost per square meter for the filler tile.</summary>
    public double Tile3_Cost { get; set; } = 0;

    public bool Tile3_Type_Visible => FillEmptySpaces;
    public bool Tile3_Cost_Visible => FillEmptySpaces;
    #endregion

    #region 05. Randomization
    /// <summary>Adjust the probability of Tile 1 appearing in the Random pattern (0-100%).</summary>
    [Range(0, 100)]
    public int RandomBias { get; set; } = 50;

    public bool RandomBias_Visible => PatternType == "Random";

    #endregion
}