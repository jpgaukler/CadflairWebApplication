using CadflairDataAccess.Models;
using Inventor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CadflairInventorAddin.Helpers
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Get the associated drawing view object for the given dimension. Valid dimensions types are: <br/>
        /// <b>LinearGeneralDimension</b><br/>
        /// <b>DiameterGeneralDimension</b><br/>
        /// <b>RadiusGeneralDimension</b><br/>
        /// <b>OrdinateDimension</b><br/>
        /// <b>AngularGeneralDimension</b><br/>
        /// </summary>
        /// <param name="dim"></param>RemoveWhiteSpace
        /// <returns>The parent drawing view, or <b>null</b> if no match if found.</returns>
        public static DrawingView GetView(this DrawingDimension dim)
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

        /// <summary>
        /// Search through property sets in the given document and return the first matching property that is found.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="propertyName"></param>
        /// <returns>The matching property, or <b>null</b> if no match if found.</returns>
        public static Inventor.Property GetProperty(this Document doc, string propertyName)
        {
            foreach(PropertySet propSet in doc.PropertySets)
            {
                foreach(Property prop in propSet)
                {
                    if (prop.Name == propertyName) return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// Find a parameter in the given document that matches the supplied name.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parameterName"></param>
        /// <returns>The matching parameter, or <b>null</b> if no match is found.</returns>
        public static Inventor.Parameter GetParameter(this Document doc, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName)) return null;

            try
            {
                if (doc is PartDocument)
                {
                    PartDocument partDoc = (PartDocument)doc;
                    return partDoc.ComponentDefinition.Parameters[parameterName];
                }

                if (doc is AssemblyDocument)
                {
                    AssemblyDocument assemDoc = (AssemblyDocument)doc;
                    return assemDoc.ComponentDefinition.Parameters[parameterName];
                }
            }
            catch { }

            return null;
        }

        public static string[] ToStringArray(this ExpressionList expressionList)
        {
            if (expressionList == null || expressionList.Count == 0) return null;

            List<string> stringList = new List<string>();

            for (int i = 1; i <= expressionList.Count; i++)
            {
                stringList.Add(expressionList[i].Replace("\"", ""));
            }

            return stringList.ToArray();
        }


        public static Inventor.Color ToInventorColor(this System.Drawing.Color color)
        {
            return Globals.InventorApplication.TransientObjects.CreateColor(color.R, color.G, color.B);
        }

        public static System.Drawing.Color ToSystemDrawingColor(this Inventor.Color color)
        {
            return System.Drawing.Color.FromArgb(color.Red, color.Green, color.Blue);
        }

        public static System.Windows.Media.Color ToSystemMediaColor(this Inventor.Color color)
        {
            return System.Windows.Media.Color.FromRgb(color.Red, color.Green, color.Blue);
        }



        #region methods to assist with preparing files for upload 

        private const string _parameterValidationSetName = "CadflairParameterValidation";
        private const string _minAttributeName = "MinValue";
        private const string _maxAttributeName = "MaxValue";

        /// <summary>
        /// Add attributes to a numeric parameter to capture max and min values to be used with form validatoin on the Cadflair site.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public static void AddValidationAttributes(this Parameter parameter, double minValue, double maxValue)
        {
            //purge unused attributes
            Document doc = (Document)parameter.Parent;
            doc.AttributeManager.PurgeAttributeSets(_parameterValidationSetName, false, out _);

            //clear previous values and add attributes
            if (parameter.AttributeSets.NameIsUsed[_parameterValidationSetName])
            {
                parameter.AttributeSets[_parameterValidationSetName].Delete();
            }

            AttributeSet attSet = parameter.AttributeSets.Add(_parameterValidationSetName);
            attSet.Add(_minAttributeName, ValueTypeEnum.kDoubleType, Math.Min(minValue, maxValue));
            attSet.Add(_maxAttributeName, ValueTypeEnum.kDoubleType, Math.Max(minValue, maxValue));
        }

        /// <summary>
        /// Get the min allowable value from the attribute set that is tied to this parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static double? GetMinAttributeValue(this Parameter parameter)
        {
            if (!parameter.AttributeSets.NameIsUsed[_parameterValidationSetName]) return null;

            AttributeSet attSet = parameter.AttributeSets[_parameterValidationSetName];

            if (!attSet.NameIsUsed[_minAttributeName]) return null;

            return (double?)attSet[_minAttributeName].Value;
        }

        /// <summary>
        /// Get the max allowable value from the attribute set that is tied to this parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static double? GetMaxAttributeValue(this Parameter parameter)
        {
            if (!parameter.AttributeSets.NameIsUsed[_parameterValidationSetName]) return null;

            AttributeSet attSet = parameter.AttributeSets[_parameterValidationSetName];

            if (!attSet.NameIsUsed[_maxAttributeName]) return null;

            return (double?)attSet[_maxAttributeName].Value;
        }

        #endregion
    }

}
