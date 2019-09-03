$(document).on("keydown", "#item_barcode", function (e) {
    return isNumber(e);
});

/*
Developer: Sajid Khan
Date: 7-13-19
Action: Checkin button click
output: Checkin Model Show
*/
$(document).on('click', "#EditPurhaseOrderCheckedIn", function () {

    var producthtml = "";
    var po_number = $(this).parents("tr").children("td:first").text();
    var vendor = $(this).parents("tr").children("td:nth-child(3)").text();

    if (po_number == "") {
        po_number = $(".orderNumber").text();
    }
    if (vendor == "") {
        vendor = $("#poVendor").val();
    }
    console.log(po_number + " -- " + vendor);
    $("#checkinModalLongTitle").text("Purchase Order #" + po_number + " - " + vendor + " - Check-in");
    var poid = $(this).attr("data-ID");
    var shipping = $(this).attr("data-shipping");
    $("#checkin_form #poShipping").val(shipping);
    $('.loading').removeClass("d-none");
    $.ajax({
        url: '/purchaseorders/PurchaseOrderItemDetails/' + poid,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);
        if (data != "0") {

            if (data.itemsList.length > 0) {
                $(data.itemsList).each(function (e, i) {
                    console.log(i.product_name + " (" + i.small_sku + ")-" + i.size);
                    producthtml += "<div class='form-row align-items-center product_name_with_small_sku_size'><div class='form-group col-md-6' ><label>" + i.product_name + " (" + i.small_sku + ")-" + i.size + "</label></div><div class='form-group col-md-6'><input type='text' class='form-control' name='item_barcode' id='item_barcode' data-itemid=" + i.item_id + " value=" + i.barcode + " maxlength='8'></div></div>";
                });
            }

            $(".po_doc_section").empty();
            if (data.documents.length > 0) {

                $(data.documents).each(function (e, i) {
                    $(".po_doc_section").append("<a href='uploads/" + i.url + "' target='_blank' class='documentsLink' data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
                });

                $(".po_doc_section").removeClass('d-none');

            } else {
                $(".po_doc_section").addClass('d-none');
            }


            $(".poTracking").val("");
            $("#wrapper_checkin_tracking").empty().html("");
            if (data.tracking.length > 0) {
                $(data.tracking).each(function (e, i) {
                    if (e == 0) {
                        $(".poTracking").val(i.tracking_number);
                        $(".poTracking").attr("id", i.po_tracking_id);
                    } else {
                        $("#wrapper_checkin_tracking").append('<div class="tracking_add_more_box"><input type="text" class="form-control d-inline-block poTracking" name="poTracking[]" id="' + i.po_tracking_id + '" value="' + i.tracking_number + '" style="width: 90%"> <a href="#" class="h4 red_color remove_tracking">-</a></div>');
                    }
                });
            }

        }

        $('.loading').addClass("d-none");
        $(".wrapper_product").empty().html(producthtml);
        $("#checkin_form").attr("data-poid", poid);
        $("#modalcheckin").modal("show");
    });
});

/*
Developer: Sajid Khan
Date: 7-13-19
Action: Checkin button click submit form data to item and purchase order update with status
output: Success or failed alert show
*/
$(document).on('click', ".submit-check-in", function () {
    var po_id = $("#checkin_form").attr("data-poid");
    var shipping = $("#checkin_form #poShipping").val();

    console.log(po_id + " -- " + shipping);

    var jsonData = {};

    jsonData["shipping"] = shipping;

    jsonData['tracking'] = [];


    $(".poTracking").removeAttr("id");
    $(".poTracking").val("");
    $('#checkin_form .poTracking:visible').each(function () {
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

    jsonData['items_barcodes'] = [];

    $(".product_name_with_small_sku_size :input").each(function () {

        if ($(this).val().length == 8) {

            var jsonItemData = {
                "item_id": $(this).attr("data-itemid"),
                "barcode": $(this).val()
            };
            jsonData['items_barcodes'].push(jsonItemData);
        }

    })

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/updatepurchaseordercheckin/' + po_id,
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        console.log(data);

        if (data == "Success") {

            $('.postatus-' + po_id).text('Recieved');
            $('.checked-' + po_id).removeClass('btn-warning').addClass('btn-success').removeAttr('id').text('Checked-In');
            $('.removeRed-' + po_id).removeClass('text-danger font-weight-bold ');
            $('.removeRed-' + po_id + ' a').removeClass('text-danger font-weight-bold ');
            $('.days-' + po_id).text('0 Day');

            var fileLength = $("#checkin_form #poUploadImage")[0].files.length;
            if (fileLength != 0) {
                var files = $("#checkin_form #poUploadImage")[0].files;

                var formData = new FormData();
                formData.append('po_id', po_id);

                for (var i = 0; i != files.length; i++) {
                    formData.append("purchaseOrderDocuments", files[i]);
                }

                $.ajax({
                    url: "/purchaseorders/purchaseorder_doc",
                    type: 'POST',
                    data: formData,
                    
                    processData: false,
                    contentType: false,
                }).always(function (data) {
                    console.log(data);
                    if (data == "0") {
                        console.log("Exception Error");
                       // alertBox('poAlertMsg', 'red', 'Purchase order document not updated Exception Error');
                    } else {
                       // console.log(data.responseText);
                    }
                });
            }
            $('#modalcheckin').modal('hide');
            alertBox('poAlertMsg', 'green', 'Purchase order updated successfully');

            $('#checkin_form')[0].reset();
        } else {
            alertBox('poAlertMsg', 'red', 'Purchase order is not updated');
        }

    })


});


/*
Developer: Sajid Khan
Date: 7-13-19
Action: Checkin button click submit form data to item and purchase order update with status
output: Success or failed alert show
*/
$(document).on('click', ".add-check-in", function () {
    var po_id = $("#po_id").val();
    
    var jsonData = {};

    jsonData["shipping"] = 0;
    jsonData['tracking'] = [];
    jsonData['items_barcodes'] = [];
    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/updatepurchaseordercheckin/' + po_id,
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        console.log(data);

        if (data == "Success") {
            alertInnerBox('poAlertMsg', 'green', 'Purchase order updated successfully');
        } else {
           // alertInnerBox('poAlertMsg', 'red', 'Purchase order is not updated');
        }
        $('.poAlertMsg .alert').css({ 'width': '100%', 'margin-top': '-15px' })

    })


});