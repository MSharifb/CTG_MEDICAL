
/* - Please make sure the existance of the following containers before using this script - 

0. frm
1. divEmpList
2. ImgOpenEmpList
e.g. 
<img id="ImgOpenEmpList" 
             src='@Url.Content("~/Content/Images/btn_search.gif")'
             data-request-url1="@Url.Action("EmployeeSearchForPGM", "Employee")?searchEmpType=active"
             data-request-url2="@Url.Action("GetEmployeeInfo", "PGMCommon")" />

3. EmployeeId
4. EmpID
5. EmployeeInitial
6. EmployeeName
7. EmployeeDesignation

*/



var getEmployeeInfoActionUrl;

$(function () {
    try {
        
        $("#ImgOpenEmpList").css('cursor', 'pointer');

        $("#divEmpList").dialog({
            autoOpen: false,
            modal: true,
            height: 600,
            width: 900,
            title: 'Employee',
            beforeclose: function (event, ui) { Closing(); }
        });

        
        $("#ImgOpenEmpList").live('click',
            function (e) {
                e.preventDefault();
                
                getEmployeeInfoActionUrl = $(this).data("request-url2");
                
                return openEmployee(this);
            });

        EmptyCallFunc();

    } catch (err) {
        console.log(err.message);
    }
});

function openEmployee(e) {

    try {
        var url = $(e).data("request-url1");
        url = url.replace('PGM', 'PRM');
        
        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'text',
            timeout: 50000,
            error: function () {
                alert('System is unable to load data please try again.');
            },
            success: function (result) {
                $('#divEmpList').html(result);
            }
        });

        $("#divEmpList").dialog('open');

    } catch (err) {
        console.log(err.message);
    }

    return false;
}



function EmptyCallFunc() {
    // -- Do nothing
}

function Closing() {

}

function setData(id) {
    try {
        $('#EmployeeId').val(id);
        GetEmployeeInfo();
        $("#divEmpList").dialog('close');
    } catch (err) {
        console.log(err.message);
    }
}

function GetEmployeeInfo() {
    try {
        var empId = $('#EmployeeId').val();
        var form = $('#frm');
        var serializedForm = form.serialize();

        $('#EmpID').val('');
        $('#EmployeeInitial').val('');
        $('#EmployeeName').val('');
        $('#EmployeeDesignation').val('');

        if (empId > 0) {
            $.post(getEmployeeInfoActionUrl, serializedForm, function (obj) {

                if (obj.Result == 'InActiveEmployee') {
                    alert('Inactive employee is not allow for this operation.');
                }

                else if (obj.Result == false) {
                    alert('System is unable to load data please try again.');
                }

                else {
                    $("#EmpID").val(obj.EmpID);
                    $('#EmployeeName').val(obj.EmployeeName);
                    $("#EmployeeDesignation").val(obj.EmployeeDesignation);
                    $("#EmployeeInitial").val(obj.EmployeeInitial);
                }

            }, "json");
        }
    } catch (err) {
        console.log(err.message);
    }
    return false;
}

