$(document).on('click', "#AddNewStyleButton", function () {
    debugger;
    var newVendorForm = $("#newAddStyleForm input");
    var jsonData = {};

    selectedProject = $('#ExistingProductSelect option:selected');
    if (selectedProject.data("product_id") > 0) {
        jsonData["product_id"] = selectedProject.data("product_id");
    } 


    jsonData["po_id"] = $('#newAddStyleForm #po_id').val();
    jsonData["vendor_id"] = $('#newAddStyleForm #vendor_id').val();
    jsonData["product_name"] = $(newVendorForm[0]).val();
    jsonData["vendor_color_name"] = $(newVendorForm[1]).val();
    jsonData["product_cost"] = $(newVendorForm[2]).val();
    jsonData["product_retail"] = $(newVendorForm[3]).val();
    jsonData["product_type_id"] = $('#StyleType option:selected').val();

    

    jsonData["vendor_style_sku"] = [];
    $('#po_input_fields_wrap .vendorSkuBox').each(function () {
        var style_sku = {};
        style_sku["style_vendor_size"] = $(this).find('#styleVendorSize').val();
        style_sku["style_size"] = $(this).find('#styleSize').val();
        style_sku["style_sku"] = $(this).find('#styleSku').val();
        style_sku["style_qty"] = $(this).find('#styleSkuQty').val();
        jsonData["vendor_style_sku"].push(style_sku);
    });


    $.ajax({
        
        url: location.origin + '/styles/create',
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
                url: location.origin + "/styles/newdoc",
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
        url: location.origin + "/styles/newdoc",
        type: 'POST',
        data: formData,
        dataType: 'json',
        processData: false,
        contentType: false,
    }).always(function (data) {
        console.log(data);
    });

});
$(document).ready(function () {
    var max_fields = 10; //maximum input boxes allowed
    var wrapper = $("#po_input_fields_wrap"); //Fields wrapper
    var add_button = $(".add_field_button"); //Add button ID
    $(".vendorSkuBox").remove();
    var x = 1; //initlal text box count
    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();
        if (x < max_fields) { //max input box allowed
            x++; //text box increment
            $(wrapper).append('<div class="pb-2  vendorSkuBox"> <input type="text" class="form-control d-inline w-25" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /> <input type="text" class="form-control d-inline w-25" name="styleSize" id="styleSize" placeholder="Size" /> <input type="text" class="form-control d-inline w-25" name="styleSku" id="styleSku" placeholder="SKU" /> <input type="text" class="form-control d-inline w-25" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.
        }
    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault(); $(this).parent('div').remove(); x--;
    })

});