
$(function () {
    
    

    //delete dialog
    $('#delete-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "Yes": function () {             
                $.post(deleteLinkObj[0].href, function (obj) {  //Post to action

                    if (obj.Success) {
                        $('#message').html("<b style='color:Green'>" + obj.Message + "</b>");
                        //                        $("#successfull-dialog .msg").html(obj.Message).css("color", "Green");
                        //                        $('#successfull-dialog').dialog('open');
                        jQuery("#jqGrid").trigger("reloadGrid");
                        jQuery("#jqGrid1").trigger("reloadGrid");
                    }
                    else {
                        $("#message").html("<b>" + obj.Message + "</b>").css("color", "red");
                        //                        $("#error-dialog .msg").html(obj.Message).css("color", "red");
                        //                        $('#error-dialog').dialog('open');
                    }
                });
                $(this).dialog("close");
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });
    //delete dialog for link
    $(".delete-dialog-link").dialog({
        autoOpen: false,
        modal: true,
        width: 400,
        resizable: false
    });

    //success dialog
    $('#successfull-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        }
    });
    //error dialog
    $('#error-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        }
    });

   

    // link delete function call
    $(".delete-confirm").click(function (e) {
        //console.log('call');
        
        e.preventDefault();
        var targetUrl = $(this).attr("href");

        $(".delete-dialog-link").dialog({
            buttons: {
                "Yes": function () {
                    window.location.href = targetUrl;
                },
                "No": function () {
                    $(this).dialog("close");
                }
            }
        });

        $(".delete-dialog-link").dialog("open");
    });

});

// Dialog for Rollback

$(function () {

    //rollback dialog
    $('#rollback-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "Yes": function () {                
                $.post(rollbackLinkObj[0].href, function (obj) {  //Post to action

                    if (obj.Success) {
                        $('#message').html("<b style='color:Green'>" + obj.Message + "</b>");
                        jQuery("#jqGrid").trigger("reloadGrid");
                        jQuery("#jqGrid1").trigger("reloadGrid");
                    }
                    else {
                        $("#message").html("<b>" + obj.Message + "</b>").css("color", "red");
                    }
                });
                $(this).dialog("close");
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

    //approve dialog
    $('#approve-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "Yes": function () {
                //alert(rollbackLinkObj[0].href);
                $.post(rollbackLinkObj[0].href, function (obj) {  //Post to action

                    if (obj.Success) {
                        $('#message').html("<b style='color:Green'>" + obj.Message + "</b>");
                        jQuery("#jqGrid").trigger("reloadGrid");
                        jQuery("#jqGrid1").trigger("reloadGrid");
                    }
                    else {
                        $("#message").html("<b>" + obj.Message + "</b>").css("color", "red");
                    }
                });
                $(this).dialog("close");
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

    //rollback dialog for link
    $(".rollback-dialog-link").dialog({
        autoOpen: false,
        modal: true,
        width: 400,
        resizable: false
    });

    //success dialog
    $('#successfull-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        }
    });

    //error dialog
    $('#error-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        }
    });


    // link delete function call
    $(".rollback-confirm").click(function (e) {
        //console.log('call');
        e.preventDefault();
        var targetUrl = $(this).attr("href");

        $(".rollback-dialog-link").dialog({
            buttons: {
                "Confirm": function () {
                    window.location.href = targetUrl;
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

        $(".rollback-dialog-link").dialog("open");
    });

});

// End rollback dialog

$(function () {

    //process dialog
    $('#process-dialog').dialog({
        autoOpen: false, width: 400, resizable: false, modal: true, //Dialog options
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            "Yes": function () {
                $.post(deleteLinkObj[0].href, function (obj) {  //Post to action

                    if (obj.Success) {
                        $('#message').html("<b style='color:Green'>" + obj.Message + "</b>");
                        jQuery("#jqGrid").trigger("reloadGrid");
                        jQuery("#jqGrid1").trigger("reloadGrid");
                    }
                    else {
                        $("#message").html("<b>" + obj.Message + "</b>").css("color", "red");
                    }
                });
                $(this).dialog("close");
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

});


function parseDate(str) {
    var mdy = str.split('-');
    return new Date(mdy[2], mdy[1], mdy[0]);
}

function setDatePicker() {
    $("input[id*=Date]").width('100');
    $("input[id*=Date]").datepicker({ buttonImage: '../Content/Images/calendar-blue.gif', dateFormat: 'dd-mm-yy', buttonImageOnly: true, showOn: "button", buttonText: 'Choose' });
}

function clearValidationSummary() {

    $('.field-validation-error')
               .removeClass('field-validation-error')
                .addClass('field-validation-valid');

    $('.input-validation-error')
                .removeClass('input-validation-error')
                 .addClass('valid');
}

$.fn.removeAttrs = function () {
    // Convert all passed arguments to an array
    var args = Array.prototype.slice.call(arguments),
        attr;

    // Loop, removing the first array item on each iteration
    while (attr = args.shift())
        this.removeAttr(attr);

    // Return the jQuery object for chaining
    return this;
}


