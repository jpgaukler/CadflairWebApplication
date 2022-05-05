$(document).ready(function () {
    $('#startWorkitem').click(startWorkitem);
    $('#getWorkItemStatus').click(getWorkItemStatus);
    $('#viewModel').click(viewModel);
    $(".form-control").blur(validateInputs);

    startConnection();
});

function writelog(text) {
    $('#outputlog').append('<div style="border-top: 1px dashed #C0C0C0">' + text + '</div>');
    var elem = document.getElementById('outputlog');
    elem.scrollTop = elem.scrollHeight;
}

var connection;
var connectionId;

function startConnection(onReady) {
    if (connection && connection.connectionState) { if (onReady) onReady(); return; }
    connection = new signalR.HubConnectionBuilder().withUrl("/api/signalr/designautomation").build();
    connection.start()
        .then(function () {
            connection.invoke('getConnectionId')
                .then(function (id) {
                    connectionId = id; // we'll need this...
                    if (onReady) onReady();
                });
        });

    connection.on("downloadResult", function (url) {
        console.log('Download result file here: ' + url);
    });

    connection.on("onComplete", function (message) {
        console.log(message);
    });

    connection.on("translationComplete", function (urn) {
        launchViewer(urn);
    });

    //connection.on("readyForTranslation", function (urn) {
    //    //console.log(res);
    //    //console.log('id: ' + id);
    //    console.log('urn: ' + urn);
    //    //console.log('rootfilename: ' + rootFileName);
    //});
}

function validateInputs() {
    if ($('#dresserHeight').val()) {
        if ($('#dresserHeight').val() > 240) { $('#dresserHeight').val(240); }
        else if ($('#dresserHeight').val() < 24) { $('#dresserHeight').val(24); }
        else { $('#dresserHeight').val(Number($('#dresserHeight').val()).toFixed()); }
    }

    if ($('#dresserWidth').val()) {
        if ($('#dresserWidth').val() > 240) { $('#dresserWidth').val(240); }
        else if ($('#dresserWidth').val() < 24) { $('#dresserWidth').val(24); }
        else { $('#dresserWidth').val(Number($('#dresserWidth').val()).toFixed()); }
    }

    if ($('#dresserDepth').val()) {
        if ($('#dresserDepth').val() > 72) { $('#dresserDepth').val(72); }
        else if ($('#dresserDepth').val() < 18) { $('#dresserDepth').val(18); }
        else { $('#dresserDepth').val(Number($('#dresserDepth').val()).toFixed()); }
    }

    if ($('#legExposure').val()) {
        var maxLegHeight;
        if ($('#dresserHeight').val() > 10) {
            maxLegHeight = $('#dresserHeight').val() - 10
        }
        else {
            maxLegHeight = 100
        }

        if ($('#legExposure').val() > maxLegHeight) { $('#legExposure').val(maxLegHeight); }
        else if ($('#legExposure').val() < 1) { $('#legExposure').val(1); }
        else { $('#legExposure').val(Number($('#legExposure').val()).toFixed()); }
    }
}

var outputBucketKey = null;
var active_workItemID = null;

function startWorkitem() {
    startConnection(function () {
        var formData = new FormData();
        formData.append('data', JSON.stringify({
            activityName: "jgaukler.UpdateDresserParams+test_version",
            browerConnectionId: connectionId,
            dresserWidth: $('#dresserWidth').val(),
            dresserHeight: $('#dresserHeight').val(),
            dresserDepth: $('#dresserDepth').val(),
            legExposure: $('#legExposure').val(),
            drawerRows: $('#rowsDropdown').val(),
            drawerColumns: $('#columnsDropdown').val(),
            finishStyle: $('#finishDropdown').val(),
            edgeStyle: $('#edgeDropdown').val(),
            handleStyle: $('#handleDropdown').val()
        }));

        console.log('Starting work item...');
        $.ajax({
            url: 'api/forge/designautomation/workitems/createdressermodel',
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (res) {
                active_workItemID = res.workItemId;
                outputBucketKey = res.outputBucketKey;
                console.log(res.result + res.workItemId + ' Output bucket key: ' + res.outputBucketKey);
            }
        });
    });
}

function getWorkItemStatus() {
    $.ajax({
        type: 'GET',
        url: 'api/forge/designautomation/workitems/workitemid',
        data: { id: active_workItemID },
        success: function (res) {
            console.log(res.report);
        }
    })
}

function getObjectURN(bucketKey, objectName) {
    return $.ajax({
        type: 'GET',
        url: 'api/forge/oss/buckets/bucket/object',
        data: { bucketKey: bucketKey, objectName: objectName },
    })
        .then(data => data.objectURN)
        .catch(err => null);
}

function getForgeToken() {
    return fetch('/api/forge/oauth/token')
        .then(res => res.json())
        .then(data => data.access_token)
        .catch(err => null);
}

function getTranslation(urn, access_token) {
    return $.ajax({
        type: 'GET',
        url: 'https://developer.api.autodesk.com/modelderivative/v2/designdata/' + urn + '/manifest',
        headers: { 'Authorization': 'Bearer ' + access_token },
    })
        .then(res => res.status)
        .catch(err => null);
}

//function translateObject(urn, rootFileName) {
//    $.ajax({
//        type: 'POST',
//        url: '/api/forge/modelderivative/jobs',
//        contentType: 'application/json',
//        data: JSON.stringify({ 'objectURN': urn, 'rootFileName': rootFileName }),
//    })
//        .then(res => console.log('Translation started! Please wait a moment.'))
//        .catch(err => console.log('The translation could not be started.'));

//}

function translateObject(urn, rootFileName) {
    $.ajax({
        type: 'POST',
        url: '/api/forge/modelderivative/jobs',
        contentType: 'application/json',
        data: JSON.stringify({ 'objectURN': urn, 'rootFileName': rootFileName }),
    })
        .then(res => console.log('Translation started! Please wait a moment.'))
        .catch(err => console.log('The translation could not be started.'));

}

async function viewModel() {
    if (!$('#dresserHeight').val()) { console.log('height is empty'); return; }
    if (!$('#dresserWidth').val()) { console.log('width is empty'); return; }
    if (!$('#dresserDepth').val()) { console.log('depth is empty'); return; }
    if (!$('#legExposure').val()) { console.log('leg height is empty'); return; }

    const urn = await getObjectURN(outputBucketKey, "Dresser.zip");
    if (!urn) {
        //this object is not found so we need to start a new workitem to generate it
        if (active_workItemID) { console.log('a workitem is already running'); return; }
        else { startWorkitem(); return; }
    }
    console.log('urn: ' + urn);

    const access_token = await getForgeToken();
    if (!access_token) {
        console.log('error, no access token'); return;
    }

    const translation = await getTranslation(urn, access_token);
    if (!translation) {
        //no translation was found so we need to translate the object in order to view it
        translateObject(urn, "Dresser Assembly.iam");
        return;
    }

    if (translation === 'success') {
        //show the model to the user!
        launchViewer(urn);
        //reset values to null so that another request can be started
        active_workItemID = null;
        outputBucketKey = null;
        return;
    }
    else {
        console.log('The translation job still running. Please try again in a moment.'); return;
    }
}




