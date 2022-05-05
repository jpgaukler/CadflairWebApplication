//var viewer;

//async function launchViewer(urn) {
//    var access_token = await getForgeToken();

//    var options = {
//        env: 'AutodeskProduction',
//        accessToken: access_token
//    };

//    Autodesk.Viewing.Initializer(options, () => {
//        viewer = new Autodesk.Viewing.GuiViewer3D($('#forgeViewer'), { extensions: ['Autodesk.DocumentBrowser'] });
//        viewer.start();
//        var documentId = 'urn:' + urn;
//        Autodesk.Viewing.Document.load(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
//    });
//}

//function onDocumentLoadSuccess(doc) {
//    var viewables = doc.getRoot().getDefaultGeometry();
//    viewer.loadDocumentNode(doc, viewables).then(i => {
//        // documented loaded, any action?
//    });
//}

//function onDocumentLoadFailure(errorCode, errorMsg) {
//    console.error('onDocumentLoadFailure() - errorCode:' + errorCode + '\n- errorMessage:' + errorMsg);
//}

//function getForgeToken() {
//    return fetch('/api/forge/oauth/token')
//        .then(res => res.json())
//        .then(data => data.access_token)
//        .catch(err => null);
//}