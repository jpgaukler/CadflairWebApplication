using CadflairInventorAddin.Helpers;
using CadflairInventorLibrary.Helpers;
using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CadflairInventorAddin.Commands
{
    internal static class DrawingAttributesCommand
    {

        public static ButtonDefinition AddDimensionAttributesButton { get; set; }
        public static ButtonDefinition RefreshDimensionsButton { get; set; }

        //button handlers
        public static void AddDimensionAttributesButton_OnExecute(NameValueMap Context)
        {
            AddAutomationAttributes((DrawingDocument)Globals.InventorApplication.ActiveDocument);
        }

        public static void RefreshDimensionsButton_OnExecute(NameValueMap Context)
        {
            RefreshAutomatedDrawingObjects((DrawingDocument)Globals.InventorApplication.ActiveDocument);
        }



        private const string sheetAttributesSetName = "SheetAutomation";
        private const string dimensionAttributesSetName = "TextPosition";
        private const string viewAttributesSetName = "ViewPlacement";

        private static void AddAutomationAttributes(DrawingDocument doc, Inventor.Application app = null)
        {
            if (Globals.InventorApplication == null)
            {
                Globals.InventorApplication = app;
            }

            Transaction transaction = Globals.InventorApplication.TransactionManager.StartTransaction((_Document)doc, "Refresh Automated Drawing Objects");

            try
            {
                //remove any unused attribute sets
                doc.AttributeManager.PurgeAttributeSets(sheetAttributesSetName, false, out _);
                doc.AttributeManager.PurgeAttributeSets(dimensionAttributesSetName, false, out _);
                doc.AttributeManager.PurgeAttributeSets(viewAttributesSetName, false, out _);

                foreach (Sheet sheet in doc.Sheets)
                {
                    //clear previous values
                    if (sheet.AttributeSets.NameIsUsed[sheetAttributesSetName])
                    {
                        sheet.AttributeSets[sheetAttributesSetName].Delete();
                    }

                    //add attribute to sheet to indicate that is it set up for automation
                    AttributeSet sheetAttributes = sheet.AttributeSets.Add(sheetAttributesSetName);

                    #region add placement attributes to drawing views to help with scaling and positioning later

                    foreach (DrawingView view in sheet.DrawingViews)
                    {
                        //clear previous values
                        if (view.AttributeSets.NameIsUsed[viewAttributesSetName])
                        {
                            view.AttributeSets[viewAttributesSetName].Delete();
                        }

                        AttributeSet viewAttributes = view.AttributeSets.Add(viewAttributesSetName);

                        if (view.ParentView == null)
                        {
                            if (view.Camera.ViewOrientationType == ViewOrientationTypeEnum.kIsoBottomLeftViewOrientation ||
                                view.Camera.ViewOrientationType == ViewOrientationTypeEnum.kIsoBottomRightViewOrientation ||
                                view.Camera.ViewOrientationType == ViewOrientationTypeEnum.kIsoTopLeftViewOrientation ||
                                view.Camera.ViewOrientationType == ViewOrientationTypeEnum.kIsoTopRightViewOrientation)
                            {
                                viewAttributes.Add("Placement", ValueTypeEnum.kStringType, "Iso");
                            }
                            else
                            {
                                viewAttributes.Add("Placement", ValueTypeEnum.kStringType, "Base");
                            }
                        }
                        else
                        {
                            if (!view.Aligned)
                            {
                                continue;
                            }

                            double x = Math.Round(view.Center.X, 5);
                            double parentX = Math.Round(view.ParentView.Center.X, 5);
                            double y = Math.Round(view.Center.Y, 5);
                            double parentY = Math.Round(view.ParentView.Center.Y, 5);

                            if (x == parentX) //aligned horizontally with base
                            {
                                if (y > parentY)
                                {
                                    viewAttributes.Add("Placement", ValueTypeEnum.kStringType, "Top");
                                }
                                else
                                {
                                    viewAttributes.Add("Placement", ValueTypeEnum.kStringType, "Bottom");
                                }
                            }

                            if (y == parentY) //aligned vertically with base
                            {
                                if (x < parentX)
                                {
                                    viewAttributes.Add("Placement", ValueTypeEnum.kStringType, "Left");
                                }
                                else
                                {
                                    viewAttributes.Add("Placement", ValueTypeEnum.kStringType, "Right");
                                }
                            }
                        }
                    }

                    #endregion

                    #region add position attributes to dimensions

                    foreach (DrawingDimension dim in sheet.DrawingDimensions)
                    {
                        //clear previous values
                        if (dim.AttributeSets.NameIsUsed[dimensionAttributesSetName])
                        {
                            dim.AttributeSets[dimensionAttributesSetName].Delete();
                        }

                        AttributeSet dimAttributes = dim.AttributeSets.Add(dimensionAttributesSetName);
                        DrawingView view = dim.GetView();

                        if (dim is LinearGeneralDimension)
                        {                            
                            LinearGeneralDimension linearDim = (LinearGeneralDimension)dim;
                            LineSegment2d dimensionLine = (LineSegment2d)linearDim.DimensionLine;

                            string location = "";
                            double offsetDistance = 0;

                            if (dimensionLine.StartPoint.X == dimensionLine.EndPoint.X)
                            {
                                //this dimension is vertical
                                if (dimensionLine.StartPoint.X < view.Left)
                                {
                                    location = "Left";
                                    offsetDistance = Math.Abs(view.Left - dimensionLine.StartPoint.X);
                                }
                                else
                                {
                                    location = "Right";
                                    offsetDistance = Math.Abs(view.Left + view.Width - dimensionLine.StartPoint.X);
                                }

                            }
                            else if (dimensionLine.StartPoint.Y == dimensionLine.StartPoint.Y)
                            {
                                //this dimension is horizontal
                                if (dimensionLine.StartPoint.Y > view.Top)
                                {
                                    location = "Top";
                                    offsetDistance = Math.Abs(dimensionLine.StartPoint.Y - view.Top);
                                }
                                else
                                {
                                    location = "Bottom";
                                    offsetDistance = Math.Abs(view.Top - view.Height - dimensionLine.StartPoint.Y);
                                }
                            }

                            dimAttributes.Add(location, ValueTypeEnum.kDoubleType, offsetDistance);
                        }
                        else if (dim is DiameterGeneralDimension || dim is RadiusGeneralDimension)
                        {
                            string nearestVerticalEdge;
                            string nearestHorizontalEdge;
                            double verticalDistance;
                            double horizontalDistance;

                            //capture nearest vertical edge (left or right)
                            double distanceFromLeft = dim.Text.Origin.X - view.Left;
                            double distanceFromRight = (view.Left + view.Width) - dim.Text.Origin.X;

                            if (Math.Abs(distanceFromLeft) < Math.Abs(distanceFromRight))
                            {
                                nearestVerticalEdge = "Left";
                                verticalDistance = distanceFromLeft;
                            }
                            else
                            {
                                nearestVerticalEdge = "Right";
                                verticalDistance = -distanceFromRight; //must be negative to be sure that point is to the right of the right edge
                            }

                            //capture nearest horizontal edge (top or bottom)
                            double distanceFromTop = dim.Text.Origin.Y - view.Top;
                            double distanceFromBottom = (view.Top - view.Height) - dim.Text.Origin.Y;

                            if (Math.Abs(distanceFromTop) < Math.Abs(distanceFromBottom))
                            {
                                nearestHorizontalEdge = "Top";
                                horizontalDistance = distanceFromTop;
                            }
                            else
                            {
                                nearestHorizontalEdge = "Bottom";
                                horizontalDistance = -distanceFromBottom;
                            }

                            dimAttributes.Add(nearestHorizontalEdge, ValueTypeEnum.kDoubleType, horizontalDistance);
                            dimAttributes.Add(nearestVerticalEdge, ValueTypeEnum.kDoubleType, verticalDistance);
                        }
                        else if (dim is OrdinateDimension)
                        {
                            OrdinateDimension ordDim = (OrdinateDimension)dim;

                            string location = "";
                            double offsetDistance = 0;

                            if (ordDim.DimensionType == DimensionTypeEnum.kHorizontalDimensionType)
                            {
                                if (dim.Text.Origin.X < view.Left)
                                {
                                    location = "Left";
                                    offsetDistance = Math.Abs(view.Left - dim.Text.Origin.X);
                                }
                                else
                                {
                                    location = "Right";
                                    offsetDistance = Math.Abs(view.Left + view.Width - dim.Text.Origin.X);
                                }

                            }
                            else if (ordDim.DimensionType == DimensionTypeEnum.kVerticalDimensionType)
                            {
                                //this dimension is horizontal
                                if (dim.Text.Origin.Y > view.Top)
                                {
                                    location = "Top";
                                    offsetDistance = Math.Abs(dim.Text.Origin.Y - view.Top);
                                }
                                else
                                {
                                    location = "Bottom";
                                    offsetDistance = Math.Abs(view.Top - view.Height - dim.Text.Origin.Y);
                                }
                            }

                            dimAttributes.Add(location, ValueTypeEnum.kDoubleType, offsetDistance);
                        }
                    }

                    #endregion

                    transaction.End();
                }
            }
            catch (Exception ex)
            {
                transaction.Abort();
                MessageBox.Show(ex.ToString(), "Unable to add attributes");
            }
        }

        public static void RefreshAutomatedDrawingObjects(DrawingDocument doc, Inventor.Application app = null)
        {
            if (Globals.InventorApplication == null)
            {
                Globals.InventorApplication = app;
            }

            Transaction transaction = Globals.InventorApplication.TransactionManager.StartTransaction((_Document)doc, "Refresh Automated Drawing Objects");

            try
            {
                foreach (Sheet sheet in doc.Sheets)
                {
                    if (!sheet.AttributeSets.NameIsUsed[sheetAttributesSetName])
                    {
                        continue;
                    }

                    #region scale the drawing views that have been set up for automation and position the views to fit within the safe sheet area

                    List<DrawingView> horizontallyAlignedViews = new List<DrawingView> { };
                    List<DrawingView> verticallyAlignedViews = new List<DrawingView> { };
                    DrawingView isoView = null;
                    DrawingView baseView = null;
                    DrawingView leftView = null;
                    DrawingView rightView = null;
                    DrawingView topView = null;
                    DrawingView bottomView = null;

                    //pull out each view type
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

                    //scale and position the iso view
                    if (isoView != null)
                    {
                        //scale the Iso view, safe area is all space above the title block within the border
                        double safeIsoWidth = Math.Abs(sheet.TitleBlock.RangeBox.MinPoint.X - sheet.TitleBlock.RangeBox.MaxPoint.X);
                        double safeIsoHeight = Math.Abs(sheet.Border.RangeBox.MinPoint.Y - sheet.Border.RangeBox.MaxPoint.Y) - Math.Abs(sheet.TitleBlock.RangeBox.MinPoint.Y - sheet.TitleBlock.RangeBox.MaxPoint.Y);

                        //calculate the scale that will result in 50% of the safe area (either vertically or horizontally) being filled by the drawing view
                        double verticalIsoScale = (0.65 * safeIsoHeight) / (isoView.Height / isoView.Scale);
                        double horizontalIsoScale = (0.65 * safeIsoWidth) / (isoView.Width / isoView.Scale);

                        isoView.Scale = Math.Min(verticalIsoScale, horizontalIsoScale);

                        //place the iso view in the upper right corner
                        double partsListHeight = 0;
                        if (sheet.PartsLists.Count > 0)
                        {
                            partsListHeight = Math.Abs(sheet.PartsLists[1].RangeBox.MinPoint.Y - sheet.PartsLists[1].RangeBox.MaxPoint.Y);
                        }

                        double isoX = sheet.Border.RangeBox.MaxPoint.X - safeIsoWidth / 2;
                        double isoY = sheet.Border.RangeBox.MaxPoint.Y - isoView.Height / 2 - 1.27 - partsListHeight;

                        isoView.Center = Globals.InventorApplication.TransientGeometry.CreatePoint2d(isoX, isoY);
                    }

                    //scale the base view, safe area is all area left of the title block within the border
                    double safeWidth = Math.Abs(sheet.Border.RangeBox.MinPoint.X - sheet.Border.RangeBox.MaxPoint.X) - Math.Abs(sheet.TitleBlock.RangeBox.MinPoint.X - sheet.TitleBlock.RangeBox.MaxPoint.X);
                    double safeHeight = Math.Abs(sheet.Border.RangeBox.MinPoint.Y - sheet.Border.RangeBox.MaxPoint.Y);

                    //get total width and height of all base drawings at 1:1 scale
                    double viewHeightSum = verticallyAlignedViews.Sum(v => v.Height / v.Scale);
                    double viewWidthSum = horizontallyAlignedViews.Sum(v => v.Width / v.Scale);

                    //calculate the scale that will result in 50% of the safe area (either vertically or horizontally) being filled by the drawing views
                    double verticalBaseScale = (0.5 * safeHeight) / viewHeightSum;
                    double horizontalBaseScale = (0.5 * safeWidth) / viewWidthSum;

                    //set base scale to the smaller scale factor
                    baseView.Scale = Math.Min(verticalBaseScale, horizontalBaseScale);

                    //initialize base view placement from the bottom left corner of the safe area (currently the drawing border)
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
                }

                transaction.End();
            }
            catch (Exception ex)
            {
                transaction.Abort();
                MessageBox.Show(ex.ToString(), "Unable to refresh drawing");
            }
        }

    }
}


        //public static void AddAttributes(string location, int offsetLevel, string textAlignment)
        //{
        //    bool stillSelecting = true;
        //    while (stillSelecting == true)
        //    {
        //        DimensionSelection selection = new DimensionSelection("Select dimesions: " + location + " - Level: " + offsetLevel.ToString() + ", Align: " + textAlignment);
        //        if (selection.SelectedDimension != null)
        //        {
        //            if (selection.SelectedDimension.AttributeSets.NameIsUsed[dimensionAttributesSetName])
        //            {
        //                selection.SelectedDimension.AttributeSets[dimensionAttributesSetName].Delete();
        //            }

        //            AttributeSet dimAttributes = selection.SelectedDimension.AttributeSets.Add(dimensionAttributesSetName);
        //            dimAttributes.Add(location, ValueTypeEnum.kIntegerType, offsetLevel);
        //            dimAttributes.Add("TextAlignment", ValueTypeEnum.kStringType, textAlignment);
        //        }
        //        else
        //        {
        //            stillSelecting = false;
        //        }
        //    }
        //}

        //public static void RefreshLinearDimensions(DrawingDocument doc)
        //{
        //    object PreviewResult = null;
        //    doc.AttributeManager.PurgeAttributeSets("TextPosition", false, out PreviewResult);

        //    //HOW CAN I MAKE THIS MORE LOGICAL?
        //    RepositionLinearDimTextOrigins(doc); //move the text origins to their designated positions, the dimension line may not move with the text if the text is too long to fit between the leaders
        //    RepositionLinearDimTextOrigins(doc); //calling this twice ensures that the all the dimension lines move to the designation origin position, I beleive this is because the dimline resizes after the first move
        //}

        //private static void RepositionLinearDimTextOrigins(DrawingDocument doc)
        //{
        //    foreach (Sheet sheet in doc.Sheets)
        //    {
        //        foreach (DrawingDimension dim in sheet.DrawingDimensions)
        //        {
        //            if (dim.AttributeSets.NameIsUsed[dimensionAttributesSetName] && (dim is LinearGeneralDimension))
        //            {
        //                LinearGeneralDimension linearDim = (LinearGeneralDimension)dim;
        //                AttributeSet textAttributes = linearDim.AttributeSets[dimensionAttributesSetName];
        //                dynamic geom = (dynamic)linearDim.IntentOne.Geometry;
        //                DrawingView view = geom.Parent;

        //                double x = 0;
        //                double y = 0;
        //                double offsetFactor = 1;

        //                if (textAttributes.NameIsUsed["Top"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Top"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = Math.Min(dimLine.StartPoint.X, dimLine.EndPoint.X) + Math.Abs(dimLine.StartPoint.X - dimLine.EndPoint.X) / 2;
        //                    y = view.Top + offsetFactor * offsetLevel;

        //                    //while (dimLine.StartPoint.Y != y)
        //                    //{
        //                    //    linearDim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y); //move origin to designated position, the dimension line may not move if the text does not fit between it the first time, so loop until it does
        //                    //}
        //                }
        //                else if (textAttributes.NameIsUsed["Bottom"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Bottom"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = Math.Min(dimLine.StartPoint.X, dimLine.EndPoint.X) + Math.Abs(dimLine.StartPoint.X - dimLine.EndPoint.X) / 2;
        //                    y = view.Top - view.Height - offsetFactor * offsetLevel;

        //                }
        //                else if (textAttributes.NameIsUsed["Left"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Left"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = view.Left - offsetFactor * offsetLevel;
        //                    y = Math.Min(dimLine.StartPoint.Y, dimLine.EndPoint.Y) + Math.Abs(dimLine.StartPoint.Y - dimLine.EndPoint.Y) / 2;

        //                }
        //                else if (textAttributes.NameIsUsed["Right"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Right"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = view.Left + view.Width + offsetFactor * offsetLevel;
        //                    y = Math.Min(dimLine.StartPoint.Y, dimLine.EndPoint.Y) + Math.Abs(dimLine.StartPoint.Y - dimLine.EndPoint.Y) / 2;

        //                }

        //                linearDim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y);

        //                //MessageBox.Show("Moving to \n" + "X: " + x.ToString() + "\n Y: " + y.ToString() , "Value: " + (dim.ModelValue / 2.54).ToString());


        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Incomplete
        ///// </summary>
        ///// <param name="doc"></param>
        //private static void SetLinearDimTextAlignment(DrawingDocument doc)
        //{
        //    foreach (Sheet sheet in doc.Sheets)
        //    {
        //        foreach (DrawingDimension dim in sheet.DrawingDimensions)
        //        {
        //            if (dim.AttributeSets.NameIsUsed[dimensionAttributesSetName] && (dim is LinearGeneralDimension))
        //            {
        //                LinearGeneralDimension linearDim = (LinearGeneralDimension)dim;
        //                AttributeSet textAttributes = linearDim.AttributeSets[dimensionAttributesSetName];
        //                dynamic geom = (dynamic)linearDim.IntentOne.Geometry;
        //                DrawingView view = geom.Parent;

        //                double x = 0;
        //                double y = 0;
        //                double offsetFactor = 1;

        //                if (textAttributes.NameIsUsed["Top"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Top"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = Math.Min(dimLine.StartPoint.X, dimLine.EndPoint.X) + Math.Abs(dimLine.StartPoint.X - dimLine.EndPoint.X) / 2;
        //                    y = view.Top + offsetFactor * offsetLevel;
        //                }
        //                else if (textAttributes.NameIsUsed["Bottom"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Bottom"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = Math.Min(dimLine.StartPoint.X, dimLine.EndPoint.X) + Math.Abs(dimLine.StartPoint.X - dimLine.EndPoint.X) / 2;
        //                    y = view.Top - view.Height - offsetFactor * offsetLevel;
        //                }
        //                else if (textAttributes.NameIsUsed["Left"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Left"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = view.Left - offsetFactor * offsetLevel;
        //                    y = Math.Min(dimLine.StartPoint.Y, dimLine.EndPoint.Y) + Math.Abs(dimLine.StartPoint.Y - dimLine.EndPoint.Y) / 2;
        //                }
        //                else if (textAttributes.NameIsUsed["Right"])
        //                {
        //                    int offsetLevel = (int)textAttributes["Right"].Value;
        //                    LineSegment2d dimLine = (LineSegment2d)dim.DimensionLine;
        //                    x = view.Left + view.Width + offsetFactor * offsetLevel;
        //                    y = Math.Min(dimLine.StartPoint.Y, dimLine.EndPoint.Y) + Math.Abs(dimLine.StartPoint.Y - dimLine.EndPoint.Y) / 2;
        //                }

        //                //MessageBox.Show("Moving to \n" + "X: " + x.ToString() + "\n Y: " + y.ToString() , "Value: " + (dim.ModelValue / 2.54).ToString());

        //                linearDim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y); //move origin to designated position, the dimension line may not move if the text does not fit between it 

        //                //if (linearDim.Text.Origin.X != x)
        //                //{
        //                //    linearDim.Text.Origin = Globals.InventorApplication.TransientGeometry.CreatePoint2d(x, y); //reposition items again to force dimension line to align with text origin, I believe this occurs because the size of the dimension line changes after the first run.
        //                //}


        //                if (textAttributes.NameIsUsed["TextAlignment"])
        //                {

        //                }

        //            }
        //        }
        //    }
        //}



//private static AttributesDialog m_attributesDialog;

//public static void StartCommnand(NameValueMap context)
//{
//    if (m_attributesDialog == null)
//    {
//        m_attributesDialog = new AttributesDialog();
//        m_attributesDialog.Show();
//    }

//}

//public static void StopCommand()
//{
//    m_attributesDialog.Dispose();
//    m_attributesDialog = null;
//}

//private static void SelectDimensions(string location, int offsetLevel, string alignment)
//{
//    bool stillSelecting = true;
//    while (stillSelecting == true)
//    {
//        DimensionSelection selection = new DimensionSelection("Select a dimension");
//        if (selection.SelectedDimension != null)
//        {
//            AddLocationAttribute(selection.SelectedDimension, location, offsetLevel, alignment);
//        }
//        else
//        {
//            stillSelecting = false;
//        }
//    }
//}

//private static void AddLocationAttribute(DrawingDimension dim, string location, int offsetLevel, string alignment)
//{
//    if (dim.AttributeSets.NameIsUsed[dimensionAttributesSetName])
//    {
//        dim.AttributeSets[dimensionAttributesSetName].Delete();
//    }

//    AttributeSet dimAttributes = dim.AttributeSets.Add(dimensionAttributesSetName);
//    dimAttributes.Add(location, ValueTypeEnum.kIntegerType, offsetLevel);
//    dimAttributes.Add("TextAlignment", ValueTypeEnum.kStringType, alignment);
//}


//public static void CreateLinearDimensionAttributes(DrawingDimension dim, string location, double offsetDistance)
//{
//    if (dim.AttributeSets.NameIsUsed[dimensionAttributesSetName])
//    {
//        dim.AttributeSets[dimensionAttributesSetName].Delete();
//    }

//    AttributeSet dimAttributes = dim.AttributeSets.Add(dimensionAttributesSetName);
//    dimAttributes.Add(location, ValueTypeEnum.kDoubleType, offsetDistance);
//}

//public static void CreateDiameterDimensionAttributes(DrawingDimension dim, string nearestVerticalEdge, double verticalDistance, string nearestHorizontalEdge, double horizontalDistance)
//{
//    if (dim.AttributeSets.NameIsUsed[dimensionAttributesSetName])
//    {
//        dim.AttributeSets[dimensionAttributesSetName].Delete();
//    }

//    AttributeSet dimAttributes = dim.AttributeSets.Add(dimensionAttributesSetName);
//    dimAttributes.Add(nearestHorizontalEdge, ValueTypeEnum.kDoubleType, horizontalDistance);
//    dimAttributes.Add(nearestVerticalEdge, ValueTypeEnum.kDoubleType, verticalDistance);
//}