$(document).on('click', "#AddNewStyleButton", function () {
    debugger;
    var newVendorForm = $("#newAddStyleForm input");
    var jsonData = {};
    jsonData["product_name"] = $(newVendorForm[0]).val();
    jsonData["vendor_color_name"] = $(newVendorForm[1]).val();
    jsonData["product_cost"] = $(newVendorForm[2]).val();
    jsonData["product_retail"] = $(newVendorForm[3]).val();
    jsonData["product_type_id"] = $('#StyleType option:selected').val();

    jsonData["product_type_id"] = $('#StyleType option:selected').val();

   

    jsonData["style_sizestyle_vendor_size"] = $(newVendorForm[4]).val();
    jsonData["style_size"] = $(newVendorForm[5]).val();
    jsonData["style_sku"] = $(newVendorForm[6]).val();
    jsonData["style_qty"] = $(newVendorForm[7]).val();
    



    //csize  vendorcsize csku cqty
 
   // jsonData["Rep_phone1"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
   // jsonData["Rep_phone2"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    $.ajax({
        
        url: 'styles/create',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data:  JSON.stringify(jsonData) ,
        processData: false,

    }).always(function (data) {
        console.log(data);
        if (data != "0") {
            console.log("New style Added");
            alert("style created . uploading files");
            var formData = new FormData();

            formData.append('product_id', data);
            formData.append('StyleImage', $('#newAddStyleForm #StyleImage')[0].files[0]);
            $.ajax({
                url: "/styles/newdoc",
                type: 'POST',
                data: formData,
                dataType: 'json',
                processData: false,
                contentType: false,
            }).always(function (data) {
                console.log(data);
                alert("files Uploded");

            });
        }
    });
});


$(document).on('click', "#AddStyleButton", function () {
    debugger;
    console.log("New style Added");
    //alert("style created . uploading files");
    var formData = new FormData();

    formData.append('product_id', 507);
    formData.append('StyleImage', $('#newAddStyleForm #StyleImage')[0].files[0]);
    $.ajax({
        url: "/addstyle/newstyle_doc",
        type: 'POST',
        data: formData,
        dataType: 'json',
        processData: false,
        contentType: false,
    }).always(function (data) {
        console.log(data);
    });

});