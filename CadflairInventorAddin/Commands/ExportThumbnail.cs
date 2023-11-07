using CadflairInventorAddin.Helpers;
using Inventor;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using Path = System.IO.Path;

namespace CadflairInventorAddin.Commands
{
    internal static class ExportThumbnail
    {
        public static ButtonDefinition ExportThumbnailButton { get; set; }

        public static void ExportThumbnailButton_OnExecute(NameValueMap Context)
        {
            try
            {
                Document doc = Globals.InventorApplication.ActiveDocument;

                string folder = Path.GetDirectoryName(doc.FullFileName);
                string filename = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(doc.FullFileName)}.png");
                SaveThumbnailToFile(doc: doc, 
                                    filename: filename, 
                                    sizeInPixels: 400,
                                    backgroundColor: Globals.InventorApplication.TransientObjects.CreateColor(255, 255, 255));


                Process.Start(folder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ExportiPartStpsButton_OnExecute");
                MessageBox.Show(ex.ToString(), "Export failed!");
            }
        }

        private static void SaveThumbnailToFile(Document doc, string filename, int sizeInPixels = 200, Inventor.Color backgroundColor = null)
        {
            try
            {
                if (!(doc is PartDocument) && !(doc is AssemblyDocument))
                    return;

                Trace.TraceInformation("Exporting thumbnail for: " + System.IO.Path.GetFileName(doc.FullFileName));


                // turn off unwanted objects
                dynamic invDoc = doc;
                bool showWorkFeatures = invDoc.ObjectVisibility.AllWorkFeatures;
                bool showSketches = invDoc.ObjectVisibility.Sketches;
                bool showSketches3D = invDoc.ObjectVisibility.Sketches3D;
                bool showSketchDimensions = invDoc.ObjectVisibility.SketchDimensions;
                bool showAnnotations3D = invDoc.ObjectVisibility.Annotations3D;

                invDoc.ObjectVisibility.AllWorkFeatures = false;
                invDoc.ObjectVisibility.Sketches = false;
                invDoc.ObjectVisibility.Sketches3D = false;
                invDoc.ObjectVisibility.SketchDimensions = false;
                invDoc.ObjectVisibility.Annotations3D = false;

                // setup camera
                dynamic inventorApp = doc.Parent;
                inventorApp.DisplayOptions.Show3DIndicator = false;
                Camera cam = inventorApp.TransientObjects.CreateCamera();
                cam.SceneObject = invDoc.ComponentDefinition;
                cam.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
                cam.Fit();
                cam.ApplyWithoutTransition();

                // apply background color
                if (backgroundColor == null)
                {
                    backgroundColor = inventorApp.TransientObjects.CreateColor(0xEC, 0xEC, 0xEC, 0.0); // hardcoded. Make as a parameter
                    //backgroundColor = inventorApp.TransientObjects.CreateColor(0, 0, 0); // black, tested this for converting to transparent background but it resulted in jagged edges
                }

                // generate image twice as large, and then downsample it (antialiasing)
                string filePathLarge = System.IO.Path.Combine(Path.GetTempPath(), "thumbnail-large.png");
                cam.SaveAsBitmap(filePathLarge, sizeInPixels * 2, sizeInPixels * 2, backgroundColor, backgroundColor);

                // reset object visibility to inital settings
                invDoc.ObjectVisibility.AllWorkFeatures = showWorkFeatures;
                invDoc.ObjectVisibility.Sketches = showSketches;
                invDoc.ObjectVisibility.Sketches3D = showSketches3D;
                invDoc.ObjectVisibility.Annotations3D = showAnnotations3D;
                invDoc.ObjectVisibility.SketchDimensions = showSketchDimensions;

                // resize image, see reference https://stackoverflow.com/a/24199315
                using (var image = Image.FromFile(filePathLarge))
                using (var destImage = new Bitmap(sizeInPixels, sizeInPixels))
                {

                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            var destRect = new Rectangle(0, 0, sizeInPixels, sizeInPixels);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    //// replace background pixels with transparent color
                    //System.Drawing.Color colorToReplace = backgroundColor.ToSystemDrawingColor();
                    //System.Drawing.Color transparentColor = System.Drawing.Color.FromArgb(0,0,0,0);
                    //for (int x = 0; x < destImage.Width; x++)
                    //{
                    //    for (int y = 0; y < destImage.Height; y++)
                    //    {
                    //        if (destImage.GetPixel(x, y).Equals(colorToReplace))
                    //            destImage.SetPixel(x, y, transparentColor);
                    //    }
                    //}

                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                    destImage.Save(filename);

                    // clean up
                    System.IO.File.Delete(filePathLarge);

                    Trace.WriteLine($"Thumbnail exported successfully: {filename}");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not export thumbnail - {ex}");
            }
        }
    }
}

