$(document).ready(function () {
    /*
    Developer: Sajid Khan
    Date: 7-12-19
    Action: Autocomplete search by vendor name like on greater than three character
    URL:
    Input: string
    output: list of vendors like matched
    */

    // Single Select
    $("#poVendor").autocomplete({
        source: function (request, response) {
            var jsonData = {};
            jsonData["columnname"] = 'vendor_name';
            jsonData["search"] = request.term;
            console.log(jsonData);

            if (request.term.length > 2) {
                $.ajax({
                    url: "/vendor/autosuggest/",
                    dataType: 'json',
                    type: 'post',
                    data: JSON.stringify(jsonData),
                    contentType: 'application/json',
                    processData: false,
                }).always(function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        response(data);
                        $('#poVendor').removeClass("errorFeild");
                        $('.errorMsg').remove();
                    } else {
                        $('#poVendor').removeClass("errorFeild");
                        $('.errorMsg').remove();
                        $('#poVendor').addClass('errorFeild');
                        $('#poVendor').parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">Record Not Found</span>')
                    }
                   
                });
            } 
            if (request.term.length == 0){
                $('#poVendor').val(""); // display the selected text
                $('#poVendor').attr("data-val", "");
            }
        },
        select: function (event, ui) {
            // Set selection
            $('#poVendor').val(ui.item.label); // display the selected text
            $('#poVendor').attr("data-val", ui.item.value);
            // $('#selectuser_id').val(ui.item.value); // save selected id to input
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $("#poVendor").val(ui.item.label);
        }
    });


//Auto Fill For product select 

 // Single Select
    $("#productSelect").autocomplete({
        source: function (request, response) {
            var jsonData = {};
            jsonData["columnname"] = 'vendor_name';
            jsonData["search"] = request.term;
            console.log(jsonData);

            if (request.term.length > 3) {
                $.ajax({
                    url: "/vendor/autosuggest/",
                    dataType: 'json',
                    type: 'post',
                    data: JSON.stringify(jsonData),
                    contentType: 'application/json',
                    processData: false,
                }).always(function (data) {
debugger;
                    console.log(data);
                    if (data.length > 0) {
                        response(data);
                        $('#poVendor').removeClass("errorFeild");
                        $('.errorMsg').remove();
                    } else {
                        $('#poVendor').removeClass("errorFeild");
                        $('.errorMsg').remove();
                        $('#poVendor').addClass('errorFeild');
                        $('#poVendor').parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">Record Not Found</span>')
                    }
                   
                });
            } 
            if (request.term.length == 0){
                $('#poVendor').val(""); // display the selected text
                $('#poVendor').attr("data-val", "");
            }
        },
        select: function (event, ui) {
            // Set selection
            $('#poVendor').val(ui.item.label); // display the selected text
            $('#poVendor').attr("data-val", ui.item.value);
            // $('#selectuser_id').val(ui.item.value); // save selected id to input
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $("#poVendor").val(ui.item.label);
        }
    });


    /*
        Developer: Azeem Hassan
        Date: 7-6-19 
        Action:checking value of input number and dot allow
        URL:
        Input:any keypress
        output: true/false
    */
    $("#poTotalQuantity,#poSubtotal,#poShipping").on("keydown", function (event) {
        if ($(this).val().indexOf('.') > -1 && event.which == 190) {
            return false;
        }
          return onlyNumbersWithDot(event);
    });
     /*
        Developer: Azeem Hassan
        Date: 7-6-19 
        Action:checking value of input number allow only
        URL:
        Input:any keypress
        output: true/false
    */
    $("#poTotalStyles,#poOrderNumber").on("keydown", function (event) {
        return isNumber(event);
    });
      /*
        Developer: Azeem Hassan
        Date: 7-6-19 
        Action: blocking special characters
        URL:
        Input:any keypress
        output: true/false
    */
    $("#poNumber,#poInvoiceNumber").on("keydown", function (event) {
       return blockspecialcharacter(event)
    });

    /*
        Developer: Azeem Hassan
        Date: 7-6-19 
        Action: allow number and back slash
        URL:
        Input:any keypress
        output: true/false
    */
    $("#poOrderDate, #poDelieveryRange").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9- \/ ]/g, ''));
        if ((event.which != 32 || $(this).val().indexOf('/') != -1) && (event.which < 45 || event.which > 57)) {
            event.preventDefault();
        }
       
    });
     /*
        Developer: Azeem Hassan
        Date: 7-6-19 
        Action: this function is getting single purchase order data or single purchase order page
        URL:
        Input: order id
        output: single purchase order
    */
    if (window.location.href.indexOf('PurchaseOrders/Single') > -1) {
        var id = window.location.href.split('Single/')[1];

        if (id != undefined && id != "") {
            $('.loading').removeClass("d-none");
            getSinglePurchaseOrder(id);
        } else {
            window.location = location.protocol + "//" + location.host;
        }
        
    }

})

var table = $('#purchaseorderlists').DataTable({
    "aaSorting": [],
    "lengthMenu": [50, 100, 200],
    "pageLength": 50
});

window.purchaseorderrownumber = "";
$('#purchaseorderlists tbody').on('click', 'tr', function (e) {
    window.purchaseorderrownumber = table.row(this)[0][0];
});

window.checkpaginationload = true;
$('#purchaseorderlists').on('page.dt', function () {

    var info = table.page.info();

    console.log('Showing page: ' + (info.page + 1) + ' of ' + info.pages);

    /*if (window.checkpaginationload == true && info.pages == (info.page + 1)) {
        console.log("Load more...");
        $('.loading').removeClass("d-none");
        var start_total = info.recordsTotal; //table4.column(0).data().length;
        console.log(start_total);
        $.ajax({
            url: "/purchaseorders/listpagination/" + start_total + "/30/true",
            type: 'GET',
            dataType: 'json',
            processData: false,
            contentType: false,
        }).always(function (data) {
            console.log(data);
            if (data.PurchaseOrdersLists.length == 0) {
                window.checkpaginationload = false;
                $('.loading').addClass("d-none");
            }
            if (data.PurchaseOrdersLists.length > 0) {
                for (var i = 0; i < data.PurchaseOrdersLists.length; i++) {
                    var data2 = data.PurchaseOrdersLists[i];
                    var statusButton = '';
                    if (data2.po_status == '5') {
                        statusButton = '<button type="button" class="btn btn-warning btn-sm" data-shipping="' + data2.shipping + '" data-id="' + data2.po_id + '" id="EditPurhaseOrderCheckedIn">Checkin</button>'
                    } else {
                        statusButton = "<button type='button' class='btn btn-success btn-sm'>Checked-In</button>";                        
                    }
                    var status = getPoStatusById(data2.po_status);
                    $('#purchaseorderlists').DataTable().row.add([data2.vendor_po_number, data2.custom_order_date, data2.vendor, data2.total_styles, "1", "2", data2.custom_delivery_window_start_end, data2.num_of_days, status, statusButton, "<button type='button' id='EditPurhaseOrder' data-id='" + data2.po_id +"' class='btn btn-light btn-sm'>Edit</button>", "<a href='javascript: void (0)' data-id='" + data2.po_id +"' id='EditPurhaseOrderNote'><i class='fa fa-edit h3'></i></a>", "<a href='javascript: void (0)' data-id='" + data2.po_id +"' id='EditPurhaseOrderDocument'><i class='fa fa-upload h3'></i></a>", "<a href='javascript: void (0)'>Claim</a>", "<a href='javascript: void (0)'>Claim</a>"]).draw(false);
                }
                $('.loading').addClass("d-none");
            }
        });
    }*/
});
function getPoStatusById(po_status, poid) {
    if (po_status == 1) {
        return '<span class="postatus-'+poid+'">Open</span>';
    } else if (po_status == 3) {
        return '<span class="postatus-'+ poid+'">In Progress</span>';
    } else if (po_status == 6) {
        return '<span class="postatus-'+ poid+'">Received</span>';
    } else {
        return '<span class="postatus-'+ poid+'">Not Received</span>';
    }
}
$('#poOrderDate').datepicker({
    dateFormat: 'm/d/yy'
});

$('#poDelieveryRange').daterangepicker({
    locale: {
        format: 'M/D/YYYY'
    }
});

$(document).on('change keydown blur', "#newPurchaseOrderForm input", function (e) {
    var poorderdate = $(this).attr('id');
    if (poorderdate == "poOrderDate") {
        $("#"+poorderdate).removeClass('errorFeild');
        $("#"+poorderdate).parents('.form-group').find('.errorMsg').remove();
    } else {
        $(this).removeClass('errorFeild');
        $(this).parents('.form-group').find('.errorMsg').remove();
    }
});
/*
    Developer: Azeem Hassan
    Date: 7-6-19 
    Action: this function is getting data from purchase order form and parsing to variable and sends to controller
    URL:/purchaseorders/newpurchaseorder
    Request: POST
    Input: new purchase order form data
    output: new purchase id
*/
$(document).on('click', "#NewPurchaseOrderButton", function () {
    _self = $(this);
    _self.attr('disabled', true);
    if (emptyFeildValidation('newPurchaseOrderForm') == false) {
        _self.attr('disabled', false);
        return false;
    }

    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');

    var vendorname = $("#newPurchaseOrderForm #poVendor").val();

    var jsonData = {};

    var delieveryRange  = $("#newPurchaseOrderForm #poDelieveryRange").val();
        delieveryRange = delieveryRange.split("-");

    var delivery_window_start = new Date(delieveryRange[0].trim());
        delivery_window_start_milliseconds = delivery_window_start.getTime();
        delivery_window_start_seconds = delivery_window_start_milliseconds / 1000;

    var delivery_window_end = new Date(delieveryRange[1].trim());
        delivery_window_end_milliseconds = delivery_window_end.getTime();
        delivery_window_end_seconds = delivery_window_end_milliseconds / 1000;

    var delivery_window = (delivery_window_start.getMonth() + 1) + "/" + delivery_window_start.getDate() + "-"+ (delivery_window_end.getMonth() + 1) + "/" + delivery_window_end.getDate() + "/" + delivery_window_end.getFullYear();

    var order_date = new Date($("#newPurchaseOrderForm #poOrderDate").val());
        order_date_milliseconds = order_date.getTime();
        order_date_seconds = order_date_milliseconds / 1000;

    var orderdate = order_date.getMonth() + 1 + "/" + order_date.getDate() + "/" + order_date.getFullYear();
    var shipping = $("#newPurchaseOrderForm #poShipping").val();

    jsonData["vendor_po_delievery_range"] = $("#newPurchaseOrderForm #poDelieveryRange").val();
    jsonData["vendor_po_number"] = $("#newPurchaseOrderForm #poNumber").val();
    jsonData["vendor_invoice_number"] = $("#newPurchaseOrderForm #poInvoiceNumber").val();
    jsonData["vendor_order_number"] = $("#newPurchaseOrderForm #poOrderNumber").val();
    jsonData["vendor_id"] = $("#newPurchaseOrderForm #poVendor").attr("data-val");
    jsonData["total_styles"] = $("#newPurchaseOrderForm #poTotalStyles").val();
    jsonData["total_quantity"] = $("#newPurchaseOrderForm #poTotalQuantity").val();
    jsonData["subtotal"] = $("#newPurchaseOrderForm #poSubtotal").val();
    jsonData["shipping"] = $("#newPurchaseOrderForm #poShipping").val();
    jsonData["order_date"] = $("#newPurchaseOrderForm #poOrderDate").val();

    jsonData["note"] = $("#newPurchaseOrderForm #poNotes").val();

    console.log(jsonData);
    var note = $("#newPurchaseOrderForm #poNotes").val();
    
    $.ajax({
        url: '/purchaseorders/newpurchaseorder',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        console.log(data);

        if (data != 0 && data > 0) {
            console.log('New row created - ' + data);
            //alertBox('poAlertMsg', 'green', 'Purchase order inserted successfully.');
            var fileLength = $("#poUploadImage")[0].files.length;
            if (fileLength != 0) {

                var files = $("#poUploadImage")[0].files;

                var formData = new FormData();

                formData.append('po_id', data);

                for (var i = 0; i != files.length; i++) {
                    formData.append("purchaseOrderDocuments", files[i]);
                }

                $.ajax({
                    url: "/purchaseorders/purchaseorder_doc",
                    type: 'POST',
                    data: formData,
                    dataType: 'json',
                    processData: false,
                    contentType: false,
                }).always(function (data) {
                    console.log(data);
                    if (data == "0") {
                        console.log("Exception Error");
                        alertBox('poAlertMsg', 'red', 'Purchase order document Exception Error.');
                    } else {
                        $("#EditPurhaseOrderDocument[data-id='" + id + "']").find(".redDotDoc").addClass("redDOtElement");
                        console.log(data.responseText);
                    }
                });
            }
            $('#purchaseorderlists').DataTable().row.add([$("#newPurchaseOrderForm #poNumber").val(), orderdate, vendorname, $("#newPurchaseOrderForm #poTotalStyles").val(), 0, 0, delivery_window, 0 + " Day", "<span class='postatus-" + data + "'>Not Received</span>", '<button type="button" class="btn btn-warning btn-sm  checked-'+data+'" data-shipping="' + shipping + '"  data-ID="' + data + ' id="EditPurhaseOrderCheckedIn">Checkin</button>', '<button type="button" id="EditPurhaseOrder" data-id="' + data + '" class="btn btn-light btn-sm">Edit</button>', '<a href="javascript:void(0)" data-ID="' + data + '" id="EditPurhaseOrderNote"><i class="fa fa-edit h3"></i></a>', '<a href="javascript:void(0)" data-ID="' + data + '" id="EditPurhaseOrderDocument"><i class="fa fa-upload h3"></i></a>', '<a href="javascript:void(0)">Claim</a>', '<a href="javascript:void(0)">Claim</a>']).draw();

            table.page('last').draw('page');

            $('#modalPurchaseOrder').modal('hide');

            if (note != "") {
                $("#EditPurhaseOrderNote[data-id='" + data + "']").find(".redDotNote").addClass("redDOtElement");
            } else {
                $("#EditPurhaseOrderNote[data-id='" + data + "']").find(".redDotNote").removeClass("redDOtElement");
            }

            alertBox('poAlertMsg', 'green', 'Purchase order inserted successfully.');
            
            $('#newPurchaseOrderForm')[0].reset();
            _self.attr('disabled', false);
        } else {
            _self.attr('disabled', false);
            alertBox('poAlertMsg', 'red', 'Purchase order not inserted.');
        }
    });
});
/*
    Developer: Azeem Hassan
    Date: 7-6-19 
    Action:converting secont to date
    URL:
    Input:time in second
    output: date
*/
function timeToDateConvert(timeinseconds) {
    var datetime = new Date(timeinseconds * 1000);

    var year = datetime.getFullYear();
    var month = (datetime.getMonth()+1);
    var date = datetime.getDate();

    if (date < 10) {
        date = date;
    }

    if (month < 10) {
        month = month;
    }

    return month + '/' + date + '/' + year;
}

/*
    Developer: Azeem Hassan
    Date: 7-6-19 
    Action:getting data from controller and sending to purchaseOrderData function to show data
    URL:/purchaseorders/details/
    REQUEST:GET
    Input:
    output: purchase order data
*/

$(document).on('click', "#EditPurhaseOrder", function () {
    $("#newPurchaseOrderForm input,textarea").val("").removeClass('errorFeild');
    $('.errorMsg').remove();
    $(".error").remove();
    $('#view_adjustment,#view_discount, #wrapper_tracking,.po_doc_section').empty().html("");
    $('.poTracking, #poNotes').val("");

    $('.po_section').removeClass('d-none');

    $("#modalPurchaseOrder #purchaseOrderModalLongTitle").text("Edit Purhase Order");
    $('#modalPurchaseOrder input,#modalPurchaseOrder button').prop("disabled", "true");
    $('#modalPurchaseOrder').modal('show');
    var id = $(this).data("id");

    $("#newPurchaseOrderForm").attr("data-currentid", id);

    $.ajax({
        url: '/purchaseorders/details/' + id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);
        purchaseOrderData(data);
        $("#modalPurchaseOrder #purchaseOrderModalLongTitle").text("Edit Purhase Order Number ("+$('#newPurchaseOrderForm #poNumber').val()+")");
        $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").attr("id", "EditPurchaseOrderButton");
        $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").html("Update");
        $('#modalPurchaseOrder input,#modalPurchaseOrder button').removeAttr("disabled");
    });

});
/*
    Developer: Azeem Hassan
    Date: 7-6-19 
    Action: creating discount 
    URL:/purchaseorders/discountcreate
    REQUEST:POST
    Input:Discount data
    output: discount id
*/
window.discount = "";
$(document).on("click", "#discount_submit", function () {

    var jsonData = {};

    jsonData["po_id"] = $("#newPurchaseOrderForm").attr("data-currentid");
    jsonData["discount_percentage"] = $("#discount_percentage").val();
    jsonData["discount_note"] = $("#discount_note").val();
    jsonData["completed_status"] = 1;

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/discountcreate',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        $("#view_discount").empty();
        $(data).each(function (e, i) {

            $('#discount_form')[0].reset();
            $('#modaladddiscount').modal('hide');

            console.log(e + " -- " + i.po_id + " - " + i.discount_percentage + " - " + i.discount_note + " - " + i.completed_status);
            $("#view_discount").append("Discount  -- " + i.discount_percentage + " - " + i.discount_note + " - " + i.completed_status);

            window.discount = jsonData;

        })
    });

});

window.adjustment = "";
/*
    Developer: Azeem Hassan
    Date: 7-6-19 
    Action: creating ledger 
    URL:/purchaseorders/ledgercreate
    REQUEST:POST
    Input:ledger data
    output: ledger id
*/
$(document).on("click", "#ledger_submit", function () {

    var jsonData = {};

    jsonData["po_id"] = $("#newPurchaseOrderForm").attr("data-currentid");
    jsonData["ledger_adjustment"] = $("#ledger_adjustment").val();
    jsonData["ledger_amount"] = $("#ledger_amount").val();
    jsonData["ledger_note"] = $("#ledger_note").val()

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/ledgercreate',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        $("#view_adjustment").empty();
        $(data).each(function (e, i) {

            $('#ledger_form')[0].reset();            
            $('#modaladdinvoice').modal('hide');

            console.log(e + " -- " + i.po_id + " - " + i.credit + " - " + i.debit + " - " + i.description);
            $("#view_adjustment").append("Adjustment -- Credit - " + i.credit + " Debit - " + i.debit + " - " + i.description + " <br>");

            window.adjustment = jsonData;

        })
    });

});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: update purchase orders data with notes and files by poid
URL: /purchaseorders/updatepurchaseorder/id
Input: int id
output: string
*/
$(document).on('click', "#EditPurchaseOrderButton", function () {

    var id = $("#newPurchaseOrderForm").data("currentid");
    var vendorname = $("#newPurchaseOrderForm #poVendor").val();
  
    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');
    var delieveryRange = $("#newPurchaseOrderForm #poDelieveryRange").val();
    delieveryRange = delieveryRange.split("-");

    var delivery_window_start = new Date(delieveryRange[0].trim());
    delivery_window_start_milliseconds = delivery_window_start.getTime();
    delivery_window_start_seconds = delivery_window_start_milliseconds / 1000;

    var delivery_window_end = new Date(delieveryRange[1].trim());
    delivery_window_end_milliseconds = delivery_window_end.getTime();
    delivery_window_end_seconds = delivery_window_end_milliseconds / 1000;

    var delivery_window = (delivery_window_start.getMonth() + 1) + "/" + delivery_window_start.getDate() + "-" + (delivery_window_end.getMonth() + 1) + "/" + delivery_window_end.getDate() + "/" + delivery_window_end.getFullYear();

    var order_date = new Date($("#newPurchaseOrderForm #poOrderDate").val());
    order_date_milliseconds = order_date.getTime();
    order_date_seconds = order_date_milliseconds / 1000;

    var orderdate = order_date.getMonth() + 1 + "/" + order_date.getDate() + "/" + order_date.getFullYear();
    var shipping = $("#newPurchaseOrderForm #poShipping").val();

    var jsonData = {};

    jsonData["vendor_po_delievery_range"] = $("#newPurchaseOrderForm #poDelieveryRange").val();
    jsonData["vendor_po_number"] = $("#newPurchaseOrderForm #poNumber").val();
    jsonData["vendor_invoice_number"] = $("#newPurchaseOrderForm #poInvoiceNumber").val();
    jsonData["vendor_order_number"] = $("#newPurchaseOrderForm #poOrderNumber").val();
    jsonData["vendor_id"] = $("#newPurchaseOrderForm #poVendor").attr("data-val");
    jsonData["total_styles"] = $("#newPurchaseOrderForm #poTotalStyles").val();
    jsonData["total_quantity"] = $("#newPurchaseOrderForm #poTotalQuantity").val();
    jsonData["subtotal"] = $("#newPurchaseOrderForm #poSubtotal").val();
    jsonData["shipping"] = $("#newPurchaseOrderForm #poShipping").val();
    jsonData["order_date"] = $("#newPurchaseOrderForm #poOrderDate").val();
    jsonData["old_note"] = window.notes;
    jsonData["note"] = $("#newPurchaseOrderForm #poNotes").val();
    var note = $("#newPurchaseOrderForm #poNotes").val();
    jsonData['tracking'] = [];

    var postatus = $('#po_status').val();
    var photos = $('#photos').val();
    var remaining = $('#remaining').val();

    $('.poTracking').each(function () {
        var tracking_json = {};

        tracking_json['track'] = $(this).val();
        if ($(this).attr("id") == "undefined") {
            tracking_json['id'] = "";
        } else {
            tracking_json['id'] = $(this).attr("id");
        }
        
        jsonData['tracking'].push(tracking_json);
    });

    console.log(jsonData['tracking']);

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/updatepurchaseorder/' + id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        console.log(data);

        if (data.responseText == "Success") {

            if (note != "") {
                $("#EditPurhaseOrderNote[data-id='" + id + "']").find(".redDotNote").addClass("redDOtElement");
            } else {
                $("#EditPurhaseOrderNote[data-id='" + id + "']").find(".redDotNote").removeClass("redDOtElement");
            }
            //alertBox('poAlertMsg', 'green', 'Purchase order is updated');
            var fileLength = $("#poUploadImage")[0].files.length;
            if (fileLength != 0) {

                var files = $("#poUploadImage")[0].files;

                var formData = new FormData();

                formData.append('po_id', id);

                for (var i = 0; i != files.length; i++) {
                    formData.append("purchaseOrderDocuments", files[i]);
                }

                $.ajax({
                    url: "/purchaseorders/purchaseorder_doc",
                    type: 'POST',
                    data: formData,
                    dataType: 'json',
                    processData: false,
                    contentType: false,
                }).always(function (data) {
                    console.log(data);
                    if (data == "0") {
                        console.log("Exception Error");
                        alertBox('poAlertMsg', 'red', 'Purchase order document not updated Exception Error');
                    } else {
                        $("#EditPurhaseOrderDocument[data-id='" + id + "']").find(".redDotDoc").addClass("redDOtElement");
                        console.log(data.responseText);
                    }
                });
            }


            if (window.purchaseorderrownumber!= "" && window.purchaseorderrownumber >= 0) {

                if (postatus == 5) {
                    $('#purchaseorderlists').dataTable().fnUpdate([$("#newPurchaseOrderForm #poNumber").val(), orderdate, vendorname, $("#newPurchaseOrderForm #poTotalStyles").val(), photos, remaining, delivery_window, 0 + " Day", getPoStatusById(postatus, id), '<button type="button" class="btn btn-warning btn-sm checked-'+id+'" data-shipping="' + shipping + '" data-ID="' + id + '" id="EditPurhaseOrderCheckedIn">Checkin</button>', '<button type="button" id="EditPurhaseOrder" data-id="' + id + '" class="btn btn-light btn-sm">Edit</button>', '<a href="javascript:void(0)" data-ID="' + id + '" id="EditPurhaseOrderNote"><i class="fa fa-edit h3"></i></a>', '<a href="javascript:void(0)" data-ID="' + id + '" id="EditPurhaseOrderDocument"><i class="fa fa-upload h3"></i></a>', '<a href="javascript:void(0)">Claim</a>', '<a href="javascript:void(0)">Claim</a>'], window.purchaseorderrownumber);
                } else {
                    $('#purchaseorderlists').dataTable().fnUpdate([$("#newPurchaseOrderForm #poNumber").val(), orderdate, vendorname, $("#newPurchaseOrderForm #poTotalStyles").val(), photos, remaining, delivery_window, 0 + " Day", getPoStatusById(postatus, id), '<button type="button" class="btn btn-success btn-sm">Checked-In</button>', '<button type="button" id="EditPurhaseOrder" data-id="' + id + '" class="btn btn-light btn-sm">Edit</button>', '<a href="javascript:void(0)" data-ID="' + id + '" id="EditPurhaseOrderNote"><i class="fa fa-edit h3"></i></a>', '<a href="javascript:void(0)" data-ID="' + id + '" id="EditPurhaseOrderDocument"><i class="fa fa-upload h3"></i></a>', '<a href="javascript:void(0)">Claim</a>', '<a href="javascript:void(0)">Claim</a>'], window.purchaseorderrownumber);
                }

                window.purchaseorderrownumber = "";
            }

            $("#newPurchaseOrderForm").attr("data-currentid", "");
            $('#modalPurchaseOrder').modal('hide');
            alertBox('poAlertMsg', 'green', 'Purchase order updated successfully');
            if (window.purchaseorderrownumber != "" && window.purchaseorderrownumber >= 0) {
                $('#newPurchaseOrderForm')[0].reset();
            }
        } else {
            alertBox('poAlertMsg', 'red', 'Purchase order is not updated');
        }

    })
});


/*
Developer: Sajid Khan
Date: 7-7-19
Action: show popup of purchase order form with input empty fields
URL: 
Input: 
output: form input fields
*/
$(document).on('click', ".model_purchase_order", function () {

    $("#newPurchaseOrderForm .error").remove();

    $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").attr("id", "NewPurchaseOrderButton");
    $("#model_purchase_order #purchaseOrderModalLongTitle").text("Add New Purchase Order");
    $("#newPurchaseOrderForm input, #newPurchaseOrderForm #poNotes").val("");
    $(".po_doc_section").empty().html();
    $("#newPurchaseOrderForm").attr("data-currentid", "");
    $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").html("Add");
    $('.po_section').addClass('d-none');

    $('#view_adjustment,#view_discount, #wrapper_tracking, .po_doc_section').empty().html("");
    $('.poTracking, #poNotes').val("");
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: dynamic tracking add more input field
URL:
Input:
output: form input fields
*/
$(document).on('click', ".add_tracking", function () {
    //Append a new row of code to the "#items" div
    $("#wrapper_tracking").append('<div class="tracking_add_more_box"><input type="text" class="form-control d-inline-block poTracking" name="poTracking[]" style="width: 90%"> <a href="#" class="h4 red_color remove_tracking">-</a></div>');
});

$(document).on('click', ".add_checkin_tracking", function () {
    //Append a new row of code to the "#items" div
    $("#wrapper_checkin_tracking").append('<div class="tracking_add_more_box"><input type="text" class="form-control d-inline-block poTracking" name="poTracking[]" style="width: 90%"> <a href="#" class="h4 red_color remove_tracking">-</a></div>');
});


/*
Developer: Sajid Khan
Date: 7-7-19
Action: remove dynamic tracking field by id
URL: /purchaseorders/trackingdelete/id
Input: int id
output: boolean
*/
$(document).on('click', ".remove_tracking", function () {
    var track_id = $(this).parent().children().attr("id");
    var track_number = $(this).parent().children().attr("value");

    console.log(track_id + " - " + track_number);

    if (typeof track_id !== "undefined" && track_id) {

        var jsonData = {};
        jsonData["po_id"] = $("#newPurchaseOrderForm").data("currentid");

        $.ajax({
            url: '/purchaseorders/trackingdelete/' + track_id,
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false
        }).always(function (data) {
            console.log(data);
            if (data == true) {
                $(".poTracking#" + track_id).parent().remove();
            }
        });
    } else {
                $(this).parent().remove();
    }    
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: Get purchase order note by id
URL: /purchaseorders/getnote/id
Input: int id
output: dynamic object of purchase order note
*/
$(document).on("click", "#EditPurhaseOrderNote", function () {
    $("#note_form #po_notes").val("");
    var id = $(this).attr("data-id");
    $("#note_form").attr("data-noteid", id);
    $("#noteModalLongTitle").text("Notes ("+$(this).parents('tr').find('td:first-child').text()+")");
    $.ajax({
        url: '/purchaseorders/getnote/' + id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);
        
        //var note = data['notes'];
        if (data.length > 0) {
            note = data[0].note;
            
            $("#note_form").attr("data-noteid", id);
            $("#note_form #po_notes").val(note);
        }
        $(".poNoteAlertMsg").empty();
        $("#modaladdnote").modal("show");
    });
});


/*
Developer: Sajid Khan
Date: 7-7-19
Action: send to new purchase order note data
URL: /purchaseorders/notecreate
Input: note data
output: string of purchase order note
*/
$(document).on("click", "#note_submit", function () {

    var _self = $(this);

    /*if ($("#po_notes").val() == "") {
        $(".poNoteAlertMsg").css("color", "red").text("Please fill empty field.");
        return false;
    }*/

    _self.attr("disabled", true);
    var jsonData = {};

    jsonData["po_id"] = $("#note_form").attr("data-noteid");
    jsonData["po_notes"] = $("#po_notes").val();

    console.log(jsonData);
    var poid = $("#note_form").attr("data-noteid");
    var note = $("#po_notes").val();
    
    $.ajax({
        url: '/purchaseorders/notecreate',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        if (data == "0") {
            _self.attr("disabled", false);
            alertBox('poAlertMsg', 'red', 'Purchase order note not updated');
        } else {
            _self.attr("disabled", false);
            $("#modaladdnote").modal("hide");
            alertBox('poAlertMsg', 'green', 'Purchase order note updated successfully.');
            if (note != "") {
                $("#EditPurhaseOrderNote[data-id='" + poid + "']").find(".redDotNote").addClass("redDOtElement");
            } else {
                $("#EditPurhaseOrderNote[data-id='" + poid + "']").find(".redDotNote").removeClass("redDOtElement");
            }
        }
        console.log(data);
       
    });
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: Get purchase order documents data by id
URL: /purchaseorders/getdocument/id
Input: int id
output:string of purchase order documents
*/

$(document).on("click", "#EditPurhaseOrderDocument", function () {
    $(".poDocAlertMsg").text("");
    $('#document_form')[0].reset();
    $("#document_form #po_document").val("");
    $("#document_form").attr("data-documentid", "");
    var id = $(this).attr("data-id");
    $("#document_form").attr("data-documentid", id);
    $("#documentModalLongTitle").text("Document (" + $(this).parents('tr').find('td:first-child').text()+")");

    $.ajax({
        url: '/purchaseorders/getdocument/' + id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);

        var docs = data['documents'];
        $(".po_doc_section").empty();
        if (docs.length > 0) {

            $(docs).each(function (e, i) {
                $(".po_doc_section").append("<a href='uploads/"+i.url+"' target='_blank' class='documentsLink' data-docid=" + i.doc_id +" data-val=" + i.url +">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
            });

            $(".po_doc_section").removeClass('d-none');

        } else {
            $(".po_doc_section").addClass('d-none');
        }
        $(".poDocAlertMsg").empty();
        $("#modaladddocument").modal("show");        
    });
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: send to new purchase order document data
URL: /purchaseorders/purchaseorder_doc
Input: document data
output: string of purchase order document
*/
$(document).on("click", "#document_submit", function () {
    var _self = $(this);
    $(".poDocAlertMsg").text("");
    if ($('#poUploadImages').val() == "") {
        $(".poDocAlertMsg").css("color", "red").text("Please upload files.");
        return false;
    }

    _self.attr("disabled", true);

    var fileLength = $("#poUploadImages")[0].files.length;
    if (fileLength != 0) {
        var files = $("#poUploadImages")[0].files;
        var poid = $('#document_form').attr("data-documentid");
        var formData = new FormData();
        formData.append('po_id', poid);

        for (var i = 0; i != files.length; i++) {
            formData.append("purchaseOrderDocuments", files[i]);
        }

        $.ajax({
            url: "/purchaseorders/purchaseorder_doc",
            type: 'POST',
            data: formData,
            dataType: 'json',
            processData: false,
            contentType: false,
        }).always(function (data) {
            console.log(data);
            if (data == "0") {
                _self.attr("disabled", false);
                alertBox('poAlertMsg', 'red', 'Purchase order document not updated');
              console.log("Exception Error");
            } else {
                console.log(data.responseText);
                _self.attr("disabled", false);
                if (data.responseText.indexOf('File Already') > -1) {
                    $(".poDocAlertMsg").css("color", "red").text(data.responseText);
                } else {    
                    $("#EditPurhaseOrderDocument[data-id='" + poid + "']").find(".redDotDoc").addClass("redDOtElement");
                    $("#modaladddocument").modal("hide");
                    alertBox('poAlertMsg', 'green', 'Purchase order document updated successfully.');
                }


               
            }
        });
    }
});


/*
Developer: Sajid Khan
Date: 7-7-19
Action: Delete purchase order data
URL: /purchaseorders/delete/id
Input: int id
output: string of purchase order
*/
$(document).on('click', "#poDelete", function () {

    var id = $("#newPurchaseOrderForm").data("currentid");

    confirmationAlertBox("Purchase Order Delete", "Are you sure that you want to delete this record?", function (result) {

        if (result == "yes") {
            $.ajax({
                url: '/purchaseorders/delete/' + id,
                dataType: 'json',
                type: 'POST',
                contentType: 'application/json',
            }).always(function (data) {
                console.log(data);
                if (data.responseText == "Success") {
                    var table = $('#purchaseorderlists').DataTable();
                    table.row(window.purchaseorderrownumber).remove().draw(false);
                    $("#modalPurchaseOrder").modal("hide");
                    alertBox('poAlertMsg', 'green', 'Purchase order deleted successfully.');

                    if (window.location.href.indexOf('PurchaseOrders/Single') > -1) {
                        window.location = location.protocol + "//" + location.host;
                    }
                } else {
                    alertBox('poAlertMsg', 'red', 'Purchase order not deleted.');
                }

            });
        }
    }) 
        
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: Get purchase order data by id
URL: 
Input: int id
output: dynamic object of purchase order data
*/
function purchaseOrderData(data) {
    
    var data = data;
       var podata = data['purchase_order'];
        if (podata.length > 0) {
            podata = data['purchase_order'][0];

            var startDate = timeToDateConvert(podata.delivery_window_start);
            var endDate = timeToDateConvert(podata.delivery_window_end);
            $("#newPurchaseOrderForm #poDelieveryRange").daterangepicker({
                startDate: startDate, // after open picker you'll see this dates as picked
                endDate: endDate,
                locale: {
                    format: 'M/D/YYYY',
                }
            }, function (start, end, label) {
                //what to do after change
            }).val(startDate + " - " + endDate); 

            $("#newPurchaseOrderForm #poVendor").val(data["vendor"][0].vendor_name);
            $("#newPurchaseOrderForm #poVendor").attr("data-val", data["vendor"][0].vendor_id);
            $("#newPurchaseOrderForm #poNumber").val(podata.vendor_po_number);
            $("#newPurchaseOrderForm #poTotalStyles").val(podata.total_styles);
            $("#newPurchaseOrderForm #poInvoiceNumber").val(podata.vendor_invoice_number);
            $("#newPurchaseOrderForm #poTotalQuantity").val(podata.total_quantity);
            $("#newPurchaseOrderForm #poOrderNumber").val(podata.vendor_order_number);
            $("#newPurchaseOrderForm #poSubtotal").val(podata.subtotal);
            $("#newPurchaseOrderForm #poOrderDate").val(timeToDateConvert(podata.order_date));
            $("#newPurchaseOrderForm #poShipping").val(podata.shipping);
            
            var it = data.Items.LineItemDetails;
            if (it.length > 0) {
                jQuery.each(it, function (i, dataNew) {
                    if (dataNew.Quantity >0) {
                        $("#itemsTable").append("<tr>");
                        //$("#itemsTable").append("<td width = '60' > <img src=" + dataNew.product_vendor_image + " width='50' /></td>");
                        $("#itemsTable").append("<td width='60'><img src='/images/dress-clipart.jpg' width='50' /></td>");
                        $("#itemsTable").append("<td class='h6'>" + dataNew.product_name + " (DP007) in " + dataNew.vendor_color_name + " - " + dataNew.sku + "</td>");
                        $("#itemsTable").append("<td><a href='#' class='h6 text-success' id='EditItemButton' data-poid=" + data["purchase_order"][0].po_id + " data-ponumber=" + data["purchase_order"][0].vendor_po_number + " data-vendorid=" + data["vendor"][0].vendor_id + " data-proid=" + dataNew.product_id+">Edit Style</a></td>");
                        $("#itemsTable").append("<td><a href='/Product/EditAttributes/" + dataNew.product_id +"'"+ " class='h6 text-primary'>Edit Attributes</a></td>");
                        $("#itemsTable").append("<td><a href='#' class='h6 text-danger'>Remove</a></td>");
                        $("#itemsTable").append("</tr>");
                    }
                });
            }
            $("#newPurchaseOrderForm #po_status").val(podata.po_status);
            $("#newPurchaseOrderForm #photos").val(podata.photos);
            $("#newPurchaseOrderForm #remaining").val(podata.remaining);
        }
        window.notes = "";
        var note = data['notes'];
        if (note.length > 0) {
            note = data['notes'][0].note;
            window.notes = note;
            $("#newPurchaseOrderForm #poNotes").val(note);
        }

        var docs = data['documents'];
        $(".po_doc_section").empty();
        if (docs.length > 0) {

            $(docs).each(function (e, i) {
                $(".po_doc_section").append("<a href='uploads/" + i.url +"' target='_blank' class='documentsLink' data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url +" <span class='podeleteImage'>×</span></a> <br>");
            });

            $(".po_doc_section").removeClass('d-none');
            
        } else {
            $(".po_doc_section").addClass('d-none');
        }

        $(".poTracking").val("");
        $("#wrapper_tracking").empty().html("");

        var track = data['tracking'];
        if (track.length > 0) {
            $(track).each(function (e, i) {
                if (e == 0) {
                    $(".poTracking").val(track[e].tracking_number);
                    $(".poTracking").attr("id",track[e].po_tracking_id);
                } else {
                    $("#wrapper_tracking").append('<div class="tracking_add_more_box"><input type="text" class="form-control d-inline-block poTracking" name="poTracking[]" id="'+track[e].po_tracking_id+'" value="' + track[e].tracking_number + '" style="width: 90%"> <a href="#" class="h4 red_color remove_tracking">-</a></div>');
                }
            });
        }

        window.adjustment = "";
        var ledger = data['ledger'];
        if (ledger.length > 0) {

            $("#view_adjustment").empty();
            $(ledger).each(function (e, i) {

                $('#ledger_form')[0].reset();
                $('#modaladdinvoice').modal('hide');

                console.log(e + " -- " + i.po_id + " - " + i.credit + " - " + i.debit + " - " + i.description);
                $("#view_adjustment").append("Adjustment -- Credit - " + i.credit + " Debit- " + i.debit + " - " + i.description + " <br>");

                var jsonData = {};
                jsonData["transaction_id"] = i.transaction_id;
                jsonData["po_id"] = i.po_id;
                jsonData["credit"] = i.credit;
                jsonData["debit"] = i.debit;
                jsonData["description"] = i.description;
                window.adjustment = jsonData;

            })
        }

        window.discount = "";
        var discount = data['discount'];        
        if (discount.length > 0) {

            $("#view_discount").empty();
            $(discount).each(function (e, i) {

                $('#discount_form')[0].reset();
                $('#modaladddiscount').modal('hide');

                console.log(e + " -- " + i.po_id + " - " + i.discount_percentage + " - " + i.discount_note + " - " + i.completed_status);
                $("#view_discount").append("Discount  -- " + i.discount_percentage + " - " + i.discount_note + " - " + i.completed_status);

                var jsonData = {};
                jsonData["po_discount_id"] = i.po_discount_id;
                jsonData["po_id"] = i.po_id;
                jsonData["discount_percentage"] = i.discount_percentage;
                jsonData["discount_note"] = i.discount_note;
                jsonData["completed_status"] = i.completed_status;
                window.discount = jsonData;

            })
        }

}

/*
Developer: Sajid Khan
Date: 7-7-19
Action: Get single page purchase order data by id
URL:
Input: int id
output: dynamic object of purchase order data
*/
function getSinglePurchaseOrder(id) {
    //$('.orderNumber').text(id)
    $('#newPurchaseOrderForm input,#newPurchaseOrderForm button,#AddItemButton').prop("disabled", "true");

    $("#newPurchaseOrderForm").attr('data-currentid',id)
    $.ajax({
        url: '/purchaseorders/details/' + id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);

        if (data.purchase_order[0].po_status == 4) {

            alert("This P.O does not exist.");
            window.location = location.protocol + "//" + location.host;
            return false;

        } else {

            $('.orderNumber').text(data.purchase_order[0].vendor_po_number);
            $('#AddItemButton').attr("data-poid", data.purchase_order[0].po_id).attr("data-ponumber", data.purchase_order[0].vendor_po_number).attr("data-vendorid", data.purchase_order[0].vendor_id);
            $('#EditItemButton').attr("data-poid", data.purchase_order[0].po_id).attr("data-ponumber", data.purchase_order[0].vendor_po_number).attr("data-vendorid", data.purchase_order[0].vendor_id);

            purchaseOrderData(data);
            $('#newPurchaseOrderForm input,#newPurchaseOrderForm button,#AddItemButton').removeAttr("disabled");

            if (data.purchase_order[0].po_status == 5) {
                $('.checkin_btn').html('<button type="button" class="btn btn-warning btn-sm checked-'+id+'" data-shipping="' + data.purchase_order[0].shipping + '" data-ID="' + data.purchase_order[0].po_id + '" id = "EditPurhaseOrderCheckedIn">Checkin</button> <button type="button" id="poDelete" class="btn btn-danger btn-sm">Delete this P.O</button >');
            } else {
                $('.checkin_btn').html('<button type="button" class="btn btn-success btn-sm">Checked-In</button>  <button type="button" id="poDelete" class="btn btn-danger btn-sm">Delete this P.O</button >');
            }

            $('.loading').addClass("d-none");
        }

        

    })

}

/*
Developer: Sajid Khan
Date: 7-13-19
Action: Add Invoice Model Top Heading
URL:
Input:
output: Add Invoice Adjustment Model Top Heading
*/
$(document).on('click', "#add_invoice_adjustment", function () {
    $("#invoice_ponumber").text(" : "+$('#newPurchaseOrderForm #poNumber').val());
});

/*
Developer: Sajid Khan
Date: 7-13-19
Action: Add Discount Model Top Heading
output: Add Discount Model Top Heading
*/
$(document).on('click', "#add_discount", function () {
    $("#discount_ponumber").text(" : " +$('#newPurchaseOrderForm #poNumber').val());
});

/*
Developer: Sajid Khan
Date: 7-13-19
Action: Delete Document or Image on click 
output: Boolean
*/
$(document).on('click', ".podeleteImage", function (e) {
    e.preventDefault();
    e.stopPropagation();    
    var _this = $(this);
    var docid = _this.parents('.documentsLink').attr('data-docid');
    var url = _this.parents('.documentsLink').attr('data-val');
    var poid = $('#document_form').attr("data-documentid");

    var jsonData = {};
    jsonData["doc_id"] = docid;
    jsonData["po_id"] = poid;
    jsonData["url"] = url;
    console.log(jsonData);

    $.ajax({
        url: "/purchaseorders/documentsdelete/" + docid,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        if (data.responseText != '0')
            _this.parents('.documentsLink').remove();

        if ($('#modaladddocument .po_doc_section a').length == 0) {
            $("#EditPurhaseOrderDocument[data-id='" + poid + "']").find(".redDotDoc").removeClass("redDOtElement");
        }
    });
});