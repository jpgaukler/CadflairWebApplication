using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.Cad_Drive;

public partial class DriveFolderBreadcrumbs
{
    // parameters
    [Parameter] public DriveFolder? DriveFolder { get; set; }
    [Parameter] public EventCallback<DriveFolder> DriveFolderClicked { get; set; }

    // fields
    private List<BreadcrumbItem> _breadcrumbItems = new();

    protected override void OnParametersSet()
    {
        _breadcrumbItems.Clear();

        if (DriveFolder == null)
            return;

        // add selected folder 
        DriveFolder folder = DriveFolder;
        _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null));

        // add parents
        while (folder.ParentFolder != null)
        {
            folder = folder.ParentFolder;
            _breadcrumbItems.Add(new BreadcrumbItem(text: folder.DisplayName, href: null));
        }

        // reverse the list so the breadcrumbs are displayed from the top down
        _breadcrumbItems.Reverse();
    }

    private async Task BreadcrumbItem_OnClick(BreadcrumbItem breadcrumbItem)
    {
        if (DriveFolder == null)
            return;

        if (DriveFolder.DisplayName == breadcrumbItem.Text)
            return;

        DriveFolder folder = DriveFolder;

        while (folder.ParentFolder != null)
        {
            folder = folder.ParentFolder;

            if (folder.DisplayName == breadcrumbItem.Text)
            {
                await DriveFolderClicked.InvokeAsync(folder);
                break;
            }
        }
    }
}