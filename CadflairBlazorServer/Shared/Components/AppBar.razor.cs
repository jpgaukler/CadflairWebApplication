using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared.Components
{
    public partial class AppBar
    {
        // services
        [Inject] NavigationManager _navManager { get; set; } = default!;
        [Inject] ISnackbar _snackbar { get; set; } = default!;
        [Inject] IJSRuntime _js { get; set; } = default!;

        // fields 
        private bool _navDrawerOpen = false;

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