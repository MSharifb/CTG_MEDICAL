$(function () {
    var mode = $("#Mode").val();
    var loadEmpUrl = $("#pageData").data("URL").initEmp, addEmpUrl = $("#pageData").data("URL").addEmp;

    if (mode == 'Edit') {
        $("#btnSave").attr('value', 'Update');
    }

    $(".chkAll").live('click', function () {
        $(".check-box").attr('checked', $(this).attr('checked'));

        //$('.check-box').each(function () {
        //    if ($(this).attr('checked')) {
        //        Employees.push($(this).val());
        //    }
        //    else {
        //        var index = Employees.indexOf($(this).val());
        //        Employees.splice(index, 1);
        //    }
        //});

    });

    //$(".check-box").live('click', function () {

    //    if ($(this).attr('checked')) {
    //        Employees.push($(this).val());
    //    }
    //    else {
    //        var index = Employees.indexOf($(this).val());
    //        Employees.splice(index, 1);
    //    }

    //});

    $("#RosterId").live('change', function () {
        LoadEmployee($("#DepartmentId").val(), $("#GradeId").val(), $("#ApplyType").val(), $("#RosterId").val(), $("#ShiftId").val());
    });

    $("#ShiftId").live('change', function () {
        LoadEmployee($("#DepartmentId").val(), $("#GradeId").val(), $("#ApplyType").val(), $("#RosterId").val(), $("#ShiftId").val());
    });

    $("#ApplyType").live('change', function () {
        LoadEmployee($("#DepartmentId").val(), $("#GradeId").val(), $("#ApplyType").val(), $("#RosterId").val(), $("#ShiftId").val());
    });

    $("#DepartmentId").live('change', function () {
        LoadEmployee($("#DepartmentId").val(), $("#GradeId").val(), $("#ApplyType").val(), $("#RosterId").val(), $("#ShiftId").val());
    });

    $("#GradeId").live('change', function () {
        LoadEmployee($("#DepartmentId").val(), $("#GradeId").val(), $("#ApplyType").val(), $("#RosterId").val(), $("#ShiftId").val());
    });

    function LoadEmployee(departmentId,gradeId,applyType,rosterId,shiftId){

        if (departmentId != "" && departmentId != undefined) {
            $("#ErrorMessage").html("");
            $.ajax({
                type: "GET",
                url: loadEmpUrl,
                data: { departmentId: departmentId, gradeId: gradeId, applyTypeId: applyType, rosterId:rosterId, shiftId: shiftId },
                success: (function (result) {
                    $("#grid tbody").html(result);
                })
            });

        }
        else
            $("#ErrorMessage").show().html("<b> Required Department and Grade Information</b>").css("color", "red");
    }

});

function RemoveDb(el, id, empId, url) {
    //  console.log(url);
    if (id == 0 || id == undefined) {
        var index = Employees.indexOf(empId);

        $("input[name=empId]").each(function () {
            if ($(this).val() == empId) {
                $(this).attr('checked', false);
                Employees.splice(index, 1);
                $(el).parent().parent().remove();
            }
        });

        console.log(Employees);
    }
    else {
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify({ Id: id }),
            contentType: "application/json; charset=utf-8",
            error: function () {
                $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> System Error!</span>  </div> ");
            },
            success: function (result) {
                $(el).parent().parent().remove();
            }
        });
    }
}
