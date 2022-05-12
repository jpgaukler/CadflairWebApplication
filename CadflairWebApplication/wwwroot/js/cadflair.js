$(document).ready(function () {
    $("#param-container").load('custom-dresser.html', bindUIEvents);

    //initialize UI
    $('#loader').hide();
    $('#review-section').hide();
    $('#settings-section').hide();
    $('#contact-section').hide();
});

//#region UI elements

function bindUIEvents() {
    //nav buttons
    $('#nav-createButton').bind('click', { id: '#create-section' }, showSection);
    $('#nav-reviewButton').bind('click', { id: '#review-section' }, showSection);
    $('#nav-reviewButton').bind('click', getBuckets);
    $('#nav-settingsButton').bind('click', { id: '#settings-section' }, showSection);
    $('#nav-contactButton').bind('click', { id: '#contact-section' }, showSection);

    //create buttons
    $('.param-group-header').bind('click', toggleParamGroup);
    $('#pdfButton').bind('click', openFile);
    $('#iamButton').bind('click', openFile);
    //$('#collapse-sidebar').bind('click', toggleSidebar);
}

function showSection(event) {
    $('section').each(function() {
        $(this).hide();
    });

    $(event.data.id).show();
}

function toggleParamGroup() {
    $(this).next().toggleClass('expanded');
    $(this).children('.chevron').toggleClass('expanded');
}

//function toggleSidebar()
//{
//    $('#sidebar-container').toggleClass('hide-sidebar');
//}

function showLoader(message) {
    $('#loaderMessage').text(message);
    $('#forgeViewer').hide();
    $('#loader').show();
}

function openFile() {
    window.open($(this).attr('href'), '_blank');
}

//#endregion

//#region SignalR

var connection = null;
var connectionId = null;
var workItemRunning = null;

async function startConnection() {
    connection = new signalR.HubConnectionBuilder().withUrl("/api/signalr/designautomation").build();

    //declare callback functions
    connection.on('downloadResult', (fileType, url) => {
        console.log(fileType + ' - ' + url);

        if (fileType == 'pdf') {
            $('#pdfButton').attr('href', url);
            $('#pdfButton').removeClass('disabled');
        }

        if (fileType == 'zip') {
            $('#iamButton').attr('href', url);
            $('#iamButton').removeClass('disabled');
        }
    });

    connection.on('onError', (error) => { console.log(error) });
    connection.on('onProgress', (message) => { console.log(message) });
    connection.on('translationRequested', () => showLoader('Generating preview...'));
    connection.on('translationComplete', launchViewer);
    connection.on('workItemComplete', (status) => {
        console.log(status);
        workItemRunning = false;
        $('#submitWorkitem').removeClass('disabled');
    });

    try {
        await connection.start();
        connectionId = await connection.invoke('getConnectionId')
        console.log('Signarl R connected - id: ' + connectionId);
    }
    catch (err) {
        console.log(err);
    }
}


//#endregion


//#region forge api requests 

var workItemRunning = false;
var currentBucketKey = null;

async function submitWorkItem(endpoint, formData) {
    if (workItemRunning == true) { console.log('workitem already running'); return; }

    showLoader('Connecting to server...');

    if (connection == null || connection.state == 'Disconnected') {
        await startConnection();
    }

    formData.append('connectionId', connectionId);

    console.log('Submitting workitem...');
    $.ajax({
        url: endpoint,
        data: formData,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (res) {

            $('#submitWorkitem').addClass('disabled');
            $('#pdfButton').addClass('disabled');
            $('#iamButton').addClass('disabled');

            if (res.result == 'Workitem started') {
                console.log(res.result + ' - Output bucket key: ' + res.outputBucketKey);
                currentBucketKey = res.outputBucketKey;
                workItemRunning = true;

                showLoader('Generating model...');
            }
            else if (res.result == 'Configuration already exists') {
                console.log(res.result + ' - Output bucket key: ' + res.outputBucketKey + ', Urn: ' + res.urn);
                currentBucketKey = res.outputBucketKey;
                launchViewer(res.urn);
            }
        }
    });
}

async function getBuckets() {
    console.log('Getting buckets...');

    $.ajax({
        url: 'api/forge/oss/buckets',
        type: 'GET',
        success: function (buckets) {
            console.log(buckets);
            buckets.each(function () {
                var template = document.getElementById('reviewBucketTemplate');
                var bucket = template.content.cloneNode(true);

                document.getElementById('review-section').appendChild(bucket);
            })
        }
    });
}

//#endregion

//#region forge viewer 

var viewer;

async function launchViewer(urn) {
    var access_token = await getForgeToken();

    var options = {
        env: 'AutodeskProduction',
        accessToken: access_token
    };

    $('#forgeViewer').show();
    $('#loader').hide();

    Autodesk.Viewing.Initializer(options, () => {
        viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('forgeViewer'), { extensions: ['Autodesk.DocumentBrowser'] });
        viewer.start();
        var documentId = 'urn:' + urn;
        Autodesk.Viewing.Document.load(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
    });
}

function onDocumentLoadSuccess(doc) {
    var viewables = doc.getRoot().getDefaultGeometry();
    viewer.loadDocumentNode(doc, viewables).then(i => {
        // documented loaded, any action?
    });
}

function onDocumentLoadFailure(errorCode, errorMsg) {
    console.error('onDocumentLoadFailure() - errorCode:' + errorCode + '\n- errorMessage:' + errorMsg);
}

function getForgeToken() {
    return fetch('/api/forge/oauth/token')
        .then(res => res.json())
        .then(data => data.access_token)
        .catch(err => null);
}

//#endregion

