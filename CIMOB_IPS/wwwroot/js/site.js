
function toogleSettings()
{
    document.getElementById("submenu-notifications").style.display = "none";
    $("#submenu-settings").slideToggle(250);
}

function toogleNotifications()
{
    if (document.getElementById("submenu-notifications").style.display !== "block") {
        var actionUrl = '/Notification/ReadNotifications';

        $.ajax({
            type: "POST",
            url: actionUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
        }).done(function (res) {
            $("#notificationsCount").html("");
        });;

    }

    document.getElementById("submenu-settings").style.display = "none";
    $("#submenu-notifications").slideToggle(250);
}


function toogleHelp() {
    hideMenus();

    $("#fade-background").toggle();
    $("#help-content").toggle(250);
    $("#help-close").toggle(250);
}

function openfyp() {
    clearfyp();
    var fyp = document.getElementById("fyp");
    var closefyp = document.getElementById("fyp-close");

    fyp.style.display = "block";
    closefyp.style.display = "block";
}

function closefyp() {
    var fyp = document.getElementById("fyp");
    var closefyp = document.getElementById("fyp-close");

    fyp.style.display = "none";
    closefyp.style.display = "none";
}

function openInviteTech() {
    clearInviteTech();
    var inviteTech = document.getElementById("invite-tech");
    var closeInviteTech = document.getElementById("invite-tech-close");

    inviteTech.style.display = "block";
    closeInviteTech.style.display = "block";
}

function closeInviteTech() {
    var inviteTech = document.getElementById("invite-tech");
    var closeInviteTech = document.getElementById("invite-tech-close");

    inviteTech.style.display = "none";
    closeInviteTech.style.display = "none";
}

function clearInviteTech() {
    document.getElementById('email-invite-tech').value = "";

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
    if (document.getElementById("submenu-settings") !== null)
        document.getElementById("submenu-settings").style.display = "none";

    if (document.getElementById("submenu-notifications") !== null)
    document.getElementById("submenu-notifications").style.display = "none";
}

function toggleHelpInst() {
    $('.help-inst').slideToggle(450).css('display', 'inline-block');
}

function closeHelpInst() {
    $('.help-inst').fadeOut(550);
}

function toggleHelpEcts() {
    $('.help-ects').slideToggle(450).css('display', 'inline-block');
}

function toggleHelpPostal() {
    $('.help-postal').slideToggle(450).css('display', 'inline-block');
}

function closeHelpPostal() {
    $('.help-postal').fadeOut(550);
}

function closeHelpEcts() {
    $('.help-ects').fadeOut(550);
}

function clearAdresses() {
    document.getElementById('county').value = "";
    document.getElementById('district').value = "";
    document.getElementById('parish').value = "";

    document.getElementById('street').value = "";
}

function adressCode() {
    clearAdresses();
    var code1 = document.getElementById('input-code1').value;
    var code2 = document.getElementById('input-code2').value;

    var postal_code = code1 + "-" + code2;

    var url = "http://www.ctt.pt/pdcp/xml_pdcp?incodpos=" + postal_code;
    $.ajax({
        type: 'POST',
        url: "/Account/Address?code=" + postal_code,
        contentType: 'application/xml; charset=utf-8',
        crossDomain: true,
        success: function (response) {
            $('Localidade', response).each(function () {
                var distrito = $(this).find('Distrito').text();
                var concelho = $(this).find('Concelho').text();
                var freguesia = $(this).find('Freguesia').text();
                

                document.getElementById('county').value = concelho;
                document.getElementById('district').value = distrito;
                document.getElementById('parish').value = freguesia;


                $('Rua', response).each(function () {
                    var street = $(this).find('Designacao').first().text();                   
                        document.getElementById('street').value = street;

                });
            });
        },
        error: function (error) {
            console.log(error);
        }
    })  
}

function applicationInstitutions() {

    if ($("#form-application").valid()) {

        var actionUrl = '/Application/RegisterApplicationInstitutions';

        var list = document.getElementById("chosen-institutions").getElementsByTagName("li");

        for (var i = 0; i < list.length; i++) {
            var institution = list[i].innerText;
            if(i==0)
                actionUrl += "?inst" + (i + 1) + "=" + institution;
            else
                actionUrl += "&inst" + (i + 1) + "=" + institution;
        }
   
        $.ajax({
            type: "POST",
            url: actionUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (res) {
           
        });;

    }


}
function toggleCourses(elem) {
    var jqString = "#" + elem.id;
    var jqElem = $(jqString);

    jqElem.next().toggle(250);
}



function countUp() {
    var length = $('#textareaCarta').val().length;
    var maxLength = document.getElementById("textareaCarta").maxLength;
    var myCounter = document.getElementById("count").textContent = length + "/" + maxLength;
}


function openErasmus() {
    $('#erasmus-programs').toggle(250);

    if ($('#lusoBrasileiras-programs').css('display') === 'block')
        $('#lusoBrasileiras-programs').toggle(250);

    if ($('#ibero-Americanas-programs').css('display') === 'block')
        $('#ibero-Americanas-programs').toggle(250);

    if ($('#vascoGama-programs').css('display') === 'block')
        $('#vascoGama-programs').toggle(250);

    if ($('#macau-programs').css('display') === 'block')
        $('#macau-programs').toggle(250);
}

function openLusoBras() {
    $('#lusoBrasileiras-programs').toggle(250);

    if ($('#erasmus-programs').css('display') === 'block')
        $('#erasmus-programs').toggle(250);

    if ($('#ibero-Americanas-programs').css('display') === 'block')
        $('#ibero-Americanas-programs').toggle(250);

    if ($('#vascoGama-programs').css('display') === 'block')
        $('#vascoGama-programs').toggle(250);

    if ($('#macau-programs').css('display') === 'block')
        $('#macau-programs').toggle(250);
}

function openIberoAmeric() {
    $('#ibero-Americanas-programs').toggle(250);

    if ($('#erasmus-programs').css('display') === 'block')
        $('#erasmus-programs').toggle(250);

    if ($('#lusoBrasileiras-programs').css('display') === 'block')
        $('#lusoBrasileiras-programs').toggle(250);

    if ($('#vascoGama-programs').css('display') === 'block')
        $('#vascoGama-programs').toggle(250);

    if ($('#macau-programs').css('display') === 'block')
        $('#macau-programs').toggle(250);
}

function openVascoGama() {
    $('#vascoGama-programs').toggle(250);

    if ($('#erasmus-programs').css('display') === 'block')
        $('#erasmus-programs').toggle(250);

    if ($('#lusoBrasileiras-programs').css('display') === 'block')
        $('#lusoBrasileiras-programs').toggle(250);

    if ($('#ibero-Americanas-programs').css('display') === 'block')
        $('#ibero-Americanas-programs').toggle(250);

    if ($('#macau-programs').css('display') === 'block')
        $('#macau-programs').toggle(250);
}

function openVascoGama() {
    $('#vascoGama-programs').toggle(250);

    if ($('#erasmus-programs').css('display') === 'block')
        $('#erasmus-programs').toggle(250);

    if ($('#lusoBrasileiras-programs').css('display') === 'block')
        $('#lusoBrasileiras-programs').toggle(250);

    if ($('#ibero-Americanas-programs').css('display') === 'block')
        $('#ibero-Americanas-programs').toggle(250);

    if ($('#macau-programs').css('display') === 'block')
        $('#macau-programs').toggle(250);
}

function openMacau() {
    $('#macau-programs').toggle(250);

    if ($('#erasmus-programs').css('display') === 'block')
        $('#erasmus-programs').toggle(250);

    if ($('#lusoBrasileiras-programs').css('display') === 'block')
        $('#lusoBrasileiras-programs').toggle(250);

    if ($('#ibero-Americanas-programs').css('display') === 'block')
        $('#ibero-Americanas-programs').toggle(250);

    if ($('#vascoGama-programs').css('display') === 'block')
        $('#vascoGama-programs').toggle(250);
}

$(document).ready(function () {
    try {
        adressCode();
    }
    catch (e) {

    }

    $("form.form-delete-app").submit(function (event) {
        event.preventDefault();
        var form = this;
        $.confirm({
            title: 'Atenção!',
            useBootstrap: false,
            modal: true,
            autoOpen: false,
            draggable: false,
            content: 'Está a cancelar uma candidatura.\nTem a certeza que pretende continuar?',
            buttons: {
                sim: {
                    text: 'Sim',
                    btnClass: 'btn-green',
                    action: function () {
                        form.submit();
                    }
                },
                nao: {
                    text: 'Não',
                    btnClass: 'btn-red',
                    action: function () {
                        event.preventDefault();
                    }
                },

            }
        })
    });
});