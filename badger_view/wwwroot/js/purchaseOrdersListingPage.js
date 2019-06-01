
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

$('#poOrderDate').datepicker();
$('#poDelieveryRange').daterangepicker();

$(document).on('click', "#NewPurchaseOrderButton", function () {

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
                    } else if (data == "File Already Exists") {
                        console.log(data);
                    } else {
                        console.log("File uploaded:" + data);
                    }
                });
            }
            $('#purchaseorderlists').DataTable().row.add([
                $("#newPurchaseOrderForm #poNumber").val(), orderdate, $("#newPurchaseOrderForm #poVendor option:selected").text()
                , $("#newPurchaseOrderForm #poTotalStyles").val(), 5, 3, delivery_window, 0 + " Day", 1, '<button type="button" class="btn btn-success btn-sm">Checked-in</button>', '<button type="button" id="EditPurhaseOrder" data-id="' + data +'" class="btn btn-light btn-sm">Edit</button>', '<a href="#"><i class="fa fa-edit h3"></i></a>', '<a href="#"><i class="fa fa-upload h3"></i></a>', '<a href="#">Claim</a>', '<a href="#">Claim</a>'
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
        date = "0" + date;
    }

    if (month < 10) {
        month = "0" + month;
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

        var podata = data[0];

       // var date = new Date(podata.order_date * 1000);

        $('select#poVendor option[value=' + podata.vendor_id+']').prop("selected", true);

        $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").attr("id", "EditPurchaseOrderButton");
        $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").html("Update");
        $('#modalPurchaseOrder input').removeAttr("disabled");

        var startDate = timeToDateConvert(podata.delivery_window_start);
          var endDate = timeToDateConvert(podata.delivery_window_end); 
        $("#newPurchaseOrderForm #poDelieveryRange").daterangepicker({
            startDate: startDate, // after open picker you'll see this dates as picked
            endDate: endDate,
            locale: {
                format: 'MM/DD/YYYY',
            }
        }, function (start, end, label) {
            //what to do after change
        }).val(startDate + " - " + endDate); 


        $("#newPurchaseOrderForm #poNumber").val(podata.vendor_po_number);
        $("#newPurchaseOrderForm #poTotalStyles").val(1);
        $("#newPurchaseOrderForm #poInvoiceNumber").val(podata.vendor_invoice_number);
        $("#newPurchaseOrderForm #poTotalQuantity").val(podata.total_quantity);
        $("#newPurchaseOrderForm #poOrderNumber").val(podata.vendor_order_number);
        $("#newPurchaseOrderForm #poSubtotal").val(podata.subtotal);
        $("#newPurchaseOrderForm #poOrderDate").val(timeToDateConvert(podata.order_date));
        $("#newPurchaseOrderForm #poShipping").val(podata.shipping);
        /*$("#newPurchaseOrderForm #poTracking").val(podata.);
        $("#newPurchaseOrderForm #poUploadImage").val(podata.);
        $("#newPurchaseOrderForm #poNotes").val(podata.);*/
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

    jsonData["vendor_po_number"] = $("#newPurchaseOrderForm #poNumber").val();
    jsonData["vendor_invoice_number"] = $("#newPurchaseOrderForm #poInvoiceNumber").val();
    jsonData["vendor_order_number"] = $("#newPurchaseOrderForm #poOrderNumber").val();
    jsonData["vendor_id"] = $("#newPurchaseOrderForm #poVendor").val();
    jsonData["defected"] = 1;
    jsonData["good_condition"] = 1;
    jsonData["total_quantity"] = $("#newPurchaseOrderForm #poTotalQuantity").val();
    jsonData["subtotal"] = $("#newPurchaseOrderForm #poSubtotal").val();
    jsonData["shipping"] = $("#newPurchaseOrderForm #poShipping").val();
    jsonData["delivery_window_start"] = delivery_window_start_seconds;
    jsonData["delivery_window_end"] = delivery_window_end_seconds;
    jsonData["po_status"] = 1;
    jsonData["po_discount_id"] = 1;
    jsonData["deleted"] = 0;
    jsonData["order_date"] = order_date_seconds;

    jsonData["created_by"] = 2;
    jsonData["updated_by"] = 1;

    jsonData["created_at"] = (new Date().getTime()) / 1000;
    jsonData["updated_at"] = (new Date().getTime()) / 1000;

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
            if (window.purchaseorderrownumber >= 0) {

                $('#purchaseorderlists').dataTable().fnUpdate([$("#newPurchaseOrderForm #poNumber").val(), orderdate, $("#newPurchaseOrderForm #poVendor option:selected").text(), 6, 5, 3, delivery_window, 0 + " Day", 1, '<button type="button" class="btn btn-success btn-sm">Checked-in</button>', '<button type="button" id="EditPurhaseOrder" data-id="'+id+'" class="btn btn-light btn-sm">Edit</button>', '<a href="#"><i class="fa fa-edit h3"></i></a>', '<a href="#"><i class="fa fa-upload h3"></i></a>', '<a href="#">Claim</a>', '<a href="#">Claim</a>'], window.purchaseorderrownumber);

                window.purchaseorderrownumber = "";
            }
            

            $("#newPurchaseOrderForm").attr("data-currentid", "");
            $('#modalPurchaseOrder').modal('hide');
            $('#newPurchaseOrderForm')[0].reset();
        }

    })
});

$(document).on('click', ".model_purchase_order", function () {
    $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").attr("id", "NewPurchaseOrderButton");
    $("#model_purchase_order #purchaseOrderModalLongTitle").text("Add New Purchase Order");
    $("#newPurchaseOrderForm input").val("");
    $("#newPurchaseOrderForm").attr("data-currentid", "");
    $("#NewPurchaseOrderButton,#EditPurchaseOrderButton").html("Add");
    $('.po_section').addClass('d-none');

});


$(document).on('click', ".add_tracking", function () {
    //Append a new row of code to the "#items" div
    $("#wrapper_tracking").append('<div class="tracking_add_more_box"><input type="text" class="form-control d-inline-block poTracking" name="poTracking[]" style="width: 90%"> <a href="#" class="h4 red_color remove_tracking">-</a></div>');
});


$(document).on('click', ".remove_tracking", function () {
    $(this).parent().remove();
});