using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class AppBar
    {
        // services
        [Inject] NavigationManager NavManager { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        // fields 
        private bool _navDrawerOpen = false;

        private async Task ContactUs_OnClick()
        {
            _navDrawerOpen = false;
            // navgate back to home page
            if (NavManager.BaseUri != NavManager.Uri)
            {
                NavManager.NavigateTo("/");
                await Task.Delay(500);
            }

            await JSRuntime.InvokeVoidAsync("anchorLink.scrollIntoView", "contact-us-tag");
        }
    }
}