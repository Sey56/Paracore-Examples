using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Autodesk.Revit.DB;

/*
DocumentType: Any
Categories: Showcase, Image, Processing
Author: Paracore Team
Dependencies: RevitAPI 2025, CoreScript.Engine, SixLabors.ImageSharp

Description:
Production-ready image processing script that resizes and filters local images.
Demonstrates ImageSharp integration for handling image assets.
*/

// 1. Setup
var p = new Params();

// 2. Logic (Image Processing)
if (!string.IsNullOrEmpty(p.InputImagePath) && File.Exists(p.InputImagePath))
{
    string outputDir = Path.GetDirectoryName(p.InputImagePath);
    string outputPath = Path.Combine(outputDir, "Processed_" + Path.GetFileName(p.InputImagePath));

    Println($"Processing image: {p.InputImagePath}");

    try 
    {
        // Load, Transform, and Save using ImageSharp
        using (Image image = Image.Load(p.InputImagePath))
        {
            image.Mutate(x => x
                .Resize(image.Width / 2, image.Height / 2) // Scale down
                .Rotate(p.RotateClockwise ? 90 : 0)       // Optional rotate
                .Grayscale());                            // High-end filter

            image.Save(outputPath);
        }

        Println($"✅ Processed image saved to: {outputPath}");
    }
    catch (Exception ex)
    {
        Println($"❌ Image Processing Error: {ex.Message}");
    }
}
else
{
    Println("⚠️ Please select a valid input image path in the parameters.");
}

// ---------------------------------------------------------
// PARAMETERS
// ---------------------------------------------------------

public class Params
{
    #region Path Settings

    /// <summary>
    /// Path to the local image file (JPG, PNG).
    /// </summary>
    [InputFile("jpg;png")]
    public string InputImagePath { get; set; }

    #endregion

    #region Processing Options

    /// Whether to rotate the image 90 degrees.
    public bool RotateClockwise { get; set; } = false;

    #endregion
}