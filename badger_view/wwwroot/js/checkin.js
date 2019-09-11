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
    $('.docTypeSection').remove();
    $('.modal-footer').find('button').attr('disabled', false)
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
    $('.loading').show();
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
            $(".po_doc_section").addClass('d-none');
            if (data.originalpo.length > 0) {

                $(data.originalpo).each(function (e, i) {
                    $(".po_doc_section").append("<a href='uploads/" + i.url + "' target='_blank' class='documentsLink' data-documentid=" + i.ref_id + " data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
                });

                $(".po_doc_section").removeClass('d-none');

            }

            if (data.shipmentinvoice.length > 0) {

                $(data.shipmentinvoice).each(function (e, i) {
                    $(".po_doc_section").append("<a href='uploads/" + i.url + "' target='_blank' class='documentsLink' data-documentid=" + i.ref_id + " data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
                });

                $(".po_doc_section").removeClass('d-none');

            }

            if (data.mainshipmentinvoice.length > 0) {

                $(data.mainshipmentinvoice).each(function (e, i) {
                    $(".po_doc_section").append("<a href='uploads/" + i.url + "' target='_blank' class='documentsLink' data-documentid=" + i.ref_id + " data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
                });

                $(".po_doc_section").removeClass('d-none');

            }

            if (data.others.length > 0) {

                $(data.others).each(function (e, i) {
                    $(".po_doc_section").append("<a href='uploads/" + i.url + "' target='_blank' class='documentsLink' data-documentid=" + i.ref_id + " data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
                });

                $(".po_doc_section").removeClass('d-none');

            }


            /*if (data.documents.length > 0) {

                $(data.documents).each(function (e, i) {
                    $(".po_doc_section").append("<a href='uploads/" + i.url + "' target='_blank' class='documentsLink' data-docid=" + i.doc_id + " data-val=" + i.url + ">" + i.url + " <span class='podeleteImage'>×</span></a> <br>");
                });

                $(".po_doc_section").removeClass('d-none');

            } else {
                $(".po_doc_section").addClass('d-none');
            }*/


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

        $('.loading').hide();
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
                formData.append('doc_type', $("#poUploadImages").attr('data-categorie'));
                for (var i = 0; i != files.length; i++) {
                    formData.append("purchaseOrderDocuments", files[i]);
                }

                $.ajax({
                    url: "/purchaseorders/purchaseorder_doc",
                    type: 'POST',
                    data: formData,                   
                    processData: false,
                    contentType: false,
                }).always(function (docdata) {
                    console.log(docdata);
                    if (docdata.indexOf('File Already') > -1) {
                        alertBox('poAlertMsg', 'red', docdata);
                    } else if (docdata == "0") {
                        console.log("Exception Error");
                        alertBox('poAlertMsg', 'red', 'Purchase order document Exception Error.');
                    } else {
                        $("#EditPurhaseOrderDocument[data-id='" + po_id + "']").find(".redDotDoc").addClass("redDOtElement");
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



function pagination(length, currentPage, itemsPerPage) {
    return {
        total: length,
        per_page: itemsPerPage,
        current_page: currentPage,
        last_page: Math.ceil(length / itemsPerPage),
        from: ((currentPage - 1) * itemsPerPage) + 1,
        to: currentPage * itemsPerPage
    };
};

//console.log(pagination(100,1,50))

function pageNumbers(total, current, itemsPerPage) {
    var count = Math.ceil(total / itemsPerPage);
    var shownPages = 3;
    var result = [];
    if (current > count - shownPages) {
        result.push(count - 2, count - 1, count);
    } else {
        result.push(current, current + 1, current + 2, '...', count);
    }
    return result;
}

//console.log(pageNumbers(7,1));

var total = $('.total_purchase_order_count').text();

if (total) {
    var currentPage = window.location.href + "/";
    var currentPageNumber = 1;

    if (window.location.href.indexOf('PurchaseOrdersCheckIn/Page') > -1) {
        currentPage = window.location.href.split('Page/')[0];
        currentPageNumber = window.location.href.split('Page/')[1];
    }

    var pagenumberData = pageNumbers(total, currentPageNumber, 50);

    var textData = "";

    if (currentPageNumber < 2) {
        textData += "<li class='page-item disabled'><a class='page-link' href='javascript:void(0)'>Previous</a></li>";
    } else {
        textData += "<li class='page-item'><a class='page-link' href=" + currentPage + "Page/" + (parseInt(currentPageNumber) - 1) + ">Previous</a></li>";
    }

    if (total < 51) {
        textData += "<li class='page-item active'><a class='page-link' href='javascript:void(0)'>1</a></li>";
    } else {
        for (i = 0; i < pagenumberData.length; i++) {

            if (pagenumberData[i] != 0) {
                if (pagenumberData[i] == currentPageNumber) {
                    textData += "<li class='page-item active'><a class='page-link' href='javascript:void(0)'>" + pagenumberData[i] + "</a></li>";
                }
                else if (pagenumberData[i] == '...') {
                    textData += "<li class='page-item disabled'><a class='page-link' href='javascript:void(0)'>...</a></li>";
                } else {
                    textData += "<li class='page-item'><a class='page-link' href=" + currentPage + "Page/" + pagenumberData[i] + ">" + pagenumberData[i] + "</a></li>";
                }
            }
           
        }
    }

    if (currentPageNumber == pagenumberData[pagenumberData.length - 1]) {
        textData += "<li class='page-item disabled'><a class='page-link' href='javascript:void(0)'>Next</a></li>";
    } else {
        textData += "<li class='page-item'><a class='page-link' href=" + currentPage + "Page/" + (parseInt(currentPageNumber) + 1) + ">Next</a></li>";
    }
    $('.custom_pagination .pagination').append(textData);
}



