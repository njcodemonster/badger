$('#photoshootDate').datepicker();

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
            type: 'post',
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

        $.ajax({
            url: '/Photoshoots/addProductInPhotoshoot/' + ProductId + "/" + PhotoShootId,
            type: 'GET',
            contentType: 'application/json',
            processData: false,
            
        }).always(function (data) {
            alert(data);
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
        
        $.ajax({
            url: '/Photoshoots/addNewPhotoshoot/',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,
             
        }).always(function (data) {
            alert(data);


        });
    } else {
        $(".newShootError").removeClass("d-none");
    }
}
