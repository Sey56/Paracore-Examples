using System.IO;
using System.Text;

var p = new Params();

if (p.TargetLevel == null)
    throw new Exception("Target Level is required.");

// ---------------------------------------------------------
// 1. FILTERING
// ---------------------------------------------------------
var wallCollector = new FilteredElementCollector(Doc)
    .OfClass(typeof(Wall))
    .WhereElementIsNotElementType()
    .Cast<Wall>()
    .Where(w => w.LevelId == p.TargetLevel.Id);

// Filter by Wall Type
if (p.TargetWallTypes != null && p.TargetWallTypes.Any())
{
    var typeIds = p.TargetWallTypes.Select(t => t.Id).ToHashSet();
    wallCollector = wallCollector.Where(w => typeIds.Contains(w.GetTypeId()));
}

// Filter by Thickness (p.MinThickness is in Feet due to [Unit("mm")])
var targetWalls = wallCollector
    .Where(w => w.Width > p.MinThickness)
    .ToList();

var targetWallIds = targetWalls.Select(w => w.Id).ToHashSet();

// 2. SPATIAL CHECK (Find Doors)
var doors = new FilteredElementCollector(Doc)
    .OfClass(typeof(FamilyInstance))
    .OfCategory(BuiltInCategory.OST_Doors)
    .Cast<FamilyInstance>()
    .Where(d => d.Host != null && targetWallIds.Contains(d.Host.Id))
    .ToList();

// 3. DATA INTEGRATION
var doorData = new List<DoorRecord>();
var csvMap = new Dictionary<string, string>();
int matchCount = 0;

if (!string.IsNullOrEmpty(p.CsvInput) && File.Exists(p.CsvInput))
{
    // Format: Mark,NewComment
    var lines = File.ReadAllLines(p.CsvInput);
    foreach (var line in lines)
    {
        var parts = line.Split(',');
        if (parts.Length >= 2)
        {
            var key = parts[0].Trim();
            var val = parts[1].Trim();
            if (!string.IsNullOrEmpty(key)) csvMap[key] = val;
        }
    }
}

foreach (var d in doors)
{
    var mark = d.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString() ?? "";
    var host = d.Host as Wall;
    var hostName = host?.Name ?? "Unknown";
    
    double dist = 0;
    if (p.ReferencePoint != null && d.Location is LocationPoint lp)
    {
        dist = lp.Point.DistanceTo(p.ReferencePoint);
    }

    doorData.Add(new DoorRecord 
    { 
        DoorId = d.Id.Value, 
        Mark = mark, 
        HostWallType = hostName, 
        DistanceToReference = Math.Round(dist, 2) 
    });

    if (csvMap.ContainsKey(mark)) matchCount++;
}

// 4. TRANSACTION
if (p.ExecuteUpdate && (p.AuditMode == "Data Sync" || p.AuditMode == "Full Report"))
{
    Transact("Sync Door Data", () =>
    {
        foreach (var d in doors)
        {
            var mark = d.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString();
            if (mark != null && csvMap.TryGetValue(mark, out var newComment))
            {
                d.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)?.Set(newComment);
            }
        }
    });
}

// 5. VISUALIZATION & OUTPUT
Println($"Processed {targetWalls.Count} walls and {doors.Count} doors.");
Println($"CSV Matches found: {matchCount}");

Table(doorData);

var pieData = doors.GroupBy(d => d.Symbol.Family.Name)
    .Select(g => new { Label = g.Key, Value = g.Count() });
PieChart(pieData);

var barData = targetWalls.GroupBy(w => w.Name)
    .Select(g => new { Label = g.Key, Value = g.Sum(w => w.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH)?.AsDouble() ?? 0) });
BarChart(barData);

if (!string.IsNullOrEmpty(p.ReportOutput) && p.AuditMode == "Full Report")
{
    var summary = $"Level: {p.TargetLevel.Name}, Doors Processed: {doors.Count}, Matches: {matchCount}";
    File.WriteAllText(p.ReportOutput, summary);
}

// ---------------------------------------------------------
// HELPERS
// ---------------------------------------------------------
public class DoorRecord
{
    public long DoorId { get; set; }
    public string Mark { get; set; }
    public string HostWallType { get; set; }
    public double DistanceToReference { get; set; }
}

// ---------------------------------------------------------
// PARAMS
// ---------------------------------------------------------
public class Params
{
    #region Target

    [Required]
    /// The Level to audit
    public Level TargetLevel { get; set; }

    [Select(SelectionType.Point)]
    /// Reference point for distance calculation
    public XYZ ReferencePoint { get; set; }

    /// Filter by specific wall types
    public List<WallType> TargetWallTypes { get; set; }

    [Unit("mm")]
    /// Minimum wall thickness to process
    public double MinThickness { get; set; } = 100;

    #endregion

    #region Settings

    [Segmented]
    /// Audit operation mode
    public string AuditMode { get; set; } = "Geometry Only";
    public List<string> AuditMode_Options => ["Geometry Only", "Data Sync", "Full Report"];

    [Color]
    /// Brand color for UI
    public string BrandColor { get; set; } = "#FF5733";

    #endregion

    #region Data Exchange

    [InputFile("csv")]
    /// CSV file with Mark,NewComment
    public string CsvInput { get; set; }

    [OutputFile("csv")]
    /// Output report file path
    public string ReportOutput { get; set; }

    [Confirm("RUN DATA SYNC")]
    /// Enable modification of door comments
    public bool ExecuteUpdate { get; set; } = false;

    #endregion
}