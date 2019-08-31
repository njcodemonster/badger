$(document).on('click', "#AddNewBarcodeButton", function () {
    debugger;


    $('#modalAddBarcode').modal('show');
    $('#BarcodeFrom').val("");
    $('#barcodeid').val("");
    $('#BarcodeTo').val("");
    $('#Size').val("");
});

$(document).on('click', "#EditBarcode", function () {
    debugger;


    $('#modalAddBarcode').modal('show');
    var id = $(this).data("id");
    if (id == "") {
        id = -1;
    }
    $('#barcodeid').val(id);
    
    $('#BarcodeFrom').val($(this).data("from"));
    $('#BarcodeTo').val($(this).data("to"));
    $('#Size').val($(this).data("size"));
});

$(document).on('click', ".SaveBarcodeButton", function () {
    debugger;
    AddBarcode();
});

$("#AddBarcodeForm #BarcodeFrom").on("keydown", function (event) {
    debugger;
    if (event.which == 13) {
        AddBarcode();
        return false;
    }
    return isNumber(event);
});
$("#AddBarcodeForm #BarcodeTo").on("keydown", function (event) {
    debugger;
    if (event.which == 13) {
        AddBarcode();
        return false;
    }
    return isNumber(event);
});

$("#AddBarcodeForm #Size").on("keydown", function (event) {
    debugger;
    if (event.which == 13) {
        AddBarcode();
        return false;
    }
});

$(document).on('click', "#deleteBarcode", function () {
    debugger;
    var id = $(this).data("id");
    confirmationAlertBox("Barcode Delete", "Are you sure that you want to delete this record?", function (result) {

        if (result == "yes") {
            $.ajax({
                url: '/barcode/delete/' + id,
                dataType: 'json',
                type: 'POST',
                contentType: 'application/json',
            }).always(function (data) {
                console.log(data);
                if (data == true) {
                    alertBox('poAlertMsg', 'green', 'Barcode deleted');
                    location.reload();
                    
                }
                else {
                    alertBox('poAlertMsg', 'red', 'Barcode not deleted.');
                }

            });
        }
    }) 
});

function AddBarcode() {
    var barcodeid = $('#barcodeid').val();
    var BarcodeFrom = $('#BarcodeFrom').val();
    var BarcodeTo = $('#BarcodeTo').val();
    var Size = $('#Size').val();
    //empty or spaces validations
    if (!Size.replace(/\s/g, '').length) {
        $('#Size').val("");
    }
    if (!BarcodeFrom.replace(/\s/g, '').length) {
        $('#BarcodeFrom').val("");
    }
    if (!BarcodeTo.replace(/\s/g, '').length) {
        $('#BarcodeTo').val("");
    }
    if (emptyFeildValidation('AddBarcodeForm') == false) {
        return false;
    }
    //validation if range is 8 in length
    if (BarcodeFrom.length != 8) {
        alertBox('poAlertMsg', 'red', 'Barcode Ranges must be 8 in length');
        return false;
    }
    if (BarcodeTo.length != 8) {
        alertBox('poAlertMsg', 'red', 'Barcode Ranges must be 8 in length');
        return false;
    }
    //chceck if from is less then to
    if (Number(BarcodeFrom) > Number(BarcodeTo)) {
        alertBox('poAlertMsg', 'red', 'Barcode From should be less than Barcode from');
        return false;
    }
    //validation from db if the range is exist between any of the defined range 
    var jsonData = {};
    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');


    jsonData["barcode_from"] = BarcodeFrom;
    jsonData["barcode_to"] = BarcodeTo;
    jsonData["size"] = Size;
    jsonData["id"] = barcodeid;

    $.ajax({

        url: location.origin + '/barcode/validate',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        console.log(data);
        if (data == false) {
            alertBox('poAlertMsg', 'red', 'Barcode Range already exist, please enter other range and retry');
            return false;
        }
        else {
            //Success Save Method Now All Validations Done
            $.ajax({

                url: location.origin + '/barcode/create',
                dataType: 'json',
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify(jsonData),
                processData: false,

            }).always(function (data) {
                console.log(data);
                if (data == false) {
                    alertBox('poAlertMsg', 'red', 'Barcode Insertion Failed');
                    return false;
                }
                else {
                    //Success Save Method Now All saving Done
                    if (barcodeid == -1) {
                        //insert
                        alertBox('poAlertMsg', 'green', 'Barcode Inserted Successfully');
                    }
                    else {
                        alertBox('poAlertMsg', 'green', 'Barcode Updated Successfully');

                    }

                    $('#modalAddBarcode').modal('hide');
                    location.reload();
                }
                $('.poAlertMsg').html('')
            });
        }
        $('.poAlertMsg').html('')
    });
}