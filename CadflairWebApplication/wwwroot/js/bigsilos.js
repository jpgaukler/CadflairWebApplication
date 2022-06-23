function loadPageSpecificContent() {
    $("#paramContainer").load('html/parameters/silo-params.html', bindEvents);
}

function bindEvents() {
    $(".form-inputbox").blur(validateInputs);
    $('#submitWorkitem').click(createSiloModel);
}
function validateInputs() {
    if ($('#dresserHeight').val()) {
        if ($('#dischargeHeight').val() < 1) { $('#dischargeHeight').val(1); }
        else if ($('#dischargeHeight').val() > 120) { $('#dischargeHeight').val(120); }
        else { $('#dischargeHeight').val(Number($('#dischargeHeight').val()).toFixed()); }
    }

    if ($('#ladderAngle').val()) {
        if ($('#ladderAngle').val() < 0) { $('#ladderAngle').val(0); }
        else if ($('#ladderAngle').val() > 360) { $('#ladderAngle').val(360); }
        else { $('#ladderAngle').val(Number($('#ladderAngle').val()).toFixed()); }
    }
}

const modelBucketKey = 'cadflair.bigsilos.models';
const pdfBucketKey = 'cadflair.bigsilos.pdfs';
const stpBucketKey = 'cadflair.bigsilos.stps';

function createSiloModel() {
    if (workItemRunning == true) {console.log('workitem already running'); return;}
    if (!$('#dischargeHeight').val()) { alert('Please enter a value for the discharge height.'); return; }
    if (!$('#ladderAngle').val()) { alert('Please enter a value for the ladder location.'); return; }

    validateInputs();

    var formData = new FormData();

    //activity info
    formData.append('activityId', 'cadflair.CreateModelConfiguration+v1');
    formData.append('inputBucketKey', 'cadflair.silobasemodel');
    formData.append('inputObjectKey', 'Silo.zip');
    formData.append('pathInZip', 'Silo Configurator.ipt');
    formData.append('modelBucketKey', modelBucketKey);
    formData.append('pdfBucketKey', pdfBucketKey);
    formData.append('stpBucketKey', stpBucketKey);
    formData.append('outputObjectKey', $('#outputObjectKey').val());

    //inventor params
    formData.append('inventorParams', JSON.stringify({
        outputObjectKey: $('#outputObjectKey').val(),
        partNumber: '1234',
        innerDiam: $('#innerDiam').val() + " ft",
        siloHeight: $('#siloHeight').val() + " ft",
        coneAngle: $('#coneAngle').val() + " deg",
        outletDiam: $('#outletDiam').val() + " in",
        dischargeHeight: $('#dischargeHeight').val() + " in",
        ladderAngle: $('#ladderAngle').val() + " deg"
    }));

    createModelConfiguration('api/forge/designautomation/workitems/createmodelconfiguration', formData);
}

//function createSiloModel() {
//    if (workItemRunning == true) { console.log('workitem already running'); return; }
//    if (!$('#dischargeHeight').val()) { alert('Please enter a value for the discharge height.'); return; }
//    if (!$('#ladderAngle').val()) { alert('Please enter a value for the ladder location.'); return; }

//    validateInputs();

//    var formData = new FormData();
//    formData.append('inventorParams', JSON.stringify({
//        innerDiam: $('#innerDiam').val() + " ft",
//        siloHeight: $('#siloHeight').val() + " ft",
//        coneAngle: $('#coneAngle').val() + " deg",
//        outletDiam: $('#outletDiam').val() + " in",
//        dischargeHeight: $('#dischargeHeight').val() + " in",
//        ladderAngle: $('#ladderAngle').val() + " deg"
//    }));

//    submitWorkItem('api/forge/designautomation/workitems/createsilomodel', formData);
//}
