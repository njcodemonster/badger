$(document).ready(function () {

    $("#poTotalQuantity,#poSubtotal,#poShipping").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    $("#poTotalStyles,#poOrderNumber").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if ((event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    $("#poNumber,#poInvoiceNumber").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^A-Za-z0-9]/gi, ''));
        if ((event.which < 48 || event.which > 57) && (event.which < 97 || event.which > 122) && (event.which < 65 || event.which > 90)) {
            event.preventDefault();
        }
    });

    $("#poOrderDate, #poDelieveryRange").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9- \/ ]/g, ''));
        if ((event.which != 32 || $(this).val().indexOf('/') != -1) && (event.which < 45 || event.which > 57)) {
            event.preventDefault();
        }
       
    });
})

var table = $('#purchaseorderlists').DataTable({ "aaSorting": [] });

window.purchaseorderrownumber = "";
$('#purchaseorderlists tbody').on('click', 'tr', function (e) {
    window.purchaseorderrownumber = table.row(this)[0][0];
});

window.vendor_options = '';
$(document).on('click', "#NewVendorButton", function () {
    var newVendorForm = $("#newVendorForm input");
    var jsonData = {};
    jsonData["vendor_name"] = $(newVendorForm[0]).val();
    jsonData["corp_name"] = $(newVendorForm[1]).val();
    jsonData["statement_name"] = $(newVendorForm[3]).val();
    jsonData["vendor_code"] = $(newVendorForm[4]).val();
    jsonData["vendor_street"] = $(newVendorForm[5]).val();
    jsonData["vendor_suite_number"] = $(newVendorForm[6]).val();
    jsonData["vendor_city"] = $(newVendorForm[7]).val();
    jsonData["vendor_zip"] = $(newVendorForm[8]).val();
    jsonData["vendor_state"] = $(newVendorForm[9]).val();
    jsonData["Rep_first_name"] = $(newVendorForm[10]).val();
    jsonData["Rep_email"] = $(newVendorForm[12]).val();
    jsonData["Rep_phone1"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    jsonData["Rep_phone2"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    $.ajax({

        url: '/vendor/newvendor',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {

        console.log(data);

        if (data > 0) {
            console.log("New Vender Added");

            $('#newPurchaseOrderForm #poVendor').append($("<option></option>")
                .attr("value", data)
                .text($('#newVendorForm #vendorName').val()));

            window.vendor_options = '';

            window.vendor_options = $("#poVendor > option").clone();

            $('#poVendor').empty().append(window.vendor_options);

            $('#modalvendor').modal('hide'); 
            $('#newVendorForm')[0].reset();
        }
    });
});


$(document).on('click', '.model_purchase_order', function () {

    if (window.vendor_options != '') {
        $('#poVendor').empty().append(window.vendor_options);
    }
    
});

$('#poOrderDate').datepicker({
    format: 'm/d/yyyy'
});

$('#poDelieveryRange').daterangepicker({
    locale: {
        format: 'M/D/YYYY'
    }
});

$(document).on('click', "#NewPurchaseOrderButton", function () {

    var errorNumber = 0;
    $(".error").remove();

    if ($("#newPurchaseOrderForm #poVendor").val() == "Choose...") {
        $('#poVendor').next().after('<br><span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poDelieveryRange").val().length < 1) {
        $('#poDelieveryRange').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poNumber").val().length < 1) {
        $('#poNumber').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poTotalStyles").val().length < 1) {
        $('#poTotalStyles').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poInvoiceNumber").val().length < 1) {
        $('#poInvoiceNumber').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poTotalQuantity").val().length < 1) {
        $('#poTotalQuantity').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poOrderNumber").val().length < 1) {
        $('#poOrderNumber').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poSubtotal").val().length < 1) {
        $('#poSubtotal').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poOrderDate").val().length < 1) {
        $('#poOrderDate').after('<span class="error">This field is required</span>');
        errorNumber++;
    }
    if ($("#newPurchaseOrderForm #poShipping").val().length < 1) {
        $('#poShipping').after('<span class="error">This field is required</span>');
        errorNumber++;
    }

    if (errorNumber > 0) {
        return false;
    }

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

    jsonData["vendor_po_delievery_range"] = $("#newPurchaseOrderForm #poDelieveryRange").val();
    jsonData["vendor_po_number"] = $("#newPurchaseOrderForm #poNumber").val();
    jsonData["vendor_invoice_number"] = $("#newPurchaseOrderForm #poInvoiceNumber").val();
    jsonData["vendor_order_number"] = $("#newPurchaseOrderForm #poOrderNumber").val();
    jsonData["vendor_id"] = $("#newPurchaseOrderForm #poVendor").val();
    jsonData["total_styles"] = $("#newPurchaseOrderForm #poTotalStyles").val();
    jsonData["total_quantity"] = $("#newPurchaseOrderForm #poTotalQuantity").val();
    jsonData["subtotal"] = $("#newPurchaseOrderForm #poSubtotal").val();
    jsonData["shipping"] = $("#newPurchaseOrderForm #poShipping").val();
    jsonData["po_status"] = 1;
    jsonData["order_date"] = $("#newPurchaseOrderForm #poOrderDate").val();
    jsonData["created_by"] = 2;

    jsonData["note"] = $("#newPurchaseOrderForm #poNotes").val();

    console.log(jsonData);

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
            alert('New row created - ' + data);

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
                    } else {
                        console.log(data.responseText);
                    }
                });
            }
            $('#purchaseorderlists').DataTable().row.add([
                $("#newPurchaseOrderForm #poNumber").val(), orderdate, $("#newPurchaseOrderForm #poVendor option:selected").text()
                , $("#newPurchaseOrderForm #poTotalStyles").val(), 5, 3, delivery_window, 0 + " Day", "Open", '<button type="button" class="btn btn-success btn-sm">Checked-in</button>', '<button type="button" id="EditPurhaseOrder" data-id="' + data + '" class="btn btn-light btn-sm">Edit</button>', '<a href="javascript:void(0)" data-ID="' + data + '" id="EditPurhaseOrderNote"><i class="fa fa-edit h3"></i></a>', '<a href="javascript:void(0)" data-ID="' + data +'" id="EditPurhaseOrderDocument"><i class="fa fa-upload h3"></i></a>', '<a href="javascript:void(0)">Claim</a>', '<a href="javascript:void(0)">Claim</a>'
            ]).draw();

            table.page('last').draw('page');

            $('#modalPurchaseOrder').modal('hide'); 

            /*window.vendor_options = '';

            window.vendor_options = $("#poVendor > option").clone();

            $('#poVendor').empty().append(window.vendor_options);*/

            $('#newPurchaseOrderForm')[0].reset();
        }
    });
});

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

    /*var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var year = a.getFullYear();
    var month = months[a.getMonth()];
    var date = a.getDate();
    var hour = a.getHours();
    var min = a.getMinutes();
    var sec = a.getSeconds();
    var time = date + ' ' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec;*/

    return month + '/' + date + '/' + year;
}


$(document).on('click', "#EditPurhaseOrder", function () {
    $(".error").remove();
    $('#view_adjustment,#view_discount, #wrapper_tracking,.po_doc_section').empty().html("");
    $('.poTracking, #poNotes').val("");

    $('.po_section').removeClass('d-none');

    $("#modalPurchaseOrder #purchaseOrderModalLongTitle").text("Edit Purhase Order");
    $('#modalPurchaseOrder input').prop("disabled", "true");
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

            $("#newPurchaseOrderForm #poVendor").val(podata.vendor_id);
            $("#newPurchaseOrderForm #poNumber").val(podata.vendor_po_number);
            $("#newPurchaseOrderForm #poTotalStyles").val(podata.total_styles);
            $("#newPurchaseOrderForm #poInvoiceNumber").val(podata.vendor_invoice_number);
            $("#newPurchaseOrderForm #poTotalQuantity").val(podata.total_quantity);
            $("#newPurchaseOrderForm #poOrderNumber").val(podata.vendor_order_number);
            $("#newPurchaseOrderForm #poSubtotal").val(podata.subtotal);
            $("#newPurchaseOrderForm #poOrderDate").val(timeToDateConvert(podata.order_date));
            $("#newPurchaseOrderForm #poShipping").val(podata.shipping);
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
                $(".po_doc_section").append("File "+(e + 1) + ": <a href="+i.url+">" + i.url+"</a> <br>");
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

        $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").attr("id", "EditPurchaseOrderButton");
        $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").html("Update");
        $('#modalPurchaseOrder input').removeAttr("disabled");
    });

});

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

            /*$("#ledger_form").attr("data-adjustment", i.transaction_id);

            if (i.credit > 0) {
                $('#ledger_adjustment option[value=credit]').attr('selected', 'selected');
                $("#ledger_amount").val(i.credit);
            } else {
                $('#ledger_adjustment option[value=debit]').attr('selected', 'selected');
                $("#ledger_amount").val(i.debit);
            }
            
            $("#ledger_note").val(i.description);*/
            
            window.adjustment = jsonData;

            //$("#ledger_submit").attr("id", "update_ledger_submit");
        })
    });

});

$(document).on('click', "#EditPurchaseOrderButton", function () {

    var id = $("#newPurchaseOrderForm").data("currentid");

    var jsonData = {};

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

    jsonData["vendor_po_delievery_range"] = $("#newPurchaseOrderForm #poDelieveryRange").val();
    jsonData["vendor_po_number"] = $("#newPurchaseOrderForm #poNumber").val();
    jsonData["vendor_invoice_number"] = $("#newPurchaseOrderForm #poInvoiceNumber").val();
    jsonData["vendor_order_number"] = $("#newPurchaseOrderForm #poOrderNumber").val();
    jsonData["vendor_id"] = $("#newPurchaseOrderForm #poVendor").val();
    jsonData["total_styles"] = $("#newPurchaseOrderForm #poTotalStyles").val();
    jsonData["total_quantity"] = $("#newPurchaseOrderForm #poTotalQuantity").val();
    jsonData["subtotal"] = $("#newPurchaseOrderForm #poSubtotal").val();
    jsonData["shipping"] = $("#newPurchaseOrderForm #poShipping").val();
    jsonData["po_status"] = 1;
    jsonData["order_date"] = $("#newPurchaseOrderForm #poOrderDate").val();
    jsonData["updated_by"] = 2;

    jsonData["old_note"] = window.notes;
    jsonData["note"] = $("#newPurchaseOrderForm #poNotes").val();

    jsonData['tracking'] = [];

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
                    } else {
                        console.log(data.responseText);
                    }
                });
            }


            if (window.purchaseorderrownumber >= 0) {

                $('#purchaseorderlists').dataTable().fnUpdate([$("#newPurchaseOrderForm #poNumber").val(), orderdate, $("#newPurchaseOrderForm #poVendor option:selected").text(), $("#newPurchaseOrderForm #poTotalStyles").val(), 5, 3, delivery_window, 0 + " Day", "Open", '<button type="button" class="btn btn-success btn-sm">Checked-in</button>', '<button type="button" id="EditPurhaseOrder" data-id="' + id + '" class="btn btn-light btn-sm">Edit</button>', '<a href="javascript:void(0)" data-ID="' + id + '" id="EditPurhaseOrderNote"><i class="fa fa-edit h3"></i></a>', '<a href="javascript:void(0)" data-ID="' + id +'" id="EditPurhaseOrderDocument"><i class="fa fa-upload h3"></i></a>', '<a href="javascript:void(0)">Claim</a>', '<a href="javascript:void(0)">Claim</a>'], window.purchaseorderrownumber);

                window.purchaseorderrownumber = "";
            }
            

            $("#newPurchaseOrderForm").attr("data-currentid", "");
            $('#modalPurchaseOrder').modal('hide');
            $('#newPurchaseOrderForm')[0].reset();
        }

    })
});

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


$(document).on('click', ".add_tracking", function () {
    //Append a new row of code to the "#items" div
    $("#wrapper_tracking").append('<div class="tracking_add_more_box"><input type="text" class="form-control d-inline-block poTracking" name="poTracking[]" style="width: 90%"> <a href="#" class="h4 red_color remove_tracking">-</a></div>');
});

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

$(document).on("click", "#EditPurhaseOrderNote", function () {
    $("#note_form #po_notes").val("");
    $("#note_form").attr("data-noteid", "");
    var id = $(this).attr("data-id");

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
        $("#modaladdnote").modal("show");
    });
});

$(document).on("click", "#note_submit", function () {

    var jsonData = {};

    jsonData["po_id"] = $("#note_form").attr("data-noteid");
    jsonData["po_notes"] = $("#po_notes").val();

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/notecreate',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        $("#modaladdnote").modal("hide");
    });
});

$(document).on("click", "#EditPurhaseOrderDocument", function () {
    $('#document_form')[0].reset();
    $("#document_form #po_document").val("");
    $("#document_form").attr("data-documentid", "");
    var id = $(this).attr("data-id");
    $("#document_form").attr("data-documentid", id);
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
                $(".po_doc_section").append("File " + (e + 1) + ": <a href=" + i.url + ">" + i.url + "</a> <br>");
            });

            $(".po_doc_section").removeClass('d-none');

        } else {
            $(".po_doc_section").addClass('d-none');
        }
        $("#modaladddocument").modal("show");        
    });
});

$(document).on("click", "#document_submit", function () {
    var fileLength = $("#poUploadImages")[0].files.length;
    if (fileLength != 0) {
        var files = $("#poUploadImages")[0].files;

        var formData = new FormData();
        formData.append('po_id', $('#document_form').attr("data-documentid"));

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
            } else {
                console.log(data.responseText);
                $("#modaladddocument").modal("hide");
            }
        });
    }
});


$(document).on('click', "#poDelete", function () {

    var id = $("#newPurchaseOrderForm").data("currentid");
    
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
            }
           
        });
});