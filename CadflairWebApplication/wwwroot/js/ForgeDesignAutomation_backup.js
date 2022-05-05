$(document).ready(function () {
    //$('#getWorkItemStatus').click(getWorkItemStatus);
    $('#viewModel').click(viewModel);
    $(".form-control").blur(validateInputs);

    startConnection();
});

function writeLog(text) {
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
        writeLog('<a href="' + url + '">Download result file here</a>');
    });

    connection.on("onComplete", function (message) {
        writeLog(message);
    });
}

function getForgeToken(callback) {
    fetch('/api/forge/oauth/token').then(res => { res.json().then(data => { callback(data.access_token, data.expires_in); }); });
}

function validateInputs() {
    if ($('#dresserHeight').val()) {
        if ($('#dresserHeight').val() > 240) $('#dresserHeight').val(240);
        else $('#dresserHeight').val(Number($('#dresserHeight').val()).toFixed());
    }

    if ($('#dresserWidth').val()) {
        if ($('#dresserWidth').val() > 240) $('#dresserWidth').val(240);
        else $('#dresserWidth').val(Number($('#dresserWidth').val()).toFixed());
    }

    if ($('#dresserDepth').val()) {
        if ($('#dresserDepth').val() > 72) $('#dresserDepth').val(72);
        else $('#dresserDepth').val(Number($('#dresserDepth').val()).toFixed());
    }

    if ($('#legExposure').val()) {
        if ($('#legExposure').val() > 60) $('#legExposure').val(60);
        else { $('#legExposure').val(Number($('#legExposure').val()).toFixed()); }
    }
}

function startWorkitem() {
    if (!$('#dresserHeight').val()) { console.log('height is empty'); return; }
    if (!$('#dresserWidth').val()) { console.log('width is empty'); return; }
    if (!$('#dresserDepth').val()) { console.log('depth is empty'); return; }
    if (!$('#legExposure').val()) { console.log('leg height is empty'); return; }

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

        writeLog('Starting work item...');
        $.ajax({
            url: 'api/forge/designautomation/workitems',
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (res) {
                active_workItemID = res.workItemId;
                outputBucketKey = res.outputBucketKey;
                writeLog(res.result + res.workItemId + ' Output bucket key: ' + res.outputBucketKey);
            }
        });
    });
}

var active_workItemID = null;

function getWorkItemStatus() {
    $.ajax({
        type: 'GET',
        url: 'api/forge/designautomation/workitems/workitemid',
        data: { id: active_workItemID },
        success: function (res) {
            writeLog(res.report);
        }
    })
}

var outputBucketKey = null;

function viewModel() {
    var outputObjectName = "Dresser.zip";
    var rootFileName = "Dresser Assembly.iam";

    var getURN = $.ajax({
        type: 'GET',
        url: 'api/forge/oss/buckets/bucket/object',
        data: { bucketKey: outputBucketKey, objectName: outputObjectName },
    });

    //proceed if urn is found
    getURN.done(function (res) {
        var urn = res.objectURN;

        getForgeToken(function (access_token) {
            //see if this objext has been translated
            var translationStatus = $.ajax({
                type: 'GET',
                url: 'https://developer.api.autodesk.com/modelderivative/v2/designdata/' + urn + '/manifest',
                headers: { 'Authorization': 'Bearer ' + access_token },
            });

            //if so open the object on the viewer
            translationStatus.done(function (res) {
                if (res.status === 'success') launchViewer(urn);
                else writeLog('The translation job still running: ' + res.progress + '. Please try again in a moment.')
            });

            // if not then start the translation
            translationStatus.fail(function (err) {
                writeLog('file is not yet translated')

                var translateObject = $.ajax({
                    type: 'POST',
                    url: '/api/forge/modelderivative/jobs',
                    contentType: 'application/json',
                    data: JSON.stringify({ 'objectURN': urn, 'rootFileName': rootFileName }),
                });

                translateObject.done(function (res) { writeLog('Translation started! Please wait a moment.') });
                translateObject.fail(function (err) { writeLog('The translation could not be started.') });
            });
        });
    })

    //write error if request fails
    getURN.fail(function (err) { writeLog('No object found') })
}

