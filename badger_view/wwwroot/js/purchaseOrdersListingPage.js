
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

        }


    });



});
