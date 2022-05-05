$(document).ready(function () {
    $(".form-control").blur(validateInputs);
    $('#viewModel').click(createDresserModel);

    startConnection();
});

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

    connection.on("onComplete", function (message) {
        console.log(message);
    });

    connection.on("downloadResult", function (url) {
        console.log('Download result file here: ' + url);
    }); 

    connection.on("translationComplete", function (urn) {
        launchViewer(urn);
    });
}

function validateInputs() {
    if ($('#dresserHeight').val()) {
        if ($('#dresserHeight').val() > 72) { $('#dresserHeight').val(72); }
        else if ($('#dresserHeight').val() < 24) { $('#dresserHeight').val(24); }
        else { $('#dresserHeight').val(Number($('#dresserHeight').val()).toFixed()); }
    }

    if ($('#dresserWidth').val()) {
        if ($('#dresserWidth').val() > 120) { $('#dresserWidth').val(120); }
        else if ($('#dresserWidth').val() < 24) { $('#dresserWidth').val(24); }
        else { $('#dresserWidth').val(Number($('#dresserWidth').val()).toFixed()); }
    }

    if ($('#dresserDepth').val()) {
        if ($('#dresserDepth').val() > 40) { $('#dresserDepth').val(40); }
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

function createDresserModel() {
    if (!$('#dresserHeight').val()) { console.log('height is empty'); return; }
    if (!$('#dresserWidth').val()) { console.log('width is empty'); return; }
    if (!$('#dresserDepth').val()) { console.log('depth is empty'); return; }
    if (!$('#legExposure').val()) { console.log('leg height is empty'); return; }

    validateInputs();

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
                if (res.result == 'Workitem started') {
                    console.log(res.result + '\n' +
                                '   Output bucket key: ' + res.outputBucketKey);
                }
                else if (res.result == 'Configuration already exists') {
                    console.log(res.result + '\n' +
                                '   Output bucket key: ' + res.outputBucketKey + '\n' +
                                '   Urn: ' + res.urn);
                    launchViewer(res.urn);
                }
            }
        });
    });
}




