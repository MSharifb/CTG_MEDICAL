$(function () {
    $(".addItem").click(function () {
        var table = $(this).parent().parent().find('table:first');
        $.ajax({
            url: this.href,
            cache: false,
            success: function (html) { 
                $(table).append(html);
            }
        });
        return false;
    });

    $("a.deleteRow").live("click", function () {        
        $(this).parents("tr:first").remove();
        return false;
    });
});
 

/*------------------------------Validation Extension from Xhalent--------------------------------*/
//url:http://xhalent.wordpress.com/2011/01/24/applying-unobtrusive-validation-to-dynamic-content/


(function ($) {
    $.validator.unobtrusive.parseDynamicContent = function (selector) {
        //use the normal unobstrusive.parse method

        //$.validator.unobtrusive.parse(selector);
        $(selector).find('*[data-val = true]').each(function(){
        
            $.validator.unobtrusive.parseElement(this,false);
        });

        //get the relevant form
        var form = $(selector).first().closest('form');
        
        //get the collections of unobstrusive validators, and jquery validators
        //and compare the two
        var unobtrusiveValidation = form.data('unobtrusiveValidation');
        var validator = form.validate();
        $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
            if (validator.settings.rules[elname] == undefined) {
                var args = {};
                $.extend(args, elrules);
                args.messages = unobtrusiveValidation.options.messages[elname];
                //alert('here1');
                
                $('[name="' + elname + '"]').rules("add", args);
            } else {
                $.each(elrules, function (rulename, data) {
                    rulename = rulename;
                    data = data;
                    if (validator.settings.rules[elname][rulename] == undefined) {
                        var args = {};
                        args[rulename] = data;
                        args.messages = unobtrusiveValidation.options.messages[elname][rulename];
                        alert('here');
                        
                        $('[name="' + elname + '"]').rules("add", args);
                    }
                });
            }
        });
    }
})($);
/*--------------------------------------------------------------------------------------------------*/
