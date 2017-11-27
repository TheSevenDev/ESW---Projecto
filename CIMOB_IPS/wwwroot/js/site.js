function toogleSettings()
{
    document.getElementById("submenu-notifications").style.display = "none";
    var settingsMenu = document.getElementById("submenu-settings");
    settingsMenu.style.display = ((settingsMenu.style.display !== 'block') ? 'block' : 'none')
}

function toogleNotifications()
{
    document.getElementById("submenu-settings").style.display = "none";
    var notificationMenu = document.getElementById("submenu-notifications");

    notificationMenu.style.display = ((notificationMenu.style.display !== 'block') ? 'block' : 'none')
}

function toogleHelp() {
    hideMenus();
    var fade = document.getElementById("fade-background");
    var help = document.getElementById("help-content");
    var closeHelp = document.getElementById("help-close");

    fade.style.display = ((fade.style.display !== 'block') ? 'block' : 'none')
    help.style.display = ((help.style.display !== 'block') ? 'block' : 'none')
    closeHelp.style.display = ((closeHelp.style.display !== 'block') ? 'block' : 'none')
}

function tooglefyp() {
    var fyp = document.getElementById("fyp");
    var closefyp = document.getElementById("fyp-close");

    fyp.style.display = ((fyp.style.display !== 'block') ? 'block' : 'none')
    closefyp.style.display = ((closefyp.style.display !== 'block') ? 'block' : 'none')
}



function hideMenus()
{
    document.getElementById("submenu-settings").style.display = "none";
    document.getElementById("submenu-notifications").style.display = "none";
}

