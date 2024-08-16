let theHandler = {
    "checkSession": function () {
        const landing = "/data/session-check";
        const sls = '/';
        let pth = document.location.pathname;
        if (pth.startsWith(sls)) { pth = pth.substring(1); }
        const loc = pth.split(sls)[0];
        let dta = { id: 10, location: loc };
        const requested = {
            "formName": "check-session",
            "payload": JSON.stringify(dta)
        };
        $.ajax({
            type: "POST",
            url: landing,
            data: JSON.stringify(requested),
            dataType: "json",
            success: function (resultData) {
                theResponder.handle(resultData);
            }
        });
    },
    "pre_submit": function (formName) {
        switch (formName) {
            case "form-login":
                if (setIconState) { setIconState(0, true); }
                if (setStatusMessage) { setStatusMessage(0, 'Sending form data', true); }
                break;
            case "form-register":
                if (setIconState) { setIconState(1, true); }
                if (setStatusMessage) { setStatusMessage(1, 'Sending form data', true); }
                break;
            default:
        }
    },
    "post_submit": function (formName) {
        switch (formName) {
            case "form-login":
                if (setIconState) { setIconState(0, false); }
                if (setStatusMessage) { setStatusMessage(0, '', false); }
                if (loginCompletedAction) { loginCompletedAction(); }
                break;
            case "form-register":
                if (setIconState) { setIconState(1, false); }
                if (setStatusMessage) { setStatusMessage(1, '', false); }
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
    },
    "fetch": function (uuidx, positionId, count) {
        const landing = "/data/fetch"
        let requested = {
            "formName": String("mailbox"),
            "payload": String(uuidx)
        };
        $.ajax({
            type: "POST",
            url: landing,
            data: JSON.stringify(requested),
            dataType: "json",
            success: function (resultData) {
                if (!mailbox?.controls?.preview) { return; }
                const answer = theResponder.translate(resultData);
                if (answer.statusCode != 200) { return; }
                let message = answer.message;
                let control = "".concat("#", mailbox.controls.preview);
                $(control).html(message);
                theHandler.setIndex(positionId, count, requested.payload);
            }
        });

    },
    "setIndex": function (positionId, count, recordId) {
        if (!positionId || !count) { return; }
        if (!mailbox || !mailbox.controls || !mailbox.controls.preview) { return; }
        if (!maillist_init || !maillist_init.setScroll) { return; }
        let pos = parseInt(positionId) + 1;
        let title = "Correspondence ( ~0 of ~1 )"
            .replace("~0", String(pos))
            .replace("~1", String(count));
        $("#mailbox-sub-header").text(title);

        const mbox = document.getElementById(mailbox.controls.maillist);
        const cnt = mbox.childElementCount - 1;
        for (let c = 0; c < cnt; c++) {
            const mailitem = mbox.children[c];
            const uuidx = mailitem.children[1].children[2].innerText;
            let isactive = uuidx == recordId;
            if (isactive) {
                mailitem.classList.add('active');
            } else {
                mailitem.classList.remove('active');
            }
        }
        maillist_init.setScroll();
    },
    "filter": function (instruction) {
        if (!instruction) { return; }
        const landing = "/data/filter-status"
        let requested = {
            "formName": String("history-filter"),
            "payload": String(instruction)
        };
        $.ajax({
            type: "POST",
            url: landing,
            data: JSON.stringify(requested),
            dataType: "json",
            success: function (resultData) {
                const answer = theResponder.translate(resultData);
                if (answer.statusCode != 200) { return; }
                const target = document.location.href;
                window.location = target;
            }
        });
    },
    "purchase": function (uuidx) {
        setTimeout(function () {
            $("#bttn-purchase-icon").removeClass("d-none");
            $("#bttn-purchase-icon-spin").addClass("d-none");
        }, 1500);
        let destination = "/invoice/purchase?id=~0".replace("~0", uuidx);
        document.location = destination;
    },
    "verify": function (uuidx) {
        setTimeout(function () {
            $("#bttn-download-icon").removeClass("d-none");
            $("#bttn-download-icon-spin").addClass("d-none");
        }, 1500);
        const landing = "/data/download-verify";
        let dta = { Id: uuidx };
        const requested = {
            "formName": "check-download",
            "payload": JSON.stringify(dta)
        };
        const nne = 'd-none';
        const boxes = ["#dv-user-download-error", "#dv-user-download-message"];
        $.ajax({
            type: "POST",
            url: landing,
            data: JSON.stringify(requested),
            dataType: "json",
            success: function (resultData) {
                $("#btn-user-interaction-download").removeAttr("disabled");
                $("#bttn-download-icon").removeClass(nne);
                $("#bttn-download-icon-spin").addClass(nne);
                let rsp = theResponder.translate(resultData);
                if (rsp.statusCode != 200) {
                    $(boxes[1]).text(rsp.message);
                    boxes.forEach(box => { $(box).removeClass(nne); });
                    return;
                }
                document.location = rsp.redirectTo;
            },
            error: function () {
                $(boxes[1]).text("An unexpected error occurred.");
                boxes.forEach(box => { $(box).removeClass(nne); });
                $("#btn-user-interaction-download").removeAttr("disabled");
            }
        });
    }
}

let theResponder = {
    "handle": function (data) {
        const current = theResponder.translate(data);
        let hostname = document.location.protocol + '//' + document.location.host;
        let navigationPage = hostname + current.redirectTo;
        if (current.statusCode == 200 && current.redirectTo.length > 0 ||
            current.statusCode == 408 && current.redirectTo.length > 0) {
            window.document.location = navigationPage;
        }
    },
    "translate": function (json) {
        let rsp = {
            "statusCode": 0,
            "message": "",
            "redirectTo": "/home"
        };
        if (undefined == json || null == json) return rsp;
        if (!isNaN(json.statusCode)) { rsp.statusCode = parseInt(json.statusCode); }
        if (null !== json.message) { rsp.message = String(json.message); }
        if (null !== json.redirectTo) { rsp.redirectTo = String(json.redirectTo); }
        return rsp;
    }
}
let theMenu = {
    "config": {
        parents: ["#my-account-parent-option", "#my-search-parent-option"],
        children: ["#my-account-options", "#my-search-options"]
    },
    "show_child": function (index) {
        const dnone = "d-none";
        const selected = "menu-selected";
        const idx = parseInt(index);
        if (isNaN(idx) || idx < 0 || idx > 1) { return; }
        theMenu.config.parents.forEach(c => $(c).removeClass(selected));
        const childElement = theMenu.config.children[idx];
        if ($(childElement).is(":visible")) {
            $(childElement).addClass(dnone);
            return;
        }
        const parent = theMenu.config.parents[idx];
        theMenu.config.children.forEach(c => $(c).addClass(dnone));
        $(childElement).removeClass(dnone);
        $(parent).addClass(selected);
    },
    "hide_children": function () {
        const dnone = "d-none";
        const selected = "menu-selected";
        theMenu.config.parents.forEach(c => $(c).removeClass(selected));
        theMenu.config.children.forEach(c => $(c).addClass(dnone));
    }
}

function logoutRequested() {
    const $btn = $('#btn-my-account-logout-show');
    if ($btn.length !== 0) {
        $('#btn-my-account-logout-show').trigger('click');
        return;
    }
    let rsp = confirm('Are you sure you want to logout?');
    if (!rsp) return;
    verifyAndPost('user', 'logout');
}

function verifyAndPost(src, target) {
    if (!src || !target) return;
    let command = "".concat(String(src).toLowerCase(), "-", String(target).toLowerCase());
    if (command != 'user-logout') { return; }
    document.location = "/logout";
}

function changeViewHandler(viewName) {
    const fallbackPage = '/my-account/home';
    let constructedPage = '';
    if (viewName == 'myaccount-home') { constructedPage = '/my-account/home' }
    if (viewName == 'myaccount-permissions') { constructedPage = '/my-account/permissions' }
    if (viewName == 'myaccount-profile') { constructedPage = '/my-account/profile' }
    if (viewName == 'mysearch-home') { constructedPage = '/search' }
    if (viewName == 'mysearch-active') { constructedPage = '/search/active' }
    if (viewName == 'mysearch-purchases') { constructedPage = '/search/purchases' }
    if (viewName == 'mysearch-history') { constructedPage = '/search/history' }
    if (constructedPage.length === 0) { constructedPage = fallbackPage; }
    document.location = constructedPage;
}

function resetCacheItem(text) {
    const landing = "/data/reset-cache";
    const dtable = "#detail-table";
    let dta = { Name: String(text) };
    const requested = {
        "formName": "form-cache-manager",
        "payload": JSON.stringify(dta)
    };
    $(dtable).fadeOut('slow');
    $.ajax({
        type: "POST",
        url: landing,
        data: JSON.stringify(requested),
        dataType: "json",
        success: function (resultData) {
            let rsp = theResponder.translate(resultData);
            if (rsp.statusCode == 200 || rsp.statusCode == 408) {
                document.location.reload();
            }
            else {
                $(dtable).fadeIn('slow');
            }
        },
        error: function() {
            $(dtable).fadeIn('slow');
        }
    });
}

window.jsHandler = theHandler;

window.addEventListener('DOMContentLoaded', () => {
    let queries = [
        "body > div.box > div.customrow.content",
        "body > div.box > div.customrow.header",
        "body > div.box > div.customrow.footer",
        "div.cover-container"
    ];
    queries.forEach(query => {
        let $element = $(query);
        if ($element.length !== 0) {
            $element.on('click', (function () { theMenu.hide_children(); }));
            $element.on('mouseover', (function () { theMenu.hide_children(); }));
        }
    });
});

const httpsRedirect = () => {
    return;
    /*
    if (location.protocol !== 'https:')
        location.replace('https://' + location.href.split('//')[1]);
    */
};

httpsRedirect();