﻿/*
Developer: Sajid Khan
Date: 7-5-19
Action: Add new style
URL: /styles/create
Input: styles data
Output: string of style
*/
$(document).on('click', ".AddNewStyleButton", function () {
    debugger;
    var action = $(this).attr('data-action');
     
    var newVendorForm = $("#newAddStyleForm input");
    if (emptyFeildValidation('newAddStyleForm') == false) {
        return false;
    }
    var jsonData = {};
    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');
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
            });
            alertBox('poAlertMsg', 'green', 'New style inserted successfully');
            if (action == 'refreshValue') {
                $("#modaladdstylec input,textarea,select").val("").removeClass('errorFeild');
               
            } else {
                $('#modaladdstylec').modal('hide')
            }
        }
        $('.poAlertMsg').html('')
    });
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: Add new style document
URL: /styles/newdoc
Input: string file path
Output: string of style doc 
*/
/*$(document).on('click', "#AddStyleButton", function () {
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

});*/



/*
Developer: Sajid Khan
Date: 7-5-19
Action: default show style size, quantity etc
URL: 
Input: 
Output: input fields show dynamic 
*/
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
            if ($('.vendorSkuBox:visible').length > 0) {
                var lastsku = $('.vendorSkuBox:last').find('#styleSku').val()
                if (lastsku.indexOf('-') > -1) {
                    lastsku = lastsku.split('-')
                    var lastskuNum = parseInt(lastsku[1])
                    lastskuNum = lastskuNum + 1
                    lastsku = lastsku[0] + '-' + lastskuNum
                } else {
                      lastsku = '';
                }

            } else {
                lastsku = '';
            }
            var options = '';
            if (window.sku_sizes.length) {
                for (i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (i == 0)
                     selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
            }
            $(wrapper).append('<div class="pb-2  vendorSkuBox form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> <div class="form-group col-md-3"><select type="text" class="form-control d-inline required" name="styleSize" id="styleSize">'+options+'</select></div> <div class="form-group col-md-3"><input style="text-transform: uppercase;" type="text" maxlength="7" value="'+lastsku+'" class="form-control d-inline  required" name="styleSku" id="styleSku" placeholder="SKU" /></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.
        }
    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault(); $(this).parent('div').remove(); x--;
    })

});

$(document).on('blur', "#styleSku", function (event) {
    var patt = new RegExp('^([A-Z]{2})([0-9]{3})([-]{1})([0-9]{1})$');
    var value = $(this).val().toUpperCase();
    if (patt.test(value) == false) {
        $(this).addClass('errorFeild')
    } else {
        $(this).removeClass('errorFeild')
    }
    
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: Select dropdown data show by id 
URL:  purchaseorders/lineitems/productid/purchaseorderid
Input: int product id, int purchase order id
Output: get data in fields
*/
$(document).on('change', '#modaladdstylec #ExistingProductSelect', function () {
    var SelectedProduct = $(this.options[this.selectedIndex]);
     $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');
    SelectedProductID = SelectedProduct.data("product_id");
    productImage = SelectedProduct.data("product_vendor_image");
    SelectedProductTytle = $(this.options[this.selectedIndex]).val();
    $('#modaladdstylec #product_title').val($(this.options[this.selectedIndex]).attr('data-name'));
    $('#modaladdstylec #product_unit_cost').val(SelectedProduct.data('product_unit_cost'));
    $('#modaladdstylec #product_color').val(SelectedProduct.data('product_color'));
    $('#modaladdstylec #product_retail').val(SelectedProduct.data('product_retail'));
    var SeletedPOID = SelectedProduct.data("po_id");
    $('#modaladdstylec StyleType option').removeAttr('selected');
    if (SelectedProduct.data('product_type') == 1) {
        $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=1]').val()).change()
    }
    else {
        $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=2]').val()).change()
    }
    $(".style_doc_section").empty();
    if (productImage != null) {

        //$(".style_doc_section").append("<img src='/images/dress-clipart.jpg' width='50' />");
        $(".style_doc_section").append("<img src='" + productImage+"' width='50' />  <br>");
        //$(".style_doc_section").append("<a onclick='return false' class='documentsLink' data-proid=" + SelectedProductID + " data-val=" + productImage + ">" + productImage + " <span class='podeleteImage'>×</span></a>");

        $(".style_doc_section").removeClass('d-none');

    } else {
        $(".style_doc_section").addClass('d-none');
    }
    $.ajax({
        url: '/purchaseorders/lineitems/' + SelectedProductID + '/' + SeletedPOID,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        //data = ;
        //console.log(data);

        var wrapper = $("#po_input_fields_wrap"); //Fields wrapper
        $(".vendorSkuBox").remove();
        $(".vendorSkuBox_disabled").remove();
        var options = '';
        if (window.sku_sizes) {
            for (i = 0; i < sku_sizes.length; i++) {
                var selected = "";
                if (i == 0)
                    selected = "selected";
                options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
            }
        }
        for (x = 0; x < data.length; x++) {
            $(wrapper).append('<div class="pb-2 vendorSkuBox_disabled form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="csize[' + x + ']" placeholder="Vendor Size"  disabled /></div><div class="form-group col-md-3"><select class="form-control d-inline" name="" value = ""  disabled>' + options +'</select></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="size[' + x + ']" placeholder="Size" value="' + data[x].sku + '" style="text-transform: uppercase;"  disabled /></div> <div class="form-group col-md-3"> <input type="text" class="form-control d-inline " name="cqty[' + x + ']" placeholder="Qty" value="' + data[x].line_item_ordered_quantity + '"  disabled />  '); // add input boxes.

        }
         $('.poAlertMsg').html('')

    });
});

/*
Developer: Sajid Khan
Date: 7-7-19
Action: Get Data of items by vendor id and show in dropdown and fields
Input: int purchase order id, int vendor id
Output: string of vendor products
*/
$(document).on('click', "#AddItemButton", function () {
    debugger;
    var CurrentPOID = $(this).data("poid");
    $('.errorMsg').remove();
    $("#modaladdstylec input,textarea,select").val("").removeClass('errorFeild');
    var CurrentVendorId = $(this).data("vendorid");
    var CurrentProductId = $(this).data("proid");
    var productImage = $(this).data("product_vendor_image");
    $('.poNumber').text( $(this).data("ponumber"))
    $('#modaladdstylec input').val("");
    $('#modaladdstylec #StyleSubType option').each(function () {
        if (this.innerText != "Choose..." && this.innerText != "...") {
            this.remove();
        }
    });

    $('#modaladdstylec').modal('show');
  //  alert("Please wait for the data to load");
   
    $.ajax({

        url: '/vendor/products/' + CurrentVendorId,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        data_vendor = data.vendor;
        window.sku_sizes = data.Sizes;
        data = data.vendorProducts;

        $('#modaladdstylec #ExistingProductSelect option').remove();
        $('#modaladdstylec #ExistingProductSelect').append("<option id='-1' value=''>Choose...</option>");
        var last_sku_family = "";

        $('#po_id').val(CurrentPOID);
        $('#vendor_id').val(CurrentVendorId);
        $(".vendorSkuBox_disabled").remove();
        $(".vendorSkuBox").remove();

        if (data.length) {
            for (i = 0; i < data.length; i++) {
                if (CurrentProductId != null) {
                    $('#modaladdstylec #ExistingProductSelect').append("<option value='" + data[i].product_id + "' data-product_type='" + data[i].product_type_id + "' data-product_vendor_image='" + data[i].product_vendor_image + "' data-product_color='" + data[i].vendor_color_name + "' data-product_unit_cost='" + data[i].product_cost + "' data-product_retail='" + data[i].product_retail + "' data-Product_id='" + data[i].product_id + "'  data-skufamily='" + data[i].sku_family + "'  data-po_id='" + CurrentPOID + "' data-name='" + data[i].product_name + "' >" + data[i].product_name + "</option>");
                }
                else {
                    $('#modaladdstylec #ExistingProductSelect').append("<option data-product_type='" + data[i].product_type_id + "' data-product_color='" + data[i].vendor_color_name + "' data-product_unit_cost='" + data[i].product_cost + "' data-product_retail='" + data[i].product_retail + "' data-Product_id='" + data[i].product_id + "'  data-skufamily='" + data[i].sku_family + "'  data-po_id='" + CurrentPOID + "' data-name='" + data[i].product_name + "' >" + data[i].product_name + "</option>");
                }
                    last_sku_family = data[i].sku_family;
            }
            if (CurrentProductId != null) {
                $('#modaladdstylec #ExistingProductSelect').val(CurrentProductId).trigger('change');
                $('#modaladdstylec #ExistingProductSelect').prop('disabled', true);
            }
            var vendorCode = last_sku_family.substring(0, 2);
            var sku_number = parseInt(last_sku_family.substr(2)) + 1;
            var new_sku = vendorCode + sku_number;

        } else {
            var vendorCode = data_vendor[0].vendor_code;
            var sku_number = 100;
            var new_sku = vendorCode + sku_number;
        }
        var wrapper = $("#po_input_fields_wrap"); //Fields wrapper


        //var sku_sizes = ["","XS", "S", "M", "L"];

        for (x = 1; x <= sku_sizes.length; x++) {
            var dropdown = '<div class="form-group col-md-3" ><select class="form-control" name="styleSize"  id="styleSize" >';
            var selected = "";
            var y = 1;
            
            $.each(sku_sizes, function () {
                if (x == y++) { selected = " selected "; }
                dropdown += " <option value='" + this.attribute_id + "'  " + selected + ">" + this.attribute_display_name + "</option>";
                selected = "";
               // dropdown += " <option value='1' >as</option>";
            });
            dropdown += "</select></div>";

            var str1 = '<div class="pb-2  vendorSkuBox form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div>';
            var str2 = ' <div class= "form-group col-md-3" > <input type="text" maxlength="7" style="text-transform: uppercase; " class="form-control d-inline  required" name="styleSku" id="styleSku" placeholder="SKU" value="' + new_sku + '-' + x +'" /></div > <div class="form-group col-md-3"><input type="text" class="form-control d-inline  required" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div >';
            var res = str1.concat(dropdown, str2);

            $(wrapper).append(res);
           // $(wrapper).append('<div class="pb - 2  vendorSkuBox form - row"> <div class="form - group col - md - 3"><input type="text" class="form - control d - inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> < div class= "form - group col - md - 3" > <input type="text" class="form - control d - inline  required" name="styleSize" id="styleSize" placeholder="Size" value="' + sku_sizes[x] + '" /></div < div class= "form - group col - md - 3" > <input type="text" maxlength="7" style="text - transform: uppercase; " class="form - control d - inline  required" name="styleSku" id="styleSku" placeholder="SKU" value="' + new_sku + ' - ' + x +'" /></div > <div class="form - group col - md - 3"><input type="text" class="form - control d - inline  required" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div > '); // add input boxes.");
             // $(wrapper).append('<div class="pb - 2  vendorSkuBox form - row"> <div class="form - group col - md - 3"><input type="text" class="form - control d - inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> < div class= "form - group col - md - 3" > '+dropdown+'</div> <div class= "form - group col - md - 3" > <input type="text" maxlength="7" style="text - transform: uppercase; " class="form - control d - inline  required" name="styleSku" id="styleSku" placeholder="SKU" value="' + new_sku + ' - ' + x +'" /></div > <div class="form - group col - md - 3"><input type="text" class="form - control d - inline  required" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div > '); // add input boxes.");


        }
       
        console.log(data);
    });
});

$(document).on('change', "#StyleType", function (event) {
    if ($(this).val() == '2') {
        $('.vendorSkuArea').hide();
    } else {
        $('.vendorSkuArea').show();
        if ($('.vendorSkuArea #styleVendorSize').length == 0) {
            var options = '';
            if (window.sku_sizes) {
                for (i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (i == 0)
                        selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
            }
            for (x = 1; x < 5; x++) {
                $("#po_input_fields_wrap").append('<div class="pb-2  vendorSkuBox form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> <div class="form-group col-md-3"><select class="form-control d-inline  required" name="styleSize" id="styleSize">' + options +'</select></div> <div class="form-group col-md-3"><input type="text" maxlength="7" style="text-transform: uppercase;" class="form-control d-inline  required" name="styleSku" id="styleSku" placeholder="SKU" value = "" /></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline  required" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.

            }
        }
    }
});

$(document).on('change', "#modaladdstylec input,#modaladdstylec select", function () {
   $(this).removeClass('errorFeild');
   $(this).parents('.form-group').find('.errorMsg').remove();
});

$(document).on('keydown', "#product_title,#product_color", function (e) {
    return allLetterAllow(e)
});

$(document).on('keydown', "#product_unit_cost,#product_retail", function (e) {
    return onlyNumbersWithDot(e)
});


