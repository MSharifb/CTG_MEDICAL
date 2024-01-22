/**
 * Project-specific JavaScripts
 *
 * @project POMS_MPA
 * @since  1.0.0
 */

$(document).ready(function ($) {

    /**
* Left Navigation Control
* ---------------------------
*/
    // make menu active on page text (on page load)
    $('.submenu > li > a').each(function (index, element) {
        // if page text == menu text
        var this_item = $(element);
        if (this_item.text().trim() == $('.page-title').text().trim()) {

            var parent_lis = this_item.parents('li'),
                parent_uls = this_item.parents('.submenu');

            parent_lis.addClass('active');
            parent_uls.addClass('active');
        }
    });

    var target_menu = $('#main-menu ul.menu'),
        submenu = $('.submenu');

    // hide sub menus that are not active
    submenu.not('.active').hide();

    // let's listen mouse events
    target_menu.find('li.has-submenu > a').live('click', function (e) {
        e.preventDefault();

        var this_menu_item = $(this),
            this_submenu = this_menu_item.siblings('.submenu'),
            this_item_li = this_menu_item.parent('li.has-submenu');

        if (this_submenu.is(':hidden')) {
            this_submenu.slideDown().addClass('active');
            this_item_li.addClass('active');
        } else {
            this_submenu.slideUp().removeClass('active');
            this_item_li.removeClass('active');
        }
    });

  
    $('#user-menu').live('click', function () {
        var user_dropdown = $('.user-dropdown-menu');
        if(user_dropdown.is(':visible')) {
            user_dropdown.hide();
        } else {
            user_dropdown.show();
        }
    });


    $('#zone-menu').live('click', function () {
        var user_dropdown = $('.zone-dropdown-menu');
        if (user_dropdown.is(':visible')) {
            user_dropdown.hide();
        } else {
            user_dropdown.show();
        }
    });

});


function toggle_fullscreen() {
    var fullscreenEnabled = document.fullscreenEnabled || document.mozFullScreenEnabled || document.webkitFullscreenEnabled;
    if (fullscreenEnabled) {
        if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement) {
            launchIntoFullscreen(document.documentElement);
        } else {
            exitFullscreen();
        }
    }
}

// Thanks to http://davidwalsh.name/fullscreen
function launchIntoFullscreen(element) {
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.webkitRequestFullscreen) {
        element.webkitRequestFullscreen();
    } else if (element.msRequestFullscreen) {
        element.msRequestFullscreen();
    }
}

function exitFullscreen() {
    if (document.exitFullscreen) {
        document.exitFullscreen();
    } else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
    } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
    }
} 
