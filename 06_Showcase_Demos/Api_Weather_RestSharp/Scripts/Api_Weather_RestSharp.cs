using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Autodesk.Revit.DB;

/*
DocumentType: Project
Categories: Showcase, API, Web
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, RestSharp

Description:
Connects to a live weather API to fetch site data based on the project's location.
Demonstrates RestSharp integration within Revit scripts.
*/

// 1. Setup
var p = new Params();

// 2. Preparation (API Request)
if (p.RunWeatherCheck)
{
    // Rename to avoid collision with Params properties
    double targetLat = p.Lat;
    double targetLon = p.Lon;

    // Auto-extract from Project if enabled
    if (p.UseProjectLocation)
    {
        var site = Doc.SiteLocation;
        targetLat = site.Latitude * 180 / Math.PI;  // Convert Radians to Degrees
        targetLon = site.Longitude * 180 / Math.PI;
        Println($"Using Project Location: {Doc.Title}");
    }

    Println($"Checking weather for Lat: {targetLat:F4}, Lon: {targetLon:F4}...");

    // Initialize RestSharp Client
    var client = new RestClient("https://api.open-meteo.com/v1");
    var request = new RestRequest("forecast");
    
    request.AddParameter("latitude", targetLat);
    request.AddParameter("longitude", targetLon);
    request.AddParameter("current_weather", "true");

    try 
    {
        // Execute Synchronously (Revit Safe)
        var response = client.Get(request);
        
        if (response.IsSuccessful && response.Content != null)
        {
            Println("✅ Weather Data Received.");
            
            // 3. Render Table
            try 
            {
                var data = JsonDocument.Parse(response.Content);
                var current = data.RootElement.GetProperty("current_weather");
                
                var tableData = new List<object>
                {
                    new { Property = "Temperature", Value = $"{current.GetProperty("temperature").GetDouble()}°C" },
                    new { Property = "Wind Speed", Value = $"{current.GetProperty("windspeed").GetDouble()} km/h" },
                    new { Property = "Wind Direction", Value = $"{current.GetProperty("winddirection").GetDouble()}°" },
                    new { Property = "Weather Code", Value = current.GetProperty("weathercode").GetInt32().ToString() },
                    new { Property = "Time", Value = current.GetProperty("time").GetString() }
                };

                Table(tableData);
                Println("See the 'Table' tab for details.");
            }
            catch (Exception ex)
            {
                Println("⚠️ Could not format weather data into a table. Check raw output below:");
                Println(response.Content);
            }
        }
        else
        {
            Println($"🚫 API Error: {response.ErrorMessage}");
        }
    }
    catch (Exception ex)
    {
        Println($"❌ Error: {ex.Message}");
    }
}

// ---------------------------------------------------------
// PARAMETERS
// ---------------------------------------------------------

public class Params
{
    #region Site Coordinates

    /// <summary>
    /// If true, the script will automatically use the Latitude/Longitude 
    /// defined in the Revit Project Information (Site Location).
    /// </summary>
    public bool UseProjectLocation { get; set; } = true;

    /// <summary>
    /// Manual latitude override.
    /// Only used if 'UseProjectLocation' is disabled.
    /// </summary>
    [EnabledWhen("UseProjectLocation", "false")]
    public double Lat { get; set; } = 51.5074;

    /// <summary>
    /// Manual longitude override.
    /// Only used if 'UseProjectLocation' is disabled.
    /// </summary>
    [EnabledWhen("UseProjectLocation", "false")]
    public double Lon { get; set; } = -0.1278;

    #endregion

    #region Action

    /// Toggle to trigger the API request.
    public bool RunWeatherCheck { get; set; } = true;

    #endregion
}