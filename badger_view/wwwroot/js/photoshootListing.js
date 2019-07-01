// Photoshoots/shootInProgress page accordian code here...

$('.card-header').click(function () {
    var thisPhotoshoot = $(this);
    var photoshootId = thisPhotoshoot.attr("data-photoshootId");
    if ($("#collapse_" + photoshootId).is(":hidden")) {
        getPhotoshootProducts(photoshootId); 
    } else {
        $("#collapse_" + photoshootId).html("");
    }
});


function getPhotoshootProducts(photoshootId) {
    $("#collapse_" + photoshootId).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>');
        $.ajax({
            url: '/photoshoots/getPhotoshootInProgressProducts/' + photoshootId,
            type: 'GET',
            dataType: "html",
        }).always(function (data) {
            $("#collapse_" + photoshootId).html(data);
            var PhotoshootInProgressProductsList = $("#collapse_" + photoshootId + " .datatable_js_ps ").DataTable({
                columnDefs: [
                    { targets: 'no-sort', orderable: false }
                ]
            });
        })
    
}
/**********************************************************/


$('#photoshootDate').datepicker({
    format: 'm/d/yyyy'
});
var datatable_js_ps = $('.datatable_js_ps').DataTable();

function selectAllCheckbox() {

    $(".select_menu").hide();
    $(".unselect_menu").show();
    $(".select-box").attr("checked", true);

}

function unselectAllCheckbox() {

    $(".select_menu").show();
    $(".unselect_menu").hide();
    $(".select-box").attr("checked", false);

}

function addNewPhotoshoot() {
    $("#modalAddNewPhotoshoot").modal('show');
}

function AddToShootSingle(shootRowId, selectValue) {
    window.photoShootRowId = shootRowId;
    $("#AddToPhotoshootProductId").val(shootRowId);
    if (selectValue == 1) {
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
            $(jsonPhotoshootsModelsList).each(function (i, val) {
                $("#AllModels").append(new Option(val.model_name, val.model_id));
            });
            $("#modalAddNewPhotoshoot").modal('show');
        }); 
    }
}

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

function moveSelectedToPhotoshoot() {
    var productAddToShoot = [];
    $.each($("input[name='productAddToShoot']:checked"), function () {
        productAddToShoot.push($(this).val());
    });
    if (productAddToShoot.length > 0) {
        var product_ids = productAddToShoot.join(","); 
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

            $("#AllModels").find('option').remove()
            $(jsonPhotoshootsModelsList).each(function (i, val) {
                $("#AllModels").append(new Option(val.model_name, val.model_id));
            });
            $("#modalAddNewPhotoshoot").modal('show');
        });
    }
    
}

function updatephotoshootStatus(productId, photoshoot_id, status) {
    
    $("#collapse_" + photoshoot_id).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>');

    var statusUpdate = "";
    if (status == 0) {
        statusUpdate = "NotStarted";
    } else if (status == 2) {
        statusUpdate = "SendToEditor";
    }

    $.ajax({
        url: "/photoshoots/UpdatePhotoshootProductStatus/" + productId + "/" + statusUpdate,
        dataType: 'html',
        type: 'GET',
        contentType: 'application/json',
        processData: false,

    }).always(function (data) {

        setTimeout(function () {
            getPhotoshootProducts(photoshoot_id);
            $("#collapse_" + photoshoot_id).collapse("show");
        }, 1000);
    }); 

}

function changeShootStatusOnSendToEditor(productId, status) {
    $(".loading-box").css("visibility", "visible");
    var statusUpdate = "";
    if (status == 0) {
        statusUpdate = "NotStarted";
    } else if (status == 1) {
        statusUpdate = "InProgress";
    }

    $.ajax({
        url: "/photoshoots/UpdatePhotoshootProductStatus/" + productId + "/" + statusUpdate,
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

function updateMultipleProductStatusOnInprogress(PhotoshootId, Status) {
    var productAddToShootNotStarted = [];
    $(".select-box:checked").each(function () {
        console.log($(this).val())
        productAddToShootNotStarted.push($(this).val());
    }) 
    $("#collapse_" + PhotoshootId).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;"><div class= "spinner-border" role = "status" style = " " ><span class="sr-only">Loading...</span></div></div>');
    console.log((productAddToShootNotStarted));
    if (productAddToShootNotStarted.length > 0) {
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
                getPhotoshootProducts(PhotoshootId);
            } else {
                //alert(data)
            }
        });
        console.log(product_ids);
    } else {
        getPhotoshootProducts(PhotoshootId);
    }
    
}

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
