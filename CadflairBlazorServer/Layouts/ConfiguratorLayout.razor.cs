using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Layouts
{
    public partial class ConfiguratorLayout
    {
        // services
        [Inject] NavigationManager _navManager { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] IJSRuntime _js { get; set; } = default!;


        private MudTheme _customTheme = new()
        {
            Palette = new PaletteLight()
            {
                //Primary = Colors.Blue.Default,
                Primary = "#50C0FF",
                AppbarBackground = Colors.Grey.Darken4,
                //AppbarBackground = Colors.Grey.Darken3,
            },
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.Blue.Lighten1
            },
            Typography = new()
            {
                H1 = new()
                {
                    FontSize = "4rem",
                }
            }
        };

        //protected override async Task OnInitializedAsync()
        //{
        //}
    }
}