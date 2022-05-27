$(document).ready(function () {
    $("#paramContainer").load('configurator/custom-dresser.html', bindUIEvents);

    //initialize UI
    $('#loader').hide();
    $('#reviewSection').hide();
    $('#settingsSection').hide();
    $('#contactSection').hide();
});

//#region UI elements

function bindUIEvents() {
    //nav buttons
    $('#navCreateButton').bind('click', { id: '#createSection' }, showSection);
    $('#navReviewButton').bind('click', { id: '#reviewSection' }, showSection);
    $('#navReviewButton').bind('click', getBuckets);
    $('#navSettingsButton').bind('click', { id: '#settingsSection' }, showSection);
    $('#navContactButton').bind('click', { id: '#contactSection' }, showSection);

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
    connection = new signalR.HubConnectionBuilder().withUrl("api/signalr/designautomation").build();

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

var loadedBuckets = null;

async function getBuckets() {
    let startAt = null;
    //if (loadedBuckets != null) {
    //    startAt = loadedBuckets[loadedBuckets.length - 1].bucketKey;
    //}
    //console.log('start at: ' + startAt);

    $.ajax({
        url: 'api/forge/oss/buckets?startAt=' + startAt,
        type: 'GET',
        success: function (res) {
            loadedBuckets = res.buckets;

            let buckets = res.buckets;
            let bucketTemplate = document.getElementById('bucketTemplate').content;
            let bucketList = document.getElementById('bucketList');

            //clear the current buckets from the webpage
            bucketList.innerHTML = '';

            //add list of buckets to the page
            for (let i = 0; i < buckets.length; i++) {
                //console.log(buckets[i]);

                //clone template and populate bucket info
                let bucketView = bucketTemplate.cloneNode(true);
                bucketView.querySelector('.key').innerText = buckets[i].bucketKey;
                bucketView.querySelector('.create-date').innerText = 'Date created: ' + buckets[i].createdDate;
                bucketView.querySelector('.policy-key').innerText = 'Policy key: ' + buckets[i].policyKey;

                bucketView.querySelector('.bucket').addEventListener('click', getObjects);
                bucketView.querySelector('.delete-bucket-icon').addEventListener('click', deleteBucket);

                bucketList.appendChild(bucketView);
            }
        },
        error: function (err) {
            console.log(err);
            console.log(err.responseText);

            let bucketTemplate = document.getElementById('bucketTemplate').content;
            let bucketList = document.getElementById('bucketList');

            //add list of buckets to the page
            for (let i = 0; i < 11; i++) {
                //clone template and populate bucket info
                let bucketView = bucketTemplate.cloneNode(true);
                bucketList.appendChild(bucketView);
            }

        }
    });
}


async function deleteBucket() {
    let bucket = $(this).parent();
    let bucketKey = bucket.children('.key').text();
    console.log('Deleting bucket: ' + bucketKey);

    $.ajax({
        url: 'api/forge/oss/buckets/delete?bucketKey=' + bucketKey,
        type: 'DELETE',
        success: function (result) {
            console.log('Bucket deleted. ' + result);
            bucket.remove();
        },
        error: function (err) {
            console.log(err);
            console.log(err.responseText);
        }
    });
}

async function getObjects() {
    let bucketKey = $(this).children('.bucket-info').children('.key').text();
    console.log('Get Objects: ' + bucketKey);

    $.ajax({
        url: 'api/forge/oss/objects?bucketKey=' + bucketKey,
        type: 'GET',
        success: function (res) {
            let objects = res.objects;

            //add list of buckets to the page
            for (let i = 0; i < objects.length; i++) {
                console.log(objects[i]);
                if (objects[i].objectKey.includes('.zip')) {
                    //launchViewer(objects[i].objectId);
                    getThumbnail(objects[i].objectId, this);
                }
            }
        },
        error: function (err) {
            console.error(err);
            console.error(err.responseText);
        }
    });
}

async function getThumbnail(urn, bucketView) {
    console.log('Getting thumbnail: ' + urn);

    $.ajax({
        url: 'api/forge/modelderivative/thumbnail?urn=' + urn,
        type: 'GET',
        success: function (res) {
            let imageString = res.base64String;
            console.log(imageString);
            imageTag.addClass('image-added');
            imageTag.attr('src', `data:image/png;base64,${imageString}`);
        },
        error: function (err) {
            console.error(err);
            console.error("error:" + err.responseText);
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
        viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('forgeViewer2'), { extensions: ['Autodesk.DocumentBrowser'] });
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
    return fetch('api/forge/oauth/token')
        .then(res => res.json())
        .then(data => data.access_token)
        .catch(err => console.log(err));
}

//#endregion

