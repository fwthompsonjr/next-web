
let jsSearchForm = {
    attributes: {
        name: "style",
        hide: "display: none",
        block: "display: block"
    },
    fields: [],
    controls: {
        state: "#cbo-search-state",
        county: "#cbo-search-county",
        button: "#search-submit-button",
        errormessage: "#search-submit-error-message",
        rows: "#table-search tr[name = 'tr-search-dynamic']",
        start: "#tbx-search-startdate",
        end: "#tbx-search-enddate",
    },
    serialized: function () {
        let js = {
            "state": $(jsSearchForm.controls.state).find("option:selected").attr("dat-state-index"),
            "county": {
                "name": $(jsSearchForm.controls.county).find("option:selected").attr("dat-county-index"),
                "value": parseInt($(jsSearchForm.controls.county).val())
            },
            "details": [],
            "start": Date.parse($(jsSearchForm.controls.start).val()),
            "end": Date.parse($(jsSearchForm.controls.end).val()),
        };
        $(jsSearchForm.controls.rows).each(function () {
            let $tr = $(this);
            if ($tr.is(":not(:hidden)")) {
                const lb = $tr.find("label").text();
                const cboption = $tr.find("select").find("option:selected");
                js.details.push({ "name": lb, "text": cboption.text(), "value": cboption.val() });
            }
        });
        return js;
    },
    seticon: function (isSpinning) {
        const dsb = "disabled";
        const hddn = "d-none";
        const icons = ["#search-submit-icon", "#search-submit-icon-spin"];
        const bttn = jsSearchForm.controls.button;
        const a = isSpinning ? 0 : 1;
        const b = isSpinning ? 1 : 0;
        $(icons[a]).addClass(hddn);
        $(icons[b]).removeClass(hddn);
        if (isSpinning) {
            $(bttn).attr(dsb, dsb);
        } else {
            $(bttn).removeAttr(dsb);
        }
    },
    onsubmitclicked: function () {
        const dsb = "disabled";
        let isValid = jsSearchForm.validate();
        if (!isValid) { return; }
        let submission = JSON.stringify(jsSearchForm.serialized());
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        jsSearchForm.seticon(true);
        const arr = jsSearchForm.fields;
        try {
            arr.forEach(a => $(a).attr(dsb, dsb));
            handler.submit("frm-search", submission);
        } catch {
            jsSearchForm.seticon(false);
        }
        finally {
            arr.forEach(a => $(a).removeAttr(dsb));
        }
    },
    validate: function () {
        const errclass = "error";
        let emsg = $(jsSearchForm.controls.errormessage);
        let js = jsSearchForm.serialized();
        emsg.addClass("d-none");
        emsg.text('Please fill all required fields.');
        if (null == js) return false;
        if (js.state.length == 0) {
            $(jsSearchForm.controls.state).addClass(errclass);
            emsg.text('State is a required field.');
            emsg.removeClass("d-none");
            return false;
        }
        $(jsSearchForm.controls.state).removeClass(errclass);
        if (js.county.value == '0') {
            $(jsSearchForm.controls.county).addClass(errclass);
            emsg.text('County is a required field.');
            emsg.removeClass("d-none");
            return false;
        }
        $(jsSearchForm.controls.county).removeClass(errclass);
        let errors = [];
        $(jsSearchForm.controls.rows).each(function () {
            let $tr = $(this);
            if ($tr.is(":not(:hidden)")) {
                const cbo = $tr.find("select");
                const cboption = cbo.find("option:selected");
                if (null == cboption || null == cboption.val() || cboption.val().length == 0) {
                    errors.push("#" + cbo.attr("id"));
                    cbo.addClass(errclass);
                    if (errors.length == 1) { emsg.removeClass("d-none"); }
                }
                else {
                    cbo.removeClass(errclass);
                }
            }
        });
        if (errors.length > 0) { return false; }
        $(jsSearchForm.controls.start).removeClass(errclass);
        if (isNaN(js.start)) {
            $(jsSearchForm.controls.start).addClass(errclass);
            emsg.text('Please enter a valid start date.');
            emsg.removeClass("d-none");
            return false;
        }
        $(jsSearchForm.controls.end).removeClass(errclass);
        if (isNaN(js.end)) {
            $(jsSearchForm.controls.end).addClass(errclass);
            emsg.text('Please enter a valid end date.');
            emsg.removeClass("d-none");
            return false;
        }
        let d1 = new Date(js.start);
        let d2 = new Date(js.end);
        if (d2.getTime() < d1.getTime()) {
            $(jsSearchForm.controls.end).addClass(errclass);
            emsg.text('End date should be later than start date.');
            emsg.removeClass("d-none");
            return false;
        }
        let difference = Math.round(d2 - d1) / (1000 * 3600 * 24);
        if (difference > 7) {
            $(jsSearchForm.controls.end).addClass(errclass);
            emsg.text('Date range should be (7) days or less.');
            emsg.removeClass("d-none");
            return false;
        }
        const currentDate = new Date();
        if (d1.getTime() > currentDate.getTime()) {
            $(jsSearchForm.controls.start).addClass(errclass);
            emsg.text('Start date can not be in the future.');
            emsg.removeClass("d-none");
            return false;
        }
        if (d2.getTime() > currentDate.getTime()) {
            $(jsSearchForm.controls.end).addClass(errclass);
            emsg.text('End date can not be in the future.');
            emsg.removeClass("d-none");
            return false;
        }
        const minumimDate = 1514764800095;
        if (d1.getTime() < minumimDate) {
            $(jsSearchForm.controls.start).addClass(errclass);
            emsg.text('Start date can not be prior to Jan 1, 2018.');
            emsg.removeClass("d-none");
            return false;
        }
        if (d2.getTime() < minumimDate) {
            $(jsSearchForm.controls.end).addClass(errclass);
            emsg.text('End date can not be prior to Jan 1, 2018.');
            emsg.removeClass("d-none");
            return false;
        }
        return true;
    },
    initialize: function () {
        let jscontrols = jsSearchForm.controls;
        $(jscontrols.state).attr("onchange", "jsSearchForm.stateChanged()");
        $(jscontrols.county).attr("onchange", "jsSearchForm.countyChanged()");
        $(jscontrols.button).attr("onclick", "jsSearchForm.onsubmitclicked()");
        $(jscontrols.state).val("0");
        $(jscontrols.county).val("0");
        $(jscontrols.start).val(null);
        $(jscontrols.end).val(null);
        let arr = jsSearchForm.fields;
        arr.push(jsSearchForm.controls.state);
        arr.push(jsSearchForm.controls.county);
        arr.push(jsSearchForm.controls.rows + " select");
        arr.push(jsSearchForm.controls.start);
        arr.push(jsSearchForm.controls.end);
        arr.forEach(a => $(a).attr("onblur", "jsSearchForm.oncontrolleave()"));
    },
    oncontrolleave: function () {
        let arr = jsSearchForm.fields;
        arr.forEach(a => $(a).removeClass('error'));
        let emsg = $(jsSearchForm.controls.errormessage);
        if (!emsg.hasClass("d-none")) { jsSearchForm.validate(); }
    },
    stateChanged: function () {
        const attrName = "dat-state-index";
        let cbo = $(jsSearchForm.controls.state);
        let cboCounty = $(jsSearchForm.controls.county);
        let rows = $(jsSearchForm.controls.rows);
        let a = jsSearchForm.attributes;
        rows.attr(a.name, a.hide);
        cboCounty.val("0");
        let options = cboCounty.find("option").not("[value='0']");
        options.attr(a.name, a.hide);
        let attr = cbo.find("option:selected").attr(attrName);
        if (null != attr && attr.length > 0) {
            let selector = "option[~0 = '~1']".replace('~0', attrName).replace('~1', attr);
            cboCounty.find(selector).removeAttr(a.name);
        }
    },
    containsCounty: function (rwselector, countyIndex) {
        let selector = $(rwselector).find("select");
        if (null == selector) return false;
        let search = "option[dat-county-index='~0']".replace('~0', countyIndex);
        let children = selector.find(search);
        return children != null;
    },
    countyOptionDisplay: function (rwselector, countyIndex) {
        let selector = $(rwselector).find("select");
        if (null == selector) return;
        let search = "option[dat-county-index='~0']".replace('~0', countyIndex);
        let children = selector.find(search);
        let a = jsSearchForm.attributes;
        let options = selector.find("option").not("[value='']");
        options.attr(a.name, a.hide);
        selector.val('');
        if (children == null || children.length == 0) return;
        let option1 = children.first();
        let groupName = option1.attr("dat-row-name");
        if (groupName == null) { groupName = "Select:" }
        $(rwselector).find("label").text(groupName);
        children.removeAttr(a.name);
        $(rwselector).removeAttr(a.name);
    },
    countyChanged: function () {
        let a = jsSearchForm.attributes;
        let cbo = $(jsSearchForm.controls.county);
        let countyId = cbo.val();
        let rows = $(jsSearchForm.controls.rows);
        rows.attr(a.name, a.hide);
        rows.each(function (rw) {
            let rowid = "#" + $(this).attr("id");
            let hasCounty = jsSearchForm.containsCounty(rowid, countyId);
            if (hasCounty) {
                // set options 
                jsSearchForm.countyOptionDisplay(rowid, countyId);
            }
        });
    }
}


let jssearchviews = {
    "links": [
        "#nvlink-subcontent-search",
        "#nvlink-subcontent-search-history",
        "#nvlink-subcontent-search-purchases",
        "#nvlink-subcontent-search-active"],
    "views": [
        "#dv-search-container",
        "#dv-subcontent-history",
        "#dv-subcontent-purchases"],
    "setIndex": function (index) {
        const actv = "active";
        const search_parent = "#dv-subcontent-search";
        const dnone = "d-none";
        let id = parseInt(index);
        if (id == 3) {
            let handler = window.jsHandler;
            if (undefined === handler || null === handler || !(handler)) { return; }
            handler.reload('mysearch-actives');
            return;
        }
        if (isNaN(id) || id < 0 || id > 2) { return; }
        if (id == 0) {
            $(search_parent).show();
        } else {
            $(search_parent).hide();
        }
        for (let i = 0; i < jssearchviews.links.length; i++) {
            let nv = jssearchviews.links[i];
            let vw = jssearchviews.views[i];
            if (i == id) {
                $(nv).addClass(actv);
                $(vw).addClass(actv);
                $(vw).removeClass(dnone);
                $(vw).show();
            } else {
                $(nv).removeClass(actv);
                $(vw).removeClass(actv);
                $(vw).hide();
            }
        }
    }
}
let jssearchpager = {
    "pagers": ["#cbo-subcontent-history-pager", "#cbo-subcontent-purchases-pager", "#cbo-subcontent-preview-pager"],
    "history_row_select": "table[automationid='search-history-table'] tr[data-row-number]",
    "history_current_row": "table[automationid='search-history-table'] tr[data-row-number='~0']",
    "preview_name": "table[automationid='search-preview-table']",
    "preview_content": null,
    "preview_reset": function () {
        jssearchpager.initialize();
        let previewName = jssearchpager.preview_name;
        $(previewName).html(jssearchpager.preview_content);
    },
    "initialize": function () {
        if (null != jssearchpager.preview_content) { return; }
        let previewName = jssearchpager.preview_name;
        jssearchpager.preview_content = $(previewName).html();
    },
    toggle_filter: function () {
        const rwname = "#dv-search-history-filter";
        const dn = "d-none";
        let cbx = "#cbo-search-history-filter";
        let counter = "#span-search-history-filter-count";
        jssearchpager.set_page(0);
        if ($(rwname).hasClass(dn)) {
            $(cbx).val('');
            $(counter).text('');
            $(rwname).removeClass(dn);
        }
        else {
            $(rwname).addClass(dn);
        }
    },
    apply_history_filter: function () {
        let cbx = "#cbo-search-history-filter";
        let counter = "#span-search-history-filter-count";
        let stats = $(cbx).val();
        $(counter).text('');
        if (stats.length == 0) {
            jssearchpager.set_page(0);
            return;
        }
        let t_body = $('#cbo-subcontent-history-pager').closest('table').find('tbody');
        let i_pos = 0;
        const historyrows = t_body.find("tr[search-uuid]");
        historyrows.each(function () {
            let rw = $(this);
            let currentStatus = rw.find('span[name="search-status"]').text().trim();
            if (currentStatus == stats) {
                rw.show();
                i_pos++;
            }
            else {
                rw.hide();
            }
        });
        $(counter).text(i_pos);
    },
    get_status_class: function (searchStatus) {
        if (undefined == searchStatus || null == searchStatus) { return null; }
        switch (searchStatus) {
            case "Completed": return "text-success";
            case "Processing": return "text-warning-emphasis";
            case "Error": return "text-danger";
            case "Purchased": return "text-info";
            case "Downloaded": return "text-primary";
            default: return null;
        }
    },
    bind_preview: function (jscontent) {
        const tableRowCount = 5;
        const temp_mapping = [
            "case-number|caseNumber",
            "case-type|caseType",
            "court|court",
            "date-filed|dateFiled",
            "user-name|name"
        ];
        const template = "".concat(
            "<tr name='injected-content' data-row-number='0' data-page-number='0' data-position='even'>",
            $("#tr-subcontent-preview-data-template").html(),
            "</tr>");
        let tempjs = JSON.parse(jscontent);
        if (tempjs.length === 0) {
            $("#frm-search-history-submit-button").trigger("click");
            return;
        }
        let t_body = $(jssearchpager.pagers[2]).closest('table').find('tbody');
        let t_foot = $(jssearchpager.pagers[2]).closest('table').find('tfoot');
        t_foot.removeClass("d-none");
        $("#cbo-subcontent-preview-pager").html("");
        tempjs.forEach(function (j) {
            t_body.append(template);
        });
        let i_pos = 0;
        $("#tr-subcontent-preview-no-data").hide();
        $("#td-subcontent-preview-record-count").text("".concat("Records: ", tempjs.length));
        t_body.find("tr[name='injected-content']").each(function () {
            let rw = $(this);
            const pgnbr = Math.floor(i_pos / tableRowCount);
            rw.attr("data-row-number", i_pos);
            rw.attr("data-page-number", pgnbr);
            rw.attr("data-position", i_pos % 2 == 0 ? "even" : "odd");
            temp_mapping.forEach(function (m) {
                let src = tempjs[i_pos];
                const finder = "span[name='~0']".replace("~0", m.split('|')[0]);
                const fld = m.split('|')[1];
                rw.find(finder).text(src[fld]);
                if (m == "search-status") {
                    let clsStatus = jssearchpager.get_status_class(src[fld]);
                    if (clsStatus != null) {
                        rw.find(finder).addClass(clsStatus);
                    }
                }
            });
            if (pgnbr > 0) { rw.hide(); }
            i_pos++;
        });
        const mxCount = tempjs.length;
        for (let i = 0; i < mxCount; i += tableRowCount) {
            const pgnbr = Math.floor(i / tableRowCount);
            const mx = Math.min(i + tableRowCount, mxCount);
            const lbl = "".concat("Records: ", i + 1, " to ", mx);
            const child = "<option value='~0'>~1</option>".replace("~0", pgnbr).replace("~1", lbl);
            $("#cbo-subcontent-preview-pager").append(child);
        }
        $("#cbo-subcontent-preview-pager").val(0);
        $("#frm-search-history-submit-button").trigger("click");
    },
    "set_page": function (cboId) {
        let indx = parseInt(cboId);
        if (isNaN(indx) || indx < 0 || indx > jssearchpager.pagers.length) { return; }
        let keyname = jssearchpager.pagers[indx];
        let pg = $(keyname).val();
        let rows = $(keyname).closest('table').find('tbody').find('tr[data-page-number]');
        rows.each(function () {
            let $current = $(this);
            let attr = $current.attr("data-page-number");
            let hscls = $current.attr("class");
            if (typeof hscls !== typeof undefined && hscls !== false) { $current.removeAttr('class'); }
            if (attr == pg) { $current.show() }
            else { $current.hide(); }
        });
        $(keyname).trigger('blur');
    },
    "get_preview_details": function (rw) {
        return {
            "requested-date": rw.querySelector("a").innerText,
            "state-abbr": rw.querySelector("span[name='state-abbr']").innerText,
            "county-name": rw.querySelector("span[name='county-name']").innerText,
            "begin-date": rw.querySelector("span[name='begin-date']").innerText,
            "ending-date": rw.querySelector("span[name='ending-date']").innerText,
            "search-status": rw.querySelector("span[name='search-status']").innerText
        }
    },
    "show_preview": function (row_index) {
        jssearchpager.preview_reset();
        let indx = parseInt(row_index);
        if (isNaN(indx)) { return; }
        let historyName = jssearchpager.pagers[0];
        let $rows = $(historyName).closest('table').find('tbody').find('tr[data-page-number]');
        let $rw = $rows[indx];
        let search_index = $rw.getAttribute('search-uuid');
        if (undefined == search_index || null == search_index) { return; }
        let details = jssearchpager.get_preview_details($rw);
        let heading = $(jssearchpager.pagers[2]).closest('table').find('thead');
        let searchStatus = details["search-status"];
        const headings = [
            "requested-date",
            "state-abbr",
            "county-name",
            "begin-date",
            "ending-date",
            "search-status"];
        // populate table headings 
        headings.forEach(function (h) {
            let finder = "span[name='~0']".replace("~0", h);
            heading.find(finder).text(details[h]);
            if (h == "search-status") {
                let clsStatus = jssearchpager.get_status_class(searchStatus);
                if (clsStatus != null) {
                    heading.find(finder).addClass(clsStatus);
                }
            }
        });
        let finder = "span[name='search-uuid']";
        heading.find(finder).text(search_index);
        if (details["search-status"] != "Completed") {
            $("#frm-search-history-submit-button").trigger("click");
            $("my-search-preview-bttn").attr("disabled", "disabled");
            return;
        } else {
            $("my-search-preview-bttn").removeAttr("disabled");
        }
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        let obj = JSON.stringify({ "id": search_index });
        handler.submit("frm-search-preview", obj);
    },
    get_invoice: function () {
        let customfind = "table[automationid='search-preview-table'] > thead > tr > td > span[name='search-status']";
        let txt = $(customfind).text();
        if (txt != "Completed") {
            $("#my-search-preview-bttn-close").trigger("click");
            return;
        }
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        $("#my-search-preview-bttn").attr("disabled", "disabled");
        let uuidx = $("table[automationid='search-preview-table']")
            .find("tr[name='requested-date']")
            .find("span[name='search-uuid']")
            .text();
        let obj = JSON.stringify({ "id": uuidx });
        handler.submit("frm-search-invoice", obj);
    }
}
let jsPurchases = {
    "initialize": async function () {
        $("#purchases-history-00-a").hide();
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        await handler.getPurchases();
    },
    "reload": function () {
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        handler.reload('mysearch-purchases');
    },
    open_file: function () {
        let itm = document.getElementById('user-download-file-name');
        if (undefined === itm || null === item) { return; }
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        let txt = itm.innerText;
        handler.tryOpenExcel(txt);
    },
    toggle_filter: function () {
        const rwname = "#dv-purchases-history-filter";
        const dn = "d-none";
        let cbx = "#cbo-purchases-history-filter";
        let counter = "#span-purchases-history-filter-count";
        jssearchpager.set_page(1);
        if ($(rwname).hasClass(dn)) {
            $(cbx).val('');
            $(counter).text('');
            $(rwname).removeClass(dn);
        }
        else {
            $(rwname).addClass(dn);
        }
    },
    apply_filter: function () {
        let cbx = "#cbo-purchases-history-filter";
        let counter = "#span-purchases-history-filter-count";
        let stats = $(cbx).val();
        $(counter).text('');
        if (stats.length == 0) {
            jssearchpager.set_page(1);
            return;
        }
        let t_body = $('#cbo-subcontent-purchases-pager').closest('table').find('tbody');
        let i_pos = 0;
        let historyrows = t_body.find("tr[data-row-number]");
        historyrows.each(function () {
            let rw = $(this);
            let currentStatus = rw.find('span[name="purchase-status"]').text().trim();
            if (currentStatus == stats) {
                rw.show();
                i_pos++;
            }
            else {
                rw.hide();
            }
        });
        $(counter).text(i_pos);
    },
    show_submission_error: function (html) {
        $("#purchases-history-00-error").removeClass("text-success");
        $("#purchases-history-00-error").addClass("text-danger");
        $("#purchases-history-00-error").html(html);
        $("#purchases-history-00-a").fadeIn(500);
    },
    show_submission_success: function (html) {
        $("#purchases-history-00-error").removeClass("text-danger");
        $("#purchases-history-00-error").addClass("text-success");
        $("#purchases-history-00-error").html(html);
        $("#purchases-history-00-a").fadeIn(500);
    },
    make_download: async function () {
        let submission = {
            "id": $("#static-search-uuid").val(),
            "name": "-- prompt for folder --"
        };
        if (null === submission.id || submission.id.length == 0) {
            jsPurchases.show_submission_error("Invoice Id is missing or invalid.");
            return;
        }
        let handler = window.jsHandler;
        if (undefined === handler || null === handler || !(handler)) { return; }
        await handler.makeDownload(JSON.stringify(submission));
    },
    cancel_request: function () {
        const dlb = "disabled";
        $("#purchases-history-00").hide();
        $("#purchases-history-00-a").hide();
        $("#purchases-history-00-error").html("");
        $("#purchases-history-01").show();
        $("#dv-subcontent-purchases div.card-footer").show();
        $("#frm-purchase-history-download-button").setAttr(dlb, dlb);
        $("#formFile").val('');
        $("#static-item-name").text(" - ");
    },
    bind_purchase: function (jscontent) {
        let USDollar = new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        });
        const tableRowCount = 5;
        const temp_mapping = [
            "search-uuid|referenceId",
            "purchase-date|purchaseDate",
            "item-name|itemType",
            "item-quantity|itemCount",
            "total-price|price",
            "purchase-status|statusText",
            "external-uuid|externalId"
        ];
        const template = "".concat(
            "<tr name='injected-content' data-row-number='0' data-page-number='0' data-position='even'>",
            $("#tr-subcontent-purchases-data-template").html(),
            "</tr>");
        let tempjs = JSON.parse(jscontent);
        if (tempjs.length === 0) {
            // show no data and exit 
            return;
        }
        let t_body = $(jssearchpager.pagers[1]).closest('table').find('tbody');
        let t_foot = $(jssearchpager.pagers[1]).closest('table').find('tfoot');
        t_foot.removeClass("d-none");
        $("#cbo-subcontent-purchases-pager").html("");
        tempjs.forEach(function (j) {
            t_body.append(template);
        });
        let i_pos = 0;
        $("#tr-subcontent-purchases-no-data").hide();
        $("#td-subcontent-purchases-record-count").text("".concat("Records: ", tempjs.length));
        t_body.find("tr[name='injected-content']").each(function () {
            let rw = $(this);
            let pgnbr = Math.floor(i_pos / tableRowCount);
            rw.attr("data-row-number", i_pos);
            rw.attr("data-page-number", pgnbr);
            rw.attr("data-position", i_pos % 2 == 0 ? "even" : "odd");
            temp_mapping.forEach(function (m) {
                let src = tempjs[i_pos];
                let finder = "span[name='~0']".replace("~0", m.split('|')[0]);
                let fld = m.split('|')[1];
                rw.find(finder).text(src[fld]);
                if (fld == "price") {
                    let usprice = USDollar.format(src[fld]);
                    rw.find(finder).text(usprice);
                }
                if (fld == 'purchaseDate') {
                    let nwdate = src[fld].replace('T', '<br>');
                    rw.find(finder).html(nwdate);
                }
                if (fld == "statusText") {
                    let clsStatus = jssearchpager.get_status_class(src[fld]);
                    if (clsStatus != null) {
                        rw.find(finder).addClass(clsStatus);
                    }
                }
            });
            if (pgnbr > 0) { rw.hide(); }
            i_pos++;
        });
        let mxCount = tempjs.length;
        for (let i = 0; i < mxCount; i += tableRowCount) {
            let pgnbr = Math.floor(i / tableRowCount);
            let mx = Math.min(i + tableRowCount, mxCount);
            let lbl = "".concat("Records: ", i + 1, " to ", mx);
            let child = "<option value='~0'>~1</option>".replace("~0", pgnbr).replace("~1", lbl);
            $("#cbo-subcontent-purchases-pager").append(child);
        }
        $("#cbo-subcontent-purchases-pager").val(0);
        if ($("#purchases-history-00").hasClass("d-none")) {
            $("#purchases-history-00").removeClass("d-none");
            $("#purchases-history-00").hide();
        }
        t_body.find("tr[name='injected-content']").on('click', function () {
            const dlb = "disabled";
            const dn = "d-none";
            const rwname = "#dv-purchases-history-filter";
            if (!($(rwname).hasClass(dn))) {
                jsPurchases.toggle_filter();
            }
            let displays = ["#static-item-name", "#static-item-from-to", "#static-item-invoice-id", "#static-search-uuid"];
            displays.forEach(function (d) { $(d).val(" - "); })
            $("#purchases-history-00").hide();
            $("#purchases-history-00-a").hide();
            $("#purchases-history-00-error").html("");
            let $rwi = $(this);
            let txt = $rwi.find("span[name='item-name']").text();
            let sts = $rwi.find("span[name='purchase-status']").text();
            if (sts != 'Purchased') {
                $("#frm-purchase-history-download-button").setAttr(dlb, dlb);
                $("#purchases-history-01").fadeIn(500);
                $("#dv-subcontent-purchases div.card-footer").show();
                return;
            }
            $("#purchases-history-01").hide();
            $("#frm-purchase-history-download-button").removeAttr(dlb);
            $("#formFile").val('');
            let aa = txt.indexOf("from");
            let aname = txt.substring(0, aa).trim().split(':')[1].trim();
            let afrom = txt.substring(aa).trim();
            let inbr = $rwi.find("span[name='external-uuid']").text();
            let nwvales = [
                aname,
                afrom,
                inbr,
                $rwi.find("span[name='external-uuid']").text()
            ];
            for (let ii = 0; ii < nwvales.length; ii++) {
                let fnd = displays[ii];
                $(fnd).val(nwvales[ii]);
            }
            $("#dv-subcontent-purchases div.card-footer").hide();
            $("#purchases-history-00").fadeIn(500);
        })
    }
}
jsSearchForm.initialize();
jsPurchases.initialize();

