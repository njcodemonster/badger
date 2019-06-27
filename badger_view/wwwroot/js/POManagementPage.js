$(document).on('click', "#AddItemButton", function () {
    var CurrentPOID = $(this).data("poid");
    var CurrentVendorId = $(this).data("vendorid");
    $('#modaladdstylec input').val("");
    $('#modaladdstylec #StyleSubType option').each(function () {
        if (this.innerText != "Choose..." && this.innerText != "...") {
            this.remove();
        }
    });

    $('#modaladdstylec').modal('show');
    alert("Please wait for the data to load");
   
    $.ajax({

        url: '/vendor/products/' + CurrentVendorId,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        
        $('#modaladdstylec #ExistingProductSelect option').remove();
        $('#modaladdstylec #ExistingProductSelect').append("<option id='-1'>Choose...</option>");
        for (i = 0; i < data.length; i++) {

            $('#modaladdstylec #ExistingProductSelect').append("<option data-product_type='" + data[i].product_type_id+"' data-product_color='" + data[i].vendor_color_name + "' data-product_unit_cost='" + data[i].product_cost + "' data-product_retail='" + data[i].product_retail+"' data-Product_id='" + data[i].product_id+"'>" + data[i].product_name + "</option>");
        }
       
        console.log(data);
    });
});
$(document).on('change', '#modaladdstylec #ExistingProductSelect', function () {
    var SelectedProduct = $(this.options[this.selectedIndex]);
    SelectedProductID = SelectedProduct.data("product_id");
    SelectedProductTytle = $(this.options[this.selectedIndex]).val();
    $('#modaladdstylec #product_tytle').val(SelectedProductTytle);
    $('#modaladdstylec #product_unit_cost').val(SelectedProduct.data('product_unit_cost'));
    $('#modaladdstylec #product_color').val(SelectedProduct.data('product_color'));
    $('#modaladdstylec #product_retail').val(SelectedProduct.data('product_retail'));
    $('#modaladdstylec StyleType option').removeAttr('selected');
    if (SelectedProduct.data('product_type') == 1) {
        $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=1]').val()).change()
    }
    else {
        $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=2]').val()).change()
    }

});

$(document).ready(function () {
    var itemids = "";
    $(".item_note").each(function () {
        itemids += $(this).attr('data-itemid') + ",";
    })

    if (itemids != "") {
        itemids = itemids.substring(0, itemids.length - 1);
        console.log(itemids);

        $.ajax({
            url: '/purchaseorders/getitemnotes/' + itemids,
            dataType: 'json',
            type: 'Get',
            contentType: 'application/json',
        }).always(function (data) {
            console.log(data);
            if (data.length > 0) {
                $(data).each(function (e, i) {
                    $(".item_note").each(function () {
                        if ($(this).attr('data-itemid') == i.ref_id) {
                            $(this).val(i.note);
                        }
                    });
                });
            }

        });
    }

});

$(document).on('change', ".item_note", function () {
    var jsonData = {};

    jsonData["item_id"] = $(this).attr('data-itemid');
    jsonData["item_note"] = $(this).val();

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/itemnotecreate',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        if (data == "0") {
            $(this).val("");
        }
    });

});


$(document).on("click", "#AddDocument", function () {
    $('#document_form')[0].reset();
    var id = $(this).attr("data-itemid");
    $("#document_form").attr("data-documentid", id);
    $.ajax({
        url: '/purchaseorders/getitemdocument/' + id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);

        $(".po_doc_section").empty();
        if (data.length > 0) {

            $(data).each(function (e, i) {
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
            url: "/purchaseorders/itemdocumentcreate",
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

$(document).on("change", ".item_status", function () {
    var item_id = $(this).attr('data-itemid');
    var item_status = $(this).val();
    var jsondata = $("input#" + item_id).val();
    var itemdata = JSON.parse(jsondata);
    var id = itemdata.item_id
    itemdata.item_status_id = parseInt(item_status);
    $("input#" + item_id).val(JSON.stringify(itemdata));

    console.log($("input#" + item_id).val());

    $.ajax({
        url: "/purchaseorders/itemupdate/" + id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(itemdata),
        processData: false
    }).always(function (data) {
        console.log(data);
    });
})

$(document).on("change", ".sku_weight", function () {

    var sku_id = $(this).attr('id');
    var sku_weight = $(this).val();

    console.log(sku_id + " -- " + sku_weight);

    var jsonData = {};
    jsonData["sku_id"] = sku_id;
    jsonData["weight"] = sku_weight;

    $.ajax({
        url: "/purchaseorders/skuweightupdate/" + sku_id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false
    }).always(function (data) {
        console.log(data);
        if (data.responseText == "Success") {
            $(".sku_weight").each(function () {
                if ($(this).attr('id') == sku_id) {
                    $(this).val(sku_weight);
                }
            });
        }
    });

});

$(document).on("change", ".item_barcode", function () {
    var item_id = $(this).attr('data-itemid');
    var barcode = $(this).val();
    var jsondata = $("input#" + item_id).val();
    var itemdata = JSON.parse(jsondata);
    var id = itemdata.item_id
    itemdata.barcode = barcode;
    $("input#" + item_id).val(JSON.stringify(itemdata));

    console.log($("input#" + item_id).val());

    $.ajax({
        url: "/purchaseorders/itemupdate/" + id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(itemdata),
        processData: false
    }).always(function (data) {
        console.log(data);
    });
})


$(document).on("click", ".item_row_remove", function () {
    var trdata = $(this);

    var item_id = $(this).attr('data-itemid');

    var polineitem = $(this).attr('data-polineitem');
    var quantity = $(this).attr('data-quantity');

    var item_status = 5;
    var jsondata = $("input#" + item_id).val();
    var itemdata = JSON.parse(jsondata);
    var id = itemdata.item_id
    itemdata.item_status_id = item_status;
    $("input#" + item_id).val(JSON.stringify(itemdata));

    console.log($("input#" + item_id).val());

    $.ajax({
        url: "/purchaseorders/itemupdate/" + id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(itemdata),
        processData: false
    }).always(function (data) {
        console.log(data);

        var jsonData = {};
        jsonData["line_item_id"] = polineitem;
        quantity = (quantity - 1);
        jsonData["line_item_ordered_quantity"] = quantity;

        $.ajax({
            url: "/purchaseorders/polineitemupdate/" + polineitem,
            dataType: 'json',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false
        }).always(function (data) {
            console.log(data);
            $(".item_row_remove").each(function () {
                if ($(this).attr('data-polineitem') == polineitem) {
                    $(this).attr('data-quantity', quantity)
                }
            });
            trdata.parents('tr').remove();
        });
    });
});

$(document).on("change", ".item_sku", function () {
    var sku_id = $(this).attr('id');
    var sku = $(this).val();
    var polineitem_id = $(this).attr('data-polineitem');
    var product_id = $(this).attr('data-productid');
    var product_attribute_id = $(this).attr('data-productattributeid');
    var jsonData = {};
    jsonData["sku_id"] = sku_id;
    jsonData["sku"] = sku;
    jsonData["line_item_id"] = polineitem_id;
    jsonData["product_id"] = product_id;
    jsonData["product_attribute_id"] = product_attribute_id;
    
    $.ajax({
        url: "/purchaseorders/skuupdate/" + sku_id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false
    }).always(function (data) {
        console.log(data);

        if (data.responseText == "Success") {

            $(".item_sku").each(function () {
                if ($(this).attr('id') == sku_id) {
                    $(this).val(sku);

                    var item_id = $(this).attr('data-itemid');

                    var jsondata = $("input#" + item_id).val();
                    var itemdata = JSON.parse(jsondata);
                    var id = itemdata.item_id
                    itemdata.sku = sku;
                    itemdata.sku_family = sku;
                    $("input#" + item_id).val(JSON.stringify(itemdata));

                    console.log($("input#" + item_id).val());

                    $.ajax({
                        url: "/purchaseorders/itemupdate/" + id,
                        dataType: 'json',
                        type: 'post',
                        contentType: 'application/json',
                        data: JSON.stringify(itemdata),
                        processData: false
                    }).always(function (data) {
                        console.log(data);
                    });
                }
            });
        }

    });
});
