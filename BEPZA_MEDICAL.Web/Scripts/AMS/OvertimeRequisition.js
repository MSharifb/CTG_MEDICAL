var OveerTimeDetail = [];

var model = {};
var loadEmpUrl = $("#pageData").data("URL").initEmp, addEmpUrl = $("#pageData").data("URL").addEmp, hourUrl = $("#pageData").data("URL").hourUrl, redirectUrl = $("#pageData").data("URL").redirectUrl;
$(function () {
    var Id = $("#Id").val();
    var mode = $("#Mode").val();
    var url = $("#frm").attr("action");
    
    if ((Id != null || Id != undefined || Id <= 0) && mode == 'Create' )  {
        OveerTimeDetail.length = 0;
    }
    else {
        $("#btnSave").attr('value', 'Update');
    }

    FormatTextBox();
    $("#divEmpList").dialog({ autoOpen: false, modal: true, height: 600, width: 940, title: 'Employee', beforeclose: function (event, ui) { Closing(); } });

    $("#btnSave").live('click', function (e) {
 
        e.preventDefault();
        if ($("#frm").valid()) {

            if (OveerTimeDetail.length == 0 && mode =='Create') {
                $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Detail Information Can't Empty</span>  </div> ");
            }
            else {
                $("#shiftInfo input[type!=button],  textarea, #shiftInfo select").each(function () {
                    model[$(this).attr("name")] = $(this).val();
                });
                $.each(OveerTimeDetail, function (index, value) {
                    value.FromTime = value.FromTimes;
                    value.ToTime = value.ToTimes;
                });

                model.OvertimeRequisitionDetail = OveerTimeDetail;

                $.ajax({
                    url: url,
                    type: 'POST',
                    dataType: 'json',
                    data: JSON.stringify({ model: model }),
                    contentType: "application/json; charset=utf-8",
                    error: function () {
                        $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> System Error!</span>  </div> ");
                    },
                    success: function (result) {
                        //$("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Information has been Saved successfully.</span>  </div> ");
                        //$("#shiftInfo input[type!=button], textarea, #shiftInfo select").each(function () {
                        //    $(this).val('');
                        //});
                        //$("#shiftGrid > tbody").html("");
                     //   console.log(result);
                        if (result == 1)
                            window.location = redirectUrl;
                        else
                            $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> "+ result + "</span>  </div> ");
                    }
                });
            }

        }
        else
            $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Invalid Input! </span>  </div> ");
       
    });

    $("#btnAdd").live('click', function () {
        var obj = {};
        if (InnerValid('ShiftDetailInfo')) {
            $("#ShiftDetailInfo input[type!=button], #ShiftDetailInfo textarea").each(function () {
                var outTime, inTime;
                obj[$(this).attr("name")] = $(this).val();
                if ($(this).val() != null || $(this).val() != undefined) {
                    if ($(this).attr("name") == 'ToTime' || $(this).attr("name") == 'FromTime') {
                        obj[$(this).attr("name")] = $.datepicker.parseTime('HH:mm', $(this).val());
                        obj[$(this).attr("name")+'s'] =  $(this).val();
                    }
                }
            });
            obj.sid = OveerTimeDetail.length + 1;
            if (obj.FromTime.hour < obj.ToTime.hour) {
                SetShiftData(obj);
                $("#ShiftDetailInfo input[type!=button], #ShiftDetailInfo textarea").each(function () {
                    $(this).val('');
                });
            }
            else
                $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Invalid From Time and To Time.</span>  </div> ");
        }
        
    });

    $("#ToTime").live('change', function () {
        TotalCalculate();
    });

    $("#FromTime").live('change', function () {
        TotalCalculate();
    });

    function SetShiftData(obj)
    {
        OveerTimeDetail.push(obj);
        var newRowContent = " <tr class=\"row\"><td> <label for=\"PeriodF\">" + obj.EmpID + " </label> </td><td> <label for=\"PeriodT\"> " + obj.EmployeeName + " </label> </td> <td> <label for=\"InT\"> " + obj.Designation + " </label> </td>" +
                            "<td> <label for=\"OutT\"> " + obj.FromTimes + " </label> </td> <td> <label for=\"EfDate\"> " + obj.ToTimes + " </label> </td> <td> <label for=\"Rm\"> " + obj.TotalHours + " </label> </td> <td>" +
                            "<a class=\"deleteRow fixeddeleteRow\" onclick=\"RemoveShift(this," + obj.sid + ")\" title=\"delete\" href=\"#\"> <img alt=\"delete\" style=\"border: none;\" src=\"../../Content/Images/Delete.png\" /> </a>  </td>";
        $("#shiftGrid tbody").append(newRowContent);
    }

    function TotalCalculate() {
        var totalHour = 0;
        if ($("#ToTime").val() != null && $("#FromTime").val() != null && $("#ToTime").val() != undefined && $("#FromTime").val() != undefined) {
            //totalHour = $.datepicker.parseTime('HH:mm', $("#ToTime").val()).hour - $.datepicker.parseTime('HH:mm', $("#FromTime").val()).hour;

            var hour = $.datepicker.parseTime('HH:mm', $("#ToTime").val()).hour - $.datepicker.parseTime('HH:mm', $("#FromTime").val()).hour;
            var min = $.datepicker.parseTime('HH:mm', $("#ToTime").val()).minute - $.datepicker.parseTime('HH:mm', $("#FromTime").val()).minute;
            if(min<0)
            {
                min = min + 60;
                hour--;
            }
            min = min / 60;
            hour += min;
            hour = parseInt(hour);
            var totalHour = hour;
        }
        if (totalHour > 0) {
            $.ajax({
                url: hourUrl,
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify({ Id: $("#EmpID").val(), TotalHour: totalHour }),
                contentType: "application/json; charset=utf-8",
                error: function () {
                    $("#TotalHours").val('');
                    $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Total Hours is Grater than Overtime Rule Or Employee is not Valid for Overtime.</span>  </div> ");
                },
                success: function (result) {
                    $("#TotalHours").val(totalHour);
                }
            });
        }
    }

});

function RemoveShift(el,sid) {
     $(el).parent().parent().remove();
     var arr = GetSliceArray(OveerTimeDetail, sid);
     OveerTimeDetail.length = 0;
     OveerTimeDetail = arr;
}

function RemoveDb(el, id, url) {
  //  console.log(url);
    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({Id:id}),
        contentType: "application/json; charset=utf-8",
        error: function () {
            $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> System Error!</span>  </div> ");
        },
        success: function (result) {
            $(el).parent().parent().remove();
        }
    });
}
 
function GetSliceArray(shiftDetail, sid) {
   
    var out = [];
    $.each(shiftDetail, function (index, value) {
        if (value.sid != sid)
            out.push(value);
    });
    return out;
}

function openEmployee() {

    $.ajax({
        url: loadEmpUrl,
        type: 'GET',
        dataType: 'text',
        timeout: 5000,
        error: function () {
            alert('System is unable to load data please try again.');
        },
        success: function (result) {
            $('#divEmpList').html(result);
        }
    });

    $("#divEmpList").dialog('open');

    return false;
}

function setData(id) {
    $('#EmployeeId').val(id)
    GetEmployeeInfo();
    $("#divEmpList").dialog('close');

}

function GetEmployeeInfo() {
    var empId = $('#EmployeeId').val();
    var form = $('#frm');
    var serializedForm = form.serialize();
  
    if (empId > 0) {
        $.post(addEmpUrl, serializedForm, function (obj) {
            $("#EmpID").val(obj.EmpId);
            $('#EmployeeName').val(obj.EmployeeName);
            $("#Designation").val(obj.Designation);
        }, "json");
    }

    return false;
}

function Closing() {

}

