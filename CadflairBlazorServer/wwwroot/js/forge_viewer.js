var viewer;

async function launchViewer(params) {
    var options = {
        env: 'AutodeskProduction',
        accessToken: params.token
    };

    Autodesk.Viewing.Initializer(options, () => {
        viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('forgeViewer'), { extensions: ['Autodesk.DocumentBrowser'] });
        viewer.start();

        // load from a model derivative manifest using a urn, urn must be Base64 encoded
        //Autodesk.Viewing.Document.load(`urn:${params.urn}`, onDocumentLoadSuccess, onDocumentLoadFailure);

        // load from a url (this call redirects the viewer to a location on Forge OSS)
        Autodesk.Viewing.Document.load(`./api/v1/viewer_proxy/${params.bucketKey}/${params.objectKey}`, onDocumentLoadSuccess, onDocumentLoadFailure);
    });
}

function onDocumentLoadSuccess(doc) {
    var viewables = doc.getRoot().getDefaultGeometry();
    viewer.loadDocumentNode(doc, viewables).then(i => {
        // documented loaded, any action?
        console.log('Forge Viewer: onDocumentLoadSuccess()');
    });
}

function onDocumentLoadFailure(errorCode, errorMsg) {
    console.error('Forge Viewer: onDocumentLoadFailure() - errorCode:' + errorCode + '\n- errorMessage:' + errorMsg);
}


