// Write your JavaScript code
window.onload = function () {
    hideMenus();
}

function toogleSettings()
{
    hideMenus();
    document.getElementById("submenu-notifications").style.display = "none";
    var settingsMenu = document.getElementById("submenu-settings");
    settingsMenu.style.display === "none" ? settingsMenu.style.display = "block" : settingsMenu.style.display = "none";


}

function toogleNotifications()
{
    hideMenus();
    document.getElementById("submenu-settings").style.display = "none";
    var notificationMenu = document.getElementById("submenu-notifications");
    notificationMenu.style.display === "none" ? notificationMenu.style.display = "block" : notificationMenu.style.display = "none";
}

function toogleHelp() {
    hideMenus();
    var fade = document.getElementById("fade-background");
    var help = document.getElementById("help-content");
    var closeHelp = document.getElementById("help-close");

    fade.style.display === "none" ? fade.style.display = "block" : fade.style.display = "none";
    help.style.display === "none" ? help.style.display = "block" : help.style.display = "none";
    closeHelp.style.display === "none" ? closeHelp.style.display = "block" : closeHelp.style.display = "none";
}

function hideMenus()
{
    document.getElementById("submenu-settings").style.display = "none";
    document.getElementById("submenu-notifications").style.display = "none";
}

