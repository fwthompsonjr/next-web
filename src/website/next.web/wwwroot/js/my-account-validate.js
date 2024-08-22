
let permissions_general = {
    "change_form": {
        "alertTimer": null,
        "button": "#frm-permissions-submit-button",
        "icons": {
            "normal": "#form-permissions-icon",
            "spin": "#form-permissions-icon-spin"
        },
        "error": "#frm-permissions-error-message",
        "setIcon": function (isSpinning) {
            const dsb = "disabled";
            const hddn = "d-none";
            let frm = permissions_general.change_form;
            let icons = [frm.icons.normal, frm.icons.spin];
            var bttn = frm.button;
            let a = isSpinning ? 0 : 1;
            let b = isSpinning ? 1 : 0;
            $(icons[a]).addClass(hddn);
            $(icons[b]).removeClass(hddn);
            if (isSpinning) {
                $(bttn).attr(dsb, dsb);
            } else {
                $(bttn).removeAttr(dsb);
            }
        },
        "setStatus": function (message, isActive) {
            const dsb = "disabled";
            const hddn = "d-none";
            const emptymessage = "<!-- Error messages -->"
            let frm = permissions_general.change_form;
            var dverr = frm.error
            if (isActive) {
                $(dverr).removeClass(hddn);
                $(dverr).html(message);
            } else {
                $(dverr).addClass(hddn);
                $(dverr).html(emptymessage);
            }
        },
        "setAlert": function (isActive) {
            const hddn = "d-none";
            const parentDv = "#dv-subcontent-permissions-row-00";
            const alertDv = "#dv-subcontent-permissions-success";
            let frm = permissions_general.change_form;
            if (null != frm.alertTimer) {
                clearTimeout(frm.alertTimer);
                frm.alertTimer = null;
            }
            $(parentDv).addClass(hddn);
            if (!isActive) { return; }
            $(alertDv).hide();
            $(parentDv).removeClass(hddn);
            $(alertDv).fadeIn(600);
            frm.alertTimer = setTimeout(() => {
                frm.setAlert(false);
            }, 15000);
        }
    }
}
let permissions_data = {
    "group_panel": "#dv-subcontent-permissions",
    "change_panel": "#dv-subcontent-password",
    "change_form": {
        "name": "form-change-password",
        "alertTimer": null,
        "button": "#frm-change-password-submit-button",
        "icons": {
            "normal": "#form-change-password-icon",
            "spin": "#form-change-password-icon-spin"
        },
        "error": "#frm-change-password-error-message",
        "setIcon": function (isSpinning) {
            const dsb = "disabled";
            const hddn = "d-none";
            let frm = permissions_data.change_form;
            let icons = [frm.icons.normal, frm.icons.spin];
            var bttn = frm.button;
            let a = isSpinning ? 0 : 1;
            let b = isSpinning ? 1 : 0;
            $(icons[a]).addClass(hddn);
            $(icons[b]).removeClass(hddn);
            if (isSpinning) {
                $(bttn).attr(dsb, dsb);
            } else {
                $(bttn).removeAttr(dsb);
            }
        },
        "setStatus": function (message, isActive) {
            const dsb = "disabled";
            const hddn = "d-none";
            const emptymessage = "<!-- Error messages -->"
            var dverr = permissions_data.change_form.error
            if (isActive) {
                $(dverr).removeClass(hddn);
                $(dverr).html(message);
            } else {
                $(dverr).addClass(hddn);
                $(dverr).html(emptymessage);
            }
        },
        "setAlert": function (isActive) {
            const hddn = "d-none";
            const parentDv = "#dv-subcontent-password-row-00";
            const alertDv = "#dv-subcontent-password-success";
            let frm = permissions_data.change_form;
            if (null != frm.alertTimer) {
                clearTimeout(frm.alertTimer);
                frm.alertTimer = null;
            }
            $(parentDv).addClass(hddn);
            if (!isActive) { return; }
            $(alertDv).hide();
            $(parentDv).removeClass(hddn);
            $(alertDv).fadeIn(600);
            frm.alertTimer = setTimeout(() => {
                frm.setAlert(false);
            }, 15000);
        }
    },
    "cbox": "#cbo-permissions-group",
    "password_shown": function () {
        let backup = $("#account-user-name").text();
        let currentName = $("#account-password-username").val();
        if (null != currentName && currentName.trim().length > 2) { return; }
        $("#account-password-username").val(backup);
    },
    "discounts": function () {
        let isContentVisible = $(permissions_data.group_panel).is(":visible");
        let isComboSelected = $(permissions_data.cbox).val() == "03";
        if (!isContentVisible || !isComboSelected) { return ""; }
        let choices = [];
        let state_selector = "#permissions-discounts-states-group input[type='checkbox']";
        let counties_selector = "#permissions-discounts-counties-group input[type='checkbox']";
        let selections = [state_selector, counties_selector];
        selections.forEach(s => {
            $(s).each(function () {
                const m = $(this);;
                const arr = m.attr("id").split("-");
                var obj = {
                    "StateName": arr[2],
                    "IsSelected": m.is(":checked"),
                    "CountyName": ""
                };
                if (arr.length > 3) { obj["CountyName"] = arr[3]; }
                choices.push(obj);
            });
        });
        return JSON.stringify(choices);
    },
    "subscriptions": function () {
        let isContentVisible = $(permissions_data.group_panel).is(":visible");
        let isComboSelected = $(permissions_data.cbox).val() == "02";
        if (!isContentVisible || !isComboSelected) { return ""; }
        const grp = "guest";
        let group_selector = "#permissions-subscription-group input[type='radio']:checked";
        let selection = $(group_selector).attr('value').toLowerCase();
        if (!(selection)) { selection = grp; }
        return JSON.stringify({ "Level": selection });
    },
    "changerequest": function () {
        let isContentVisible = $(permissions_data.change_panel).is(":visible");
        let isContentValid = $("#" + permissions_data.change_form.name).valid();
        if (!isContentVisible || !isContentValid) { return ""; }
        let pword = {
            "userName": $("#account-password-username").val(),
            "oldPassword": $("#account-password-old-password").val(),
            "newPassword": $("#account-password-new-password").val(),
            "confirmPassword": $("#account-password-confirmation").val()
        };
        return JSON.stringify(pword);
    },
    "submission": function () {
        const frmname = "permissions-subscription-group";
        const jsobj = window.jsHandler;
        let dta = {
            "subscription": permissions_data.subscriptions(),
            "discounts": permissions_data.discounts(),
            "changes": permissions_data.changerequest()
        }
        if (dta.subscription.length == 0 && dta.discounts.length == 0 && dta.changes.length == 0) return false;
        const isChangePassword = dta.changes.length > 0;
        const targetScreen = isChangePassword ? permissions_data.change_form : permissions_general.change_form;
        targetScreen.setIcon(false);
        targetScreen.setAlert(false);
        targetScreen.setStatus('', false);
        if (undefined === jsobj || null === jsobj) return true;
        try {
            const submitmessage = "<span class='text-warning'>Submitting form values ...</span>";
            targetScreen.setStatus(submitmessage, true);
            targetScreen.setIcon(true);
            targetScreen.setStatus('', false);
            targetScreen.setAlert(true);
            permisionChangeRequested();
            let js = JSON.stringify(dta);
            jsobj.submit(frmname, js);
        } catch (err) {
            targetScreen.setStatus(err, true);
        } finally {
            targetScreen.setIcon(false);
        }
        return false;
    }
};
function permisionChangeRequested() {
    var dv = document.getElementById('dv-subcontent-permissions-success');
    var spn = dv.getElementsByTagName('span')[0];
    dv.classList.remove('alert-success');
    dv.classList.add('alert-warning');
    spn.innerHTML = 'Please wait... submitting account change details';
    var elementStyle = dv.style;
    elementStyle.position = "relative";
    elementStyle.left = "45px";
    elementStyle.top = "10px";
    var bttn = document.getElementById('frm-permissions-submit-button');
    var iconOne = bttn.getElementsByTagName("i")[0];
    var iconSpin = bttn.getElementsByTagName("i")[1];
    var dnone = "d-none";
    iconOne.classList.add(dnone);
    iconSpin.classList.remove(dnone);
    bttn.setAttribute("disabled", "disabled");
}
function changePasswordSubmitButtonClicked() {
    permissions_data.submission();
}
function changePermissionsButtonClicked() {
    permissions_data.submission();
}
let profileAlertTimer = null;
let profileNames = ["frm-profile-address", "frm-profile-personal", "frm-profile-phone", "frm-profile-email"];
let profileButtonName = "frm-profile-submit-button";
let profileStatusDiv = "frm-profile-error-message";
var profileicons = {
    spin: "#form-profile-icon-spin",
    normal: "#form-profile-icon"
}
function setSuccessAlert(isActive) {
    const hddn = "d-none";
    const parentDv = "#dv-subcontent-profile-row-00";
    const alertDv = "#dv-subcontent-profile-success";
    if (null != profileAlertTimer) {
        clearTimeout(profileAlertTimer);
        profileAlertTimer = null;
    }
    $(parentDv).addClass(hddn);
    if (!isActive) { return; }
    $(alertDv).hide();
    $(parentDv).removeClass(hddn);
    $(alertDv).fadeIn(600);
    profileAlertTimer = setTimeout(() => { setSuccessAlert(false); }, 15000);
}
function setProfileStatusMessage(message, isActive) {
    const dsb = "disabled";
    const hddn = "d-none";
    const emptymessage = "<!-- Error messages -->"
    const pound = "#";
    var dverr = pound + profileStatusDiv
    if (isActive) {
        $(dverr).removeClass(hddn);
        $(dverr).html(message);
    } else {
        $(dverr).addClass(hddn);
        $(dverr).html(emptymessage);
    }
}
function setProfileIconState(isSpinning) {
    const dsb = "disabled";
    const hddn = "d-none";
    const pound = "#";
    let icons = [];
    var bttn = pound + profileButtonName;
    icons.push(profileicons.normal);
    icons.push(profileicons.spin);
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
function profileActionCompleted() {
    setProfileIconState(false);
    setProfileStatusMessage('', false);
    setSuccessAlert(true);
}
function isValidProfileEmail(email) {
    if (undefined === email || null === email || email.length === 0) { return true; }
    const res = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return res.test(email.toLowerCase());
}
function isValidProfilePhone(phonenumber) {
    if (undefined === phonenumber || null === phonenumber || phonenumber.length === 0) { return true; }
    var phoneno = /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    var phoneexp = /^[\s()+-]*([0-9][\s()+-]*){6,20}$/;
    var expressions = [phoneno, phoneexp];
    for (var t = 0; t < expressions.length; t++) {
        var rg = expressions[t];
        var isOk = rg.test(phonenumber);
        if (isOk) return true;
    }
    return false;
}
function isValidProfileAddress(address) {
    if (undefined === address || null === address || address.length === 0) { return true; }
    address = address.trim();
    if (address.length < 9) return false;
    const space = ' ';
    const comma = ',';
    if (address.indexOf(space) < 0) return false;
    if (address.indexOf(comma) < 0) return false;
    let parts = address.split(space);
    let zip = parts[parts.length - 1];
    if (!isZip(zip)) return false;
    return true;
}
function isZip(zip) {
    if (undefined === zip || null === zip || zip.length === 0) { return false; }
    var zp = /(^\d{5}$)|(^\d{5}-\d{4}$)/;
    return zp.test(zip);
}
function initializeProfileValidator() {
    /* 
    * NOTE: There are multiple forms on this page 
    * initialization of the jquery validators needs to be consolidated 
    * to include both profile and permissions forms 
    */
    jQuery.validator.setDefaults({
        debug: true,
        success: "valid"
    });
    jQuery.validator.addMethod("isValidProfileEmail", function (value, element, params) {
        if (!value) { return true; }
        if (value == null) { return true; }
        if (value.length == 0) { return true; }
        return isValidProfileEmail(String(value));
    }, 'Must be a valid email address.');
    jQuery.validator.addMethod("isValidProfilePhone", function (value, element, params) {
        if (!value) { return true; }
        if (value == null) { return true; }
        if (value.length == 0) { return true; }
        return isValidProfilePhone(String(value));
    }, 'Must be a valid phone number.');
    jQuery.validator.addMethod("isValidProfileAddress", function (value, element, params) {
        if (!value) { return true; }
        if (value == null) { return true; }
        if (value.length == 0) { return true; }
        return isValidProfileAddress(String(value));
    }, 'Must be a valid address.');
    jQuery.validator.addMethod("isPassword", function (value, element, params) {
        if (!value) { return false; }
        if (value == null) { return false; }
        if (value.length == 0) { return false; }
        const res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$/im;
        return res.test(String(value));
    }, 'Password must meet minimum requirement.');
}
function initializeProfileFormValidation() {
    let form_names = [];
    profileNames.forEach(p => form_names.push(p));
    form_names.push('form-change-password');
    for (var i = 0; i < form_names.length; i++) {
        var frm = document.getElementById(form_names[i]);
        var formJq = "#" + form_names[i];
        switch (i) {
            case 0: // address 
                $(formJq).validate({
                    rules: {
                        "billing-address": {
                            maxlength: 750,
                            isValidProfileAddress: true
                        },
                        "mailing-address": {
                            maxlength: 750,
                            isValidProfileAddress: true
                        }
                    }
                });
                break;
            case 1: // personal 
                $(formJq).validate({
                    rules: {
                        "first-name": {
                            required: false,
                            minlength: 1,
                            maxlength: 75
                        },
                        "last-name": {
                            required: false,
                            minlength: 1,
                            maxlength: 125
                        },
                        "company-name": {
                            required: false,
                            minlength: 2,
                            maxlength: 150
                        }
                    }
                });
                break;
            case 2: // phone 
                $(formJq).validate({
                    rules: {
                        "phone-number": {
                            required: false,
                            minlength: 7,
                            maxlength: 30,
                            isValidProfilePhone: true
                        }
                    }
                });
                break;
            case 3: // email 
                $(formJq).validate({
                    rules: {
                        "email-address": {
                            required: false,
                            minlength: 4,
                            maxlength: 255,
                            isValidProfileEmail: true
                        }
                    }
                });
                break;
            case 4: // change-password 
                $(formJq).validate({
                    rules: {
                        "account-password": {
                            required: true,
                            minlength: 8,
                            maxlength: 255,
                            isPassword: true
                        },
                        "account-password-confirmation": {
                            minlength: 8,
                            maxlength: 255,
                            equalTo: "#account-password-new-password"
                        }
                    }
                });
                break;
        }
    }
}
function profileSubmitButtonClicked() {
    const cboxid = "#cbo-profile-group";
    const find = "~0";
    const frmprefix = "frm-profile-~0";
    const suffixes = ["personal", "address", "phone", "email"];
    setSuccessAlert(false);
    setProfileIconState(false);
    setProfileStatusMessage('', false);
    var current = $(cboxid).val();
    if (null === current || current.length === 0 || suffixes.indexOf(current) < 0) {
        return false;
    }
    let target = frmprefix.replace(find, current);
    if (document.getElementById(target) == null) { return; }
    let jqobj = "#" + target;
    if ($(jqobj).valid() == false) {
        setProfileStatusMessage('Please review submission, you have one or more invalid values.', true);
        return false;
    }
    let payload = "";
    switch (current) {
        case "personal":
            const personal = [
                { NameType: "First", Name: $("#tbx-profile-first-name").val() },
                { NameType: "Last", Name: $("#tbx-profile-last-name").val() },
                { NameType: "Company", Name: $("#tbx-profile-company").val() }
            ];
            payload = JSON.stringify(personal);
            break;
        case "address":
            const address = [
                { AddressType: "Mailing", Address: $("#tbx-profile-mailing-address").val() },
                { AddressType: "Billing", Address: $("#tbx-profile-billing-address").val() }
            ];
            payload = JSON.stringify(address);
            break;
        case "phone":
            const phone = [
                { PhoneType: "Personal", Phone: $("#tbx-profile-phone-01").val() },
                { PhoneType: "Business", Phone: $("#tbx-profile-phone-02").val() },
                { PhoneType: "Other", Phone: $("#tbx-profile-phone-03").val() },
            ];
            payload = JSON.stringify(phone);
            break;
        case "email":
            const email = [
                { EmailType: "Personal", Email: $("#tbx-profile-email-01").val() },
                { EmailType: "Business", Email: $("#tbx-profile-email-02").val() },
                { EmailType: "Other", Email: $("#tbx-profile-email-03").val() },
            ];
            payload = JSON.stringify(email);
            break;
    }
    if (null === payload || payload.length == 0) {
        setProfileStatusMessage('Error: Unable to capture form submission values', true);
        return false;
    }
    let handler = window.jsHandler;
    if (undefined === handler || null === handler || !(handler)) {
        setProfileStatusMessage('Error: Unable to bind form submission handler', true);
        return true;
    }
    try {
        setProfileIconState(true);
        setProfileStatusMessage("<span class='text-warning'>Submitting form values ...</span>", true);
        handler.submit(target, payload);
        return false;
    }
    catch (err) {
        setProfileStatusMessage(err, true);
    } finally {
        setProfileIconState(false);
    }
}
function permissionsComboBoxChanged() {
    const tilde = "~0";
    const permissionsCombo = "#cbo-permissions-group";
    const permissionsAreaName = "#dv-subcontent-permissions-row-~0";
    const permissionsAreas = [
        permissionsAreaName.replace(tilde, "02"),
        permissionsAreaName.replace(tilde, "03")
    ];
    permissionsAreas.forEach(p => { $(p).hide(); });
    let item = $(permissionsCombo).val();
    let itemName = permissionsAreaName.replace(tilde, item);
    $(itemName).fadeIn(600);
    subscriptionLevelChanged();
}
function subscriptionLevelChanged() {
    const paragraphName = "#subscription-description-~0"
    var dvDescription = "#dv-permissions-subscription-description";
    var selection = $("input[name='subscription-group']:checked").val();
    if (undefined === selection || null === selection || selection.length == 0) {
        selection = "Guest";
    }
    $(dvDescription).find("p").hide();
    $(dvDescription).find("p").removeClass("d-none");
    const paragraph = paragraphName.replace("~0", selection.toLowerCase());
    $(paragraph).fadeIn(200);
}
initializeProfileValidator();
initializeProfileFormValidation();
permissionsComboBoxChanged();