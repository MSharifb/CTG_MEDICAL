$(function () {
    var mode = $("#Mode").val();
    var loadEmpUrl = $("#pageData").data("URL").initEmp, addEmpUrl = $("#pageData").data("URL").addEmp;

    if ( mode == 'Edit') {
        $("#btnSave").attr('value', 'Update');
    }

    $(".chkAll").live('click', function () {
        $(".rchk").attr('checked', $(this).attr('checked'));

        $('.rchk').each(function () {
            if ($(this).attr('checked')) {
                Employees.push($(this).val());
            }
            else {
                var index = Employees.indexOf($(this).val());
                Employees.splice(index, 1);
            }
        });

    });

    $(".rchk").live('click', function () {

        if ($(this).attr('checked')) {
            Employees.push($(this).val());
        }
        else {
            var index = Employees.indexOf($(this).val());
            Employees.splice(index, 1);
        }

    });

    $("#GradeId").live('change', function () {
        if ($("#DepartmentId").val() != "" && $("#GradeId").val() != "") {
            $("#ErrorMessage").html("");
            $.ajax({
                type: "GET",
                url: loadEmpUrl,
                data: { departmentId: $("#DepartmentId").val(), gradeId: $("#GradeId").val(), shiftTypeId: $("#ShiftType").val() },
                success: (function (result) {
                    $(".chkAll").attr('checked', false);
                    $("#grid tbody").html(result);
                })
            });

        }
        else
            $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Required Department and Grade Information</span>  </div> ");
       
    });

    $('.addItem').live('click', function (e) {
        e.preventDefault();
        var PostingEmp = new Array();
        function logArrayElements(element, index, array) {
            PostingEmp.push({ 'EmployeeId': element });
        }

        Employees.forEach(logArrayElements);
        if (PostingEmp.length > 0) {
            $.ajax({
                type: "POST",
                url: addEmpUrl,
                cache: false,
                data: {
                    'EmployeeCodes': PostingEmp,
                    shiftTypeId: $("#ShiftType").val()
                },
                success: (function (result) {
                    //$(".chkAll").attr('checked', false);
                    $(".tblOrderList tbody").html(result);
                })
            });
        }
    });


});

function RemoveDb(el, id,empId, url) {
    //  console.log(url);
    if (id == 0 || id == undefined) {
        var index = Employees.indexOf(empId);
        Employees.splice(index, 1);
        $(el).parent().parent().remove();
        $("input[name=empId]").each(function () {
            if ($(this).val() == empId) {
                $(this).attr('checked', false);
            }
        });
       
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
