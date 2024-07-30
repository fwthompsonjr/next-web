let theHandler = {
    "checkSession": function () { },
    "pre_submit": function (formName) {
        switch (formName) {
            case "form-login":
                if (setIconState) { setIconState(0, true); }
                if (setStatusMessage) { setStatusMessage(0, 'Sending form data', true); }
                break;
            default:
        }
    },
    "post_submit": function (formName) {
        switch (formName) {
            case "form-login":
                if (setIconState) { setIconState(0, false); }
                if (setStatusMessage) { setStatusMessage(0, '', false); }
                break;
            default:
        }
    },
    "submit": function (formName, text) {
        const landing = "/data/submit"
        let requested = {
            "formName": String(formName),
            "payload": String(text)
        };
        theHandler.pre_submit(requested.formName);
        $.ajax({
            type: "POST",
            url: landing,
            data: JSON.stringify(requested),
            dataType: "json",
            success: function (resultData) {
                theHandler.post_submit(requested.formName);
                theResponder.handle(resultData);
            }
        });
    }
}

let theResponder = {
    "handle": function (data) {
        const current = theResponder.translate(data);
        if (current.statusCode == 200 && current.redirectTo.length > 0) {
            window.document.location = current.redirectTo;
            // return;
        }
    },
    "translate": function (json) {
        let rsp = {
            "statusCode": 0,
            "message": "",
            "redirectTo": "/home"
        };
        if (undefined == json || null == json) return rsp;
        if (!isNaN(json.StatusCode)) { rsp.statusCode = parseInt(json.StatusCode); }
        if (null !== json.Message) { rsp.message = String(json.Message); }
        if (null !== json.RedirectTo) { rsp.redirectTo = String(json.RedirectTo); }
        return rsp;
    }
}

window.jsHandler = theHandler;