let jsCallback = {
    "submit": function (formName, text) {
        let requested = "".concat(formName, " := ", text);
        alert(requested);
    }
};