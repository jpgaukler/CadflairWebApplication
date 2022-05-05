$(document).ready(function () {
    $(".form-inputbox").blur(validateInputs);
    $('#submitWorkitem').click(createDresserModel);
});

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
    if (workItemRunning == true) {console.log('workitem already running'); return;}
    if (!$('#dresserHeight').val()) { alert('Please enter a value for the Dresser Height.'); return; }
    if (!$('#dresserWidth').val()) { alert('Please enter a value for the Dresser Width.'); return; }
    if (!$('#dresserDepth').val()) { alert('Please enter a value for the Dresser Depth.'); return; }
    if (!$('#legExposure').val()) { alert('Please enter a value for the Leg Height.'); return; }

    validateInputs();

    var formData = new FormData();
    formData.append('inventorParams', JSON.stringify({
        dresserWidth: $('#dresserWidth').val(),
        dresserHeight: $('#dresserHeight').val(),
        dresserDepth: $('#dresserDepth').val(),
        legExposure: $('#legExposure').val(),
        drawerRows: $('input[name="drawerRows"]:checked').val(),
        drawerColumns: $('input[name="drawerColumns"]:checked').val(),
        finishStyle: $('#finishDropdown').val(),
        edgeStyle: $('#edgeDropdown').val(),
        handleStyle: $('#handleDropdown').val()
    }));

    submitWorkItem('api/forge/designautomation/workitems/createdressermodel', formData);
}


//var connection = null;
//var connectionId = null;

//async function startConnection() {
//    connection = new signalR.HubConnectionBuilder().withUrl("/api/signalr/designautomation").build();

//    connection.on('onComplete', function (message) {
//        console.log(message);
//    });

//    connection.on('downloadZip', function (url) {
//        console.log('Download inventor files here: ' + url);
//    }); 

//    connection.on('downloadPdf', function (url) {
//        console.log('Download drawing Pdf here: ' + url);
//        toggleDownloadPdf(url);
//    }); 

//    connection.on('translationComplete', function (urn) {
//        workItemRunning = false;
//        launchViewer(urn);
//    });

//    try {
//        await connection.start();
//        console.log('Signal R connected');
//        connectionId = await connection.invoke('getConnectionId')
//        console.log('connectionId: ' + connectionId);       
//    } catch (err) {
//        console.log(err);
//    }
//}

