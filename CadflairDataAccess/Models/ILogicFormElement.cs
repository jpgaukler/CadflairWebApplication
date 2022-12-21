using CadflairDataAccess.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Models
{
    /// <summary>
    /// Class used to store data related to an iLogic form. This is converted to Json to be stored in the Cadflair database.
    /// </summary>
    public class ILogicFormElement
    {
        //public string Guid { get; set; } 
        public string UiElementSpec { get; set; } 
        public string Name { get; set; } 
        public string EditControlType { get; set; }
        public bool ReadOnly { get; set; }
        public string ToolTip { get; set; }
        public string EnablingParameterName { get; set; }
        public double? TrackBarMinValue { get; set; }
        public double? TrackBarMaxValue { get; set; }
        public double? TrackBarIncrement { get; set; }
        public string ParameterName { get; set; }
        public string ParameterUnits { get; set; }

        [JsonIgnore]
        public string ParameterExpression { get; set; } = string.Empty;
        public string[] ParameterExpressionList { get; set; }
        public double? ParameterMinValue { get; set; }
        public double? ParameterMaxValue { get; set; }
        //public string Base64Image { get; set; }
        //public string Base64CaptionImage { get; set; }
        public List<ILogicFormElement> Items { get; set; }

        public string GetFormJson()
        {
            return this.ToJson();
        }

        public string GetArgumentJson()
        {
            Dictionary<string, string> expressions = GetParameterExpressions(Items);
            return expressions.ToJson();
        }

        public void SetParameterExpressions(string argumentJson)
        {
            Dictionary<string,string> expressions = JsonConvert.DeserializeObject<Dictionary<string,string>>(argumentJson);
            SetParameterExpressions(Items, expressions);
        }

        private Dictionary<string, string> GetParameterExpressions(List<ILogicFormElement> items, Dictionary<string, string> values = null)
        {
            if (values == null) values = new Dictionary<string, string>();

            foreach (ILogicFormElement item in items)
            {
                if (item.ParameterName != null && !values.ContainsKey(item.ParameterName)) values.Add(item.ParameterName, item.ParameterExpression);
                if (item.Items != null) GetParameterExpressions(item.Items, values);
            }

            return values;
        }

        private void SetParameterExpressions(List<ILogicFormElement> items, Dictionary<string, string> expressions)
        {
            foreach (ILogicFormElement item in items)
            {
                if (item.ParameterName != null && expressions.ContainsKey(item.ParameterName)) item.ParameterExpression = expressions[item.ParameterName];
                if (item.Items != null) SetParameterExpressions(item.Items, expressions);
            }
        }

    }
}
