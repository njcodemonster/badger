$(document).on('click', "#NewVendorButton", function () {
    debugger;
    var newVendorForm = $("#newVendorForm input");
    var jsonData = {};
    jsonData["vendor_name"] = $(newVendorForm[0]).val();
    jsonData["corp_name"] = $(newVendorForm[1]).val();
    jsonData["statement_name"] = $(newVendorForm[3]).val();
    jsonData["vendor_code"] = $(newVendorForm[4]).val();
    jsonData["vendor_street"] = $(newVendorForm[5]).val();
    jsonData["vendor_suite_number"] = $(newVendorForm[6]).val();
    jsonData["vendor_city"] = $(newVendorForm[7]).val();
    jsonData["vendor_zip"] = $(newVendorForm[8]).val();
    jsonData["vendor_state"] = $(newVendorForm[9]).val();
    jsonData["Rep_first_name"] = $(newVendorForm[10]).val();
    jsonData["Rep_email"] = $(newVendorForm[12]).val();
    jsonData["Rep_phone1"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    jsonData["Rep_phone2"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    $.ajax({
        
        url: '/vendor/newvendor',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data:  JSON.stringify(jsonData) ,
        processData: false,

    }).always(function (data) {
        console.log(data);
        if (data != "0") {
            console.log("New Vender Added");
            alert("vendor created . uploading files");
            var formData = new FormData();
            formData.append('Vendor_id', data);
            formData.append('vendorDocument', $('#newVendorForm #vendorDocument')[0].files[0]);
            $.ajax({
                url: "/vendor/newvendor_doc",
                type: 'POST',
                data: formData,
                dataType: 'json',
                processData: false,
                contentType: false,
            }).always(function (data) {
                console.log(data);
            });
        }
    });
   


});
$(document).on('click', "#EditVendor", function () {
    $("#modalvendor #vendorModalLongTitle").text("Edit Vendor");
    $('#modalvendor input').prop("disabled","true");
    $('#modalvendor').modal('show');
    var id = $(this).data("id");
    $.ajax({

        url: '/vendor/details/'+id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
       

    }).always(function (data) {
        $("#NewVendorButton,#EditVendorButton").attr("id", "EditVendorButton");
        $('#modalvendor input').removeAttr("disabled");
        var vendor = data.vendor;
        var addresses = data.addresses;
        var reps = data.reps;
        $("#modalvendor #vendorModalLongTitle").text("Edit Vendor:" + vendor.vendor_name);
        $('#vendorName').val(vendor.vendor_name);
        $('#vendorCorpName').val(vendor.corp_name);
        $('#vendorStatmentName').val(vendor.statement_name);
        $('#vendorCode').val(vendor.vendor_code);
        // $('#vendorourCustomerNumber').val(vendor.vendor_name);
        if (addresses.length > 0) {
            var add1 = addresses[0]
            $('#vendorStreetAdress').val(add1.vendor_street);
            $('#vendorUnitNumber').val(add1.vendor_suite_number);
            $('#vendorCity').val(add1.vendor_city);
            $('#vendorZip').val(add1.vendor_zip);
            $('#vendorState').val(add1.vendor_state);
        }
        if (reps.length > 0) {
            rep1 = reps[0];
            $('#vendorRepName').val(rep1.first_name);
            $('#vendorRepEmail').val(rep1.email);
            $('#vendorRepPhone11').val(rep1.phone1);
            $('#vendorRepPhone12').val(rep1.phone1);
            $('#vendorRepPhone13').val(rep1.phone1);
        }
        
        
    });

});
$(document).on('click', "#EditVendorButton", function () {
    id = $("#newVendorForm").data("currentID");
    jsonData["vendor_name"] = $(newVendorForm[0]).val();
    jsonData["corp_name"] = $(newVendorForm[1]).val();
    jsonData["statement_name"] = $(newVendorForm[3]).val();
    jsonData["vendor_code"] = $(newVendorForm[4]).val();
    jsonData["vendor_street"] = $(newVendorForm[5]).val();
    jsonData["vendor_suite_number"] = $(newVendorForm[6]).val();
    jsonData["vendor_city"] = $(newVendorForm[7]).val();
    jsonData["vendor_zip"] = $(newVendorForm[8]).val();
    jsonData["vendor_state"] = $(newVendorForm[9]).val();
    jsonData["Rep_first_name"] = $(newVendorForm[10]).val();
    jsonData["Rep_email"] = $(newVendorForm[12]).val();
    jsonData["Rep_phone1"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    jsonData["Rep_phone2"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    $.ajax({

        url: '/vendor/newvendorddf',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) { console.log(data); });
});
$(document).on('click', "#AddNewVendorButton", function () {
    $("#NewVendorButton,#EditVendorButton").attr("id", "NewVendorButton");
    $("#modalvendor #vendorModalLongTitle").text("Add a New Vendor Profile");
    $("#newVendorForm input").val("");
    $("#newVendorForm").data("currentID","");
});