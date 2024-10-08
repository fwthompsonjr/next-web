let formNames = ["form-login", "form-register"];
let buttonNames = ["form-login-submit", "form-register-submit"];
let contacticons = {
    spin: "i[name='message-send-spinner']",
    normal: "i[name='message-send']"
}
function isValidEmail(email) {
    const res = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return res.test(email.toLowerCase());
}
function initializeValidator() {
    jQuery.validator.setDefaults({
        debug: true,
        success: "valid"
    });
    jQuery.validator.addMethod("isEmailOrUser", function (value, element, params) {
        if (!value) { return false; }
        if (value == null) { return false; }
        if (value.length == 0) { return false; }
        if (isValidEmail(String(value))) { return true; }
        return value.indexOf("@") < 0;
    }, 'Must be a valid user name.');
    jQuery.validator.addMethod("isEmail", function (value, element, params) {
        if (!value) { return false; }
        if (value == null) { return false; }
        if (value.length == 0) { return false; }
        return isValidEmail(String(value));
    }, 'Must be a valid email address.');
    jQuery.validator.addMethod("isPassword", function (value, element, params) {
        if (!value) { return false; }
        if (value == null) { return false; }
        if (value.length == 0) { return false; }
        const res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$/im;
        return res.test(String(value));
    }, 'Password must meet minimum requirement.');
}
function initializeFormValidation() {
    for (let i = 0; i < formNames.length; i++) {
        let formJq = "#" + formNames[i];
        switch (i) {
            case 0:
                $(formJq).validate({
                    rules: {
                        "username": {
                            required: true,
                            minlength: 8,
                            maxlength: 255,
                            isEmailOrUser: true
                        },
                        "login-password": {
                            required: true,
                            minlength: 8,
                            maxlength: 255,
                            isPassword: true
                        }
                    }
                });
                break;
            case 1:
                $(formJq).validate({
                    rules: {
                        "username": {
                            required: true,
                            minlength: 8,
                            maxlength: 255
                        },
                        "register-email": {
                            required: true,
                            minlength: 8,
                            maxlength: 255,
                            isEmail: true
                        },
                        "register-password": {
                            required: true,
                            minlength: 8,
                            maxlength: 255,
                            isPassword: true
                        },
                        "register-password-confirmation": {
                            minlength: 8,
                            maxlength: 255,
                            equalTo: "#register-password"
                        }
                    }
                });
                break;
        }
    }
}
function initializeFormButtons() {
    for (let i = 0; i < buttonNames.length; i++) {
        let bttn = document.getElementById(buttonNames[i]);
        if (undefined === bttn || null === bttn) { continue; }
        bttn.addEventListener("click", () => formButtonClicked(i));
    }
}
function initializeForms() {
    initializeValidator();
    initializeFormValidation();
    initializeFormButtons();
}
function handleApiResponse(indx, response) {
    if (isNaN(indx) || indx < 0 || indx > formNames.length || response.length == 0) { return ""; }
    // var obj = 
    alert(response);
}
function setIconState(indx, isSpinning) {
    const dsb = "disabled";
    const hddn = "d-none";
    const pound = "#";
    if (isNaN(indx) || indx < 0 || indx > formNames.length) { return }
    let icons = [];
    let formName = formNames[indx];
    let bttn = pound + buttonNames[indx];
    icons.push(pound + formName + "-icon");
    icons.push(pound + formName + "-icon-spin");
    let a = isSpinning ? 0 : 1;
    let b = isSpinning ? 1 : 0;
    $(icons[a]).addClass(hddn);
    $(icons[b]).removeClass(hddn);
    if (isSpinning) {
        $(bttn).attr(dsb, dsb);
    } else {
        $(bttn).removeAttr(dsb);
    }
}
function setStatusMessage(indx, message, isActive) {
    const hddn = "d-none";
    const emptymessage = "<!-- Error messages -->"
    const pound = "#";
    if (isNaN(indx) || indx < 0 || indx > formNames.length) { return }
    let formName = formNames[indx];
    let dverr = pound + formName + "-error-message"
    if (isActive) {
        $(dverr).removeClass(hddn);
        $(dverr).html(message);
    } else {
        $(dverr).addClass(hddn);
        $(dverr).html(emptymessage);
    }
}
function loginCompletedAction() {
    const indx = 0;
    const hddn = "d-none";
    const emptymessage = "<!-- Error messages -->"
    const pound = "#";
    let formName = formNames[indx];
    let dverr = pound + formName + "-error-message"
    $(dverr).addClass(hddn);
    $(dverr).html(emptymessage);
    $("#login-password").val("");
    $("#username, #login-password").attr("disabled", "disabled");
}
function formButtonClicked(formIndex) {
    let indx = parseInt(formIndex);
    try {
        if (isNaN(indx) || indx < 0 || indx > formNames.length) { return ""; }
        setIconState(indx, true);
        setStatusMessage(indx, "", false);
        let formName = formNames[indx];
        let formSelector = "#" + formName;
        let frm = $(formSelector);
        frm.validate();
        let isvalid = frm.valid();
        if (!isvalid) return;
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        let json = serializeFormToObject(indx);
        handler.submit(formName, json);
    } catch (err) {
        setStatusMessage(indx, err, true);
    }
    finally {
        setIconState(indx, false);
    }
}
//convert FormData to Object 
let serializeFormToObject = function (formIndex) {
    let indx = parseInt(formIndex);
    if (isNaN(indx) || indx < 0 || indx > formNames.length) return "";
    let formName = formNames[indx];
    let form = document.getElementById(formName);
    let objForm = {};
    let formData = new FormData(form);
    for (let key of formData.keys()) {
        objForm[key] = formData.get(key);
    }
    return JSON.stringify(objForm);
};
function docReady(fn) {
    // see if DOM is already available 
    if (document.readyState === "complete" || document.readyState === "interactive") {
        // call on next available tick 
        setTimeout(fn, 10);
    } else {
        document.addEventListener("DOMContentLoaded", fn);
    }
}
docReady(function () {
    initializeForms();
}); 