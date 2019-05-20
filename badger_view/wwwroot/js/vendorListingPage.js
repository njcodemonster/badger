$(document).on('click', "#NewVendorButton", function () {
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

    }).always(function (data) { console.log(data);});
   


});
