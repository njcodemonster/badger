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
    $('#barcodeid').val(id);
    $('#BarcodeFrom').val($(this).data("from"));
    $('#BarcodeTo').val($(this).data("to"));
    $('#Size').val($(this).data("size"));
});