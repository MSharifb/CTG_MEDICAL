

/*.............................Start Grouping CheckBox Coloumn.......................................*/

function MergeCheckboxCell(grid, groupCol) {
    var rids = $('#' + grid).jqGrid('getDataIDs');
    var celldataParent = $('#' + grid).jqGrid('getCell', rids[0], groupCol);
    var count = 0;
    var range = 0;
    for (var i = 0; i <= rids.length; i++) {
        var celldataCurrent = $('#' + grid).jqGrid('getCell', rids[i], groupCol);
        if (celldataCurrent == celldataParent) {
            count++;
        }
        else {
            for (var k = 0; k < count; k++) {
                if (k == 0) {
                    $('#' + grid).jqGrid('setCell', rids[range + k], 'cb', '', 'visible-cell');
                    SetRowSpan(grid, count);
                }
                else {
                    $('#' + grid).setCell(rids[range + k], 'cb', '', 'hide-cell');
                }
            }
            celldataParent = celldataCurrent;
            range = range + count;
            count = 1;
        }
    }
    RemoveCol(grid);
}

function SetRowSpan(grid, n) {
    $('#' + grid + ' tr').each(function () {
        if (n > 1) {
            $(this).find(".visible-cell").attr("rowspan", n);
        }
    });
    $('#' + grid + ' tr').each(function () {
        $(this).find(".visible-cell").removeClass("visible-cell");
    });
}

function RemoveCol(grid) {
    $('#' + grid + ' tr').each(function () {
        $(this).find(".hide-cell").hide();

    });
    $('#' + grid).each(function () {
        $(this).find(".jqgroup ui-row-ltr").hide();
    });
}

/*.............................End Grouping CheckBox Coloumn.......................................*/

/*.............................Start Grouping Coloumn.......................................*/

function MergeGridCellGroupWise(grid, columnNameList) {
    for (var y = 0; y <= columnNameList.length - 1; y++) {
        $.extend({
            threadedEach: MergeCell(grid, y, columnNameList)
        });
    }
    RemoveClass(grid);
}

function MergeCell(grid, rowIndex, columnNameList) {
    var i = 0;
    var row = 0;
    var celldataParent = '';
    var count = 0;
    var span = 1;
    var groupcount = 0;
    var cellObjectParent = null;
    var cellClassParent = '';
    var celldataCurrent = '';
    var cellClassCurrent = '';

    var obj = null;
    $('#' + grid + ' tbody tr td').each(function () {
        if (i > 0) {
            obj = $(this);

            if (obj.attr('aria-describedby') == grid + '_' + columnNameList[rowIndex]) {

                if (count == 0) {
                    celldataParent = obj.html();
                    cellClassParent = obj.context.className;
                    //obj.attr('Class', 'Visible');
                    //obj.html(obj.html() + '-Visible1');
                    groupcount++;
                    SetClass(grid, row, rowIndex, groupcount, columnNameList);
                    cellObjectParent = obj;
                }
                else {
                    celldataCurrent = obj.html();
                    cellClassCurrent = obj.context.className;

                }
                if (rowIndex > 0) {
                    if (cellClassCurrent == cellClassParent) {
                        if (celldataCurrent == celldataParent) {
                            span++;
                            obj.hide();
                            //obj.attr('Class', 'Hide');
                            //obj.html(obj.html() + '-Hide');
                            if (span > 1) {
                                cellObjectParent.attr('rowspan', span);
                            }
                            SetClass(grid, row, rowIndex, groupcount, columnNameList);
                        }
                        else if (celldataCurrent != celldataParent) {
                            if (count != 0) {
                                count = 0;
                                groupcount++;
                                span = 1;
                                celldataParent = obj.html();
                                cellClassParent = obj.context.className;
                                //obj.attr('Class', 'Visible');
                                //obj.html(obj.html() + '-Visible2');
                                cellObjectParent = obj;
                                SetClass(grid, row, rowIndex, groupcount, columnNameList);
                            }
                        }
                    }
                    else if (cellClassCurrent != cellClassParent) {
                        if (count != 0) {
                            count = 0;
                            groupcount++;
                            span = 1;
                            celldataParent = obj.html();
                            cellClassParent = obj.context.className;
                            //obj.attr('Class', 'Visible');
                            //obj.html(obj.html() + '-Visible3');
                            cellObjectParent = obj;
                            SetClass(grid, row, rowIndex, groupcount, columnNameList);
                        }
                    }
                    row++;
                    count++;

                }
                if (rowIndex == 0) {
                    if (celldataCurrent == celldataParent) {
                        span++;
                        obj.hide();
                        //obj.attr('Class', 'Hide');
                        //obj.html(obj.html() + '-Hide');
                        if (span > 1) {
                            cellObjectParent.attr('rowspan', span);
                        }
                        SetClass(grid, row, rowIndex, groupcount, columnNameList);
                    }
                    else {
                        if (count != 0) {
                            count = 0;
                            groupcount++;
                            span = 1;
                            celldataParent = obj.html();
                            //obj.attr('Class', 'Visible');
                            //obj.html(obj.html() + '-Visible4');
                            cellObjectParent = obj;
                            SetClass(grid, row, rowIndex, groupcount, columnNameList);
                        }
                    }

                    row++;
                    count++;
                }
            }
        }
        i++;
    });
}

function SetClass(grid, row, rowIndex, groupcount, columnNameList) {
    row = row + 1;
    // for (var y = rowIndex + 1; y <= columnNameList.length - 1; y++) {
    $('#' + grid).jqGrid('setCell', row, columnNameList[rowIndex + 1], '', columnNameList[rowIndex + 1] + '_' + groupcount);
    //}
}

function RemoveClass(grid) {
    var obj = null;
    $('#' + grid + ' tbody tr td').each(function () {
        obj = $(this);
        obj.removeAttr('Class');
    });
}

function MergeFirstGroupCell(grid, groupCol) {
    var rids = $('#' + grid).jqGrid('getDataIDs');
    var celldataParent = $('#' + grid).jqGrid('getCell', rids[0], groupCol);
    var count = 0;
    var range = 0;
    for (var i = 0; i <= rids.length; i++) {
        var celldataCurrent = $('#' + grid).jqGrid('getCell', rids[i], groupCol);
        if (celldataCurrent == celldataParent) {
            count++;
        }
        else {
            for (var k = 0; k < count; k++) {
                if (k == 0) {

                    $('#' + grid).jqGrid('setCell', rids[range + k], groupCol, '', 'visible-cell');
                    SetRowSpan(grid, count);

                }
                else {
                    $('#' + grid).jqGrid('setCell', rids[range + k], groupCol, '', 'hide-cell');
                }
            }
            celldataParent = celldataCurrent;
            range = range + count;
            count = 1;
        }
    }
    RemoveCol(grid);
}

/*.............................End Grouping Coloumn.......................................*/

/*.............................Start Grouping Coloumn By ProjectNo.......................................*/

function MergeGridCellByProjectNo(grid, groupCol) {
    var rids = $('#' + grid).jqGrid('getDataIDs');
    var rowDataParent = $('#' + grid).jqGrid('getRowData', rids[0]);
    var parentProjectNo = rowDataParent.ProjectNo;
    var count = 0;
    var range = 0;

    for (var i = 0; i <= rids.length; i++) {

        var rowId = rids[i];
        var rowDataCurrent = $('#' + grid).jqGrid('getRowData', rowId);

        var currentProjectNo = rowDataCurrent.ProjectNo;

        if (currentProjectNo == parentProjectNo) {
            count++;
        }
        else {
            for (var k = 0; k < count; k++) {
                if (k == 0) {
                    $('#' + grid).jqGrid('setCell', rids[range + k], groupCol, '', 'visible-cell');
                    SetRowSpan(grid, count);

                }
                else {
                    $('#' + grid).jqGrid('setCell', rids[range + k], groupCol, '', 'hide-cell');
                }
            }
            parentProjectNo = currentProjectNo;
            range = range + count;
            count = 1;
        }
    }
    RemoveCol(grid);
}

/*.............................End Grouping Coloumn.......................................*/

/*.............................Start CalculateFooterTotalAndFlexi.......................................*/

function CalculateFooterTotalAndFlexi(grid, e, rowId, cellName, cellValue, flexiValue) {

    var sum = $('#' + grid).jqGrid("getCol", cellName, false, "sum");

    var totalSum = parseFloat(sum);
    var flexiSum = parseFloat(sum) - parseFloat(flexiValue);

    var maxTotal = parseFloat(24);

    if (totalSum > maxTotal || flexiSum > maxTotal) {

        var dialogContent = 'Total = ' + totalSum + ', Flexi = ' + flexiSum + ' : value must be less than or equal to 24';
        var dialogDiv = $("<div>" + dialogContent + "</div>");
        dialogDiv.dialog({
            title: "Warning",
            resizable: false,
            width: 'auto',
            modal: true,
            buttons: [
                        {
                            text: "Close",
                            click: function () {
                                dialogDiv.dialog("close");
                            }
                        }
                    ]
        });

        $('#' + grid).jqGrid('setCell', rowId, cellName, 0);

    }
    else {

        var flexiTotalFooterRow = $(e.grid.sDiv).find("tr.flexiTotalFooterRow");
        if (flexiTotalFooterRow.length === 0) {
            // add second row of the footer if it's not exist
            flexiTotalFooterRow = footerRow.clone();
            flexiTotalFooterRow.removeClass("footrow").addClass("flexiTotalFooterRow ui-widget-content");
            flexiTotalFooterRow.children("td").each(function () {
                e.style.width = ""; // remove width from inline CSS
            });
            flexiTotalFooterRow.insertAfter(footerRow);
        }

        flexiTotalFooterRow.find(">td[aria-describedby=" + e.id + "_" + cellName + "]").text(
                        $.fmatter.util.NumberFormat(flexiSum, $.jgrid.formatter.number)
                    );

        var totalFooterRow = $(e.grid.sDiv).find("tr.totalFooterRow");
        if (totalFooterRow.length === 0) {
            // add second row of the footer if it's not exist
            totalFooterRow = footerRow.clone();
            totalFooterRow.removeClass("footrow").addClass("totalFooterRow ui-widget-content");
            totalFooterRow.children("td").each(function () {
                e.style.width = ""; // remove width from inline CSS
            });
            totalFooterRow.insertAfter(footerRow);
        }

        totalFooterRow.find(">td[aria-describedby=" + e.id + "_" + cellName + "]").text(
                        $.fmatter.util.NumberFormat(totalSum, $.jgrid.formatter.number)
                    );

        if (cellValue > 0) {
            //Set Row Data
            $('#' + grid).jqGrid('setCell', rowId, 'IsEdited', 'True');
        }
        //        else if (cellValue == 0) {
        //            //Set Row Data
        //            $('#' + grid).jqGrid('setCell', rowId, 'IsEdited', 'False');
        //        }


    } //End If Else

}

function CalculateFooterTotalAndFlexiWithOffDay(grid, e, rowId, cellName, cellValue, flexiValue, offDayColumnNameList) {

    var sum = $('#' + grid).jqGrid("getCol", cellName, false, "sum");

    var totalSum = parseFloat(sum);
    //var flexiSum = parseFloat(0);
    var flexiSum = parseFloat(sum) - parseFloat(flexiValue);

    //    var innerFlexiSum = parseFloat(sum) - parseFloat(flexiValue);

    //    if (innerFlexiSum > 0) {
    //        flexiSum = parseFloat(innerFlexiSum);
    //    }

    for (var i = 0; i <= offDayColumnNameList.length - 1; i++) {

        var columnName = offDayColumnNameList[i];

        if (columnName == cellName) {

            var innerIfForFlexiSum = parseFloat(totalSum);

            //            if (innerIfForFlexiSum > 0) {
            //                flexiSum = parseFloat(innerIfForFlexiSum);
            //            }

            flexiSum = parseFloat(innerIfForFlexiSum);

            break;

        }
    }

    var maxTotal = parseFloat(24);

    if (totalSum > maxTotal || flexiSum > maxTotal) {

        var dialogContent = 'Total = ' + totalSum + ', Flexi = ' + flexiSum + ' : value must be less than or equal to 24';
        var dialogDiv = $("<div>" + dialogContent + "</div>");
        dialogDiv.dialog({
            title: "Warning",
            resizable: false,
            width: 'auto',
            modal: true,
            buttons: [
                        {
                            text: "Close",
                            click: function () {
                                dialogDiv.dialog("close");
                            }
                        }
                    ]
        });

        $('#' + grid).jqGrid('setCell', rowId, cellName, 0);

    }
    else {

        var flexiTotalFooterRow = $(e.grid.sDiv).find("tr.flexiTotalFooterRow");
        if (flexiTotalFooterRow.length === 0) {
            // add second row of the footer if it's not exist
            flexiTotalFooterRow = footerRow.clone();
            flexiTotalFooterRow.removeClass("footrow").addClass("flexiTotalFooterRow ui-widget-content");
            flexiTotalFooterRow.children("td").each(function () {
                e.style.width = ""; // remove width from inline CSS
            });
            flexiTotalFooterRow.insertAfter(footerRow);
        }

        flexiTotalFooterRow.find(">td[aria-describedby=" + e.id + "_" + cellName + "]").text(
                        $.fmatter.util.NumberFormat(flexiSum, $.jgrid.formatter.number)
                    );

        var totalFooterRow = $(e.grid.sDiv).find("tr.totalFooterRow");
        if (totalFooterRow.length === 0) {
            // add second row of the footer if it's not exist
            totalFooterRow = footerRow.clone();
            totalFooterRow.removeClass("footrow").addClass("totalFooterRow ui-widget-content");
            totalFooterRow.children("td").each(function () {
                e.style.width = ""; // remove width from inline CSS
            });
            totalFooterRow.insertAfter(footerRow);
        }

        totalFooterRow.find(">td[aria-describedby=" + e.id + "_" + cellName + "]").text(
                        $.fmatter.util.NumberFormat(totalSum, $.jgrid.formatter.number)
                    );

        if (cellValue > 0) {
            //Set Row Data
            $('#' + grid).jqGrid('setCell', rowId, 'IsEdited', 'True');
        }
        //        else if (cellValue == 0) {
        //            //Set Row Data
        //            $('#' + grid).jqGrid('setCell', rowId, 'IsEdited', 'False');
        //        }


    } //End If Else

}


/*.............................End CalculateFooterTotalAndFlexi.......................................*/

/*.............................Start SetFooterTotalAndFlexi.......................................*/

function SetFooterTotalAndFlexiColumnWise(grid, e, columnNameList, footerTextColumnName) {

    for (var i = 0; i <= columnNameList.length - 1; i++) {

        var columnName = columnNameList[i];

        $.extend({
            threadedEach: SetFooterTotalAndFlexi(grid, e, columnName, footerTextColumnName)
        });
    }

}

function SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit(grid, e, columnNameList, footerTextColumnName, flexiValue, offDayColumnNameList) {

    for (var i = 0; i <= columnNameList.length - 1; i++) {

        var columnName = columnNameList[i];

        $.extend({
            threadedEach: SetFooterTotalAndFlexiForSumbitAndEdit(grid, e, columnName, footerTextColumnName, flexiValue, offDayColumnNameList)
        });
    }

}

function SetFooterTotalAndFlexi(grid, e, columnName, footerTextColumnName) {

    var sum = $('#' + grid).jqGrid("getCol", columnName, false, "sum");
    var footerRow = $(e.grid.sDiv).find("tr.footrow");

    var totalSum = parseFloat(sum);

    var flexiTotalFooterRow = $(e.grid.sDiv).find("tr.flexiTotalFooterRow");
    if (flexiTotalFooterRow.length === 0) {
        // add second row of the footer if it's not exist
        flexiTotalFooterRow = footerRow.clone();
        flexiTotalFooterRow.removeClass("footrow").addClass("flexiTotalFooterRow ui-widget-content");
        flexiTotalFooterRow.children("td").each(function () {
            e.style.width = ""; // remove width from inline CSS
        });
        flexiTotalFooterRow.insertAfter(footerRow);
    }

    flexiTotalFooterRow.find(">td[aria-describedby=" + e.id + "_" + footerTextColumnName + "]").text("Flexi:");
    flexiTotalFooterRow.find(">td[aria-describedby=" + e.id + "_" + columnName + "]").text(
                        $.fmatter.util.NumberFormat(totalSum, $.jgrid.formatter.number)
                    );

    var totalFooterRow = $(e.grid.sDiv).find("tr.totalFooterRow");
    if (totalFooterRow.length === 0) {
        // add second row of the footer if it's not exist
        totalFooterRow = footerRow.clone();
        totalFooterRow.removeClass("footrow").addClass("totalFooterRow ui-widget-content");
        totalFooterRow.children("td").each(function () {
            e.style.width = ""; // remove width from inline CSS
        });
        totalFooterRow.insertAfter(footerRow);
    }

    totalFooterRow.find(">td[aria-describedby=" + e.id + "_" + footerTextColumnName + "]").text("Total:");
    totalFooterRow.find(">td[aria-describedby=" + e.id + "_" + columnName + "]").text(
                                $.fmatter.util.NumberFormat(totalSum, $.jgrid.formatter.number)
                            );
}

function SetFooterTotalAndFlexiForSumbitAndEdit(grid, e, columnName, footerTextColumnName, flexiValue, offDayColumnNameList) {

    var sum = $('#' + grid).jqGrid("getCol", columnName, false, "sum");
    var footerRow = $(e.grid.sDiv).find("tr.footrow");

    var totalSum = parseFloat(sum);
    var flexiSum = parseFloat(0);
    //var flexiSum = parseFloat(sum) - parseFloat(flexiValue);


    if (totalSum > 0) {

        var innerIfFlexiSum = totalSum - parseFloat(flexiValue);

        //        if (innerIfFlexiSum > 0) {
        //            flexiSum = parseFloat(innerIfFlexiSum);
        //        }

        flexiSum = parseFloat(innerIfFlexiSum);

        for (var i = 0; i <= offDayColumnNameList.length - 1; i++) {

            var offDayColumnName = offDayColumnNameList[i];

            if (columnName === offDayColumnName) {

                //var innerIfForFlexiSum = totalSum - parseFloat(flexiValue);
                //                if (innerIfForFlexiSum > 0) {
                //                    flexiSum = parseFloat(innerIfForFlexiSum);
                //                }
                //flexiSum = parseFloat(innerIfForFlexiSum);

                flexiSum = parseFloat(totalSum);

                break;

            }
        }


    } //end if

    //Flexi
    var flexiTotalFooterRow = $(e.grid.sDiv).find("tr.flexiTotalFooterRow");
    if (flexiTotalFooterRow.length === 0) {
        // add second row of the footer if it's not exist
        flexiTotalFooterRow = footerRow.clone();
        flexiTotalFooterRow.removeClass("footrow").addClass("flexiTotalFooterRow ui-widget-content");
        flexiTotalFooterRow.children("td").each(function () {
            e.style.width = ""; // remove width from inline CSS
        });
        flexiTotalFooterRow.insertAfter(footerRow);
    }

    flexiTotalFooterRow.find(">td[aria-describedby=" + e.id + "_" + footerTextColumnName + "]").text("Flexi:");
    flexiTotalFooterRow.find(">td[aria-describedby=" + e.id + "_" + columnName + "]").text(
                        $.fmatter.util.NumberFormat(flexiSum, $.jgrid.formatter.number)
                    );

    //Total
    var totalFooterRow = $(e.grid.sDiv).find("tr.totalFooterRow");
    if (totalFooterRow.length === 0) {
        // add second row of the footer if it's not exist
        totalFooterRow = footerRow.clone();
        totalFooterRow.removeClass("footrow").addClass("totalFooterRow ui-widget-content");
        totalFooterRow.children("td").each(function () {
            e.style.width = ""; // remove width from inline CSS
        });
        totalFooterRow.insertAfter(footerRow);
    }

    totalFooterRow.find(">td[aria-describedby=" + e.id + "_" + footerTextColumnName + "]").text("Total:");
    totalFooterRow.find(">td[aria-describedby=" + e.id + "_" + columnName + "]").text(
                                $.fmatter.util.NumberFormat(totalSum, $.jgrid.formatter.number)
                            );
}


/*.............................End SetFooterTotalAndFlexi.......................................*/

/*.............................Start MergeGridColumnsHeader.......................................*/

function MergeGridColumnsHeader(grid, startColumnName, numberOfColumns, titleText) {

    $('#' + grid).jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
                      { startColumnName: startColumnName, numberOfColumns: numberOfColumns, titleText: '<div style="text-align:center;">' + titleText + '</div>' }
                  ]
    });

}

/*.............................End MergeGridColumnsHeader.......................................*/

/*.............................Start SetGridColumnHighLight.......................................*/

function SetGridColumnHighLight(grid, columnNameList) {

    var dataIDs = $('#' + grid).getDataIDs();

    for (var i = 0; i <= dataIDs.length; i++) {

        var rowId = dataIDs[i];

        for (var y = 0; y <= columnNameList.length - 1; y++) {

            var columnName = columnNameList[y];

            $.extend({
                threadedEach: SetGridColumnCellHighLight(grid, rowId, columnName)
            });
        }

    }

}

function SetGridColumnCellHighLight(grid, rowId, columnName) {

    $('#' + grid).jqGrid('setCell', rowId, columnName, "", 'offday-highlight');

}

/*.............................End SetGridColumnHighLight.......................................*/

/*.............................Start ChangeGridColumnHeaderTitle.......................................*/

function ChangeGridColumnHeaderTitle(grid, columnNameList, columnHeaderList) {

    for (var c = 0; c <= columnNameList.length - 1; c++) {

        var tempColumnName = columnNameList[c];
        var tempColumnHeader = columnHeaderList[c];

        var columnNameValue = parseInt(tempColumnName) + 1;
        var columnName = 'Day' + columnNameValue;

        var columnType = tempColumnHeader;

        var newHeaderTitle = tempColumnHeader + '<br/>' + tempColumnName;

        var columnHeaderHigh = 'Day' + tempColumnName;

        $.extend({
            threadedEach: SetGridColumnHeaderTitle(grid, columnName, newHeaderTitle, columnHeaderHigh)
        });

    } //end for

}

function SetGridColumnHeaderTitle(grid, columnName, newTitle, columnHeaderHigh) {

    //$('#' + grid).jqGrid('setLabel', columnName, newTitle);

    var jqGridColumnHeadIdForAddClass = '#' + grid + '_' + columnHeaderHigh;
    var jqGridColumnHeadIdForChangeText = '#jqgh_' + grid + '_' + columnHeaderHigh;

    $(jqGridColumnHeadIdForAddClass).removeClass('ui-state-default').addClass('offday-highlight');

    $(jqGridColumnHeadIdForChangeText).html('');
    $(jqGridColumnHeadIdForChangeText).html(newTitle);
}

/*.............................End ChangeGridColumnHeaderTitle.......................................*/


/*.............................Start Event Function.......................................*/

function ProjectCheck(id) {

    var dialogContent = 'Are you sure, the project is checked?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsProject', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsProject', 'False');
                            }
                        }
                    ]
    });

}

function ProjectCheck(rowId, id) {

    var dialogContent = 'Are you sure, the project is checked?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsProject', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsProject', 'False');
                            }
                        }
                    ]
    });

}

function CompleteCheck(id, e) {

    var dialogContent = 'Are you sure, the task is completed?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'True');
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'Completed', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'False');
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'Completed', 'False');
                            }
                        }
                    ]
    });

}

function CompleteCheck(rowId, id, e) {

    var dialogContent = 'Are you sure, the task is completed?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsCompleted Data 
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'True');
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'Completed', 'True');

                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                //Set Row IsCompleted Data
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'False');
                                $("#timeSheetJQGrid").jqGrid('setCell', rowId, 'Completed', 'False');
                            }
                        }
                    ]
    });

}

//(2, 51300100, rbSubmittedToPL_51300100_18, rbSubmittedToPS_51300100_18, PS, this)
function SubmittedToCheck(projectId, projectNo, typeVal, plId, psId, e) {
    //function SubmittedToCheck(projectId, projectNo, plId, psId, e) {

    //var thisTypeValueId = $(e).val();
    var thisTypeValueId = typeVal;

    var thisTypeValue;

    var employeeId = $("#EmployeeId").val();

    if (thisTypeValueId == 1) { //PL
        thisTypeValue = "PL";
    }
    else if (thisTypeValueId == 2) { //PS
        thisTypeValue = "PS";
    }

    var paramValue = JSON.stringify({ projectId: projectId, employeeId: employeeId });

    $.ajax({
        url: "/PIM/TimeSheet/CheckSubmittedTo",
        type: 'POST',
        dataType: 'json',
        data: paramValue,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {

            if (result.status == "True") {

                if (result.data != null) {

                    if (result.data == "0") {
                    }
                    else if (result.data == thisTypeValue) {
                    }
                    else if (result.data == "PS" || result.data == "PL") {

                        //Start Code

                        var dialogContent = 'You can not submit this timesheet to employee.';
                        var dialogDiv = $("<div>" + dialogContent + "</div>");
                        dialogDiv.dialog({
                            title: "Warning",
                            resizable: false,
                            width: 'auto',
                            modal: true,
                            buttons: [
                                        {
                                            text: "Ok",
                                            click: function () {

                                                if (result.data == "PL") {
                                                    $(plId).attr('checked', 'checked');
                                                    $(psId).removeAttr('checked');
                                                }
                                                else if (result.data == "PS") {
                                                    $(psId).attr('checked', 'checked');
                                                    $(plId).removeAttr('checked');
                                                }

                                                //add ur stuffs here
                                                dialogDiv.dialog("close");
                                            }
                                        },
                                        {
                                            text: "Cancel",
                                            click: function () {

                                                if (thisTypeValueId == 1) { //PL
                                                    $(plId).removeAttr('checked');
                                                    $(psId).attr('checked', 'checked');
                                                }
                                                else if (thisTypeValueId == 2) { //PS
                                                    $(psId).removeAttr('checked');
                                                    $(plId).attr('checked', 'checked');
                                                }

                                                dialogDiv.dialog("close");
                                            }
                                        }
                                      ]
                        });



                        //End Code

                    }
                    else {

                        $('#message').html("");
                        $('#message').html("<b style='color:Red'> " + result.data + " </b>");

                        if (thisTypeValueId == 1) { //PL
                            $(plId).removeAttr('checked');
                            $(psId).attr('checked', 'checked');
                        }
                        else if (thisTypeValueId == 2) { //PS
                            $(psId).removeAttr('checked');
                            $(plId).attr('checked', 'checked');
                        }
                    }
                    //End if == "0"

                } //End data null if
                else {
                    $('#message').html("");
                    $('#message').html("<b style='color:Green'> Your search criteria do not match any data. </b>");
                }

            } //End status if
            else {
                $('#message').html("");
                $('#message').html("<b style='color:Red'> " + result.data + " </b>");
            }

        },
        error: function (objAjaxRequest, strError) {
            var respText = objAjaxRequest.responseText;
            $('#message').html("");
            $('#message').html("<b style='color:Red'> " + respText + " </b>");
        }

    });


}

function SetHiddenFieldValue(dataObj) {

    $("#hdResourceInitialId").val('');
    $("#hdFortnightDateTime").val('');
    $("#hdFortnightType").val('');
    $("#hdFortnightDayNumber").val('');
    $("#hdMergeCellGroupColumnNameList").val('');
    $("#hdFooterColumnNameList").val('');
    $("#hdFooterTextColumnName").val('');
    $("#hdMergeColumnHeaderStartColumnName").val('');
    $("#hdMergeColumnHeaderNumberOfColumns").val('');
    $("#hdHighLightColumnNameList").val('');
    $("#hdOffDayColumnNameList").val('');
    $("#hdFlexiValue").val('');
    $("#hdJqGridUrl").val('');

    $("#hdResourceInitialId").val(dataObj.ResourceInitialId);
    $("#hdFortnightDateTime").val(dataObj.FortnightDateTime);
    $("#hdFortnightType").val(dataObj.FortnightType);
    $("#hdFortnightDayNumber").val(dataObj.FortnightDayNumber);
    $("#hdMergeCellGroupColumnNameList").val(dataObj.MergeCellGroupColumnNameList);
    $("#hdFooterColumnNameList").val(dataObj.FooterColumnNameList);
    $("#hdFooterTextColumnName").val(dataObj.FooterTextColumnName);
    $("#hdMergeColumnHeaderStartColumnName").val(dataObj.MergeColumnHeaderStartColumnName);
    $("#hdMergeColumnHeaderNumberOfColumns").val(dataObj.MergeColumnHeaderNumberOfColumns);
    $("#hdHighLightColumnNameList").val(dataObj.HighLightColumnNameList);

    $("#hdChangeColumnNameList").val(dataObj.ChangeColumnNameList);
    $("#hdChangeColumnHeaderList").val(dataObj.ChangeColumnHeaderList);

    $("#hdOffDayColumnNameList").val(dataObj.OffDayColumnNameList);
    $("#hdFlexiValue").val(dataObj.FlexiValue);
    $("#hdJqGridUrl").val(dataObj.JqGridUrl);
}

/*.............................End Event Function.......................................*/


/*.............................Start Create JqGrid Dynamic Coloumn.......................................*/

function CreateTimeSheetJqGridForFirstFortnight(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',
                    'IsEdited',

                    '1',
                    '2',
                    '3',
                    '4',
                    '5',
                    '6',
                    '7',
                    '8',
                    '9',
                    '10',
                    '11',
                    '12',
                    '13',
                    '14',
                    '15'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                 { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },
                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },
                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day1', index: 'Day1', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day2', index: 'Day2', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day3', index: 'Day3', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day4', index: 'Day4', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day5', index: 'Day5', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day6', index: 'Day6', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day7', index: 'Day7', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day8', index: 'Day8', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day9', index: 'Day9', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day10', index: 'Day10', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day11', index: 'Day11', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day12', index: 'Day12', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day13', index: 'Day13', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day14', index: 'Day14', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day15', index: 'Day15', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetJQGrid', "SubmittedText");

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWise('timeSheetJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName);

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {
            //function calling when cell exception occured
            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetJqGridForSecondFortnightOf16Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',
                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30',
                    '31'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },
                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day29', index: 'Day29', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day30', index: 'Day30', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day31', index: 'Day30', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetJQGrid', "SubmittedText");

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWise('timeSheetJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName);

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured


            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetJqGridForSecondFortnightOf15Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',
                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },
                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day29', index: 'Day29', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day30', index: 'Day30', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetJQGrid', "SubmittedText");

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWise('timeSheetJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName);

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetJqGridForSecondFortnightOf14Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',
                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },
                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day29', index: 'Day29', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetJQGrid', "SubmittedText");

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWise('timeSheetJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName);

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetJqGridForSecondFortnightOf13Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',
                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },

                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },


                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },
                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetJQGrid', "SubmittedText");

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWise('timeSheetJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName);

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');

            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}


/*.............................End Create JqGrid Dynamic Coloumn.......................................*/

/*.............................Start Create Update JqGrid Dynamic Coloumn.......................................*/

function ProjectCheckForUpdate(id) {

    var dialogContent = 'Are you sure, the project is checked?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //                                //Set Row IsProject Data 
                                //                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsProject', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                            }
                        }
                    ]
    });

}

function ProjectCheckForUpdate(rowId, id) {

    var dialogContent = 'Are you sure, the project is checked?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsProject', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsProject', 'False');
                            }
                        }
                    ]
    });

}

function CompleteCheckForUpdate(id, e) {

    var dialogContent = 'Are you sure, the task is completed?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsCompleted Data 
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'True');
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'Completed', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'False');
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'Completed', 'False');
                            }
                        }
                    ]
    });

}

function CompleteCheckForUpdate(rowId, id, e) {

    var dialogContent = 'Are you sure, the task is completed?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsCompleted Data 
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'True');
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'Completed', 'True');

                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                //Set Row IsCompleted Data
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'False');
                                $("#timeSheetUpdateJQGrid").jqGrid('setCell', rowId, 'Completed', 'False');
                            }
                        }
                    ]
    });

}

function CreateTimeSheetUpdateJqGridForFirstFortnight(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',
                    'HasUpdated',

                    '1',
                    '2',
                    '3',
                    '4',
                    '5',
                    '6',
                    '7',
                    '8',
                    '9',
                    '10',
                    '11',
                    '12',
                    '13',
                    '14',
                    '15'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                 { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },
                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },
                                                { name: 'HasUpdated', index: 'HasUpdated', hidden: true },

                                                { name: 'Day1', index: 'Day1', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day2', index: 'Day2', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day3', index: 'Day3', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day4', index: 'Day4', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day5', index: 'Day5', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day6', index: 'Day6', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day7', index: 'Day7', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day8', index: 'Day8', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day9', index: 'Day9', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day10', index: 'Day10', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day11', index: 'Day11', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day12', index: 'Day12', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day13', index: 'Day13', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day14', index: 'Day14', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day15', index: 'Day15', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetUpdateJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetUpdateJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

//            console.log(footerColumnNameList);
//            console.log(footerTextColumnName);
//            console.log(flexiValue);
//            console.log(offDayColumnNameList);

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetUpdateJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);

            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetUpdateJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetUpdateJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetUpdateJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetUpdateJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {
            //function calling when cell exception occured
            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetUpdateJqGridForSecondFortnightOf16Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',
                    'HasUpdated',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30',
                    '31'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },
                                                { name: 'HasUpdated', index: 'HasUpdated', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day29', index: 'Day29', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day30', index: 'Day30', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day31', index: 'Day30', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetUpdateJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetUpdateJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetUpdateJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetUpdateJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetUpdateJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetUpdateJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetUpdateJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured


            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetUpdateJqGridForSecondFortnightOf15Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',
                    'HasUpdated',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },
                                                { name: 'HasUpdated', index: 'HasUpdated', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day29', index: 'Day29', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day30', index: 'Day30', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetUpdateJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetUpdateJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetUpdateJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetUpdateJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetUpdateJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetUpdateJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetUpdateJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetUpdateJqGridForSecondFortnightOf14Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',
                    'HasUpdated',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },
                                                { name: 'HasUpdated', index: 'HasUpdated', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day29', index: 'Day29', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetUpdateJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetUpdateJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetUpdateJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetUpdateJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetUpdateJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetUpdateJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetUpdateJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetUpdateJqGridForSecondFortnightOf13Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',
                    'HasUpdated',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },

                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },


                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },

                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },
                                                { name: 'HasUpdated', index: 'HasUpdated', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day17', index: 'Day17', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day18', index: 'Day18', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day19', index: 'Day19', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day20', index: 'Day20', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day21', index: 'Day21', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day22', index: 'Day22', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day23', index: 'Day23', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day24', index: 'Day24', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day25', index: 'Day25', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day26', index: 'Day26', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day27', index: 'Day27', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} },
                                                { name: 'Day28', index: 'Day28', align: 'center', formatter: 'number', editable: true, editrules: { number: true, maxValue: 24 }, editoptions: { maxlength: 5} }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetUpdateJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetUpdateJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetUpdateJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetUpdateJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetUpdateJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetUpdateJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetUpdateJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');

            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

/*.............................End Create Update JqGrid Dynamic Coloumn.......................................*/

/*.............................Start Create Submit JqGrid Dynamic Coloumn.......................................*/

function ProjectCheckForSubmit(id) {

    var dialogContent = 'Are you sure, the project is checked?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //                                //Set Row IsProject Data 
                                //                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'IsProject', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                            }
                        }
                    ]
    });

}

function ProjectCheckForSubmit(rowId, id) {

    var dialogContent = 'Are you sure, the project is checked?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsProject Data 
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'IsProject', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                            }
                        }
                    ]
    });

}

function CompleteCheckForSubmit(id, e) {

    var dialogContent = 'Are you sure, the task is completed?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'True');
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'Completed', 'True');
                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'False');
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'Completed', 'False');
                            }
                        }
                    ]
    });

}

function CompleteCheckForSubmit(rowId, id, e) {

    var dialogContent = 'Are you sure, the task is completed?';
    var dialogDiv = $("<div>" + dialogContent + "</div>");
    dialogDiv.dialog({
        title: "Confirm",
        resizable: false,
        width: 'auto',
        modal: true,
        buttons: [
                        {
                            text: "Yes",
                            click: function () {
                                $(e).attr('checked', 'checked');
                                dialogDiv.dialog("close");
                                //Set Row IsCompleted Data 
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'IsCompleted', 'True');
                                $("#timeSheetSubmitJQGrid").jqGrid('setCell', rowId, 'Completed', 'True');

                            }
                        },
                        {
                            text: "No",
                            click: function () {
                                $(e).removeAttr('checked');
                                dialogDiv.dialog("close");
                            }
                        }
                    ]
    });

}

function CreateTimeSheetSubmitJqGridForFirstFortnight(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '1',
                    '2',
                    '3',
                    '4',
                    '5',
                    '6',
                    '7',
                    '8',
                    '9',
                    '10',
                    '11',
                    '12',
                    '13',
                    '14',
                    '15'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                 { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },
                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day1', index: 'Day1', align: 'center' },
                                                { name: 'Day2', index: 'Day2', align: 'center' },
                                                { name: 'Day3', index: 'Day3', align: 'center' },
                                                { name: 'Day4', index: 'Day4', align: 'center' },
                                                { name: 'Day5', index: 'Day5', align: 'center' },
                                                { name: 'Day6', index: 'Day6', align: 'center' },
                                                { name: 'Day7', index: 'Day7', align: 'center' },
                                                { name: 'Day8', index: 'Day8', align: 'center' },
                                                { name: 'Day9', index: 'Day9', align: 'center' },
                                                { name: 'Day10', index: 'Day10', align: 'center' },
                                                { name: 'Day11', index: 'Day11', align: 'center' },
                                                { name: 'Day12', index: 'Day12', align: 'center' },
                                                { name: 'Day13', index: 'Day13', align: 'center' },
                                                { name: 'Day14', index: 'Day14', align: 'center' },
                                                { name: 'Day15', index: 'Day15', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetSubmitJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetSubmitJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetSubmitJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetSubmitJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetSubmitJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetSubmitJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetSubmitJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {
            //function calling when cell exception occured
            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetSubmitJqGridForSecondFortnightOf16Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30',
                    '31'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' },
                                                { name: 'Day30', index: 'Day30', align: 'center' },
                                                { name: 'Day31', index: 'Day30', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetSubmitJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetSubmitJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetSubmitJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetSubmitJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetSubmitJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetSubmitJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetSubmitJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured


            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetSubmitJqGridForSecondFortnightOf15Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' },
                                                { name: 'Day30', index: 'Day30', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetSubmitJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetSubmitJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetSubmitJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetSubmitJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetSubmitJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetSubmitJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetSubmitJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetSubmitJqGridForSecondFortnightOf14Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetSubmitJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetSubmitJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetSubmitJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetSubmitJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetSubmitJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetSubmitJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetSubmitJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetSubmitJqGridForSecondFortnightOf13Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },

                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },


                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },

                                                { name: 'IsEdited', index: 'IsEdited', hidden: true, editable: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetSubmitJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetSubmitJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetSubmitJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetSubmitJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetSubmitJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetSubmitJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetSubmitJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');

            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

/*.............................End Create Submit JqGrid Dynamic Coloumn.......................................*/

/*.............................Start Create View JqGrid Dynamic Coloumn.......................................*/

function CreateTimeSheetViewJqGridForFirstFortnight(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '1',
                    '2',
                    '3',
                    '4',
                    '5',
                    '6',
                    '7',
                    '8',
                    '9',
                    '10',
                    '11',
                    '12',
                    '13',
                    '14',
                    '15'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                 { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },
                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day1', index: 'Day1', align: 'center' },
                                                { name: 'Day2', index: 'Day2', align: 'center' },
                                                { name: 'Day3', index: 'Day3', align: 'center' },
                                                { name: 'Day4', index: 'Day4', align: 'center' },
                                                { name: 'Day5', index: 'Day5', align: 'center' },
                                                { name: 'Day6', index: 'Day6', align: 'center' },
                                                { name: 'Day7', index: 'Day7', align: 'center' },
                                                { name: 'Day8', index: 'Day8', align: 'center' },
                                                { name: 'Day9', index: 'Day9', align: 'center' },
                                                { name: 'Day10', index: 'Day10', align: 'center' },
                                                { name: 'Day11', index: 'Day11', align: 'center' },
                                                { name: 'Day12', index: 'Day12', align: 'center' },
                                                { name: 'Day13', index: 'Day13', align: 'center' },
                                                { name: 'Day14', index: 'Day14', align: 'center' },
                                                { name: 'Day15', index: 'Day15', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetViewJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "SubmittedText");
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "ApprovalStatus");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetViewJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetViewJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetViewJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetViewJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);


        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetViewJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {
            //function calling when cell exception occured
            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetViewJqGridForSecondFortnightOf16Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30',
                    '31'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' },
                                                { name: 'Day30', index: 'Day30', align: 'center' },
                                                { name: 'Day31', index: 'Day30', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetViewJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "SubmittedText");
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "ApprovalStatus");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetViewJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetViewJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetViewJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetViewJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);


        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetViewJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured


            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetViewJqGridForSecondFortnightOf15Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' },
                                                { name: 'Day30', index: 'Day30', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetViewJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "SubmittedText");
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "ApprovalStatus");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetViewJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetViewJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetViewJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetViewJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);


        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetViewJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetViewJqGridForSecondFortnightOf14Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetViewJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "SubmittedText");
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "ApprovalStatus");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetViewJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetViewJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetViewJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetViewJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);


        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetViewJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetViewJqGridForSecondFortnightOf13Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'Submitted To',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },

                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },


                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },

                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetViewJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "SubmittedText");
            MergeGridCellByProjectNo('timeSheetViewJQGrid', "ApprovalStatus");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetViewJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetViewJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetViewJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetViewJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);


        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetViewJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');

            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

/*.............................End Create View JqGrid Dynamic Coloumn.......................................*/


/*.............................Start ApprovalED JqGrid Dynamic Coloumn.......................................*/

function CreateTimeSheetApprovalEDJqGridForFirstFortnight(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'PL/PS',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '1',
                    '2',
                    '3',
                    '4',
                    '5',
                    '6',
                    '7',
                    '8',
                    '9',
                    '10',
                    '11',
                    '12',
                    '13',
                    '14',
                    '15'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                 { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },
                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day1', index: 'Day1', align: 'center' },
                                                { name: 'Day2', index: 'Day2', align: 'center' },
                                                { name: 'Day3', index: 'Day3', align: 'center' },
                                                { name: 'Day4', index: 'Day4', align: 'center' },
                                                { name: 'Day5', index: 'Day5', align: 'center' },
                                                { name: 'Day6', index: 'Day6', align: 'center' },
                                                { name: 'Day7', index: 'Day7', align: 'center' },
                                                { name: 'Day8', index: 'Day8', align: 'center' },
                                                { name: 'Day9', index: 'Day9', align: 'center' },
                                                { name: 'Day10', index: 'Day10', align: 'center' },
                                                { name: 'Day11', index: 'Day11', align: 'center' },
                                                { name: 'Day12', index: 'Day12', align: 'center' },
                                                { name: 'Day13', index: 'Day13', align: 'center' },
                                                { name: 'Day14', index: 'Day14', align: 'center' },
                                                { name: 'Day15', index: 'Day15', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetApprovalEDJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetApprovalEDJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetApprovalEDJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetApprovalEDJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetApprovalEDJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetApprovalEDJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetApprovalEDJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {
            //function calling when cell exception occured
            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetApprovalEDJqGridForSecondFortnightOf16Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'PL/PS',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30',
                    '31'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' },
                                                { name: 'Day30', index: 'Day30', align: 'center' },
                                                { name: 'Day31', index: 'Day30', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetApprovalEDJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetApprovalEDJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetApprovalEDJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetApprovalEDJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetApprovalEDJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetApprovalEDJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetApprovalEDJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured


            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetApprovalEDJqGridForSecondFortnightOf15Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'PL/PS',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29',
                    '30'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },


                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' },
                                                { name: 'Day30', index: 'Day30', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetApprovalEDJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetApprovalEDJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetApprovalEDJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetApprovalEDJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetApprovalEDJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetApprovalEDJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetApprovalEDJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetApprovalEDJqGridForSecondFortnightOf14Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'PL/PS',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28',
                    '29'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },
                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },

                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },


                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' },
                                                { name: 'Day29', index: 'Day29', align: 'center' }
                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetApprovalEDJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetApprovalEDJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetApprovalEDJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetApprovalEDJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetApprovalEDJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetApprovalEDJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetApprovalEDJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');
            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

function CreateTimeSheetApprovalEDJqGridForSecondFortnightOf13Days(gridId, gridUrl) {

    $('#' + gridId).jqGrid({
        url: gridUrl,
        datatype: "json",
        mtype: 'POST',
        colNames: [
                    'Id',
                    'ProjectId',
                    'ProjectNo',
                    'Project No',
                    'IsProject',
                    'Project Title',
                    'ApprovalStatusId',
                    'IsApprovalStatus',
                    'Status',
                    'ProjectLeaderId',
                    'ProjectLeader',
                    'ProjectSupervisorId',
                    'ProjectSupervisor',
                    'IsSubmittedTo',
                    'SubmittedTo',
                    'PL/PS',
                    'ActivityId',
                    'Activity',
                    'IsCompleted',
                    'Completed',
                    'Complete',

                    'TimeSheetId',
                    'TimeSheetProjectId',
                    'TimeSheetDayId',
                    'TimeSheetActivityId',

                    'IsEdited',

                    '16',
                    '17',
                    '18',
                    '19',
                    '20',
                    '21',
                    '22',
                    '23',
                    '24',
                    '25',
                    '26',
                    '27',
                    '28'
                    ],
        //colNames: gridColumnNameList,
        //colModel: gridColumnModelList,
        colModel: [
                                                { name: 'Id', index: 'Id', key: true, hidden: true },
                                                { name: 'ProjectId', index: 'ProjectId', hidden: true },
                                                { name: 'ProjectNo', index: 'ProjectNo', hidden: true },
                                                { name: 'ProjectText', index: 'ProjectText', align: 'left', hidden: false },
                                                { name: 'IsProject', index: 'IsProject', hidden: true },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ProjectTitle', index: 'ProjectTitle', align: 'left', hidden: false },
                                                { name: 'ApprovalStatusId', index: 'ApprovalStatusId', hidden: true },
                                                { name: 'IsApprovalStatus', index: 'IsApprovalStatus', hidden: true },
                                                { name: 'ApprovalStatus', index: 'ApprovalStatus', align: 'center' },

                                                { name: 'ProjectLeaderId', index: 'ProjectLeaderId', hidden: true },
                                                { name: 'ProjectLeader', index: 'ProjectLeader', hidden: true },
                                                { name: 'ProjectSupervisorId', index: 'ProjectSupervisorId', hidden: true },
                                                { name: 'ProjectSupervisor', index: 'ProjectSupervisor', hidden: true },


                                                { name: 'IsSubmittedTo', index: 'IsSubmittedTo', hidden: true },
                                                { name: 'SubmittedTo', index: 'SubmittedTo', hidden: true },
                                                { name: 'SubmittedText', index: 'SubmittedText', align: 'center' },

                                                { name: 'ActivityId', index: 'ActivityId', hidden: true },
                                                { name: 'Activity', index: 'Activity' },
                                                { name: 'IsCompleted', index: 'IsCompleted', hidden: true },
                                                { name: 'Completed', index: 'Completed', hidden: true },
                                                { name: 'CompletedText', index: 'CompletedText', align: 'center' },

                                                { name: 'TimeSheetId', index: 'TimeSheetId', hidden: true },
                                                { name: 'TimeSheetProjectId', index: 'TimeSheetProjectId', hidden: true },
                                                { name: 'TimeSheetDayId', index: 'TimeSheetDayId', hidden: true },
                                                { name: 'TimeSheetActivityId', index: 'TimeSheetActivityId', hidden: true },

                                                { name: 'IsEdited', index: 'IsEdited', hidden: true },

                                                { name: 'Day16', index: 'Day16', align: 'center' },
                                                { name: 'Day17', index: 'Day17', align: 'center' },
                                                { name: 'Day18', index: 'Day18', align: 'center' },
                                                { name: 'Day19', index: 'Day19', align: 'center' },
                                                { name: 'Day20', index: 'Day20', align: 'center' },
                                                { name: 'Day21', index: 'Day21', align: 'center' },
                                                { name: 'Day22', index: 'Day22', align: 'center' },
                                                { name: 'Day23', index: 'Day23', align: 'center' },
                                                { name: 'Day24', index: 'Day24', align: 'center' },
                                                { name: 'Day25', index: 'Day25', align: 'center' },
                                                { name: 'Day26', index: 'Day26', align: 'center' },
                                                { name: 'Day27', index: 'Day27', align: 'center' },
                                                { name: 'Day28', index: 'Day28', align: 'center' }

                                                ],
        cmTemplate: { sortable: false, clearSearch: false },
        //cellEdit: true,
        cellsubmit: 'clientArray',
        //height: '100%',
        height: 'auto',
        autowidth: true,
        grouping: false,
        sortname: 'id',                     //default sort column name
        sortorder: "asc",                       //sorting order
        //emptyrecords: 'No data found.',
        viewrecords: true,                         //by default records show?
        loadonce: false,
        multiselect: false,                        //checkbox list
        //shrinkToFit: true,
        footerrow: true,
        caption: "Time Sheet Details Information",
        loadComplete: function (data) {

            //Get Hidden Field Value
            var hdMergeCellGroupColumnNameListValue = $("#hdMergeCellGroupColumnNameList").val();
            var hdFooterColumnNameListValue = $("#hdFooterColumnNameList").val();
            var hdFooterTextColumnNameValue = $("#hdFooterTextColumnName").val();
            var hdMergeColumnHeaderStartColumnNameValue = $("#hdMergeColumnHeaderStartColumnName").val();
            var hdMergeColumnHeaderNumberOfColumnsValue = $("#hdMergeColumnHeaderNumberOfColumns").val();
            var hdHighLightColumnNameListValue = $("#hdHighLightColumnNameList").val();

            var hdChangeColumnNameListValue = $("#hdChangeColumnNameList").val();
            var hdChangeColumnHeaderListValue = $("#hdChangeColumnHeaderList").val();

            var hdJqGridUrlValue = $("#hdJqGridUrl").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var mergeCellGroupColumnNameList = hdMergeCellGroupColumnNameListValue.split(",");
            var footerColumnNameList = hdFooterColumnNameListValue.split(",");
            var footerTextColumnName = hdFooterTextColumnNameValue;
            var mergeColumnHeaderStartColumnName = hdMergeColumnHeaderStartColumnNameValue;
            var mergeColumnHeaderNumberOfColumns = hdMergeColumnHeaderNumberOfColumnsValue;
            var highLightColumnNameList = hdHighLightColumnNameListValue.split(",");

            var changeColumnNameList = hdChangeColumnNameListValue.split(",");
            var changeColumnHeaderList = hdChangeColumnHeaderListValue.split(",");

            var jqGridUrl = hdJqGridUrlValue;
            //Pass To Array Hidden Field Value

            //Merge Cell Need: columnNameList FROM SERVER ( mergeCellGroupColumnNameList )
            MergeGridCellGroupWise('timeSheetApprovalEDJQGrid', columnNameList = mergeCellGroupColumnNameList);
            MergeGridCellByProjectNo('timeSheetApprovalEDJQGrid', "SubmittedText");

            ////------------------------------------
            //Get Hidden Field Value
            var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
            var hdFlexiValueValue = $("#hdFlexiValue").val();
            //Get Hidden Field Value

            //Pass To Array Hidden Field Value
            var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
            var flexiValue = hdFlexiValueValue;
            //Pass To Array Hidden Field Value

            //Set Total an Grand Total Need: columnNameList, footerTextColumnName FROM SERVER (footerColumnNameList, footerTextColumnName)
            SetFooterTotalAndFlexiColumnWiseForSumbitAndEdit('timeSheetApprovalEDJQGrid', this, columnNameList = footerColumnNameList, footerTextColumnName = footerTextColumnName, flexiValue, offDayColumnNameList = offDayColumnNameList);
            ////------------------------------------

            //Merge GridColumns Header Need: startColumnName, numberOfColumns FROM SERVER ( mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns )
            MergeGridColumnsHeader('timeSheetApprovalEDJQGrid', mergeColumnHeaderStartColumnName, mergeColumnHeaderNumberOfColumns, "Days");

            //GridColumn HighLight Need: columnNameList FROM SERVER ( highLightColumnNameList )
            SetGridColumnHighLight('timeSheetApprovalEDJQGrid', columnNameList = highLightColumnNameList);

            //GridColumn HeaderTitle Need: columnNameList FROM SERVER ( changeColumnNameList, changeColumnHeaderList )
            ChangeGridColumnHeaderTitle('timeSheetApprovalEDJQGrid', columnNameList = changeColumnNameList, columnHeaderList = changeColumnHeaderList);

        },
        loadError: function (xhr, status, str) {   //function calling when grid load exception occured 
            //set div text by error message

            $('#message').html("<b style='color:Red'> " + xhr.msg + " </b>");
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {

            if (rowid != 0) {

                //Get Hidden Field Value
                var hdOffDayColumnNameListValue = $("#hdOffDayColumnNameList").val();
                var hdFlexiValueValue = $("#hdFlexiValue").val();
                //Get Hidden Field Value

                //Pass To Array Hidden Field Value
                var offDayColumnNameList = hdOffDayColumnNameListValue.split(",");
                var flexiValue = hdFlexiValueValue;
                //Pass To Array Hidden Field Value


                //Calculate FooterData
                CalculateFooterTotalAndFlexiWithOffDay('timeSheetApprovalEDJQGrid', this, rowid, name, val, flexiValue, offDayColumnNameList = offDayColumnNameList);

                //Set Row Data
                //$(this).jqGrid('setCell', rowid, 'IsEdited', 'True');

            }

            return false;
        },
        //        afterEditCell: function (rowid, name, val, iRow, iCol) {
        //            if (rowid != 0) { }
        //            return false;
        //        },
        errorCell: function () {                   //function calling when cell exception occured

            $('#message').html("<b style='color:Red'> An error has occurred while processing your request. </b>");
        }

    });

    return false;
}

/*.............................End ApprovalED JqGrid Dynamic Coloumn.......................................*/