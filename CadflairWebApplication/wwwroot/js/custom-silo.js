$(document).ready(function () {
    $(".form-inputbox").blur(validateInputs);
    $('#submitWorkitem').click(createSiloModel);
});

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


function createSiloModel() {
    if (workItemRunning == true) { console.log('workitem already running'); return; }
    if (!$('#dischargeHeight').val()) { alert('Please enter a value for the discharge height.'); return; }
    if (!$('#ladderAngle').val()) { alert('Please enter a value for the ladder location.'); return; }

    validateInputs();

    var formData = new FormData();
    formData.append('inventorParams', JSON.stringify({
        innerDiam: $('#innerDiam').val() + " ft",
        siloHeight: $('#siloHeight').val() + " ft",
        coneAngle: $('#coneAngle').val() + " deg",
        outletDiam: $('#outletDiam').val() + " in",
        dischargeHeight: $('#dischargeHeight').val() + " in",
        ladderAngle: $('#ladderAngle').val() + " deg"
    }));

    submitWorkItem('api/forge/designautomation/workitems/createsilomodel', formData);
}
