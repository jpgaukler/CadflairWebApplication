namespace CadflairBlazorServer.Data
{
    public class iLogicUiElement
    {
        public string UiElementSpec { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Guid { get; set; } = string.Empty;
        public string? ToolTip { get; set; }
        public string? EnablingParameterName { get; set; }
        public string? ParameterName { get; set; }
        public string? EditControlType { get; set; }
        public string? ReadOnly { get; set; }
        public iLogicUiElement[]? Items { get; set; }
    }
}
