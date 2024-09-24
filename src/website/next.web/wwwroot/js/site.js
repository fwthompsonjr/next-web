
function preventBack() {
    window.history.forward();
}
setTimeout("preventBack()", 0);
window.onunload = function () { null };


function viewRestrictions() {
    let hostname = document.location.protocol + '//' + document.location.host + '/';
    const landing = "".concat(hostname, "my-account/account-upgrade-limits");
    window.document.location = landing;
}