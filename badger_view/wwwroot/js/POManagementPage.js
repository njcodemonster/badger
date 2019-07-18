/*
Developer: Sajid Khan
Date: 7-7-19
Action: Get Data of items by vendor id and show in dropdown and fields
Input: int purchase order id, int vendor id
Output: string of vendor products
*/
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
  //  alert("Please wait for the data to load");
   
    $.ajax({

        url: '/vendor/products/' + CurrentVendorId,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
       // var sku_family = data.vendorSkufamily;
        data = data.vendorProducts;
        $('#modaladdstylec #ExistingProductSelect option').remove();
        $('#modaladdstylec #ExistingProductSelect').append("<option id='-1'>Choose...</option>");
        var last_sku_family = "";

        $('#po_id').val(CurrentPOID);
        $('#vendor_id').val(CurrentVendorId);
        $(".vendorSkuBox_disabled").remove();
        $(".vendorSkuBox").remove();


        for (i = 0; i < data.length; i++) {

            $('#modaladdstylec #ExistingProductSelect').append("<option data-product_type='" + data[i].product_type_id + "' data-product_color='" + data[i].vendor_color_name + "' data-product_unit_cost='" + data[i].product_cost + "' data-product_retail='" + data[i].product_retail + "' data-Product_id='" + data[i].product_id + "'  data-skufamily='" + data[i].sku_family + "'  data-po_id='" + CurrentPOID +"'  >" + data[i].product_name + "</option>");
            last_sku_family = data[i].sku_family;
        }
        var vendorCode = last_sku_family.substring(0, 2);
        var sku_number = parseInt(last_sku_family.substr(2)) + 1;
        var new_sku = vendorCode + sku_number;
        var wrapper = $("#po_input_fields_wrap"); //Fields wrapper

        var sku_sizes = ["","XS", "S", "M", "L"];
        for (x = 1; x < 5; x++) {
            $(wrapper).append('<div class="pb-2  vendorSkuBox"> <input type="text" class="form-control d-inline w-25" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /> <input type="text" class="form-control d-inline w-25" name="styleSize" id="styleSize" placeholder="Size" value = "' + sku_sizes[x] + '" /> <input type="text" class="form-control d-inline w-25" name="styleSku" id="styleSku" placeholder="SKU" value = "' + new_sku + '-' + x +'" /> <input type="text" class="form-control d-inline w-25" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.

        }
       
        console.log(data);
    });
});


/*
Developer: Sajid Khan
Date: 7-7-19
Action: Select dropdown data show by id 
URL:  purchaseorders/lineitems/productid/purchaseorderid
Input: int product id, int purchase order id
Output: get data in fields
*/
$(document).on('change', '#modaladdstylec #ExistingProductSelect', function () {
    var SelectedProduct = $(this.options[this.selectedIndex]);
    SelectedProductID = SelectedProduct.data("product_id");
    SelectedProductTytle = $(this.options[this.selectedIndex]).val();
    $('#modaladdstylec #product_tytle').val(SelectedProductTytle);
    $('#modaladdstylec #product_unit_cost').val(SelectedProduct.data('product_unit_cost'));
    $('#modaladdstylec #product_color').val(SelectedProduct.data('product_color'));
    $('#modaladdstylec #product_retail').val(SelectedProduct.data('product_retail'));
    var SeletedPOID = SelectedProduct.data("po_id");
    $('#modaladdstylec StyleType option').removeAttr('selected');
    if (SelectedProduct.data('product_type') == 1) {
        $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=1]').val()).change()
    }
    else {
        $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=2]').val()).change()
    }

    $.ajax({
        url: '/purchaseorders/lineitems/' + SelectedProductID + '/' + SeletedPOID,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        //data = ;
        //console.log(data);

        var wrapper = $("#po_input_fields_wrap"); //Fields wrapper
        $(".vendorSkuBox").remove();
        $(".vendorSkuBox_disabled").remove();
        var sku_sizes = ["XS", "S", "M", "L"];
        for (x = 0; x < data.length; x++) {

            $(wrapper).append('<div class="pb-2 vendorSkuBox_disabled"> <input type="text" class="form-control d-inline w-25" name="csize[' + x + ']" placeholder="Vendor Size"  disabled /><input type="text" class="form-control d-inline w-25" name="csku[' + x + ']" placeholder="SKU" value = "' + sku_sizes[x] + '"  disabled /> <input type="text" class="form-control d-inline w-25" name="size[' + x + ']" placeholder="Size" value="' + data[x].sku + '"  disabled />  <input type="text" class="form-control d-inline w-25" name="cqty[' + x + ']" placeholder="Qty" value="' + data[x].line_item_ordered_quantity + '"  disabled />  '); // add input boxes.

        }


    });
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: it will show item note
Input: items note ids comma seperate
Output: item note data show by item note id
*/
function get_all_notes_by_ids() {
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
}

/*
Developer: Sajid Khan
Date: 7-5-19
Action: create new item note
Input:  string note
Output: item note id
*/
$(document).on('change', ".item_note", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
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
            alertInnerBox('message-' + po_id, 'red', 'Item note has error' + data.responseText);
        } else {
            alertInnerBox('message-' + po_id, 'green', 'Item note has been updated successfully');
        }
    });

});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: get item document by item doc id
Input: int item document id
Output: get item document data
*/
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

/*
Developer: Sajid Khan
Date: 7-5-19
Action: create new item document
Input: string file name
Output: item document id
*/
$(document).on("click", "#document_submit", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
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
                alertInnerBox('message-' + po_id, 'red', 'Item document has error' + data.responseText);
            } else {
                alertInnerBox('message-' + po_id, 'green', 'Item document has been updated successfully');
                console.log(data.responseText);
                $("#modaladddocument").modal("hide");
            }
        });
    }
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update item status by item id
Input: int item id
Output: string status
*/
$(document).on("change", ".item_status", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    $('.message-'+po_id).append('<div class="spinner-border text-info"></div>');
    //$(".message .spinner-border").removeClass("d-none");
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
        if (data.responseText == "Success") {
            alertInnerBox('message-' + po_id, 'green', 'Item status has been updated successfully');
        } else {
            alertInnerBox('message-' + po_id, 'red', 'Item status has error' + data.responseText);
        }

    });
});

$(".sku_weight").on("keydown", function (event) {
    return isNumber(event);
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update all same sku weight by sku id
Input: int sku id
Output: string sku
*/
$(document).on("change", ".sku_weight", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    var sku_id = $(this).attr('id');
    var sku_weight = $(this).val();
    var old_sku_weight = $(this).attr("data-weight");
    if (sku_weight == "") {
        return false;
    }
    console.log(sku_id + " -- " + sku_weight);
    var product_id = $(this).attr('data-productid');
    confirmationBox(product_id,"SKU Weight Update", "This will all same SKU weight updates, Do you want to continue?", function (result) {
        console.log(result)       
        if (result == "yes") {
            $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
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
                            $(this).attr('data-weight', sku_weight);
                        }
                    });

                    alertInnerBox('message-' + po_id, 'green', 'SKU weight has been updated successfully');

                } else {
                    alertInnerBox('message-' + po_id, 'red', 'SKU weight has error' + data.responseText);
                }
            });
        } else {
            $(".sku_weight").each(function () {
                if ($(this).attr('id') == sku_id) {
                    $(this).val(old_sku_weight);
                    $(this).attr('data-weight',old_sku_weight);
                }
            });

        }
    })
    
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update item barcode by item id
Input: int item id
Output: string item
*/
$(document).on("change", ".item_barcode", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    var item_id = $(this).attr('data-itemid');
    var barcode = $(this).val();

    $(this).removeClass('errorFeild');
    if (barcode.length < 8) {
        $(this).addClass('errorFeild');
        return false;
    }
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
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
        if (data.responseText == "Success") {
            alertInnerBox('message-' + po_id, 'green', 'Item barcode has been updated successfully');
        } else {
            alertInnerBox('message-' + po_id, 'red', 'Item barcode has error' + data.responseText);
        }
        
    });
})

/*
Developer: Sajid Khan
Date: 7-5-19
Action: status item changed by item id, it will update purchase order line item quantity
Input: int item id
Output: string item
*/
$(document).on("click", ".item_row_remove", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    var trdata = $(this);
    var item_id = $(this).attr('data-itemid');
    var polineitem = $(this).attr('data-polineitem');
    var quantity = $(this).attr('data-quantity');
    var item_status = 5;
    var product_id = $(this).attr('data-productid');
    confirmationBox(product_id, "Item Remove", "Do you want to remove this item?", function (result) {
        console.log(result)
        if (result == "yes") {

            $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
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

                    if (data.responseText == "Success") {
                        $(".item_row_remove").each(function () {
                            if ($(this).attr('data-polineitem') == polineitem) {
                                $(this).attr('data-quantity', quantity)
                            }
                        });
                        trdata.parents('tr').remove();

                        alertInnerBox('message-' + po_id, 'green', 'Item has been removed successfully');
                    } else {
                        alertInnerBox('message-' + po_id, 'red', 'Item has error' + data.responseText);
                    }


                });
            });
        }
    });
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update product, sku,item, purchase order line item, product attribute ect by sku id
Input: int product id, sku id, item id, po line item id etc
Output: string sku
*/
$(document).on("change", ".item_sku", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    var sku_id = $(this).attr('id');
    var sku = $(this).val();
    var old_sku = $(this).attr('data-sku');
    var polineitem_id = $(this).attr('data-polineitem');
    var product_id = $(this).attr('data-productid');
    var product_attribute_id = $(this).attr('data-productattributeid');
    var quantity = $(this).attr('data-quantity');

    $(this).removeClass('errorFeild');
    if (sku == "") {
        $(this).addClass('errorFeild');
        return false;
    }
    var product_id = $(this).attr('data-productid');
    confirmationBox(product_id,"SKU Update", "This will all same SKU updates, Do you want to continue?", function (result) {
        console.log(result)
        if (result == "yes") {
            $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
           
            var jsonData = {};
            jsonData["sku_id"] = sku_id;
            jsonData["sku"] = sku;
            jsonData["quantity"] = quantity;
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
                            $(this).attr('data-sku', sku);
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
                                if (data.responseText == "Success") {
                                    alertInnerBox('message-' + po_id, 'green', 'SKU has been updated successfully');
                                } else {
                                    alertInnerBox('message-' + po_id, 'red', 'SKU has error' + data.responseText);
                                }

                            });
                        }
                    });
                }

            });

        } else {

            $(".item_sku").each(function () {
                if ($(this).attr('id') == sku_id) {
                    $(this).val(old_sku);
                    $(this).attr('data-sku',old_sku)
                }
            });
        }
    });
});

$('.POList .card-header').click(function () {
    var thisPO = $(this);
    var POid = thisPO.attr("data-POId");
    if ($("#collapse_" + POid).is(":hidden")) {
        getPOdetail(POid);
    } else {
        $("#collapse_" + POid).html("");
    }
});

function getPOdetail(PO_id) {

    $("#collapse_" + PO_id).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>');

    $.ajax({
        url: "/PurchaseOrders/lineitemsdetails/" + PO_id,
        dataType: 'json',
        type: 'get',
        contentType: 'application/json',
        processData: false
    }).always(function (data) {
        //console.log(data);
        $("#collapse_" + PO_id).html("");
        $("#collapse_" + PO_id).html(data.responseText);
        get_all_notes_by_ids();
    });
}

$('.POListCheckIn .card-header .card-box').click(function () {
    var thisPO = $(this);
    var POid = thisPO.attr("data-POId");

    console.log($("#collapse_" + POid));
    console.log($("#collapse_" + POid).is(":visible"))
    console.log($("#collapse_" + POid).is(":hidden"))

    if ($("#collapse_" + POid).is(":hidden")) {
        getPurchaseOrdersItemdetails(POid);
        $("#collapse_" + POid).attr('data-colapse', true);
    } else if ($("#collapse_" + POid).attr('data-colapse')) {
        $("#collapse_" + POid).hide();
    } else {
        $("#collapse_" + POid).html("");
    }
});

function getPurchaseOrdersItemdetails(PO_id) {
    $("#collapse_" + PO_id).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>').show();

    $.ajax({
        url: "/PurchaseOrders/itemsdetails/" + PO_id,
        dataType: 'json',
        type: 'get',
        contentType: 'application/json',
        processData: false
    }).always(function (data) {
        //console.log(data);
        $("#collapse_" + PO_id).html("");
        $("#collapse_" + PO_id).html(data.responseText);
        get_all_notes_by_ids();

        $(".POListCheckIn .card .collapse").each(function () {
            var product_id = $(this).attr("id").replace("collapseOne", "-");
            var appendData = "";
            $(".item_sizes").each(function () {
                if ($(this).attr("data-orderproduct") == product_id) {
                    appendData += $(this).attr("data-size") + " (" + $(this).text() + ") ";
                }
            })
            $(".size" + product_id).text(appendData);
        });
    });
}

/*
Developer: Sajid Khan
Date: 7-16-19
Action: update item bagcode by item id
Input: int item id
Output: string item
*/
$(document).on("change", ".item_bagcode", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    var item_id = $(this).attr('data-itemid');
    var bagcode = $(this).val();

    $(this).removeClass('errorFeild');
    if (bagcode.length < 1) {
        $(this).addClass('errorFeild');
        return false;
    }
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
    var jsondata = $("input#" + item_id).val();
    var itemdata = JSON.parse(jsondata);
    var id = itemdata.item_id
    itemdata.bag_code = bagcode;
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
        if (data.responseText == "Success") {
            alertInnerBox('message-' + po_id, 'green', 'Item bag code has been updated successfully');
        } else {
            alertInnerBox('message-' + po_id, 'red', 'Item bag code has error' + data.responseText);
        }

    });
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update ra status by item id
Input: int item id
Output: string status
*/
$(document).on("change", ".item_ra_status", function () {
    var po_id = $(this).parents("tr").attr("data-prductid");
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
    //$(".message .spinner-border").removeClass("d-none");
    var item_id = $(this).attr('data-itemid');
    var ra_status = $(this).val();
    var jsondata = $("input#" + item_id).val();
    var itemdata = JSON.parse(jsondata);
    var id = itemdata.item_id
    itemdata.ra_status = parseInt(ra_status);
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
        if (data.responseText == "Success") {
            alertInnerBox('message-' + po_id, 'green', 'Ra status has been updated successfully');
        } else {
            alertInnerBox('message-' + po_id, 'red', 'Ra status has error' + data.responseText);
        }

    });
});

/*
Developer: Sajid Khan
Date: 7-18-19
Action: Onclick add weight button popup show on sku weight model
Input: 
Output: load sku weight data in model
*/
$(document).on("click", "#sku_weight", function () {
    $('.sku_weight_field').addClass("d-none");
    $('#weight_form input').val("");

    var productid = $(this).attr("data-productid");
    $('#weight_form').attr("data-productid", productid);

    var sku = $(this).attr("data-sku");
    var img_src = $(".img-"+productid).attr("src");
    console.log(sku + " -- " + img_src);

    $("#weight_form .weight_image").attr("src", img_src);
    $("#weight_form .weight_sku").text(sku);

    $(".table-data-" + productid + " tbody tr").each(function () {
        var weight = $(this).attr("data-weight");
        var size = $(this).attr("data-size");
        var skuid = $(this).attr("data-skuid");

        if (productid == $(this).attr("data-productid")) {
            console.log(size + " -- " + weight + " --- " + skuid)
            if (size.toLowerCase() == "x") {
                $("#weight_form #x_weight").parents(".x").removeClass("d-none");
                $("#weight_form #x_weight").val(weight);
                $("#weight_form #x_weight").attr("data-skuid",skuid);
            }

            if (size.toLowerCase() == "xs") {
                $("#weight_form #xs_weight").parents(".xs").removeClass("d-none");
                $("#weight_form #xs_weight").val(weight);
                $("#weight_form #xs_weight").attr("data-skuid", skuid);
            }

            if (size.toLowerCase() == "s") {
                $("#weight_form #s_weight").parents(".s").removeClass("d-none");
                $("#weight_form #s_weight").val(weight);
                $("#weight_form #s_weight").attr("data-skuid", skuid);
            }

            if (size.toLowerCase() == "m") {
                $("#weight_form #m_weight").parents(".m").removeClass("d-none");
                $("#weight_form #m_weight").val(weight);
                $("#weight_form #m_weight").attr("data-skuid", skuid);
            }

            if (size.toLowerCase() == "l") {
                $("#weight_form #l_weight").parents(".l").removeClass("d-none");
                $("#weight_form #l_weight").val(weight);
                $("#weight_form #l_weight").attr("data-skuid", skuid);
            }

        }
    });
    $('#modaladdweight').modal('show');
});

/*
Developer: Sajid Khan
Date: 7-18-19
Action: Onclick add weight button popup show on sku weight model
Input:
Output: load sku weight data in model
*/
$(document).on("click", "#weight_submit", function () {

    var productid = $('#weight_form').attr("data-productid");
    var result = false;
    var error = "";
    $('#weight_form input').each(function () {
        var sku_weight = $(this).val();
        var sku_id = $(this).attr("data-skuid");

        if ( (sku_id != 0 && sku_id != "") && (sku_weight != 0 && sku_weight != "") ) {

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
                    result = true;
                    $(".table-data-" + productid + " tbody tr").each(function () {

                        if ($(this).attr("data-skuid") == sku_id) {
                            $(this).attr("data-skuid", sku_id);
                            $(this).attr("data-weight", sku_weight);
                        }

                    });
                } else {
                    result = "error";
                    error = data.responseText;
                    console.log(error);
                }
            });           
        }
    });

   var  prevNowPlaying = setInterval(function () { 
            if (result) {
                $('#modaladdweight').modal('hide');
                alertInnerBox('message-' + productid, 'green', 'SKU weight has been updated successfully');
                clearInterval(prevNowPlaying);
            } else if (result == "error") {
                //$('#modaladdweight').modal('show');
                alertInnerBox('message-' + productid, 'red', 'SKU weight has error' + error);
                clearInterval(prevNowPlaying);
            }
    }, 1000);
   
});