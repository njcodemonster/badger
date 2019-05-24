
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

            $('#openpo').DataTable().row.add([
                $("#newPurchaseOrderForm #poNumber").val(), orderdate, $("#newPurchaseOrderForm #poVendor option:selected").text()
                , 6, 5, 3, delivery_window, 0+" Day", 1, '<button type="button" class="btn btn-success btn-sm">Checked-in</button>', '<button type="button" class="btn btn-light btn-sm">Edit</button>', '<a href="#"><i class="fa fa-edit h3"></i></a>', '<a href="#"><i class="fa fa-upload h3"></i></a>', '<a href="#">Claim</a>', '<a href="#">Claim</a>'
            ]).draw();

            $('#modalPurchaseOrder').modal('hide'); 

            /*window.vendor_options = '';

            window.vendor_options = $("#poVendor > option").clone();

            $('#poVendor').empty().append(window.vendor_options);*/

            $('#newPurchaseOrderForm')[0].reset();
        }


    });



});


$(document).on('click', "#EditPurhaseOrder", function () {
    $("#modalPurchaseOrder #purchaseOrderModalLongTitle").text("Edit Purhase Order");
    $('#modalPurchaseOrder input').prop("disabled", "true");
    $('#modalPurchaseOrder').modal('show');
    var id = $(this).data("id");
    $.ajax({
        url: '/purchaseorders/details/' + id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',


    }).always(function (data) {

            console.log(data);

       /* $("#NewVendorButton,#EditVendorButton").attr("id", "EditVendorButton");
        $('#modalvendor input').removeAttr("disabled");
        var vendor = data.vendor;
        var addresses = data.addresses;
        var reps = data.reps;
        $("#modalvendor #vendorModalLongTitle").text("Edit Vendor:" + vendor.vendor_name);
        $('#vendorName').val(vendor.vendor_name);
        $('#vendorCorpName').val(vendor.corp_name);
        $('#vendorStatmentName').val(vendor.statement_name);
        $('#vendorCode').val(vendor.vendor_code);
        // $('#vendorourCustomerNumber').val(vendor.vendor_name);
        if (addresses.length > 0) {
            var add1 = addresses[0]
            $('#vendorStreetAdress').val(add1.vendor_street);
            $('#vendorUnitNumber').val(add1.vendor_suite_number);
            $('#vendorCity').val(add1.vendor_city);
            $('#vendorZip').val(add1.vendor_zip);
            $('#vendorState').val(add1.vendor_state);
        }
        if (reps.length > 0) {
            rep1 = reps[0];
            $('#vendorRepName').val(rep1.first_name);
            $('#vendorRepEmail').val(rep1.email);
            $('#vendorRepPhone11').val(rep1.phone1);
            $('#vendorRepPhone12').val(rep1.phone1);
            $('#vendorRepPhone13').val(rep1.phone1);
        }*/


    });

});