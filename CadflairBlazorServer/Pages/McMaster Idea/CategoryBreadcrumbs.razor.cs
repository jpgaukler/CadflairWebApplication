using Microsoft.AspNetCore.Components;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class CategoryBreadcrumbs
    {
        // parameters
        [Parameter] public ProductCategory? ProductCategory { get; set; }
        [Parameter] public EventCallback<ProductCategory> ProductCategoryClicked { get; set; }

        // fields
        private List<BreadcrumbItem> _breadcrumbItems = new();

        protected override void OnParametersSet()
        {
            _breadcrumbItems.Clear();

            if (ProductCategory == null)
                return;

            // add selected folder 
            ProductCategory category = ProductCategory;
            _breadcrumbItems.Add(new BreadcrumbItem(text: category.Name, href: null));

            // add parents
            while (category.ParentCategory != null)
            {
                category = category.ParentCategory;
                _breadcrumbItems.Add(new BreadcrumbItem(text: category.Name, href: null));
            }

            // reverse the list so the breadcrumbs are displayed from the top down
            _breadcrumbItems.Reverse();
        }

        private async Task BreadcrumbItem_OnClick(BreadcrumbItem breadcrumbItem)
        {
            if (ProductCategory == null)
                return;

            if (ProductCategory.Name == breadcrumbItem.Text)
                return;

            ProductCategory category = ProductCategory;

            while (category.ParentCategory != null)
            {
                category = category.ParentCategory;

                if (category.Name == breadcrumbItem.Text)
                {
                    await ProductCategoryClicked.InvokeAsync(category);
                    break;
                }
            }
        }
    }
}