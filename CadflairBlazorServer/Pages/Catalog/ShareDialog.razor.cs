using Microsoft.AspNetCore.Components;
using QRCoder;

namespace CadflairBlazorServer.Pages.Catalog;

public partial class ShareDialog
{
    // parameters
    [CascadingParameter] public MudDialogInstance MudDialogInstance { get; set; } = default!;
    [Parameter] public string? Header { get; set; }
    [Parameter] public string Link { get; set; } = string.Empty;

    private string? _qrCodeImageAsBase64; 


    protected override void OnInitialized()
    {
        var options = new DialogOptions 
        { 
            CloseButton = false,
            DisableBackdropClick = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true,
            CloseOnEscapeKey = false 
        };

        MudDialogInstance.SetOptions(options);

        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(Link, QRCodeGenerator.ECCLevel.Q);
        Base64QRCode qrCode = new(qrCodeData);
        _qrCodeImageAsBase64 = qrCode.GetGraphic(20);
    }

    private void Ok_OnClick() => MudDialogInstance.Close();
}