using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairInventorPlugin
{
    static class DrawingAutomation
    {
        private const string sheetAttributesSetName = "SheetAutomation";
        private const string dimensionAttributesSetName = "TextPosition";
        private const string viewAttributesSetName = "ViewPlacement";

        public static void GenerateDrawing(Document doc, NameValueMap map)
        {
            Trace.TraceInformation("Generating drawing: " + System.IO.Path.GetFileName(doc.FullFileName));

            //open drawing doc
            string drawingName = System.IO.Path.GetFileNameWithoutExtension(doc.FullFileName) + ".idw";
            string fullDrawingName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), drawingName);
            DrawingDocument dwgdoc = (DrawingDocument)Globals.InventorApplication.Documents.Open(fullDrawingName, OpenVisible: false);

            //refresh drawing
            RefreshAutomatedDrawingObjects(dwgdoc);

            //export pdf
            ExportPdf(dwgdoc, (string)map.Value["outputObjectKey"]);

            //save and close
            dwgdoc.Close(SkipSave: false);
            Trace.WriteLine("Document saved:" + drawingName);
        }

        private static void RefreshAutomatedDrawingObjects(DrawingDocument doc)
        {
            Trace.TraceInformation("Refreshing automated drawing objects: " + System.IO.Path.GetFileName(doc.FullFileName));

            try
            {          
                if (doc.RequiresUpdate)
                {
                    doc.Update();
                }

                foreach (Sheet sheet in doc.Sheets)
                {
                    if (!sheet.AttributeSets.NameIsUsed[sheetAttributesSetName])
                    {
                        continue;
                    }

                    #region scale the drawing views that have been set up for automation

                    List<DrawingView> horizontallyAlignedViews = new List<DrawingView> { };
                    List<DrawingView> verticallyAlignedViews = new List<DrawingView> { };
                    DrawingView isoView = null;
                    DrawingView baseView = null;
                    DrawingView leftView = null;
                    DrawingView rightView = null;
                    DrawingView topView = null;
                    DrawingView bottomView = null;

                    foreach (DrawingView view in sheet.DrawingViews)
                    {
                        if (!view.AttributeSets.NameIsUsed[viewAttributesSetName])
                        {
                            continue;
                        }

                        AttributeSet viewAttributes = view.AttributeSets[viewAttributesSetName];
                        string placement = (string)viewAttributes["Placement"].Value;

                        //store each view in the proper list
                        switch (placement)
                        {
                            case "Iso":
                                isoView = view;
                                break;

                            case "Base":
                                verticallyAlignedViews.Add(view);
                                horizontallyAlignedViews.Add(view);
                                baseView = view;
                                break;

                            case "Top":
                                verticallyAlignedViews.Add(view);
                                topView = view;
                                break;

                            case "Bottom":
                                verticallyAlignedViews.Add(view);
                                bottomView = view;
                                break;

                            case "Left":
                                horizontallyAlignedViews.Add(view);
                                leftView = view;
                                break;

                            case "Right":
                                horizontallyAlignedViews.Add(view);
                                rightView = view;
                                break;

                            default:
                                break;
                        }
                    }

                    //scale the Iso view, safe area is all space above the title block within the border
                    double safeIsoWidth = Math.Abs(sheet.TitleBlock.RangeBox.MinPoint.X - sheet.TitleBlock.RangeBox.MaxPoint.X);
                    double safeIsoHeight = Math.Abs(sheet.Border.RangeBox.MinPoint.Y - sheet.Border.RangeBox.MaxPoint.Y) - Math.Abs(sheet.TitleBlock.RangeBox.MinPoint.Y - sheet.TitleBlock.RangeBox.MaxPoint.Y);

                    //calculate the scale that will result in 50% of the safe area (either vertically or horizontally) being filled by the drawing view
                    double verticalIsoScale = (0.65 * safeIsoHeight) / (isoView.Height / isoView.Scale);
                    double horizontalIsoScale = (0.65 * safeIsoWidth) / (isoView.Width / isoView.Scale);

                    isoView.Scale = Math.Min(verticalIsoScale, horizontalIsoScale);

                    //scale the base view, safe area is all area left of the title block within the border
                    double safeWidth = Math.Abs(sheet.Border.RangeBox.MinPoint.X - sheet.Border.RangeBox.MaxPoint.X) - Math.Abs(sheet.TitleBlock.RangeBox.MinPoint.X - sheet.TitleBlock.RangeBox.MaxPoint.X);
                    double safeHeight = Math.Abs(sheet.Border.RangeBox.MinPoint.Y - sheet.Border.RangeBox.MaxPoint.Y);

                    //get total width and height of all drawings at 1:1 scale
                    double viewHeightSum = verticallyAlignedViews.Sum(v => v.Height / v.Scale);
                    double viewWidthSum = horizontallyAlignedViews.Sum(v => v.Width / v.Scale);

                    //calculate the scale that will result in 50% of the safe area (either vertically or horizontally) being filled by the drawing views
                    double verticalBaseScale = (0.5 * safeHeight) / viewHeightSum;
                    double horizontalBaseScale = (0.5 * safeWidth) / viewWidthSum;

                    //set base scale to the smaller scale factor
                    baseView.Scale = Math.Min(verticalBaseScale, horizontalBaseScale);

                    #endregion

                    Trace.WriteLine(sheet.Name + " - Drawing views scaled.");

                    #region position the views to fit within the safe sheet area  

                    //place the iso view in the upper right corner
                    double partsListHeight = 0;
                    if (sheet.PartsLists.Count > 0)
                    {
                        partsListHeight = Math.Abs(sheet.PartsLists[1].RangeBox.MinPoint.Y - sheet.PartsLists[1].RangeBox.MaxPoint.Y);
                    }

                    double isoX = sheet.Border.RangeBox.MaxPoint.X - safeIsoWidth / 2;
                    double isoY = sheet.Border.RangeBox.MaxPoint.Y - isoView.Height / 2 - 1.27 - partsListHeight;

                    isoView.Center = Globals.InventorApplication.TransientGeometry.CreatePoint2d(isoX, isoY);

                    //initialize placement from the bottom left corner of the safe area (currently the drawing border)
                    double currentXPos = sheet.Border.RangeBox.MinPoint.X;
                    double currentYPos = sheet.Border.RangeBox.MinPoint.Y;

                    //position all horizontally aligned views spaced evenly across the safe area
                    foreach (DrawingView _view in new DrawingView[] { leftView, baseView, rightView })
                    {
                        if (_view != null)
                        {
                            double gapWidth = (safeWidth - horizontallyAlignedViews.Sum(v => v.Width)) / (horizontallyAlignedViews.Count + 1);
                            double x = currentXPos + _view.Width / 2 + gapWidth;
                            double y = _view.Center.Y;
                            _view.Center = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y);
                            currentXPos = _view.Left + _view.Width;
                        }
                    }

                    //position all vertically aligned views spaced evenly across the safe area
                    foreach (DrawingView _view in new DrawingView[] { bottomView, baseView, topView })
                    {
                        if (_view != null)
                        {
                            double gapHeight = (safeHeight - verticallyAlignedViews.Sum(v => v.Height)) / (verticallyAlignedViews.Count + 1);
                            double x = _view.Center.X;
                            double y = currentYPos + _view.Height / 2 + gapHeight;
                            _view.Center = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y);
                            currentYPos = _view.Top;
                        }
                    }

                    #endregion

                    Trace.WriteLine(sheet.Name + " - Drawing views repositioned in safe area.");

                    #region reposition dimensions to match the initial configuration

                    foreach (DrawingDimension dim in sheet.DrawingDimensions)
                    {
                        //removed unattached dims
                        if (!dim.Attached)
                        {
                            dim.Delete();
                        }

                        if (!dim.AttributeSets.NameIsUsed[dimensionAttributesSetName])
                        {
                            continue;
                        }

                        AttributeSet textAttributes = dim.AttributeSets[dimensionAttributesSetName];
                        DrawingView view = dim.GetView();

                        if (dim is LinearGeneralDimension)
                        {
                            double x = 0;
                            double y = 0;

                            if (textAttributes.NameIsUsed["Top"])
                            {
                                double offsetDistance = (double)textAttributes["Top"].Value;
                                LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
                                x = Math.Min(dimLine.StartPoint.X, dimLine.EndPoint.X) + Math.Abs(dimLine.StartPoint.X - dimLine.EndPoint.X) / 2;
                                y = view.Top + offsetDistance;
                            }
                            else if (textAttributes.NameIsUsed["Bottom"])
                            {
                                double offsetDistance = (double)textAttributes["Bottom"].Value;
                                LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
                                x = Math.Min(dimLine.StartPoint.X, dimLine.EndPoint.X) + Math.Abs(dimLine.StartPoint.X - dimLine.EndPoint.X) / 2;
                                y = view.Top - view.Height - offsetDistance;

                            }
                            else if (textAttributes.NameIsUsed["Left"])
                            {
                                double offsetDistance = (double)textAttributes["Left"].Value;
                                LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
                                x = view.Left - offsetDistance;
                                y = Math.Min(dimLine.StartPoint.Y, dimLine.EndPoint.Y) + Math.Abs(dimLine.StartPoint.Y - dimLine.EndPoint.Y) / 2;

                            }
                            else if (textAttributes.NameIsUsed["Right"])
                            {
                                double offsetDistance = (double)textAttributes["Right"].Value;
                                LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
                                x = view.Left + view.Width + offsetDistance;
                                y = Math.Min(dimLine.StartPoint.Y, dimLine.EndPoint.Y) + Math.Abs(dimLine.StartPoint.Y - dimLine.EndPoint.Y) / 2;

                            }

                            dim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y);
                        }
                        else if (dim is DiameterGeneralDimension || dim is RadiusGeneralDimension)
                        {
                            double x = 0;
                            double y = 0;

                            if (textAttributes.NameIsUsed["Top"])
                            {
                                double offsetDistance = (double)textAttributes["Top"].Value;
                                y = view.Top + offsetDistance;
                            }
                            else if (textAttributes.NameIsUsed["Bottom"])
                            {
                                double offsetDistance = (double)textAttributes["Bottom"].Value;
                                y = view.Top - view.Height + offsetDistance;

                            }

                            if (textAttributes.NameIsUsed["Left"])
                            {
                                double offsetDistance = (double)textAttributes["Left"].Value;
                                x = view.Left + offsetDistance;
                            }
                            else if (textAttributes.NameIsUsed["Right"])
                            {
                                double offsetDistance = (double)textAttributes["Right"].Value;
                                x = view.Left + view.Width + offsetDistance;
                            }

                            dim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y);
                        }
                        else if (dim is OrdinateDimension)
                        {
                            double x = 0;
                            double y = 0;

                            if (textAttributes.NameIsUsed["Top"])
                            {
                                double offsetDistance = (double)textAttributes["Top"].Value;
                                x = dim.Text.Origin.X;
                                y = view.Top + offsetDistance;
                            }
                            else if (textAttributes.NameIsUsed["Bottom"])
                            {
                                double offsetDistance = (double)textAttributes["Bottom"].Value;
                                x = x = dim.Text.Origin.X;
                                y = view.Top - view.Height - offsetDistance;

                            }
                            else if (textAttributes.NameIsUsed["Left"])
                            {
                                double offsetDistance = (double)textAttributes["Left"].Value;
                                x = view.Left - offsetDistance;
                                y = dim.Text.Origin.Y;

                            }
                            else if (textAttributes.NameIsUsed["Right"])
                            {
                                double offsetDistance = (double)textAttributes["Right"].Value;
                                x = view.Left + view.Width + offsetDistance;
                                y = dim.Text.Origin.Y;

                            }

                            dim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y);
                        }
                    }

                    #endregion

                    Trace.WriteLine(sheet.Name + " - Dimensions realigned.");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError("Refresh automated drawing objects failed - " + ex.ToString());
            }
        }

        private static DrawingView GetView(this DrawingDimension dim)
        {
            if (dim is LinearGeneralDimension)
            {
                LinearGeneralDimension linearDim = (LinearGeneralDimension)dim;
                dynamic geom = (dynamic)linearDim.IntentOne.Geometry;
                return geom.Parent;
            }
            else if (dim is DiameterGeneralDimension)
            {
                DiameterGeneralDimension diameterDim = (DiameterGeneralDimension)dim;
                dynamic geom = (dynamic)diameterDim.Intent.Geometry;
                return geom.Parent;
            }
            else if (dim is RadiusGeneralDimension)
            {
                RadiusGeneralDimension radiusDim = (RadiusGeneralDimension)dim;
                dynamic geom = (dynamic)radiusDim.Intent.Geometry;
                return geom.Parent;
            }
            else if (dim is OrdinateDimension)
            {
                OrdinateDimension ordDim = (OrdinateDimension)dim;
                dynamic geom = (dynamic)ordDim.Intent.Geometry;
                return geom.Parent;
            }
            else if (dim is AngularGeneralDimension)
            {
                AngularGeneralDimension angularDim = (AngularGeneralDimension)dim;
                dynamic geom = (dynamic)angularDim.IntentOne.Geometry;
                return geom.Parent;
            }
            else
            {
                return null;
            }
        }

        private static void ExportPdf(DrawingDocument doc, string filename)
        {
            try
            {
                Trace.TraceInformation("Exporting pdf for: " + System.IO.Path.GetFileName(doc.FullFileName));

                TranslatorAddIn oPDFAddin = (TranslatorAddIn)Globals.InventorApplication.ApplicationAddIns.ItemById["{0AC6FD96-2F4D-42CE-8BE0-8AEA580399E4}"];
                TranslationContext oContext = Globals.InventorApplication.TransientObjects.CreateTranslationContext();
                oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism;

                // Options for drawings...
                NameValueMap oOptions = Globals.InventorApplication.TransientObjects.CreateNameValueMap();
                oOptions.Value["Vector_Resolution"] = 720;
                oOptions.Value["Sheet_Range"] = PrintRangeEnum.kPrintSheetRange;

                // Create a DataMedium object
                DataMedium oDataMedium = Globals.InventorApplication.TransientObjects.CreateDataMedium();

                // Set the destination file name
                //string pdfName = System.IO.Path.GetFileNameWithoutExtension(doc.FullFileName) + ".pdf";
                string pdfName = filename + ".pdf";
                oDataMedium.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), pdfName);

                // Publish document.
                oPDFAddin.SaveCopyAs(doc, oContext, oOptions, oDataMedium);

                Trace.WriteLine("Pdf exported successfully: " + pdfName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not export pdf - " + ex.ToString());
            }
        }
    }   
    
}
