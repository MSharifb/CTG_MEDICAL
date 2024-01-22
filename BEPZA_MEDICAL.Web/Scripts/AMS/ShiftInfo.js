var ShiftDetail = [];
var BreakInfo = [];
var model = {};
$(function () {
    var Id = $("#Id").val();
    var mode = $("#Mode").val();
    var url = $("#frm").attr("action");
    var redirectUrl = $("#pageData").data("URL").redirectUrl;
    function ShowHide(){
        if ($("#ShiftType").val() == '2')
            $(".roster-ctr").hide();
        else
            $(".roster-ctr").show();
    }

    $('#ShiftType').live('change', function (e) {
        ShowHide();
    });
    ShowHide();
    if ((Id != null || Id != undefined || Id <= 0) && mode == 'Create' )  {
        ShiftDetail.length = 0;
        BreakInfo.length = 0;
    }
    else {
        $("#btnSave").attr('value', 'Update');
    }

    FormatTextBox();

    $("#btnSave").live('click', function (e) {
        e.preventDefault();
        if ($("#frm").valid()) {
            if (ShiftDetail.length == 0 && mode == 'Create') {
                    $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Shift Detail Information Can't Empty</span>  </div> ");
            }
            else {
                $("#shiftInfo input[type!=button], #shiftInfo textarea, #shiftInfo select").each(function () {
                    model[$(this).attr("name")] = $(this).val();
                });
                model["IsRoster"] = $("#IsRoster").is(":checked");
                model["IsNightShift"] = $("#IsNightShift").is(":checked");
                
                $.each(ShiftDetail, function (index, value) {
                    value.InTime = value.InTimes;
                    value.OutTime = value.OutTimes;
                });
                $.each(BreakInfo, function (index, value) {
                    value.StartTime = value.StartTimes;
                    value.EndTime = value.EndTimes;
                });
                model.AMS_ShiftInformationDetail = ShiftDetail;
                model.AMS_ShiftBreakInformationViewModel = BreakInfo;
                $.ajax({
                    url: url,
                    type: 'POST',
                    dataType: 'json',
                    data: JSON.stringify({ model: model }),
                    contentType: "application/json; charset=utf-8",
                    error: function (request, status, error) {
                        $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Period Information is Invalid Or Shift Name Can't Be Duplicate!</span>  </div> ");
                    },
                    success: function (result) {
                        window.location = redirectUrl;
                    }
                });
            }
        } 
       
    });

    $("#btnAdd").live('click', function () {
        var obj = {},isFlag = true;
        if (InnerValid('ShiftDetailInfo')) {
            $("#ShiftDetailInfo input[type!=button], #ShiftDetailInfo textarea").each(function () {
                var outTime, inTime;
                obj[$(this).attr("name")] = $(this).val();
                if ($(this).val() != null || $(this).val() != undefined) {
                    if ($(this).attr("name") == 'OutTime' || $(this).attr("name") == 'InTime') {
                        obj[$(this).attr("name")] = $.datepicker.parseTime('HH:mm', $(this).val());
                        obj[$(this).attr("name")+'s'] =  $(this).val();
                    }
                }
            });
            obj.sid = ShiftDetail.length + 1;
            var objFromDate = new Date(obj.PeriodFrom);
            var objToDate = new Date(obj.PeriodTo);
            var objEffectiveDate = new Date(obj.EffectiveDate);   // new Add

            if (objFromDate.getTime() > objToDate.getTime()) {
                isFlag = false;
            }

            if (objFromDate.getTime() > objEffectiveDate.getTime() || objEffectiveDate.getTime() > objToDate.getTime())    // new Add
                isFlag = false;

            $.each(ShiftDetail, function (index, value) {
              var fromDate =  new Date(value.PeriodFrom);
              var toDate = new Date(value.PeriodTo);

              if (objFromDate.getTime() < toDate.getTime() && fromDate.getTime() < objToDate.getTime())
                  isFlag = false;

            });

            if (isFlag) {
               
                if (!$("#IsNightShift").is(":checked")) {
                    if (obj.InTime.hour < obj.OutTime.hour) {
                        SetShiftData(obj);
                        $("#ShiftDetailInfo input[type!=button], #ShiftDetailInfo textarea").each(function () {
                            $(this).val('');
                        });
                        $("#message").html("<b></b>");
                    }
                    else
                        $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Required Valid In Time and Out Time Information</span>  </div> ");
                }
                else {
                    SetShiftData(obj);
                    $("#ShiftDetailInfo input[type!=button], #ShiftDetailInfo textarea").each(function () {
                        $(this).val('');
                    });
                    $("#message").html("<b></b>");
                }
            }
            else
             $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Required Valid Period Information</span>  </div> ");
        }
        
    });

    $("#btnBreakAdd").live('click', function () {
        //frmBreak
        var obj = {};
        if (InnerValid('frmBreak')) {
            $("#frmBreak input[type!=button], #frmBreak textarea").each(function () {
                obj[$(this).attr("name")] = $(this).val();
                if ($(this).val() != null || $(this).val() != undefined) {
                    if ($(this).attr("name") == 'StartTime' || $(this).attr("name") == 'EndTime') {
                        obj[$(this).attr("name")] = $.datepicker.parseTime('HH:mm', $(this).val());
                        obj[$(this).attr("name") + 's'] = $(this).val();
                    }
                }
            });
            obj.sid = BreakInfo.length + 1;
            if (obj.StartTime.hour <= obj.EndTime.hour) {
                SetBreakData(obj);
                $("#frmBreak input[type!=button], #frmBreak textarea").each(function () {
                    $(this).val('');
                });
            }
            else
                $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Required Valid In Time and Out Time Information</span>  </div> ");

        }

    });

    function SetShiftData(obj)
    {
        ShiftDetail.push(obj);
        var newRowContent = " <tr class=\"row\"><td> <label for=\"PeriodF\">" + obj.PeriodFrom + " </label> </td><td> <label for=\"PeriodT\"> " + obj.PeriodTo + " </label> </td> <td> <label for=\"InT\"> " + obj.InTimes + " </label> </td>" +
                            "<td> <label for=\"OutT\"> " + obj.OutTimes + " </label> </td> <td> <label for=\"EfDate\"> " + obj.EffectiveDate + " </label> </td> <td> <label for=\"Rm\"> " + obj.Remarks + " </label> </td> <td>" +
                            "<a class=\"deleteRow fixeddeleteRow\" onclick=\"RemoveShift(this," + obj.sid + ")\" title=\"delete\" href=\"#\"> <img alt=\"delete\" style=\"border: none;\" src=\"../../Content/Images/Delete.png\" /> </a>  </td>";
        $("#shiftGrid tbody").append(newRowContent);
      //  console.log(obj);
    }

    function SetBreakData(obj) {
        BreakInfo.push(obj);
        var newRowContent = " <tr class=\"row\"><td> <label for=\"PeriodF\">" + obj.BreakName + " </label> </td><td> <label for=\"PeriodT\"> " + obj.StartTimes + " </label> </td> <td> <label for=\"InT\"> " + obj.EndTimes + " </label> </td>" +
                            "<td> <label for=\"Rm\"> " + obj.Remarks + " </label> </td> <td>" +
                            "<a onclick=\"RemoveBreak(this," + obj.sid + ")\" title=\"delete\" href=\"#\"> <img alt=\"delete\" style=\"border: none;\" src=\"../../Content/Images/Delete.png\" /> </a>  </td>";
        $("#grid tbody").append(newRowContent);
        
    }

});

function RemoveShift(el,sid) {
    //var link = document.createElement("a");
    //link.innerHTML = "<img alt=\"delete\" style=\"border: none;\" src=\"/Content/Images/Delete.png\" />";
    //link.onclick = function (e) {
    //    RemoveShift(obj);
    //}
     $(el).parent().parent().remove();
    var arr = GetSliceArray(ShiftDetail, sid);
    ShiftDetail.length = 0;
    ShiftDetail = arr;
}

function RemoveBreak(el,sid) {

    $(el).parent().parent().remove();
    var arr = GetSliceArray(BreakInfo, sid);
    BreakInfo.length = 0;
    BreakInfo = arr;
    console.log(BreakInfo);

}

function RemoveDb(el, id, type, url) {
  //  console.log(url);
    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({Id:id,type:type}),
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