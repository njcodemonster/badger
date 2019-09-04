$(document).ready(function () {
    if (window.location.href.indexOf('PurchaseOrdersCheckIn/') > -1) {
        $('.collapsButton').click()
    }
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
    var po_id = $(this).parents("tr").attr("data-productid");
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
    var jsonData = {};

    jsonData["item_id"] = $(this).attr('data-itemid');
    jsonData["item_note"] = $(this).val();

    console.log(jsonData);

    $.ajax({
        url: '/purchaseorders/itemnotecreate',
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        if (data == "0") {
            $(this).val("");
           // alertInnerBox('message-' + po_id, 'red', 'Item note has error' + data.responseText);
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
    $(".poDocAlertMsg").text("");
    $('#document_form')[0].reset();
    var id = $(this).attr("data-itemid");
    var productid = $(this).attr("data-productid");
    $("#document_form").attr("data-documentid", id);
    $("#document_form").attr("data-productid", productid);
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
                $(".po_doc_section").append("<a href='../uploads/" + i.url +"' target='_blank' class='documentsLink' data-itemid="+id+" data-docid=" + i.doc_id +" data-val=" + i.url +">" + i.url + " <span class='podeleteImage'>×</span></a>");
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
    var po_id = $('#document_form').attr("data-productid");
    var itemid = $('#document_form').attr("data-documentid");
    
    $(".poDocAlertMsg").text("");
    if ($('#poUploadImages').val() == "") {
        $(".poDocAlertMsg").css("color", "red").text("Please upload files.");
        return false;
    }

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
            
            processData: false,
            contentType: false,
        }).always(function (data) {
            console.log(data);
            if (data == "0") {
                console.log("Exception Error");
                //alertInnerBox('message-' + po_id, 'red', 'Item document has error' + data.responseText);
            } else {
                if (data.indexOf('File Already') > -1) {
                    //$(".poDocAlertMsg").css("color", "red").text(data.responseText);
                    $('.message-' + po_id).empty().html("");
                } else {
                    alertInnerBox('message-' + po_id, 'green', 'Item document has been updated successfully');
                    $("#AddDocument[data-itemid='"+itemid+"']").find(".redDotDoc").addClass("redDOtElement");
                    $("#modaladddocument").modal("hide");
                }
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
    var result = false;
    var po_id = $(this).parents("tr").attr("data-productid");
    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');

    if ($(this).parents("td").find(".checkitemstatus").is(':checked')) {
        var checkdata = $(this).parents("td").find(".checkitemstatus").attr("data-status");
        var item_status = $(this).val();

        $('.item_status').each(function () {
            if ($(this).attr("data-status") == checkdata) {

                $(this).val(item_status);
                //$('.item_status option[value=' + item_status + ']').prop("selected", true);
                var item_id = $(this).attr('data-itemid');
                var jsondata = $("input#" + item_id).val();
                var itemdata = JSON.parse(jsondata);
                itemdata.item_status_id = item_status;
                $("input#" + item_id).val(JSON.stringify(itemdata));

                console.log($("input#" + item_id).val());

                $.ajax({
                    url: "/purchaseorders/itemupdate/" + item_id,
                    
                    type: 'post',
                    contentType: 'application/json',
                    data: JSON.stringify(itemdata),
                    processData: false
                }).always(function (data) {
                    console.log(data);
                    if (data == "Success") {
                        result = true;
                    } else {
                        result = "error";
                    }

                });
            }
        });

        var checkInterval = setInterval(function () {
            if (result) {
                alertInnerBox('message-' + po_id, 'green', 'Item status has been updated successfully');
                clearInterval(checkInterval);
            } else if (result == "error") {
                //alertInnerBox('message-' + po_id, 'red', 'Item status has error' + data.responseText);
                clearInterval(checkInterval);
            }
        }, 1000);

    } else {
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
            
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(itemdata),
            processData: false
        }).always(function (data) {
            console.log(data);
            if (data == "Success") {
                alertInnerBox('message-' + po_id, 'green', 'Item status has been updated successfully');
            } else {
                //alertInnerBox('message-' + po_id, 'red', 'Item status has error' + data.responseText);
            }

        });
    }
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
    var po_id = $(this).parents("tr").attr("data-productid");
    var sku_id = $(this).attr('id');
    var sku_weight = $(this).val();
    var old_sku_weight = $(this).attr("data-weight");
    if (sku_weight == "") {
        return false;
    }
    console.log(sku_id + " -- " + sku_weight);
    var product_id = $(this).attr('data-productid');
    confirmationBox(product_id, "SKU Weight Update", "This will all same SKU weight updates, Do you want to continue?", function (result) {
        console.log(result)
        if (result == "yes") {
            $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
            var jsonData = {};
            jsonData["sku_id"] = sku_id;
            jsonData["weight"] = sku_weight;

            $.ajax({
                url: "/purchaseorders/skuweightupdate/" + sku_id,
                
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify(jsonData),
                processData: false
            }).always(function (data) {
                console.log(data);
                if (data == "Success") {
                    $(".sku_weight").each(function () {
                        if ($(this).attr('id') == sku_id) {
                            $(this).val(sku_weight);
                            $(this).attr('data-weight', sku_weight);
                        }
                    });

                    alertInnerBox('message-' + po_id, 'green', 'SKU weight has been updated successfully');

                } else {
                   // alertInnerBox('message-' + po_id, 'red', 'SKU weight has error' + data.responseText);
                }
            });
        } else {
            $(".sku_weight").each(function () {
                if ($(this).attr('id') == sku_id) {
                    $(this).val(old_sku_weight);
                    $(this).attr('data-weight', old_sku_weight);
                }
            });

        }
    });
    
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update item barcode by item id
Input: int item id
Output: string item
*/
$(document).on("keydown", ".item_barcode", function (e) {
    return isNumber(e);
});

$(document).on("change", ".item_barcode", function (e) {
    var _self = $(this);
    var po_id = $(this).parents("tr").attr("data-productid");

    var item_status = $(this).parents("tr").find(".item_status").val();

    if (item_status == 1) {
        $(this).val("");
        alertInnerBox('message-' + po_id, 'red', 'Change item status:');
        return false;
    }

    var item_id = $(this).attr('data-itemid');
    var old_barcode = $(this).attr('data-barcode');
    var size = $(this).parents("tr").attr('data-size');
    var barcode = $(this).val();
    var barcodeRanges = JSON.parse( $("#allBarcodeRanges").val());
    $(this).removeClass('errorFeild');
    if (barcode.length < 8) {
        $(this).addClass('errorFeild');
        return false;
    }

    if (old_barcode == barcode) {
        _self.removeClass('errorFeild');
        return false;
    }
    //checking barcode range 
    var hasMatch = false;
    var index = -1;
    for (var i = 0; i < barcodeRanges.length; i++) {

        var single = barcodeRanges[i];

        if (barcode >= single.barcode_from && barcode <= single.barcode_to ) {
            hasMatch = true;
            index = i;
            break;
        }
    }
    if (hasMatch) {
        var currentBarcode = barcodeRanges[index];
        if (currentBarcode.size.toLowerCase() != size.toLowerCase()) {
            //_self.addClass('errorFeild');
            //alertInnerBox('message-' + po_id, 'red', 'The barcode you entered matched a size "' + currentBarcode.size.toUpperCase() + '" but you are trying to use it for an "' + size.toUpperCase() + '". Are you sure you want to continue?  ');
            debugger;
            confirmationBox(po_id, "", "The barcode you entered matched a size " + currentBarcode.size.toUpperCase() + " but you are trying to use it for an " + size.toUpperCase() + ". Are you sure you want to continue? ", function (result) {
                console.log(result)
                if (result == "yes") {
                    _self.removeClass('errorFeild');
                    $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
                    $.ajax({
                        url: "/purchaseorders/checkbarcodeexist/" + barcode,
                        
                        type: 'Get',
                        contentType: 'application/json',
                    }).always(function (data) {
                        console.log(data);
                        if (data == true) {
                            _self.addClass('errorFeild');
                            alertInnerBox('message-' + po_id, 'red', 'Item barcode has already exist - ' + barcode);
                            return false;
                        } else {
                            var jsondata = $("input#" + item_id).val();
                            var itemdata = JSON.parse(jsondata);
                            var id = itemdata.item_id
                            itemdata.barcode = barcode;
                            $("input#" + item_id).val(JSON.stringify(itemdata));

                            console.log($("input#" + item_id).val());

                            $.ajax({
                                url: "/purchaseorders/itemupdate/" + id,
                                
                                type: 'post',
                                contentType: 'application/json',
                                data: JSON.stringify(itemdata),
                                processData: false
                            }).always(function (data) {
                                console.log(data);
                                if (data == "Success") {
                                    _self.attr('data-barcode', barcode);
                                    alertInnerBox('message-' + po_id, 'green', 'Item barcode has been updated successfully');
                                } else {
                                    //alertInnerBox('message-' + po_id, 'red', 'Item barcode has error' + data.responseText);
                                }

                            });
                        }
                    });
                }
                else {
                    $(this).text = old_barcode;
                    $(this).addClass('errorFeild');
                    return false;

                }
            });
        }
        else {
            _self.removeClass('errorFeild');
            $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
            $.ajax({
                url: "/purchaseorders/checkbarcodeexist/" + barcode,
                
                type: 'Get',
                contentType: 'application/json',
            }).always(function (data) {
                console.log(data);
                if (data == true) {
                    _self.addClass('errorFeild');
                    alertInnerBox('message-' + po_id, 'red', 'Item barcode has already exist - ' + barcode);
                    return false;
                } else {
                    var jsondata = $("input#" + item_id).val();
                    var itemdata = JSON.parse(jsondata);
                    var id = itemdata.item_id
                    itemdata.barcode = barcode;
                    $("input#" + item_id).val(JSON.stringify(itemdata));

                    console.log($("input#" + item_id).val());

                    $.ajax({
                        url: "/purchaseorders/itemupdate/" + id,
                        
                        type: 'post',
                        contentType: 'application/json',
                        data: JSON.stringify(itemdata),
                        processData: false
                    }).always(function (data) {
                        console.log(data);
                        if (data == "Success") {
                            _self.attr('data-barcode', barcode);
                            alertInnerBox('message-' + po_id, 'green', 'Item barcode has been updated successfully');
                        } else {
                           // alertInnerBox('message-' + po_id, 'red', 'Item barcode has error' + data.responseText);
                        }

                    });
                }
            });
        }
    }
    else {
        _self.removeClass('errorFeild');
        $('.message-' + po_id).append('<div class="spinner-border text-info"></div>');
        $.ajax({
            url: "/purchaseorders/checkbarcodeexist/" + barcode,
            
            type: 'Get',
            contentType: 'application/json',
        }).always(function (data) {
            console.log(data);
            if (data == true) {
                _self.addClass('errorFeild');
                alertInnerBox('message-' + po_id, 'red', 'Item barcode has already exist - ' + barcode);
                return false;
            } else {
                var jsondata = $("input#" + item_id).val();
                var itemdata = JSON.parse(jsondata);
                var id = itemdata.item_id
                itemdata.barcode = barcode;
                $("input#" + item_id).val(JSON.stringify(itemdata));

                console.log($("input#" + item_id).val());

                $.ajax({
                    url: "/purchaseorders/itemupdate/" + id,
                    
                    type: 'post',
                    contentType: 'application/json',
                    data: JSON.stringify(itemdata),
                    processData: false
                }).always(function (data) {
                    console.log(data);
                    if (data == "Success") {
                        _self.attr('data-barcode', barcode);
                        alertInnerBox('message-' + po_id, 'green', 'Item barcode has been updated successfully');
                    } else {
                       // alertInnerBox('message-' + po_id, 'red', 'Item barcode has error' + data.responseText);
                    }

                });
            }
        });
    }




   
})

/*
Developer: Sajid Khan
Date: 7-5-19
Action: status item changed by item id, it will update purchase order line item quantity
Input: int item id
Output: string item
*/
$(document).on("click", ".item_row_remove", function () {
    var trdata = $(this);
    var item_id = $(this).attr('data-itemid');
    var polineitem = $(this).attr('data-polineitem');
    var quantity = $(this).attr('data-quantity');
    var item_status = 5;
    var product_id = $(this).attr('data-productid');
    var poid = $(this).attr('data-poid');
    confirmationBox(product_id, "Item Remove", "Do you want to remove this item?", function (result) {
        console.log(result)
        if (result == "yes") {

            $('.message-' + product_id).append('<div class="spinner-border text-info"></div>');
            var jsondata = $("input#" + item_id).val();
            var itemdata = JSON.parse(jsondata);
            var id = itemdata.item_id
            itemdata.item_status_id = item_status;
            $("input#" + item_id).val(JSON.stringify(itemdata));

            console.log($("input#" + item_id).val());

            $.ajax({
                url: "/purchaseorders/itemupdate/" + id,
                
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
                    
                    type: 'post',
                    contentType: 'application/json',
                    data: JSON.stringify(jsonData),
                    processData: false
                }).always(function (data) {
                    console.log(data);

                    if (data == "Success") {
                        $(".item_row_remove").each(function () {
                            if ($(this).attr('data-polineitem') == polineitem) {
                                $(this).attr('data-quantity', quantity)
                            }
                        });
                        trdata.parents('tr.remove-' + item_id).remove();
                        $("#collapse_" + poid).html("");
                        getPurchaseOrdersItemdetails(poid);
                        alertInnerBox('message-' + product_id, 'green', 'Item has been removed successfully');
                    } else {
                        //alertInnerBox('message-' + product_id, 'red', 'Item has error' + data.responseText);
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
    var _self = $(this);
    var po_id = $(this).parents("tr").attr("data-productid");
    var sku_id = $(this).attr('id');
    var sku = $(this).val();
    var old_sku = $(this).attr('data-sku');
    var polineitem_id = $(this).attr('data-polineitem');
    var product_id = $(this).attr('data-productid');
    var product_attribute_id = $(this).attr('data-productattributeid');
    var quantity = $(this).attr('data-quantity');
    var product_id = $(this).attr('data-productid');

    $(this).removeClass('errorFeild');
    if (sku == "") {
        $(this).addClass('errorFeild');
        return false;
    }

    var patt = new RegExp('^([A-Z]{2})([0-9]{3})([-]{1})([0-9]{1})$');
    var value = sku.toUpperCase();
    if (patt.test(value) == false) {
        $(this).addClass('errorFeild');
        return false;
    } else {
        $(this).removeClass('errorFeild');
    }

    if (old_sku == sku) {
        _self.removeClass('errorFeild');
        return false;
    }


    $.ajax({
        url: "/purchaseorders/checkskuexist/" + sku,
        
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        console.log(data);
        if (data == true) {
            _self.addClass('errorFeild');
            alertInnerBox('message-' + po_id, 'red', 'SKU has already exist - ' + sku);
            //_self.val(old_sku);
            return false;
        } else {
            confirmationBox(product_id, "SKU Update", "This will all same SKU updates, Do you want to continue?", function (result) {
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
                        
                        type: 'post',
                        contentType: 'application/json',
                        data: JSON.stringify(jsonData),
                        processData: false
                    }).always(function (data) {
                        console.log(data);

                        if (data == "Success") {

                            $(".item_sku").each(function () {
                                if ($(this).attr('id') == sku_id) {
                                    $(this).val(sku);
                                    $(this).attr('data-sku', sku);
                                    var item_id = $(this).attr('data-itemid');

                                    var jsondata = $("input#" + item_id).val();
                                    var itemdata = JSON.parse(jsondata);
                                    var id = itemdata.item_id
                                    itemdata.sku = sku;
                                    $("input#" + item_id).val(JSON.stringify(itemdata));

                                    console.log($("input#" + item_id).val());

                                    $.ajax({
                                        url: "/purchaseorders/itemupdate/" + id,
                                        
                                        type: 'post',
                                        contentType: 'application/json',
                                        data: JSON.stringify(itemdata),
                                        processData: false
                                    }).always(function (data) {
                                        console.log(data);
                                        if (data == "Success") {
                                            alertInnerBox('message-' + po_id, 'green', 'SKU has been updated successfully');
                                        } else {
                                           // alertInnerBox('message-' + po_id, 'red', 'SKU has error' + data.responseText);
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
                            $(this).attr('data-sku', old_sku)
                        }
                    });
                }
            });   
        }

    });


    
   
});


/*
Developer: Sajid Khan
Date: 7-20-19
Action: Get All purchase orders data in po management page
Output: string html data
*/
$('.POList .card-header').click(function () {
    var thisPO = $(this);
    var POid = thisPO.attr("data-POId");
    if ($("#collapse_" + POid).is(":hidden")) {
        getPOdetail(POid);
    } else {
        $("#collapse_" + POid).html("");
    }
});

/*
Developer: Sajid Khan
Date: 7-20-19
Action: Get single Product Detail by purchase order id in po management page
Output: string html data
*/
function getPOdetail(PO_id) {

    $("#collapse_" + PO_id).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>');

    $.ajax({
        url: "/PurchaseOrders/lineitemsdetails/" + PO_id,
        
        type: 'get',
        contentType: 'application/json',
        processData: false
    }).always(function (data) {
        //console.log(data);
        $("#collapse_" + PO_id).html("");
        $("#collapse_" + PO_id).html(data);
        get_all_notes_by_ids();
    });
}


/*
Developer: Sajid Khan
Date: 7-20-19
Action: Get All purchase orders data in po management checkin page
Output: string html data
*/
$('.POListCheckIn .card-header .card-box').click(function () {
    var thisPO = $(this);
    var POid = thisPO.attr("data-POId");

    console.log($("#collapse_" + POid));
    console.log($("#collapse_" + POid).is(":visible")) 
    console.log($("#collapse_" + POid).is(":hidden"))

    if ($("#collapse_" + POid).is(":hidden")) {
        if ($("#collapse_" + POid).find('.card-body').length == 0) {
             getPurchaseOrdersItemdetails(POid);
        }
        $("#collapse_" + POid).show();
        $("#collapse_" + POid).attr('data-colapse', true);
    } else if ($("#collapse_" + POid).attr('data-colapse')) {
        $("#collapse_" + POid).hide();
    } else {
        $("#collapse_" + POid).html("");
    }
});

/*
Developer: Sajid Khan
Date: 7-20-19
Action: Get single Product Detail by purchase order id n po management checkin page
Output: string html data
*/
function getPurchaseOrdersItemdetails(PO_id) {
    $("#collapse_" + PO_id).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>').show();

    $.ajax({
        url: "/PurchaseOrders/itemsdetails/" + PO_id,
        
        type: 'get',
        contentType: 'application/json',
        processData: false
    }).always(function (data) {
        //console.log(data);
        $("#collapse_" + PO_id).html("");
        $("#collapse_" + PO_id).html(data);
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
Output: string string
*/
$(document).on("change", ".item_bagcode", function () {
    var po_id = $(this).parents("tr").attr("data-productid");
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
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(itemdata),
        processData: false
    }).always(function (data) {
        console.log(data);
        if (data == "Success") {
            alertInnerBox('message-' + po_id, 'green', 'Item bag code has been updated successfully');
        } else {
            //alertInnerBox('message-' + po_id, 'red', 'Item bag code has error' + data.responseText);
        }

    });
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: update ra status by item id
Output: string status
*/
$(document).on("change", ".item_ra_status", function () {
    var po_id = $(this).parents("tr").attr("data-productid");
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
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(itemdata),
        processData: false
    }).always(function (data) {
        console.log(data);
        if (data == "Success") {
            alertInnerBox('message-' + po_id, 'green', 'Ra status has been updated successfully');
        } else {
           // alertInnerBox('message-' + po_id, 'red', 'Ra status has error' + data.responseText);
        }

    });
});

/*
Developer: Sajid Khan
Date: 7-18-19
Action: Onclick add weight button popup show on sku weight model
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
    var html = '<div class="form-group col-md-3 pl-5"></div>';
    $(".table-data-" + productid + " tbody tr:visible").each(function () {
        var weight = $(this).attr("data-weight");
        var size = $(this).attr("data-size").toLowerCase();
        var skuid = $(this).attr("data-skuid");
        html += '<div class="form-group col-md-2 text-center sku_weight_field"><strong>' + size.toUpperCase() + '</strong><input data-skuid="' + skuid + '" type="text" value="' + weight + '" class="form-control" id="' + size + '_weight" name="' + size + '_weight"></div>';
    });
    $('.sku_weight_inputs').html(html);
    $('#modaladdweight').modal('show');
});

/*
Developer: Sajid Khan
Date: 7-18-19
Action: Update Weight Submit form data
Output: string 
*/
$(document).on("click", "#weight_submit", function () {

    var productid = $('#weight_form').attr("data-productid");
    var result = false;
    var error = "";
    var sku_weight = "";
    var sku_id = "";
    var sku = {};
    sku["skuData"] = [];
    $('#weight_form input').each(function () {
        sku_weight = $(this).val();
        sku_id = $(this).attr("data-skuid");

        if ( (sku_id != 0 || sku_id != "") && (sku_weight != 0 || sku_weight != "") ) {

            var jsonData = {};
            jsonData["sku_id"] = sku_id;
            jsonData["weight"] = sku_weight;
            sku["skuData"].push(jsonData);
        }
    });

    $.ajax({
        url: "/purchaseorders/MultipleskuWeightUpdate/",
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(sku),
        processData: false,
        async: false,
    }).always(function (data) {
        console.log(data);
        if (data == "Success") {
            result = true;
            for (i = 0; i < sku.skuData.length; i++) {
                    $(".table-data-" + productid + " tbody tr[data-skuid='"+sku.skuData[i].sku_id+"']").attr("data-weight", sku.skuData[i].weight);
                
            }
            $('#modaladdweight').modal('hide');
            alertInnerBox('message-' + productid, 'green', 'SKU weight has been updated successfully');
   
        } else {
            result = "error";
            //error = data.responseText;
            //console.log(error);
            //alertInnerBox('message-' + productid, 'red', 'SKU weight has error' + error);

        }
    }); 
   
});


/*
Developer: Sajid Khan
Date: 7-20-19
Action: Product Wash type status change 
output: Boolean
*/
$(document).on("change", ".wash_type_status", function () {
        var product_id = $(this).attr("data-productid");
        $('.message-' + product_id).append('<div class="spinner-border text-info"></div>');
        var wash_type_id = $(this).val();

    console.log(product_id + " -- " + wash_type_id);

            var jsonData = {};
            jsonData["product_id"] = product_id;
            jsonData["wash_type_id"] = wash_type_id;

            $.ajax({
                url: "/purchaseorders/productwashtypeupdate/" + product_id,
                
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify(jsonData),
                processData: false
            }).always(function (data) {
                console.log(data);
                if (data == "Success") {
                    alertInnerBox('message-' + product_id, 'green', 'Product wash type has been updated successfully');
                } else {
                   // alertInnerBox('message-' + product_id, 'red', 'Product wash type has an error' + data.responseText);
                }

            });
});

/*
Developer: Sajid Khan
Date: 7-19-19
Action: Delete Document or Image on click 
output: Boolean
*/
$(document).on('click', ".podeleteImage", function (e) {
    e.preventDefault();
    e.stopPropagation();

    var _this = $(this);
    var docid = _this.parents('.documentsLink').attr('data-docid');
    var itemid = _this.parents('.documentsLink').attr('data-itemid');
    var url = _this.parents('.documentsLink').attr('data-val');
    var poid = $('#document_form').attr("data-documentid");

    var jsonData = {};
    jsonData["doc_id"] = docid;
    jsonData["po_id"] = poid;
    jsonData["url"] = url;
    jsonData["item"] = "item";
    jsonData['itemid'] = itemid;
    console.log(jsonData);

    $.ajax({
        url: "/purchaseorders/documentsdelete/" + docid,
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,
    }).always(function (data) {
        console.log(data);
        if (data != '0')
            _this.parents('.documentsLink').remove();

        if ($('#modaladddocument .po_doc_section a').length == 0) {
            $("#AddDocument[data-itemid='"+itemid+"']").find(".redDotDoc").removeClass("redDOtElement");
        }
    });
});


/*
Developer: Sajid Khan
Date: 7-26-19
Action: When ra status zero than check box show and when checked checkbox than ra status dropdown show and hide checkbox
output: ra status dropdown show on checkbox checked and hide checkbox
*/
$(document).on('change', '.checkrastatus', function (e) {

    if ($(this).is(':checked')) {
        $(this).hide();
        $(this).parents("td").find("select.item_ra_status:first").removeClass("d-none");
    }
});


/*
Developer: Sajid Khan
Date: 08-07-19
Action: When checked checkbox than checked status dropdown status changed
Output: Alert notification success of failed etc
*/
$(document).on('change', '.checkitemstatus', function (e) {
    var checkdata = $(this).attr("data-status");
    if ($(this).is(':checked')) {
        $(".checkitemstatus").each(function () {
            if ($(this).attr("data-status") == checkdata) {
                $(this).prop('checked',true);
            }
        })
    } else {
        $(".checkitemstatus").each(function () {
            if ($(this).attr("data-status") == checkdata) {
                $(this).prop('checked',false);
            }
        })
    }

});
