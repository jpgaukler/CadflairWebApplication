var viewer;

async function launchViewer(params) {
    var options = {
        env: 'AutodeskProduction',
        accessToken: params.token
    };

    //console.log(params);
    //console.log('token: ');
    //console.log(params.token);
    //console.log('urn: ' + params.urn);

    Autodesk.Viewing.Initializer(options, () => {
        viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('forgeViewer'), { extensions: ['Autodesk.DocumentBrowser'] });
        viewer.start();

        // urn must be Base64 encoded
        var documentId = 'urn:' + params.urn;
        Autodesk.Viewing.Document.load(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
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


