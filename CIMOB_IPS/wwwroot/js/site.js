onload = start;

function start() {

    autoPlayGallery();
    setColor();

    if (document.getElementById("loading-program") !== null)
        showProgramDetails(1);

    if (document.getElementById("interviews") !== null) {
        loadInterviews();
    }

}


function toogleSettings() {
    document.getElementById("submenu-notifications").style.display = "none";
    $("#submenu-settings").toggle();
}

function toogleNotifications() {
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

function openHelp() {
    hideMenus();

    document.getElementById("fade-background").style.display = "block";
    document.getElementById("help-content").style.display = "block";
    document.getElementById("help-close").style.display = "block";
}

function closeHelp() {
    hideMenus();

    document.getElementById("fade-background").style.display = "none";
    document.getElementById("help-content").style.display = "none";

    document.getElementById("help-close").style.display = "none";
    document.getElementById("application-details").style.display = "none";
    $('.loading-div').hide();
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

function openProfilePreview(techID) {

    var actionUrl = '/Mobility/TechnicianPreview?IdTechnician=';
    $.ajax({
        type: "POST",
        url: actionUrl += techID,
        dataType: "html",
    }).done(function (result) {
        $("#profile-previewdiv").html(result);
    });;

}

function closeProfilePreview() {
    document.getElementById("profile-previewdiv").innerHTML = "";
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

function hideMenus() {
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

function chooseInstitution(institution) {
    if (institution.parentNode.getAttribute('id') === 'not-chosen-institutions')
        document.getElementById("institutions-chosen").appendChild(institution);
    else
        document.getElementById("not-chosen-institutions").appendChild(institution);
}

function enableApplicationButton()
{
    document.getElementById("submit-application-btn").disabled = false;
}

function applicationInstitutions() {
    if ($("#form-application").valid()) 
        var actionUrl = '/Application/RegisterApplicationInstitutions';

        var list = document.getElementById("chosen-institutions").getElementsByTagName("li");

        if (list.length === 0)
        {
            document.getElementById("no-institutions-chosen-error").style.display = "block";
            $("#form-application").preventDefault();
            return;
        }
        else
        {
            document.getElementById("no-institutions-chosen-error").style.display = "none";

            for (var i = 0; i < list.length; i++) {
                var institution = list[i].innerText;
                if (i === 0)
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

            });
        }
    }


function toggleCourses(elem) {
    var jqString = "#" + elem.id;
    var jqElem = $(jqString);

    jqElem.next().toggle(250);
}

function toggleMotivationCard(elem) {
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
        var id = this["idApplication"].value;
        event.preventDefault();
        var form = this;
        $.confirm({
            title: 'Atenção!',
            useBootstrap: false,
            modal: true,
            autoOpen: false,
            draggable: false,
            content: '<span style="font-size:1.25vmax"> Está a cancelar uma candidatura.\nTem a certeza que pretende continuar? </span>'+
                    '<br><br> <span style="font-size:1.25vmax">Caso pretenda indique o motivo do cancelamento</span>.<textarea  type="text" maxlength="512" rows="10" id="reason" style="margin-left:0;margin-top:1%;width:100%;resize:none">',
            buttons: {
                sim: {
                    text: 'Confirmar',
                    btnClass: 'btn-green',
                    action: function () {
                        $.ajax({
                            type: 'POST',
                            dataType: 'json',
                            contentType: 'application/json', 
                            url: '/Application/CancelationReason?applicationID=' + id + '&reason=' + document.getElementById("reason").value,
                            error: function (error) {
                            },
                            success: function () {
                               form.submit();
                            }
                        });
                    }
                },
                nao: {
                    text: 'Cancelar',
                    btnClass: 'btn-red',
                    action: function () {
                        event.preventDefault();
                    }
                },

            }
        })
    });

    $("form.evaluate-application-form").submit(function (event) {
        event.preventDefault();
        var form = this;
        $.confirm({
            title: 'Atenção!',
            useBootstrap: false,
            modal: true,
            autoOpen: false,
            draggable: false,
            content: 'Deseja avaliar, desta forma, a presente candidatura?\nEsta acção é irreversível.',
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


function loadInterviews() {
    $('#interviews').monthly({
        mode: 'event',
        xmlUrl: '/js/interviews.xml',
        maxWidth: false,
        dayNames: ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"],
        
    });
}


function autoPlayGallery() {
    function Move() {
        var i;
        var obj;
        for (i = 1; i < 5; i++) {
            if (document.getElementById('i' + i) !== null && document.getElementById('i' + i).checked) {
                i = (i % 4) + 1;
                document.getElementById('i' + i).checked = true;

                document.getElementById('img' + i - 1).style.display = "none";
                document.getElementById('img' + i).style.display = "block";

                break;
            }
        }

    }
    setInterval(Move, 6400);
}

function setColor() {
    var value = Math.floor(Math.random() * (Math.floor(6) - Math.ceil(1) + 1)) + 1;

    switch (value) {
        case 1:
            document.documentElement.style.setProperty('--main-color', '#F18735'); //principal (menu) laranja
            document.documentElement.style.setProperty('--secundary-color', '#EDEDED'); //secundária (letras menu)
            document.documentElement.style.setProperty('--hover-color', '#383838'); //hover
            document.documentElement.style.setProperty('--hover-button', '#F1873599'); //hoverDotsSlider
            break;
        case 2:
            document.documentElement.style.setProperty('--main-color', '#F5B738'); //amarelo torrado
            document.documentElement.style.setProperty('--hover-button', '#F5B73899'); //hoverDotsSlider
            break;
        case 3:
            document.documentElement.style.setProperty('--main-color', '#C1D53C'); //verde
            document.documentElement.style.setProperty('--hover-button', '#C1D53C99'); //hoverDotsSlider
            break;
        case 4:
            document.documentElement.style.setProperty('--main-color', '#508EC7'); //azul
            document.documentElement.style.setProperty('--secundary-color', '#EDEDED'); //secundária (letras menu)
            document.documentElement.style.setProperty('--hover-color', '#383838'); //hover
            document.documentElement.style.setProperty('--hover-button', '#508EC799'); //hoverDotsSlider
            break;
        case 5:
            document.documentElement.style.setProperty('--main-color', '#D53874'); //rosa DD1D66
            document.documentElement.style.setProperty('--secundary-color', '#EDEDED'); //secundária (letras menu)
            document.documentElement.style.setProperty('--hover-color', '#383838'); //hover
            document.documentElement.style.setProperty('--hover-button', '#D5387499'); //hoverDotsSlider
            break;
        default:
            document.documentElement.style.setProperty('--main-color', '#707FBA'); //roxo
            document.documentElement.style.setProperty('--secundary-color', '#EDEDED'); //secundária (letras menu)
            document.documentElement.style.setProperty('--hover-color', '#383838'); //hover
            document.documentElement.style.setProperty('--hover-button', '#707FBA99'); //hoverDotsSlider
        
    }
}



function evaluateApplication() {

    var ects_done = document.getElementById("ects_done").value;
    var ects_all = document.getElementById("ects_all").value;
    var motivations = document.getElementById("motivations").value;
    var interview = document.getElementById("interview").value;
    var average_grades = document.getElementById("average_grades").value;
    var evaluate_result = document.getElementById("evaluate_result").innerHTML;

    var final_classification = document.getElementById("final_classification");

    var classification = ((ects_done / ects_all) * 100) * 0.35 + ((motivations * 0.5 + interview * 0.5)) * 0.35 + (average_grades * 5) * 0.30;

    if (!isFinite(classification))
        classification = 0;

    final_classification.value = classification.toFixed(2);

    if (classification >= 50) {
        document.getElementById("evaluate_result").innerHTML = "Aprovado!";
        document.getElementById("evaluate_result").style.color = "green";
        document.getElementById("technicianDd").disabled = false;
        document.getElementById("institutionDd").disabled = false;
    }
    else {
        document.getElementById("evaluate_result").innerHTML = "Não aprovado";
        document.getElementById("evaluate_result").style.color = "red";
        document.getElementById("technicianDd").disabled = true;
        document.getElementById("institutionDd").disabled = true;
    }

}

function confirmApplication(appId) {

    var confirmUrl = "/Application/Confirm/?appId=" + appId;

    $.confirm({
        title: 'Atenção!',
        useBootstrap: false,
        modal: true,
        autoOpen: false,
        draggable: false,
        content: 'Deseja confirmar a candidatura e prosseguir com a mobilidade?\nEsta acção é irreversível.',
        buttons: {
            sim: {
                text: 'Sim',
                btnClass: 'btn-green',
                action: function () {
                    $.ajax({
                        type: 'POST',
                        dataType: 'html',
                        url: confirmUrl,
                        error: function (error) {
                        }
                    });
                }
            },
            nao: {
                text: 'Não',
                btnClass: 'btn-red',
                action: function () {
                }
            },

        }
    });
}

function showApplicationDetails(applicationID) {
    $("#application-details").html("");

    $('.loading-div').show();

    document.getElementById("fade-background").style.display = "block";
    document.getElementById("application-details").style.display = "block";

    var detailsURL = "/Application/Details/" + applicationID;

    $.ajax(
        {
            type: 'GET',
            dataType: 'html',
            url: detailsURL,
            beforeSend: function () {
                $('#loading-div').show();
            },
            success: function (result) {
                setTimeout(function () {
                    $("#application-details").html(result);
                    $('.loading-div').hide();
                }, 1000)
            },
            error: function (error) {
            }
        });
}

function showApplicationEvaluation(applicationID) {
    $("#application-details").html("");

    $('.loading-div').show();

    document.getElementById("fade-background").style.display = "block";
    document.getElementById("application-details").style.display = "block";

    var detailsURL = "/Application/EvaluationDetails/" + applicationID;

    $.ajax(
        {
            type: 'GET',
            dataType: 'html',
            url: detailsURL,
            beforeSend: function () {
                $('#loading-div').show();
            },
            success: function (result) {
                setTimeout(function () {
                    $("#application-details").html(result);
                    $('.loading-div').hide();
                }, 1000)
            },
            error: function (error) {
            }
        });
}

function scheduleInterview(applicationID) {
    $("#application-details").html("");

    $('.loading-div').show();

    document.getElementById("fade-background").style.display = "block";
    document.getElementById("application-details").style.display = "block";

    var detailsURL = "Application/ScheduleInterview/" + applicationID;

    $.ajax(
        {
            type: 'GET',
            dataType: 'html',
            url: detailsURL,
            beforeSend: function () {
                $('#loading-div').show();
            },
            success: function (result) {
                setTimeout(function () {
                    $("#application-details").html(result);
                    $('.loading-div').hide();
                }, 150)
            },
            error: function (error) {
            }
        });
}

function reScheduleInterview(applicationID) {
    $("#application-details").html("");

    $('.loading-div').show();

    document.getElementById("fade-background").style.display = "block";
    document.getElementById("application-details").style.display = "block";

    var detailsURL = "/Application/RescheduleInterview/" + applicationID;

    $.ajax(
        {
            type: 'GET',
            dataType: 'html',
            url: detailsURL,
            beforeSend: function () {
                $('#loading-div').show();
            },
            success: function (result) {
                setTimeout(function () {
                    $("#application-details").html(result);
                    $('.loading-div').hide();
                }, 150)
            },
            error: function (error) {
            }
        });
}

function viewProfile(studentId) {
    $("#application-details").html("");

    $('.loading-div').show();

    document.getElementById("fade-background").style.display = "block";
    document.getElementById("application-details").style.display = "block";

    var detailsURL = "../Profile/ViewStudentProfile/" + studentId;

    $.ajax(
        {
            type: 'GET',
            dataType: 'html',
            url: detailsURL,
            beforeSend: function () {
                $('#loading-div').show();
            },
            success: function (result) {
                setTimeout(function () {
                    $("#application-details").html(result);
                    $('.loading-div').hide();
                }, 1000)
            },
            error: function (error) {
            }
        });
}

function showProgramDetails(programId) {
    switchDetailsTo(programId);
    $(".program-details").html("");

    $('.loading-program-div').show();

    var detailsURL = "Program/Details?programID=" + programId;

    $.ajax(
        {
            type: 'GET',
            dataType: 'html',
            url: detailsURL,
            beforeSend: function () {
                $('.loading-program-div').show();
            },
            success: function (result) {
                setTimeout(function () {
                    $(".program-details").html(result);
                   $('.loading-program-div').hide();
                }, 1650)
            },
            error: function (error) {
            }
        });
}

function switchDetailsTo(programId) {
    switch (programId) {
        case 1:
            $("#programErasmus").addClass("selected-program-banner");
            $("#programErasmus").removeClass("program-banner");

            $("#programLuso").addClass("program-banner");
            $("#programLuso").removeClass("selected-program-banner");

            $("#programIbero").addClass("program-banner");
            $("#programIbero").removeClass("selected-program-banner");

            $("#programVasco").addClass("program-banner");
            $("#programVasco").removeClass("selected-program-banner");

            $("#programMacau").addClass("program-banner");
            $("#programMacau").removeClass("selected-program-banner");
            break;

        case 4:
            $("#programErasmus").addClass("program-banner");
            $("#programErasmus").removeClass("selected-program-banner");

            $("#programLuso").addClass("selected-program-banner");
            $("#programLuso").removeClass("program-banner");

            $("#programIbero").addClass("program-banner");
            $("#programIbero").removeClass("selected-program-banner");

            $("#programVasco").addClass("program-banner");
            $("#programVasco").removeClass("selected-program-banner");

            $("#programMacau").addClass("program-banner");
            $("#programMacau").removeClass("selected-program-banner");
            break;

        case 5:
            $("#programErasmus").addClass("program-banner");
            $("#programErasmus").removeClass("selected-program-banner");

            $("#programLuso").addClass("program-banner");
            $("#programLuso").removeClass("selected-program-banner");

            $("#programIbero").addClass("selected-program-banner");
            $("#programIbero").removeClass("program-banner");

            $("#programVasco").addClass("program-banner");
            $("#programVasco").removeClass("selected-program-banner");

            $("#programMacau").addClass("program-banner");
            $("#programMacau").removeClass("selected-program-banner");
            break;

        case 6:
            $("#programErasmus").addClass("program-banner");
            $("#programErasmus").removeClass("selected-program-banner");

            $("#programLuso").addClass("program-banner");
            $("#programLuso").removeClass("selected-program-banner");

            $("#programIbero").addClass("program-banner");
            $("#programIbero").removeClass("selected-program-banner");

            $("#programVasco").addClass("selected-program-banner");
            $("#programVasco").removeClass("program-banner");

            $("#programMacau").addClass("program-banner");
            $("#programMacau").removeClass("selected-program-banner");
            break;

        case 7:
            $("#programErasmus").addClass("program-banner");
            $("#programErasmus").removeClass("selected-program-banner");

            $("#programLuso").addClass("program-banner");
            $("#programLuso").removeClass("selected-program-banner");

            $("#programIbero").addClass("program-banner");
            $("#programIbero").removeClass("selected-program-banner");

            $("#programVasco").addClass("program-banner");
            $("#programVasco").removeClass("selected-program-banner");

            $("#programMacau").addClass("selected-program-banner");
            $("#programMacau").removeClass("program-banner");
            break;
    }
}



function minmax(value, min, max) {
    if (value < min || isNaN(parseFloat(value)))
        return min;
    else if (value > max)
        return max;

    return value;
}

function preview_image(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('edit-image-preview');
        output.src = reader.result;
    }
    reader.readAsDataURL(event.target.files[0]);
}