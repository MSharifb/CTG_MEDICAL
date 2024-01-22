function fnValidate() {
    var error = "";


    var notValid = 0;


    //required field validator
    $("input.required,textarea.required").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, this.value.length > 0);
    });

    //integer field validator
    $("input.integer").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsInteger(this.value));
    });

    //integer field validator Not Required
    $("input.integerNR").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 1) this.value = 0;
        notValid += MarkError(this, IsInteger(this.value));
    });

    //integer field validator Non Zero
    $("input.integerNZ").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value == 0) notValid += MarkError(this, false);
        else notValid += MarkError(this, IsInteger(this.value));
    });

    //integer field validator
    $("input.word").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsWord(this.value));
    });


    //decimal field validator
    $("input.double").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsDouble(this.value));
    });

    //decimal field validator Not Required
    $("input.doubleNR").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 1) this.value = 0;
        notValid += MarkError(this, IsDouble(this.value));
    });

    //decimal field validator Non Zero
    $("input.doubleNZ").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value == 0) notValid += MarkError(this, false);
        else notValid += MarkError(this, IsDouble(this.value));
    });

    //Date field validator dd-MM-yyyy
    $("input.date").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsDate(this.value, false));
    });

    //Date field validator dd-MM-yyyy
    $("input.cndate").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, !IsPrevDate(this.value, false));
    });

    //Date field validator dd-MM-yyyy
    $("input.dateNR").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsDate(this.value, true));
    });

    //Time field validator hh:mm tt
    $("input.time").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsTime(this.value));
    });

    //Time field validator hh:mm tt (not mandatory)
    $("input.timeNR").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 1) return true;
        notValid += MarkError(this, IsTime(this.value));
    });

    // drop down List Validation
    $("select.required").each(function(i) {
        notValid += MarkErrorForDDL(this, this.selectedIndex > 0);
    });

    //email field validator
    $("input.email").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 1) return true;
        var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
        notValid += MarkError(this, emailPattern.test(this.value));
    });
    
    // password
    $("input.password").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 8)
            notValid += MarkError(this, false);

    });


    if (notValid > 0) {
        $("span[id$=lblMsg]").html("<b>Please provide valid information for the required field(s)</b>" + error).css("color", "red");
        return false;
    }
    else {
        $("span[id$=lblMsg]").html("").css("color", "Green");
        return true;
    }


}

//grid view check
function fnValidateRow(bItem) {
    var error = "";

    var Row = bItem.parentNode.parentNode;

    var notValid = 0;


    //required field validator
    $(Row).find("input[class='required'],textarea[class='required']").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, this.value.length > 0);
    });

    //integer required field validator
    $(Row).find("input.integer").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsInteger(this.value));
    });

    //integer field validator Not Required
    $(Row).find("input.integerNR").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 1) this.value = 0;
        notValid += MarkError(this, IsInteger(this.value));
    });

    //integer field validator Non Zero
    $(Row).find("input.integerNZ").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value == 0) notValid += MarkError(this, false);
        else notValid += MarkError(this, IsInteger(this.value));
    });

    //decimal field validator
    $(Row).find("input.double").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsDouble(this.value));
    });

    //decimal field validator Not Required
    $(Row).find("input.doubleNR").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value.length < 1) this.value = 0;
        notValid += MarkError(this, IsDouble(this.value));
    });

    //decimal field validator Non Zero
    $(Row).find("input.doubleNZ").each(function(i) {
        this.value = $.trim(this.value);
        if (this.value == 0) notValid += MarkError(this, false);
        else notValid += MarkError(this, IsDouble(this.value));
    });

    //Date field validator dd-MM-yyyy
    $(Row).find("input.date").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsDate(this.value, false));
    });




    //Date field validator dd-MM-yyyy
    $(Row).find("input.dateNR").each(function(i) {
        this.value = $.trim(this.value);
        notValid += MarkError(this, IsDate(this.value, true));
    });


    // drop down List Validation
    $(Row).find("select.required").each(function(i) {
        notValid += MarkErrorForDDL(this, this.selectedIndex > 0);
    });

    
    if (notValid > 0) {
        //        $("span[id$=lblMsg]").html("<b>Please provide valid information for the required field(s)</b><br>" + error).css("color", "red");
        $("#message").html("<b>Please provide valid information for the required field(s)</b><br>" + error).css("color", "red");
        return false;
    }
    else {
        $("span[id$=lblMsg]").html("").css("color", "black");
        return true;
    }


}

//check valid integer 1,15,18
function IsInteger(val) {
    var re = new RegExp("^[0-9]+$");
    return val.match(re);
}


//check valid decimal number  125,125.50
function IsDouble(val) {
    if (val.toString().indexOf('-') != -1) return false;
    return !isNaN(val) && (val.length > 0);
}

function IsWord(val) {
    var re = new RegExp("^[A-Za-z0-9_]+$");
    return val.match(re);
}


//check valid date 25/12/1989
function IsDate(val, allowBalnk) {
    if (allowBalnk && val.length == 0) return true;
    var dateaprts = val.split('-');
    if (dateaprts[0].length != 2 || dateaprts[1].length != 2 || dateaprts[2].length != 4) return false;
    var dt = new Date(dateaprts[2], dateaprts[1] - 1, dateaprts[0]);
    return (dt.getDate() == dateaprts[0] && dt.getMonth() == dateaprts[1] - 1 && dt.getFullYear() == dateaprts[2])
}

//check valid date 25/12/1989
function IsPrevDate(val, allowBalnk) {
    if (allowBalnk && val.length == 0) return true;

    var today = new Date();

    var dateaprts = val.split('-');
    var dt = new Date(dateaprts[2], dateaprts[1] - 1, dateaprts[0]);

    var difference = today - dt;

    days = Math.floor(difference / (1000 * 60 * 60 * 24));

    return days > 0;
}

//check valid time 12:15 PM
function IsTime(val) {
    var err = 0
    if (val.length != 8) err = 1
    hh = val.substring(0, 2)// Hour f
    c = val.substring(2, 3)// ':'
    mm = val.substring(3, 5)// Min b
    e = val.substring(5, 6)// ' '
    tt = val.substring(6, 8)// AM/PM

    if (hh < 0 || hh > 12) err = 1
    if (mm < 0 || mm > 59) err = 1
    if (tt != 'AM' && tt != 'PM') err = 1
    if (err == 1) { return false; }
    else { return true };
}

function MarkError(control, isValid) {
    //$(control.offsetParent).find("BR,p").remove();//#C5D3E4
    if (isValid) {
        $(control).css("border", "solid 1px #006600");


        return 0;
    }
    else {
        //    $(control.offsetParent).append("<br><p style='color:red'>Required</p>");
        //error +=$(this).attr('id').split('txt')[1]+'<br>';
        //        $(control).css("border", "solid 1px red");
        $(control).css({ 'border-color': '#FF0000'}); 
        return 1;
    }

}



function MarkErrorForDDL(control, isValid) {
    if (isValid) {
        $(control).css("color", "black");
        return 0;
    }
    else {
        $(control).css({ 'border-color': '#FF0000', 'background-color': '#FFF2F2' });        
        return 1;
    }

}


function hideMsg() {
    document.getElementById("<%=lblMsg.ClientID%>").innerHTML = "";
}
//-- allow only number --//
function allowOnlyNumbers(evt) {

    var keyCode = "";
    if (window.event) {
        keyCode = window.event.keyCode;
        evt = window.event;
    }
    else if (evt) keyCode = evt.which;
    else return true;
    if ((keyCode == null) || (keyCode == 0) || (keyCode == 8) || (keyCode == 9) || (keyCode == 13) || (keyCode == 27))
        return true;
    if ((keyCode < 48 || keyCode > 57)) {
        evt.returnValue = false;
        return false;
    }
}
//-- check decimal value --//
function allowDecimalValue(evt, fieldValue) {

    var keyCode = "";
    if (window.event) {
        keyCode = window.event.keyCode;
        evt = window.event;
    }
    else if (evt) keyCode = evt.which;
    else return true;

    fieldArray = fieldValue.split(""); //take every character of field value in array

    //if((keyCode==null) || (keyCode==0) || (keyCode==8) || (keyCode==9) || (keyCode==13) || (keyCode==27) )

    if ((keyCode > 47 && keyCode < 58) || (keyCode == 45) || (keyCode == 46)) {
        if ((keyCode == 46) || (keyCode == 45)) {
            for (var x = 0; x <= fieldValue.length; x++) {
                if ((keyCode == 46) && (fieldArray[x] == ".")) {
                    evt.returnValue = false;
                    return false;
                }
                else if ((keyCode == 45) && (fieldArray[x] == "-")) {
                    evt.returnValue = false;
                    return false;
                }
            }
        }
        return true;
    }
    else {
        evt.returnValue = false;
        return false;
    }
}

//-- checks the length of a text field --//
function checkFieldLength(evt, fieldValue, maxLen) {
    var keyCode = "";
    if (window.event) {
        keyCode = window.event.keyCode;
        evt = window.event;
    }

    if (fieldValue.length >= maxLen) {
        evt.returnValue = false;
        return false;
    }
    else return true;
}

//-- checks valid key for a date field --//
function allowValidDate(evt, fieldValue) {
    var keyCode = "";
    if (window.event) {
        keyCode = window.event.keyCode;
        evt = window.event;
    }
    else if (evt) keyCode = evt.which;
    else return true;
    dateArray = fieldValue.split(""); //take every character of field value in array

    if (fieldValue.length == 0) {
        if ((keyCode < 48 || keyCode > 51))// (0-3)
        {
            evt.returnValue = false;
            return false;
        }
    }
    if (fieldValue.length == 1) {
        // If first digit of date is '3' then second digit should be (0-1)  
        if (dateArray[0] == "3") {
            if (keyCode < 48 || keyCode > 49)// (0-1)
            {
                evt.returnValue = false;
                return false;
            }
        }
        // If first digit of date is '0' then second digit should not be '0'
        if (dateArray[0] == "0") {
            if (keyCode == 48)// (0)
            {
                evt.returnValue = false;
                return false;
            }
        }
        else if ((keyCode < 48 || keyCode > 57))//(0-9)
        {
            evt.returnValue = false;
            return false;
        }
    }
    if (fieldValue.length == 2) {
        if ((keyCode != 45))// (-)
        {
            evt.returnValue = false;
            return false;
        }
    }
    if (fieldValue.length == 3) {
        if ((keyCode < 48 || keyCode > 49))//(0-1)
        {
            evt.returnValue = false;
            return false;
        }
    }
    if (fieldValue.length == 4) {
        // If first digit of month is '1' then second digit should be (0-2) 
        if (dateArray[3] == "1") {
            if (keyCode < 48 || keyCode > 50)// (0-1)
            {
                evt.returnValue = false;
                return false;
            }
        }
        // If first digit of month is '0' then second digit should not be '0' 
        if (dateArray[3] == "0") {
            if (keyCode == 48)// (0-1)
            {
                evt.returnValue = false;
                return false;
            }
        }
        if ((keyCode < 48 || keyCode > 57))//(0-9)
        {
            evt.returnValue = false;
            return false;
        }
    }
    if (fieldValue.length == 5) {
        if ((keyCode != 45))// (-)
        {
            evt.returnValue = false;
            return false;
        }
    }
    if ((fieldValue.length > 5) && (fieldValue.length < 10)) {
        if ((keyCode < 48 || keyCode > 57))//(0-9)
        {
            evt.returnValue = false;
            return false;
        }
    }
    // check if year is not less then 0001
    if (fieldValue.length == 9) {
        if (dateArray[6] == "0" && dateArray[7] == "0" && dateArray[8] == "0") {
            if (keyCode == 48) {
                evt.returnValue = false;
                return false;
            }
        }
    }
    // check if field length is greater than 9
    if (fieldValue.length > 9) {
        evt.returnValue = false;
        return false;
    }

    else return true;
}
function CheckAnyValidDate(date)// dd-mm-yyyy 
{
    var dateValid = true;

    try {
        var dateSeperator = "";
        //See what the character is that seperates the date parts...
        if (date.indexOf("-") > 0) {
            dateSeperator = "-";
        }

        else {
            //if it's not one of the ones listed above, then we don't have a valid date...
            dateValid = false;
        }

        var dateArray = new Array(5);
        dateArray = date.split(dateSeperator);

        if (dateArray[0].length > 2) {
            dateValid = false;
        }
        if (dateArray[1].length > 2) {
            dateValid = false;
        }
        if (dateArray[2].length != 4) {
            dateValid = false;
        }

        //If any of the parts aren't numbers, then we don't have a date
        if (isNaN(dateArray[0]) || isNaN(dateArray[1]) || isNaN(dateArray[2])) {
            dateValid = false;
        }

        var iDate = parseInt(dateArray[0], 10);
        var iMonth = parseInt(dateArray[1], 10);
        var iYear = parseInt(dateArray[2], 10);

        //If the year is before 1900 it's not valid...
        if (iYear < 1900) {
            dateValid = false;
        }
        //Make sure our month is in range...
        else if (iMonth < 0 || iMonth > 12) {
            dateValid = false;
        }
        //Jan, Mar, May, July, Aug, Oct and Dec must have between 1 and 31 days...
        else if ((iMonth == 1 || iMonth == 3 || iMonth == 5 || iMonth == 7 || iMonth == 8 || iMonth == 10 || iMonth == 12)
				   && (iDate < 0 || iDate > 31)) {
            dateValid = false;
        }
        //April, June, Sept, and Nov must have between 1 and 30 days...
        else if ((iMonth == 2 || iMonth == 6 || iMonth == 9 || iMonth == 11)
				   && (iDate < 0 || iDate > 30)) {
            dateValid = false;
        }
        //Feb must have 28 days unless it's a leap year. If the year is evenly divisable by 4, then we're in a leap year
        //NOTE: that this will fail for the year 2100, since 2100 is not a leap century
        //			(even though we really don't have to worry about this yet)
        else if ((iMonth == 2) && (iYear % 4 == 0) && (iDate < 0 || iDate > 29)) {
            dateValid = false;
        }
        //Now we handle non-leap year Feb's...
        else if ((iMonth == 2) && (iYear % 4 != 0) && (iDate < 0 || iDate > 28)) {
            dateValid = false;
        }

    }
    catch (e) {
        dateValid = false;
    }
    return dateValid;
}

function CheckValidCurrentDate(date, currentyear, currentmonth, currentday)// check valid current date: dd-mm-yyyy
{

    var dateValid = true;

    try {
        var dateSeperator = "";
        //See what the character is that seperates the date parts...
        if (date.indexOf("-") > 0) {
            dateSeperator = "-";
        }
        else {
            //if it's not one of the ones listed above, then we don't have a valid date...
            dateValid = false;
        }

        var dateArray = new Array(5);
        dateArray = date.split(dateSeperator);

        if (dateArray[0].length > 2) {
            dateValid = false;
        }
        if (dateArray[1].length > 2) {
            dateValid = false;
        }
        if (dateArray[2].length != 4) {
            dateValid = false;
        }

        //If any of the parts aren't numbers, then we don't have a date
        if (isNaN(dateArray[0]) || isNaN(dateArray[1]) || isNaN(dateArray[2])) {
            dateValid = false;
        }

        var iDate = parseInt(dateArray[0], 10);
        var iMonth = parseInt(dateArray[1], 10);
        var iYear = parseInt(dateArray[2], 10);

        //If the year is before 1900 or later than the current date, it's not valid...
        if (iYear < 1900 || iYear > currentyear) {
            dateValid = false;
        }
        //Make sure our month is in range...
        else if (iMonth < 0 || iMonth > 12) {
            dateValid = false;
        }
        //Jan, Mar, May, July, Aug, Oct and Dec must have between 1 and 31 days...
        else if ((iMonth == 1 || iMonth == 3 || iMonth == 5 || iMonth == 7 || iMonth == 8 || iMonth == 10 || iMonth == 12)
				   && (iDate < 0 || iDate > 31)) {
            dateValid = false;
        }
        //April, June, Sept, and Nov must have between 1 and 30 days...
        else if ((iMonth == 2 || iMonth == 6 || iMonth == 9 || iMonth == 11)
				   && (iDate < 0 || iDate > 30)) {
            dateValid = false;
        }
        //Feb must have 28 days unless it's a leap year. If the year is evenly divisable by 4, then we're in a leap year
        //NOTE: that this will fail for the year 2100, since 2100 is not a leap century
        //			(even though we really don't have to worry about this yet)
        else if ((iMonth == 2) && (iYear % 4 == 0) && (iDate < 0 || iDate > 29)) {
            dateValid = false;
        }
        //Now we handle non-leap year Feb's...
        else if ((iMonth == 2) && (iYear % 4 != 0) && (iDate < 0 || iDate > 28)) {
            dateValid = false;
        }
        else {
            //Now see if we can create a Date object with our date parts. If so, then we have a valid date...
            //			var validDate:Date = new Date(iYear, iMonth, iDate);
            //check for current date
            if ((iYear == currentyear && iMonth > currentmonth) || (iYear == currentyear && iMonth == currentmonth && iDate > currentday)) {
                dateValid = false;
            }
        }
    }
    catch (e) {
        dateValid = false;
    }
    return dateValid;
}

function FuturedateValidatecheck(date, currentyear, currentmonth, currentday) ////checking for future date only
{

    var dateValid = true;

    try {
        var dateSeperator = "-";

        var dateArray = new Array(5);
        dateArray = date.split(dateSeperator);

        var iDate = parseInt(dateArray[0], 10);
        var iMonth = parseInt(dateArray[1], 10);
        var iYear = parseInt(dateArray[2], 10);

        //			//checking for future date.
        if (iYear > currentyear || (iYear == currentyear && iMonth > currentmonth) || (iYear == currentyear && iMonth == currentmonth && iDate > currentday)) {
            dateValid = false;
        }
    }

    catch (e) {
        dateValid = false;
    }
    return dateValid;
}



