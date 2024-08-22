if (typeof isValidEmail != 'function') {
    window.isValidEmail = function (email) {
        const res = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return res.test(email.toLowerCase());
    };
}
let authformvalidators = [
    {
        "name": "isEmailOrUser",
        "action": function (value, element, params) {
            if (!value) { return false; }
            if (value == null) { return false; }
            if (value.length == 0) { return false; }
            if (isValidEmail(String(value))) { return true; }
            return value.indexOf("@") < 0;
        },
        "message": "Must be a valid user name."
    },
    {
        "name": "isEmail",
        "action": function (value, element, params) {
            if (!value) { return false; }
            if (value == null) { return false; }
            if (value.length == 0) { return false; }
            return isValidEmail(String(value));
        },
        "message": "Must be a valid email address."
    },
    {
        "name": "isPassword",
        "action": function (value, element, params) {
            if (!value) { return false; }
            if (value == null) { return false; }
            if (value.length == 0) { return false; }
            const res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$/im;
            return res.test(String(value));
        },
        "message": "Password must meet minimum requirement."
    }
];
let jsreauthenticate = {
    "name": "form-re-authorize",
    "jqid": "#form-re-authorize",
    "bttn": "#form-re-authorize-submit",
    "init": function () {
        let formJq = jsreauthenticate.jqid;
        let buttonJq = jsreauthenticate.bttn;
        let submitaction = "jsreauthenticate.authenticate( 'user', 're-authorize')";
        $(buttonJq).attr("onclick", submitaction);
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
    },
    "authenticate": async function (usr, process) {
        if (usr != 'user' || process != 're-authorize') { return; }
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        let isformvalid = jsreauthenticate.isvalid();
        if (!isformvalid) { return; }
        let payload = jsreauthenticate.serialize();
        await handler.reauthenticate(payload);
    },
    "show": function () {
        const btn = "#btn-account-authorize-show";
        $(btn).click();
    },
    "update_session": function () {
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        handler.checkSession();
    },
    "isvalid": function () {
        let formJq = jsreauthenticate.jqid;
        $(formJq).validate();
        return $(formJq).valid();
    },
    "serialize": function () {
        const usr = "#form-re-authorize-username";
        const pwd = "#form-re-authorize-password";
        let js = {
            "username": $(usr).val(),
            "login-password": $(pwd).val()
        };
        return JSON.stringify(js);
    }
};
authformvalidators.forEach(v => {
    if ($.validator.methods[v.name] == null) {
        $.validator.addMethod(v.name, v.action, v.message);
    }
});
jsreauthenticate.init();
let jsReauthorizeInterval = setInterval(() => { jsreauthenticate.update_session(); }, 15000); 