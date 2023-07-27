using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Shared
{
    public partial class CatalogFolderBreadcrumbs
    {
        // parameters
        [Parameter] public CatalogFolder? CatalogFolder { get; set; }
        [Parameter] public EventCallback<CatalogFolder> CatalogFolderClicked { get; set; }

        // fields
        private List<BreadcrumbItem> _breadcrumbItems = new();

        protected override void OnParametersSet()
        {
            _breadcrumbItems.Clear();

            if (CatalogFolder == null)
                return;

            // add selected folder 
            CatalogFolder folder = CatalogFolder;
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
            if (CatalogFolder == null)
                return;

            if (CatalogFolder.DisplayName == breadcrumbItem.Text)
                return;

            CatalogFolder folder = CatalogFolder;

            while (folder.ParentFolder != null)
            {
                folder = folder.ParentFolder;

                if (folder.DisplayName == breadcrumbItem.Text)
                {
                    await CatalogFolderClicked.InvokeAsync(folder);
                    break;
                }
            }
        }
    }
}