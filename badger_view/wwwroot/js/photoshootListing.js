// Photoshoots/shootInProgress page accordian code here...
/*
Developer: Mohi
Date: 7-3-19
Action: Open accordian in Photoshoot inProgress page and call function getPhotoshootProducts();
Input: Null
Output: 
*/
$('.card-header').click(function () {
    $(".PhotoshootList .collapse").html("");
    var thisPhotoshoot = $(this);
    var photoshootId = thisPhotoshoot.attr("data-photoshootId");
    if ($("#collapse_" + photoshootId).is(":hidden")) {
        getPhotoshootProducts(photoshootId); 
    } else {
        $("#collapse_" + photoshootId).html("");
    }
});

/*
Developer: Mohi
Date: 7-3-19
Action: Get photoshoot products in photoshoot inProgress page
Input: PhotoshootId
Output: add products in photoshoot div 
*/
var PhotoshootInProgressProductsList;
function getPhotoshootProducts(photoshootId) {
    PhotoshootInProgressProductsList = null;
    var htmlLoader = $("#inprogressContainer_" + photoshootId + "  .loading-box-light ");
    htmlLoader.fadeIn(200);
        $.ajax({
            url: '/photoshoots/getPhotoshootInProgressProducts/' + photoshootId,
            type: 'GET',
            dataType: "html",
        }).always(function (data) {
            
            htmlLoader.fadeOut(200, function () {
                $("#collapse_" + photoshootId).html(data);
                PhotoshootInProgressProductsList = $("#collapse_" + photoshootId + " .datatable_js_ps ").DataTable({
                    columnDefs: [
                        { targets: 'no-sort', orderable: false }
                    ]
                });
            });

        })
}
/**********************************************************/


var datatable_js_ps = $('.datatable_js_ps').DataTable({
    "columnDefs": [
        { "orderable": false, "targets": 1 }
    ]
});


/*
Developer: Mohi
Date: 7-3-19
Action: Check All Checkboxes
Input: Null
Output: 
*/
function selectAllCheckbox() {

    $(".select_menu").hide();
    $(".unselect_menu").show();
    $(".select-box").prop("checked", true);

}

/*
Developer: Mohi
Date: 7-3-19
Action: Uncheck All Checkboxes
Input: Null
Output:
*/
function unselectAllCheckbox() {

    $(".select_menu").show();
    $(".unselect_menu").hide();
    $(".select-box").prop("checked", false);

}

/*
Developer: Mohi
Date: 7-3-19
Action: Open add new photoshoots modal on photoshoot not started
Input: Null
Output:
*/
function addNewPhotoshoot() {
    $("#modalAddNewPhotoshoot").modal('show');
}


/*
Developer: Mohi
Date: 7-3-19
Action: get all photoshoots & models and add in Add photoshoot modal and open modal
Input: photoshoot productID, photoshoot status 
Output: it will add photoshoots & models in add new photoshoot modal and open
*/
function AddToShootSingle(shootProductId,vendorId,poId, statusId) {
    window.photoShootRowId = shootProductId;
    var ProductName = $("#tableRow_" + shootProductId + " .productName").html();
    $("#AddToPhotoshootProductId").val(shootProductId);
    if (statusId == 1) {
        GetSmallestItem(shootProductId, vendorId, poId);
        $.ajax({
            url: '/photoshoots/getPhotoshootAndModels',
            
            type: 'GET',
            contentType: 'application/json',
            processData: false,

        }).always(function (data) {
            data = JSON.parse(data);
            var jsonPhotoshootsList = data.photoshootsList;
            var jsonPhotoshootsModelsList = data.photoshootsModelList;
            var allPhotoshootListHTML = '';

            $(jsonPhotoshootsList).each(function (i, val) {
                allPhotoshootListHTML += GetPhotoshootTr(val);
            });

            DestroyPhotoshootTable();
            $("#prev-photoshoots > tbody").html(allPhotoshootListHTML);
            InitializePhotoshootTable();
            // $(".allPhotoshootList").html(allPhotoshootListHTML);

            $("#AllModels").find('option').remove()
            $("#AllModels").append(new Option("Choose...", ""));
            $(jsonPhotoshootsModelsList).each(function (i, val) {
                $("#AllModels").append(new Option(val.model_name, val.model_id));
            });
            $("#modalAddNewPhotoshoot .modal-title").html("Add to photoshoot <small> (" + ProductName + ") </small>");

            $("#modalAddNewPhotoshoot").modal('show');
        }); 
    } 
}

function GetNameValueMapping(list) {
    var returnList = [];
    var obj = {};
    $.each(list, function (index, value) {
        
        if (value.name.indexOf('barcode') > -1)
            obj.barcode = value.value;
        else
            obj.item_id = value.value;

        if (obj.item_id && obj.barcode) {
            returnList.push(obj);
            obj = {};
        }
    });
    return returnList;
}

function GetSmallestItem(productIds) {
    $.ajax({
        url: '/photoshoots/smallestItem/',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(productIds),
        processData: false,
        success: function (responseList) {
            $("#sku-rows").empty();
            $.each(responseList, function (index, response) {

                var newIndex = index > 0 ? (index - 1) : index;
                var $itemRow = $("#item_wrapper_" + newIndex).clone();
                var $barcode = $itemRow.find("input[name='items[" + newIndex + "].barcode']");
                var $itemId = $itemRow.find("input[name='items[" + newIndex + "].item_id']");
                var $itemLabel = $itemRow.find("div[id='items[" + newIndex + "].sku_label']");

                $itemRow.attr("id", "item_wrapper_" + index);
                $barcode.attr("name", "items[" + index + "].barcode");
                $itemId.attr("name", "items[" + index + "].item_id");
                $itemLabel.attr("id", "items[" + index + "].sku_label");

                $itemId.val(response.item_id);
                $itemLabel.text(response.sku);

                if (response.barcode > 0) {
                    $barcode.val(response.barcode).prop("disabled", true);
                }
                else {
                    $barcode.prop("disabled", false);
                }
                
                $("#sku-rows").append($itemRow);
            });
        }
    });
}

var photoshootTable;
function InitializePhotoshootTable() {
    photoshootTable = $("#prev-photoshoots").DataTable({
        "paging": false,
        "ordering": false,
        "info": false,
        "scrollY": "200px",
        language: {
            "search": "",
            "searchPlaceholder": "Search...",
            'processing': '<div><i class="fa fa-spinner fa-3x fa-spin"></i></div>'
        }
    });
    
}

function GetPhotoshootTr(val) {
    return '<tr><td><div class="form-check"><input class="form-check-input selectedPhotoshoot"'+
    ' type="radio" name="selectedPhotoshoot" id="' + val.photoshoot_id + '" value="' + val.photoshoot_id + '" ></td > <td><label class="form-check-label" for="' + val.photoshoot_id + '">' + val.photoshoot_name + '</label></td></div ></tr > ';
}

function DestroyPhotoshootTable() {
    if (photoshootTable) {
        photoshootTable.destroy();
    }
}

/*
Developer: Mohi
Date: 7-3-19
Action: it will add products in already exits photoshoots
Input: ProductId, PhotoShootId
Output: it closes the add new modal and remove the prodcuts from the current list
*/
function AddToExistingPhotoshoot() {
    $(".existingShootError").addClass("d-none");
    var productList = $("#AddToPhotoshootProductId").val();
    var PhotoShootId = $('input[name=selectedPhotoshoot]:checked').val();
    var itemList = $("#sku-rows").find("input").serializeArray();
    var items = GetNameValueMapping(itemList);
    if (productList.length > 0 && typeof PhotoShootId !== "undefined") {
        $("#_modal_loader").fadeIn(200);
        $.ajax({
            url: '/Photoshoots/addProductInPhotoshoot/' + PhotoShootId,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ products: JSON.parse(productList), items: items }),
            processData: false,
        }).always(function (data) {

            if (data == "Success") {
                productList = JSON.parse(productList);
                $("#_modal_loader").fadeOut(200);
                $("#modalAddNewPhotoshoot").modal('hide');
                $.each(productList, function (index, value) {
                    datatable_js_ps
                        .row($("#shootRow_" + value.product_id).parents('tr'))
                        .remove()
                        .draw();
                });
            } else {
                $("#_modal_loader").fadeOut(200);
            }   
        });

    } else {
        $(".existingShootError").removeClass("d-none");
    }
}

/*
Developer: Mohi
Date: 7-3-19
Action: it will add new photoshoot first and product in that
Input: ProductId, PhotoShootId
Output: it closes the add new modal and remove the selected prodcut from the current list
*/

function AddToNewPhotoshoot() {
    $(".newShootError").addClass("d-none");
    var ProductId = $("#AddToPhotoshootProductId").val();

    var PhotoshootModelId   = $('#AllModels').val();
    var PhotoshootModelName = $("#AllModels option:selected").text();
    var PhotoshootDate = $('#photoshootDate').val();

    var shoot_start_date = new Date(PhotoshootDate);
    shoot_date_milliseconds = shoot_start_date.getTime();
    shoot_date_seconds = shoot_date_milliseconds / 1000;

    var jsonData = {};
    jsonData["photoshoot_name"] = PhotoshootModelName + " " + PhotoshootDate; 
    jsonData["model_id"] = PhotoshootModelId;
    jsonData["product_id"] = ProductId;
    jsonData["shoot_start_date"] = shoot_date_seconds;
    jsonData["shoot_end_date"] = 0;

    if (typeof ProductId !== "undefined" && PhotoshootDate != "" && PhotoshootModelId != "") {
        $("#_modal_loader").fadeIn(200);
        $.ajax({
            url: '/Photoshoots/addNewPhotoshoot/',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,
             
        }).always(function (data) {
            if (data == "Success") {
                $("#_modal_loader").fadeOut(200);
                $("#modalAddNewPhotoshoot").modal('hide');
                if (ProductId.indexOf(',') == -1) {
                    datatable_js_ps
                        .row($("#shootRow_" + ProductId).parents('tr'))
                        .remove()
                        .draw();
                } else {
                    var arrayProductId = ProductId.split(",");
                    $.each(arrayProductId, function (i) {
                        datatable_js_ps
                            .row($("#shootRow_" + arrayProductId[i]).parents('tr'))
                            .remove()
                            .draw();
                    });
                }
                
            } else {
                $("#_modal_loader").fadeOut(200);
            }    
        });
    } else {
        $(".newShootError").removeClass("d-none");
    }
}


/*
Developer: Mohi
Date: 7-3-19 
Action: it will select all the selected product ids and get all photoshoots & models and add in Add photoshoot modal and open modal
Input: ProductIds
Output: it closes the add new modal and remove the selected prodcut from the current list
*/
function moveSelectedToPhotoshoot() {
    var productAddToShoot = [];
    var productNameList = [];

    $.each($("input[name='productAddToShoot']:checked"), function () {
        var product = {};
        var id = $(this).val();

        productName = $("#tableRow_" + id + " .productName").html();
        productNameList.push("&nbsp; " + productName);

        product.product_id = id;
        product.product_name = productName;
        product.vendor_id = $(this).data('vendor-id');
        productAddToShoot.push(product);
    });

    if (productAddToShoot.length > 0) {
        GetSmallestItem(productAddToShoot);
        $("#AddToPhotoshootProductId").val(JSON.stringify(productAddToShoot));
        $.ajax({
            url: '/photoshoots/getPhotoshootAndModels',
            
            type: 'GET',
            contentType: 'application/json',
            processData: false,

        }).always(function (data) {
            data = JSON.parse(data);
            var jsonPhotoshootsList = data.photoshootsList;
            var jsonPhotoshootsModelsList = data.photoshootsModelList;
            var allPhotoshootListHTML = '';

            $(jsonPhotoshootsList).each(function (i, val) {
                allPhotoshootListHTML += GetPhotoshootTr(val);
            });

            DestroyPhotoshootTable();
            $("#prev-photoshoots > tbody").html(allPhotoshootListHTML);
            InitializePhotoshootTable();
           // $(".allPhotoshootList").html(allPhotoshootListHTML);

            $("#AllModels").find('option').remove();
            $("#AllModels").append(new Option("Choose...", ""));
            $(jsonPhotoshootsModelsList).each(function (i, val) {
                $("#AllModels").append(new Option(val.model_name, val.model_id));
            });
            $("#modalAddNewPhotoshoot .modal-title").html("Add to photoshoot <small> (" + productNameList + ") </small>");
            $("#modalAddNewPhotoshoot").modal('show');
        });
    }
    
}

/*
Developer: Mohi
Date: 7-3-19
Action: it will take photoshoot id, product id, status and it update the status of that product
Input: photoshoot id, product id, status
Output: it removes the current product from the list
*/
function updatephotoshootStatus(productId, photoshootId, status) {

    var htmlLoader = $(".loading-box ");
    htmlLoader.css("visibility", "visible");

    $.ajax({
        url: "/photoshoots/UpdatePhotoshootProductStatus/" + productId + "/" + status,
        dataType: 'html',
        type: 'GET',
        contentType: 'application/json',
        processData: false,

    }).always(function (data) {
        if (data == "Success") {
            PhotoshootInProgressProductsList
                .row($("#shootRow_" + productId).parents('tr'))
                .remove()
                .draw();
        }
        htmlLoader.css("visibility", "hidden");

    });

}

/*
Developer: Mohi
Date: 7-3-19
Action: it will take   product id, status and it update the status of that product on SentToEditor pages
Input: product id, status
Output: it removes the current product from the list
*/
function changeShootStatusOnSendToEditor(productId, status) {
    $(".loading-box").css("visibility", "visible");
      
    $.ajax({
        url: "/photoshoots/UpdatePhotoshootProductStatus/" + productId + "/" + status,
        dataType: 'html',
        type: 'GET',
        contentType: 'application/json',
        processData: false,

    }).always(function (data) {

        datatable_js_ps
            .row($("#shootRow_" + productId).parents('tr'))
            .remove()
            .draw();
        $(".loading-box").css("visibility", "hidden");
    });

}

/*
Developer: Mohi
Date: 7-3-19
Action: it will take selected product id, photoshoot id, status and it update the status of all that products on Inprogress pages
Input: product id, status
Output: it removes the current products from the list
*/
function updateMultipleProductStatusOnInprogress(PhotoshootId, Status) {

    $(".loading-box").css("visibility", "visible");
    var productAddToShootNotStarted = [];
    $(".select-box:checked").each(function () {
        console.log($(this).val())
        productAddToShootNotStarted.push($(this).val());
    }) 
 
    if (productAddToShootNotStarted.length > 0) {
        var product_ids = productAddToShootNotStarted.join(","); 
        var jsonData = {};
        jsonData["product_id"] = product_ids;
        jsonData["status"] = Status;

        $.ajax({
            url: '/Photoshoots/updateMultiplePhotoshootStatus/',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,

        }).always(function (data) {
            if (data == "Success") { 
                if (product_ids.indexOf(',') == -1) {
                    PhotoshootInProgressProductsList
                        .row($("#shootRow_" + product_ids).parents('tr'))
                        .remove()
                        .draw();
                } else {
                    var arrayProductId = product_ids.split(",");
                    $.each(arrayProductId, function (i) {
                        PhotoshootInProgressProductsList
                            .row($("#shootRow_" + arrayProductId[i]).parents('tr'))
                            .remove()
                            .draw();
                    });
                }


            } $(".loading-box").css("visibility", "hidden");
        }); 
    }
    
}

/*
Developer: Mohi
Date: 7-3-19
Action: it will take selected product id,  status and it update the status of all that products on SentToEditor pages
Input: product id, status
Output: it removes the current products from the list
*/
function updateMultipleProductStatusOnSentToEditor(Status) {
    var productAddToShootNotStarted = [];
    $(".select-box:checked").each(function () {
        console.log($(this).val())
        productAddToShootNotStarted.push($(this).val());
    })
    if (productAddToShootNotStarted.length > 0) {
        $(".loading-box").css("visibility", "visible");
        var product_ids = productAddToShootNotStarted.join(",");
        console.log(product_ids);
        var jsonData = {};
        jsonData["product_id"] = product_ids;
        jsonData["status"] = Status;

        $.ajax({
            url: '/Photoshoots/updateMultiplePhotoshootStatus/',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,

        }).always(function (data) {
            if (data == "Success") {

                if (product_ids.indexOf(',') == -1) {
                    datatable_js_ps
                        .row($("#shootRow_" + product_ids).parents('tr'))
                        .remove()
                        .draw();
                } else {
                    var arrayProductId = product_ids.split(",");
                    $.each(arrayProductId, function (i) {
                        datatable_js_ps
                            .row($("#shootRow_" + arrayProductId[i]).parents('tr'))
                            .remove()
                            .draw();
                    });
                }

            } else {
                
            }
            $(".loading-box").css("visibility", "hidden");
        });
        console.log(product_ids);
    } else {
        
    }
}

/*
Developer: Mohi
Date: 7-8-19
Action: it will add new photoshoot model 
Input: Name, height, hair, Ethnicity
Output: it will add new model & update models in Add new photoshoot modals 
*/
function addNewPhotoshootModel(callFrom) {
    var name    = $("#model_name").val();
    var height  = $("#model_height").val();
    var hair    = $("#model_hair").val();
    var ethnicity = $("#model_ethnicity").val();

    if (name != '' && height != '' && hair != '' && ethnicity != '') {
        if (callFrom == "summary") {
            var htmlLoader = $(".loading-box ");
            htmlLoader.css("visibility", "visible");
        } else {
            $("#_modal_loader").fadeIn(200);
        }
        var jsonData = {};
        jsonData["model_name"] = name;
        jsonData["model_height"] = height;
        jsonData["model_hair"] = hair;
        jsonData["model_ethnicity"] = ethnicity;

        $.ajax({
            url: '/Photoshoots/addNewPhotoshootModel/',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,

        }).always(function (data) {
            if (callFrom == "summary") {
                if (data != "0") {
                    $("#openphotoshot .custom-select").each(function () {
                        console.log($(this).val());
                        $(this).append(new Option(name, data));
                    })
                    $("#modaladdmodel").modal('hide');
                    $('#NewModelForm')[0].reset();
                    htmlLoader.css("visibility", "hidden");
                } else {

                }
            }
            else {
                if (data != "0") {
                    $("#modaladdmodel").modal('hide');
                    $("#AllModels").append(new Option(name, data));
                    //$('#AllModels').val(data);
                    $('#NewModelForm')[0].reset();
                    $("#_modal_loader").fadeOut(200);
                } else {

                }
            }
            
        });
    }
}

/*
Developer: Mohi
Date: 7-19-19
Action: it will update photoshoot model & schedule date 
Input: Photoshootid
Output: 
*/

function editPhotoshootSummary(photoshootId) {
    var jsonData = {};
    var photoshootScheduledDate = $("#rowId_" + photoshootId + " .scheduled_date").val();
    var shoot_start_date = new Date(photoshootScheduledDate);
    shoot_date_milliseconds = shoot_start_date.getTime();
    shoot_date_seconds = shoot_date_milliseconds / 1000;

    jsonData["photoshootId"]            = photoshootId;
    jsonData["photoshootModelId"]       = $("#rowId_" + photoshootId + " .custom-select").val();
    jsonData["photoshootScheduledDate"] = shoot_date_seconds;
    jsonData["photoshootNotes"]         = $("#rowId_" + photoshootId + " .notes").val();
    jsonData["photoshootName"]     = $("#rowId_" + photoshootId + " .custom-select option:selected").text() + photoshootScheduledDate;

    $.ajax({
        url: '/Photoshoots/EditPhotoshootSummary/',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        
    });
}

function AddPhotoshootSummaryNotes(photoshootId) {
    var jsonData = {};
    
    jsonData["photoshootId"] = photoshootId;
    jsonData["photoshootNotes"] = $("#rowId_" + photoshootId + " .notes").val();

    $.ajax({
        url: '/Photoshoots/AddPhotoshootNotes/',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {

    });
}



/*
Developer: Mohi
Date: 7-18-19
Action: it will first select all photoshoot ids and get their note
Input: photoshootids
Output: it will add notes into note textarea
*/

$(document).ready(function () { 
    $('.loading').hide();
    var photoshootids = [];
    $(".ps_notes textarea").each(function () {
        console.log($(this).val())
        photoshootids.push($(this).attr('data-id'));
    })
    if (photoshootids.length > 0) {
        photoshootids = photoshootids.join(",");
        $.ajax({
            url: '/photoshoots/getphotoshootnotes/' + photoshootids,
            
            type: 'Get',
            contentType: 'application/json',
        }).always(function (data) {
            if (data.length > 0) {
                $(data).each(function (e, i) {
                    $(".ps_notes textarea").each(function () {
                        if ($(this).attr('data-id') == i.ref_id) {
                            $(this).val(i.note);
                        }
                    });
                });
            }
        });
    }
});

function ValidateBarcode(e) {
    var barcode = $(e).val();
    if (barcode && BarcodeLength(barcode)) {
        $.ajax({
            url: '/barcode/validatebarcode/' + barcode,
            type: 'Get',
            contentType: 'application/json',
            success: function (data) {
                BindValidationClass(data, e);
            }
        });
    }
    else {
        BindValidationClass(false, e);
    }
}

function BindValidationClass(data, e) {
    if (data === "True") {
        $(e).addClass('valid-input');
        $(e).removeClass('valid-input-error');
    }
    else {
        $(e).removeClass('valid-input');
        $(e).addClass('valid-input-error');
    }
}

function BarcodeLength(barcode) {
    return barcode.length == 8;
}

function isOnlyNumbers(e) {
    var numberRegex = new RegExp(/[0-9 -()+]+$/);
    var value = $(e).val();
    var isNumber = numberRegex.test(value);
    if (!isNumber) {
        $(e).val('');
    }
}