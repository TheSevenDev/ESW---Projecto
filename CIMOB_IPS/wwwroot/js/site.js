// Write your JavaScript code.

function toogleSettings()
{   
    document.getElementById("submenu-notifications").style.display = "none";
    var settingsMenu = document.getElementById("submenu-settings");
    settingsMenu.style.display === "none" ? settingsMenu.style.display = "block" : settingsMenu.style.display = "none";


}


function toogleNotifications()
{
    document.getElementById("submenu-settings").style.display = "none";
    var notificationMenu = document.getElementById("submenu-notifications");
    notificationMenu.style.display === "none" ? notificationMenu.style.display = "block" : notificationMenu.style.display = "none";
}