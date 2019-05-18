$(document).on('click', "#NewPurchaseOrderButton", function () {

    window.tas = $("#newPurchaseOrderForm input");
    return false;
    var newPurchaseOrderForm = $("#newPurchaseOrderForm input");
    var jsonData = {};

    /*jsonData["vendor_po_number"] = $(newPurchaseOrderForm[0]).val();
    jsonData["vendor_invoice_number"] = $(newPurchaseOrderForm[1]).val();
    jsonData["vendor_order_number"] = $(newPurchaseOrderForm[3]).val();
    jsonData["vendor_id"] = $(newPurchaseOrderForm[4]).val();
    jsonData["defected"] = $(newPurchaseOrderForm[5]).val();
    jsonData["good_condition"] = 2;
    jsonData["total_quantity"] = 1;
    jsonData["subtotal"] = $(newPurchaseOrderForm[5]).val()
    jsonData["shipping"] = $(newPurchaseOrderForm[5]).val()
    jsonData["delivery_window_start"] = $(newPurchaseOrderForm[5]).val()
    jsonData["delivery_window_end"] = $(newPurchaseOrderForm[5]).val()
    jsonData["po_status"] = $(newPurchaseOrderForm[5]).val()
    jsonData["po_discount_id"] = $(newPurchaseOrderForm[5]).val()
    jsonData["deleted"] = $(newPurchaseOrderForm[5]).val()
    jsonData["created_by"] = $(newPurchaseOrderForm[5]).val()
    jsonData["updated_by"] = $(newPurchaseOrderForm[5]).val()
    jsonData["order_date"] = $(newPurchaseOrderForm[5]).val()
    jsonData["created_at"] = $(newPurchaseOrderForm[5]).val()
    jsonData["updated_at"] = $(newPurchaseOrderForm[5]).val()*/
     
   // jsonData["vendor_since"] = newVendorForm.find('#vendorName').val();

    /*$.ajax({
        url: '/purchaseorders/newpurchaseorder',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) { console.log(data); });*/



});
