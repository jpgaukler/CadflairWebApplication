namespace CadflairDataAccess.Models
{
    public class ILogicUiElement
    {
        public string UiElementSpec { get; set; } 
        public string Name { get; set; } 

        //public string Guid { get; set; } 
        public string ToolTip { get; set; }
        public string EnablingParameterName { get; set; }
        public string ParameterName { get; set; }
        public string ParameterUnits { get; set; }
        public string ParameterExpression { get; set; }
        public string[] ParameterExpressionList { get; set; }
        public double? ParameterMinValue { get; set; }
        public double? ParameterMaxValue { get; set; }
        public string EditControlType { get; set; }
        public bool ReadOnly { get; set; }

        //public string Base64Image { get; set; }
        //public string Base64CaptionImage { get; set; }

        public ILogicUiElement[] Items { get; set; }
    }
}
