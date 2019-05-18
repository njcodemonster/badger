$(document).on('click', "#NewVendorButton", function () {
    var newVendorForm = $("#newVendorForm input");
    var jsonData = {};
    jsonData["vendor_name"] = $(newVendorForm[0]).val();
    jsonData["corp_name"] = $(newVendorForm[1]).val();
    jsonData["statement_name"] = $(newVendorForm[3]).val();
    jsonData["vendor_code"] = $(newVendorForm[4]).val();
    jsonData["our_customer_number"] = $(newVendorForm[5]).val();
   
    $.ajax({
        
        url: '/vendor/newvendor',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data:  JSON.stringify(jsonData) ,
        processData: false,

    }).always(function (data) { console.log(data);});
   


});
