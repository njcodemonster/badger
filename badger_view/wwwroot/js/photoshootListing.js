// Photoshoots/shootInProgress page accordian code here...

$('.card-header').click(function () {
    var thisPhotoshoot = $(this);
    var photoshootId = thisPhotoshoot.attr("data-photoshootId");
    $("#collapse_" + photoshootId).html('<div style="width:100%;height: 100px;z-index: 999; text-align:center;">< div class= "spinner-border text-light" role = "status" style = "margin-top: 20%; margin-left: 48%;" ><span class="sr-only">Loading...</span></div ></div >');

    if ($("#collapse_" + photoshootId).is(":hidden")) {

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
    } else {
        $("#collapse_" + photoshootId).html("");
    }
});

/**********************************************************/


$('#photoshootDate').datepicker();
var datatable_js_ps = $('.datatable_js_ps').DataTable();

function selectAllCheckbox() {
    $(".select-box").attr("checked", true);
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
        });

        $("#modalAddNewPhotoshoot").modal('show');
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
    jsonData["active_status"] = 0;
    jsonData["created_by"] = 2;
    jsonData["updated_by"] = 1;
    jsonData["created_at"] = (new Date().getTime()) / 1000;
    jsonData["updated_at"] = (new Date().getTime()) / 1000;

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
        });

        $("#modalAddNewPhotoshoot").modal('show');

    }
    
}