const actv = "active";
const names = ["home", "profile", "permissions", "password"];
function showLogout() {
    let bttn = "#btn-my-account-logout-show";
    $(bttn).click();
    return null;
}
function setDisplay(name) {
    if (names.indexOf(name) < 0) { return; }
    setActiveDiv(name);
    const themenu = document.getElementById("masthead-nav-menu");
    let thelinks = themenu.getElementsByTagName("a");
    for (let i = 0; i < thelinks.length; i++) {
        let a = thelinks[i];
        const aname = a.getAttribute("name");
        if (null === aname || undefined === aname) { continue; }
        if (aname.indexOf(name) >= 0) {
            a.classList.add(actv);
        } else {
            a.classList.remove(actv);
        }
    }
}
function setActiveDiv(name) {
    if (names.indexOf(name) < 0) { return; }
    const divnames = ["dv-subcontent-home", "dv-subcontent-profile", "dv-subcontent-permissions", "dv-subcontent-password"];
    for (let i = 0; i < divnames.length; i++) {
        const d = document.getElementById(divnames[i]);
        if (null === d || undefined === d) { continue; }
        if (divnames[i].indexOf(name) >= 0) {
            d.classList.add(actv);
        } else {
            d.classList.remove(actv);
        }
    }
}
function getPageName() {
    const defaultName = "home";
    const colon = ":";
    var current_title = $(document).attr('title');
    if (undefined === current_title || null === current_title || current_title.length === 0) return defaultName;
    if (current_title.indexOf(colon) < 0 || current_title.split(colon).length < 2) return defaultName;
    var landing = current_title.split(':')[1].trim();
    return landing == "account" ? "myaccount-~0" : "home-~0";
}
function getDisplay() {
    const find = "~0";
    let pageName = getPageName();
    const themenu = document.getElementById("masthead-nav-menu");
    let thelinks = themenu.getElementsByTagName("a");
    for (let i = 0; i < thelinks.length; i++) {
        let a = thelinks[i];
        const aname = a.getAttribute("name");
        if (undefined === aname || null === aname) { continue; }
        if (!a.classList.contains(actv)) { continue; }
        return pageName.replace(find, aname.split("-")[1]);
    }
    return "";
}
function reloadContent() {
    const jsobj = window.jsHandler;
    if (undefined === jsobj || null === jsobj) return;
    const currentLocation = getDisplay();
    if (undefined === currentLocation || null === currentLocation || currentLocation.length === 0) return;
    const linkName = "#footer-reload-link";
    const headerlinks = "#masthead-nav-menu a[name]";
    const links = [linkName, headerlinks];
    const sectionMain = "main";
    const animationInterval = 400;
    links.forEach(n => {
        $(n).bind('click', false);
        $(n).attr('href', 'javascript:void()');
    })
    $(sectionMain).css({ opacity: 1.0, visibility: "visible" }).animate({ opacity: 0 }, animationInterval);
    const restoreInterval = animationInterval * 3;
    setTimeout(
        function () {
            const jsobj = window.jsHandler;
            const currentLocation = getDisplay();
            jsobj.reload(currentLocation);
        }, restoreInterval);
}
function profileComboBoxChanged() {
    const hidden = "d-none";
    const cboxid = "#cbo-profile-group";
    const find = "~0";
    const dvprefix = "div[name='dv-subcontent-profile-~0']";
    const dvparent = "#dv-subcontent-profile";
    const divnames = [
        dvprefix.replace(find, "personal"),
        dvprefix.replace(find, "address"),
        dvprefix.replace(find, "phone"),
        dvprefix.replace(find, "email")];
    for (var i = 0; i < divnames.length; i++) {
        var dv = divnames[i];
        $(dv).addClass(hidden);
    }
    var current = $(cboxid).val();
    if (null === current || current.length === 0) {
        $(dvparent).find("div.card-footer").hide();
        return;
    }
    var target = dvprefix.replace(find, current);
    $(target).removeClass(hidden);
    $(dvparent).find("div.card-footer").show();
}
profileComboBoxChanged()