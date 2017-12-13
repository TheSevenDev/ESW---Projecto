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
    clearfyp();
    var fyp = document.getElementById("fyp");
    var closefyp = document.getElementById("fyp-close");

    fyp.style.display = ((fyp.style.display !== 'block') ? 'block' : 'none')
    closefyp.style.display = ((closefyp.style.display !== 'block') ? 'block' : 'none')
}


function toggleInviteTech() {
    clearInviteTech();
    var inviteTech = document.getElementById("invite-tech");
    var closeInviteTech = document.getElementById("invite-tech-close");

    inviteTech.style.display = ((inviteTech.style.display !== 'block') ? 'block' : 'none')
    closeInviteTech.style.display = ((closeInviteTech.style.display !== 'block') ? 'block' : 'none')
}

function clearInviteTech() {
    document.getElementById('email-invite-tech').value = "";
    document.getElementById('isAdmin-invite-tech').value = false;

    document.getElementById('inviteTech-message').innerText = "";
    document.getElementById('inviteTech-error').innerText = "";
}


function clearfyp() {
    document.getElementById('email-fyp').value = "";
    document.getElementById('fyp-message').innerText = "";
    document.getElementById('fyp-error').innerText = "";
}



function hideMenus()
{
    document.getElementById("submenu-settings").style.display = "none";
    document.getElementById("submenu-notifications").style.display = "none";
}

