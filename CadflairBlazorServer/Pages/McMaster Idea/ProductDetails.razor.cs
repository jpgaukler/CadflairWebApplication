using Microsoft.AspNetCore.Components;
using Row = CadflairDataAccess.Models.Row;

namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public partial class ProductDetails
    {
        // services
        [Inject] DataServicesManager  DataServicesManager { get; set; } = default!;
        [Inject] ForgeServicesManager  ForgeServicesManager { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ISnackbar Snackbar { get; set; } = default!;
        [Inject] IDialogService DialogService { get; set; } = default!;
        [Inject] ILogger<ProductDetails> Logger { get; set; } = default!;

        // parameters
        [Parameter] public string CompanyName { get; set; } = string.Empty;
        [Parameter] public string ProductDefinitionName { get; set; } = string.Empty;
        [Parameter] public string PartNumber { get; set; } = string.Empty;

        // fields
        private Subscription? _subscription;
        private ProductDefinition? _productDefinition;
        private ProductTable _productTable = new();
        private Row _row = new();
        private ForgeViewer? _forgeViewer;
        private Attachment? _2dAttachment;
        private Attachment? _3dAttachment;
        private Attachment? _activeAttachment;
        private Attachment? _selectedDownload;
        private bool _initializing = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _subscription = await DataServicesManager.SubscriptionService.GetSubscriptionBySubdirectoryName(CompanyName);
                _productDefinition = await DataServicesManager.McMasterService.GetProductDefinitionByNameAndSubscriptionId(ProductDefinitionName, _subscription.Id);
                _productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_productDefinition.Id);
                _row = _productTable.Rows.First(i => i.PartNumber == PartNumber);
                _selectedDownload = _row.Attachments.FirstOrDefault();
                _2dAttachment = _row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".pdf"));
                _3dAttachment = _row.Attachments.FirstOrDefault(i => i.ForgeObjectKey.Contains(".stp"));
                _initializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred while initializing product details page!");
                NavigationManager.NavigateTo("/notfound");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // load a model into the viewer
            if (_activeAttachment == null)
            {
                if (_3dAttachment != null)
                {
                    await View3D_OnClick();
                }
                else if (_2dAttachment != null)
                {
                    await View2D_OnClick();
                }
            }
        }

        private async Task View2D_OnClick()
        {
            //Snackbar.Add("calling 2D viewer load");

            if (_forgeViewer == null)
            {
                //Snackbar.Add("viewer is null");
                return;
            }

            if (_2dAttachment == null)
            {
                //Snackbar.Add("attachment is null");
                return;
            }

            _activeAttachment = _2dAttachment;
            await _forgeViewer.ViewDocument(_productDefinition!.ForgeBucketKey, _2dAttachment.ForgeObjectKey);
            StateHasChanged();
        }

        private async Task View3D_OnClick()
        {
            //Snackbar.Add("calling 3D viewer load");

            if (_forgeViewer == null)
            {
                //Snackbar.Add("viewer is null");
                return;
            }

            if (_3dAttachment == null)
            {
                //Snackbar.Add("attachment is null");
                return;
            }

            _activeAttachment = _3dAttachment;
            await _forgeViewer.ViewDocument(_productDefinition!.ForgeBucketKey, _3dAttachment.ForgeObjectKey);
            StateHasChanged();
        }

        private void View2DMobile_OnClick()
        {
            if (_2dAttachment == null)
                return;

            DialogParameters parameters = new()
            {
                { nameof(ViewerMobileDialog.BucketKey) , _productDefinition!.ForgeBucketKey },
                { nameof(ViewerMobileDialog.ObjectKey) , _2dAttachment.ForgeObjectKey },
            };

            DialogService.Show<ViewerMobileDialog>("View 2D", parameters);
        }

        private void View3DMobile_OnClick()
        {
            if (_3dAttachment == null)
                return;

            DialogParameters parameters = new()
            {
                { nameof(ViewerMobileDialog.BucketKey) , _productDefinition!.ForgeBucketKey },
                { nameof(ViewerMobileDialog.ObjectKey) , _3dAttachment.ForgeObjectKey },
            };

            DialogService.Show<ViewerMobileDialog>("View 3D", parameters);
        }

        private void ShareButton_OnClick()
        {
            DialogParameters parameters = new()
            {
                { nameof(ShareDialog.Link) , NavigationManager.Uri },
            };

            DialogService.Show<ShareDialog>("Share", parameters);
        }


        private async Task Download_OnClick()
        {
            if (_selectedDownload == null)
                return;

            string url = await ForgeServicesManager.ObjectStorageService.GetSignedDownloadUrl(_productDefinition!.ForgeBucketKey, _selectedDownload.ForgeObjectKey);
            NavigationManager.NavigateTo(url);
        }

        private void MorrisTable()
        {
            //string[] types = { "Pipe", "Tube" };
            //string[] pipeSchedules = { "Sch 5", "Sch 10", "Sch 40" };
            double[] bendAngles = { 30, 45, 90 };
            Dictionary<string, double> tubeGuages = new()
            {
                { "11 ga", .120 },
                { "14 ga", .083 },
                { "16 ga", .063 },
            };

            dynamic[] tubeSizes =
            {
                new 
                {
                    NominalSize = 1,
                    OuterDiameter = 1,
                    CLRs = new double[] { 3, 6, 12 },
                    Tangent = 4
                },
                new 
                {
                    NominalSize = 1.25,
                    OuterDiameter = 1.25,
                    CLRs = new double[] { 3, 5 },
                    Tangent = 4
                },
                new 
                {
                    NominalSize = 1.5,
                    OuterDiameter = 1.5,
                    CLRs = new double[] {2.5, 6, 7.5, 9, 12, 15, 18, 24, 30},
                    Tangent = 4
                },
                new 
                {
                    NominalSize = 1.75,
                    OuterDiameter = 1.75,
                    CLRs = new double[] {2.5, 8, 9, 12, 15, 24, 30, 36, 48},
                    Tangent = 4
                },
                new 
                {
                    NominalSize = 2,
                    OuterDiameter = 2,
                    CLRs = new double[] {2.5, 3, 3.5, 4, 5, 6, 8, 8.5, 9, 10, 12, 15, 18, 24, 30},
                    Tangent = 4
                },
                new 
                {
                    NominalSize = 2.5,
                    OuterDiameter = 2.5,
                    CLRs = new double[] {4, 6, 9, 12, 15, 17, 24, 30, 36, 48},
                    Tangent = 5
                },
                new 
                {
                    NominalSize = 3,
                    OuterDiameter = 3,
                    CLRs = new double[] {4, 4.5, 7.5, 9, 10, 12, 15, 18, 24, 30, 36, 48},
                    Tangent = 6
                },
                new 
                {
                    NominalSize = 3.5,
                    OuterDiameter = 3.5,
                    CLRs = new double[] {3.5, 4.5, 6, 8.75, 12, 15, 16, 18, 24, 30, 36, 48},
                    Tangent = 7
                },
                new 
                {
                    NominalSize = 4,
                    OuterDiameter = 4,
                    CLRs = new double[] {4, 5, 6, 7.75, 10, 12, 16, 18, 20, 24, 30, 32, 36, 48, 60},
                    Tangent = 8
                },
                new 
                {
                    NominalSize = 5,
                    OuterDiameter = 5,
                    CLRs = new double[] {5.5, 7.5, 12.5, 17, 22, 24, 30, 36, 42, 60},
                    Tangent = 10 
                },
                new 
                {
                    NominalSize = 6,
                    OuterDiameter = 6,
                    CLRs = new double[] {9, 15, 18, 24, 30, 36, 42, 48, 60, 72},
                    Tangent = 12 
                },
                new 
                {
                    NominalSize = 8,
                    OuterDiameter = 8,
                    CLRs = new double[] {12, 20, 32, 48, 60, 72},
                    Tangent = 16 
                },
            };

            foreach(var guage in tubeGuages )
            {
                foreach (var angle in bendAngles)
                {
                    foreach (var size in tubeSizes)
                    {
                        foreach (var clr in size.CLRs)
                        {
                            Console.WriteLine($"{size.NominalSize}, {guage.Key}, {size.OuterDiameter}, {guage.Value}, {clr}, {size.Tangent}, {angle}");
                        }
                    }
                }
            }


        }

    }
}