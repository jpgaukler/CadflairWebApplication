using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared.Layouts
{
    public partial class MainLayout 
    {
        // services
        [Inject] NavigationManager _navManager { get; set; } = default!;
        [Inject] AuthenticationService _authenticationService { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] IJSRuntime _js { get; set; } = default!;

        // fields
        private MudThemeProvider? _mudThemeProvider;
        private bool _isDarkMode = false;
        private bool _navDrawerOpen = false;

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

        private async Task ContactUs_OnClick()
        {
            _navDrawerOpen = false;
            // navgate back to home page
            if (_navManager.BaseUri != _navManager.Uri)
            {
                _navManager.NavigateTo("/");
                await Task.Delay(500);
            }

            await _js.InvokeVoidAsync("anchorLink.scrollIntoView", "contact-us-tag");
        }

    }
}