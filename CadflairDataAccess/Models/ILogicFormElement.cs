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

        /// <summary>
        /// Get the entire representation of the Ilogic form as a json object. Null values and default values (empty string and arrays) will be ignored.
        /// </summary>
        /// <returns></returns>
        public string GetFormJson()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore //ignore empty strings and arrays, need to see if this ignores value of 0 for numeric data types
            };

            return JsonConvert.SerializeObject(this, settings); ;
        }

        /// <summary>
        /// Get the current value of all parameter expressions as a json object.
        /// </summary>
        /// <returns></returns>
        public string GetArgumentJson()
        {
            Dictionary<string, string> expressions = GetParameterExpressions(Items);
            return JsonConvert.SerializeObject(expressions);
        }

        /// <summary>
        /// Get a list of ILogicFormElements that represent parameters. Call SetParameterExpressions() to set the default values before calling this function.
        /// </summary>
        /// <returns></returns>
        public List<ILogicFormElement> GetParameterList()
        {
            return GetParameterList(Items);
        }

        /// <summary>
        /// Set the value of all parameter expressions from a json object.
        /// </summary>
        /// <param name="argumentJson"></param>
        public void SetParameterExpressions(string argumentJson)
        {
            Dictionary<string, string> expressions = JsonConvert.DeserializeObject<Dictionary<string, string>>(argumentJson);
            SetParameterExpressions(Items, expressions);
        }



        #region "recursive functions for getting and setting parameters"

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

        private List<ILogicFormElement> GetParameterList(List<ILogicFormElement> items, List<ILogicFormElement> parameterList = null)
        {
            if (parameterList == null) parameterList = new List<ILogicFormElement>();

            foreach (ILogicFormElement item in items)
            {
                if (item.ParameterName != null) parameterList.Add(item);
                if (item.Items != null) GetParameterList(item.Items, parameterList);
            }

            return parameterList;
        }

        #endregion
    }
}
