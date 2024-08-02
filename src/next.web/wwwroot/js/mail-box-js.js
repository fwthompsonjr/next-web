
let mailbox = {
    "isWorking": false,
    "controls": {
        "maillist": "dv-mail-item-list",
        "preview": "dv-mail-item-preview",
        "previewtemplate": "tarea-preview-html",
        "previewcurrent": "tarea-current-html"
    },
    "getElementText": function (elementName) {
        try {
            let js = document.getElementById(elementName).innerText.trim();
            return js;
        } catch {
            return "";
        }
    },
    "preview": {
        "clear": function () {
            let html = mailbox.getElementText(mailbox.controls.previewtemplate);
            document.getElementById(mailbox.controls.preview).innerHTML = html;
        }
    },
    "fetch": {
        "item": function (id) {
            try {
                // get html content based on item id 
                if (isNaN(id)) { return; }
                // client side validation 
                const inx = parseInt(id);
                const mbox = document.getElementById(mailbox.controls.maillist);
                const count = mbox.childElementCount - 1;
                if (inx < 0 || inx > count) return;
                mailbox.preview.clear();
                let handler = window.jsHandler;
                if (undefined === handler || null === handler || !(handler)) {
                    return;
                }
                const mailitem = mbox.children[inx];
                const uuidx = mailitem.children[1].children[2].innerText;
                handler.fetch(uuidx);
            } catch {
                mailbox.preview.clear();
            }
        }
    }
}
let maillist_init = {
    "setScroll": function () {
        const selectedItem = "a[name='link-mail-items-template'].active";
        if ($(selectedItem).length === 0) return;
        $(selectedItem).get(0).scrollIntoView();
    }
}
function fetch_item(id) {
    if (isNaN(id)) {
        return;
    }
    mailbox.fetch.item(id);
}
function docReady(fn) {
    // see if DOM is already available 
    if (document.readyState === "complete" || document.readyState === "interactive") {
        // call on next available tick 
        setTimeout(fn, 1);
    } else {
        document.addEventListener("DOMContentLoaded", fn);
    }
}
docReady(function () {
    maillist_init.setScroll();
});