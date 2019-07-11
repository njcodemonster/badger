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


$('#photoshootDate').datepicker({
    format: 'm/d/yyyy'
});
var datatable_js_ps = $('.datatable_js_ps').DataTable();


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
    $(".select-box").attr("checked", true);

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
    $(".select-box").attr("checked", false);

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
function AddToShootSingle(shootProductId, statusId) {
    window.photoShootRowId = shootProductId;
    var ProductName = $("#tableRow_" + shootProductId + " .productName").html();
    $("#AddToPhotoshootProductId").val(shootProductId);
    if (statusId == 1) {
        $.ajax({
            url: '/photoshoots/getPhotoshootAndModels',
            dataType: 'json',
            type: 'GET',
            contentType: 'application/json',
            processData: false,

        }).always(function (data) {

            var jsonPhotoshootsList = data.photoshootsList;
            var jsonPhotoshootsModelsList = data.photoshootsModelList;
            var allPhotoshootListHTML = '';

            $(jsonPhotoshootsList).each(function (i, val) {
                allPhotoshootListHTML += '<div class="form-check"><input class="form-check-input selectedPhotoshoot" type = "radio" name = "selectedPhotoshoot" id = "' + val.photoshoot_id + '" value = "' + val.photoshoot_id + '" ><label class="form - check - label" for="' + val.photoshoot_id + '">' + val.photoshoot_name + '</label></div >';
            });
            $(".allPhotoshootList").html(allPhotoshootListHTML);

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

/*
Developer: Mohi
Date: 7-3-19
Action: it will add products in already exits photoshoots
Input: ProductId, PhotoShootId
Output: it closes the add new modal and remove the prodcuts from the current list
*/
function AddToExistingPhotoshoot() {
    $(".existingShootError").addClass("d-none");
    var ProductId = $("#AddToPhotoshootProductId").val();
    var PhotoShootId = $('input[name=selectedPhotoshoot]:checked').val();
    if (typeof ProductId !== "undefined" && typeof PhotoShootId !== "undefined") {
        $("#_modal_loader").fadeIn(200);
        $.ajax({
            url: '/Photoshoots/addProductInPhotoshoot/' + ProductId + "/" + PhotoShootId,
            type: 'GET',
            contentType: 'application/json',
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
        var id = $(this).val();
        productAddToShoot.push($(this).val());
        productName = $("#tableRow_" + id + " .productName").html();
        productNameList.push("&nbsp; "+productName );

    });
    if (productAddToShoot.length > 0) {
        var product_ids = productAddToShoot.join(","); 
        var all_products = productNameList.join(", ");
        $("#AddToPhotoshootProductId").val(productAddToShoot.join(","));
        $.ajax({
            url: '/photoshoots/getPhotoshootAndModels',
            dataType: 'json',
            type: 'GET',
            contentType: 'application/json',
            processData: false,

        }).always(function (data) {

            var jsonPhotoshootsList = data.photoshootsList;
            var jsonPhotoshootsModelsList = data.photoshootsModelList;
            var allPhotoshootListHTML = '';

            $(jsonPhotoshootsList).each(function (i, val) {
                allPhotoshootListHTML += '<div class="form-check"><input class="form-check-input selectedPhotoshoot" type = "radio" name = "selectedPhotoshoot" id = "' + val.photoshoot_id + '" value = "' + val.photoshoot_id + '" ><label class="form - check - label" for="' + val.photoshoot_id + '">' + val.photoshoot_name + '</label></div >';
            });
            $(".allPhotoshootList").html(allPhotoshootListHTML);

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
function addNewPhotoshootModel() {
    $("#_modal_loader").fadeIn(200);
    var name    = $("#model_name").val();
    var height  = $("#model_height").val();
    var hair    = $("#model_hair").val();
    var ethnicity = $("#model_ethnicity").val();

    if (name != '' && height != '' && hair != '' && ethnicity != '') {

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
            if (data != "0") {
                $("#modaladdmodel").modal('hide');
                $("#AllModels").append(new Option(name, data));
                //$('#AllModels').val(data);
                $('#NewModelForm')[0].reset();
                $("#_modal_loader").fadeOut(200);
            } else {
                
            }
        });
    }
}
