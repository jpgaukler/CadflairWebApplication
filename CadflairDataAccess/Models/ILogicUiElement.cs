namespace CadflairDataAccess.Models
{
    public class ILogicUiElement
    {
        public string UiElementSpec { get; set; } 
        public string Name { get; set; } 
        public string Guid { get; set; } 
        public string ToolTip { get; set; }
        public string EnablingParameterName { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterMinValue { get; set; }
        public string ParameterMaxValue { get; set; }
        public string EditControlType { get; set; }
        public string ReadOnly { get; set; }
        public string Base64Image { get; set; }
        public string Base64CaptionImage { get; set; }
        public ILogicUiElement[] Items { get; set; }
    }
}
