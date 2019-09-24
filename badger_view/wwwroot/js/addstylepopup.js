/*
Developer: Rizvan Ali
Date: 7-5-19
Action: Delete Product from PO 
URL: /styles/delete
Input: styles data
Output: string of style
*/
$(document).on('click', ".DeletefromPOButton", function () {

    var jsonData = {};
    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');
    
    if (SelectedProductID != null) {
        confirmationAlertInnerBox("Delete from PO", "Are you sure that you want to delete this record?", function (result) {
            var po_id = parseInt($('#newAddStyleForm #po_id').val());
            if (result == "yes") {


               
                var product_id = SelectedProductID;
                $.ajax({

                    url: location.origin + '/styles/deleteFromPO/' + product_id + '/' + po_id,
                    dataType: 'json',
                    type: 'get',
                    processData: false,

                }).always(function (data) {
                    console.log(data);
                    if (data == true) {
                        $('#modaladdstylec').modal('hide');
                        alertBox('poAlertMsg', 'green', 'Product deleted successfully');
                        $("#collapse_" + po_id).hide();
                        $("#collapse_" + po_id).html('');
                    }
                    else {
                        alertBox('poAlertMsg', 'red', 'Product delete failed because some product are already recieved');
                    }
                });
            }
            else {
                $('.text-info').hide();
            }
        }) 
        }
                
    else {
        //error for selecting a product first to delete 
        alertBox('poAlertMsg', 'red', 'Please select a product');
    }






});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: Add new style
URL: /styles/create
Input: styles data
Output: string of style
*/

var Original_sku = "";
var new_sku = "";
var SelectedProductID;
var data_Categories;
var productSubCategoriesAction = [];
var IsLineItemExists = false;
var CurrentVendorId;
var CurrentPOID;



$(document).on('click', ".AddNewStyleButton", function () {
    $('.loading').show();
    var action = $(this).attr('data-action');
    var IsUpdate = false;
    var newVendorForm = $("#newAddStyleForm input");


    if ($('.errorFeild').length > 0) {
        $('.loading').hide();
        return false;
    }
    if (emptyFeildValidation('newAddStyleForm') == false) {
        $('.loading').hide();
        return false;
    }


    var jsonData = {};
    selectedProject = $('#ExistingProductSelect option:selected');
    if (SelectedProductID && SelectedProductID > 0) {
        jsonData["product_id"] = SelectedProductID;
        IsUpdate = true;
    }


    jsonData["po_id"] = CurrentPOID;
    jsonData["vendor_id"] = CurrentVendorId;
    jsonData["product_name"] = $('#newAddStyleForm #product_title').val();
    jsonData["vendor_color_name"] = $('#newAddStyleForm #product_color').val();
    jsonData["product_cost"] = $('#newAddStyleForm #product_unit_cost').val();
    jsonData["product_retail"] = $('#newAddStyleForm #product_retail').val();
    jsonData["product_name_no"] = $('#newAddStyleForm #product_title_no').val();
    jsonData["product_color_code"] = $('#newAddStyleForm #product_color_code').val();
    jsonData["product_type_id"] = $('#StyleType option:selected').val();
    jsonData["IsLineItemExists"] = IsLineItemExists;
    jsonData["product_subtype_ids"] = productSubCategoriesAction;
    jsonData["sku_family"] = new_sku;

    jsonData["vendor_style_sku"] = [];
    $('#po_input_fields_wrap .vendorSkuBox').each(function () {
        var style_sku = {};
        var styleVendorSize = $(this).find('#styleVendorSize').val();
        var styleSize = $(this).find('#styleSize').val();
        var styleSku = $(this).find('#styleSku').val();
        var styleSkuQty = $(this).find('#styleSkuQty').val();
        var styleSkuAttr = $($(this).find('#styleSku')).attr('disabled');
        var IsNewSku = false;
        var OriginalQty = 0;
        if (typeof styleSkuAttr !== typeof undefined && styleSkuAttr !== false) {
            IsNewSku = false;
        } else {
            IsNewSku = true;
        }
        if (IsLineItemExists) {
            OriginalQty = $(this).find('#styleSkuQty').data().originalquantity;
        }
        style_sku["style_vendor_size"] = styleVendorSize;
        style_sku["style_size"] = styleSize;
        style_sku["style_sku"] = styleSku;
        style_sku["style_qty"] = styleSkuQty;
        style_sku["IsNewSku"] = IsNewSku;
        style_sku["original_qty"] = OriginalQty;
        if (styleVendorSize != null && styleSize != null && styleSku != null && styleSkuQty) {
            jsonData["vendor_style_sku"].push(style_sku);
        }
    });

 
    if (jsonData["vendor_style_sku"].length== 0) {
        alertBox('poAlertMsg', 'red', 'Atleast one SKU is required.');
        $('.loading').hide();
        return;

    }

    var calulationValuesJson = $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("calculationvalues");
    var totalQty = $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("total_quantity");
    var totalStyles = $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("total_styles");
    if (calulationValuesJson == "" || calulationValuesJson == null) {

        var Qty = 0;
        var style_sku = jsonData["vendor_style_sku"];
        $.each(style_sku, function (index, value) {
            var styleQty = parseInt(value.style_qty);
            Qty = Qty + styleQty;


        })
        if (Qty > totalQty) {
            alertBox('poAlertMsg', 'red', 'Cannot add/update product , Total Quantity limit reached. Please increase total quantity in Purchase Order to proceed.');
            $('.loading').hide();
            return;
        }
    } else {

        var TotalQty = calulationValuesJson.filter(function (el) {
            return el.calculation_id == 5;
        });

        if (IsUpdate == false) {
            var _TotalStyles = calulationValuesJson.filter(function (el) {
                return el.calculation_id == 6;
            });

            if ((_TotalStyles[0].value + 1) > totalStyles) {
                alertBox('poAlertMsg', 'red', 'Cannot add/update product , Total Style Count limit reached. Please increase total style in Purchase Order to proceed.');
                $('.loading').hide();
                return;
            }

        }
       
        var Qty = TotalQty[0].value;
        var style_sku = jsonData["vendor_style_sku"];
        $.each(style_sku, function (index, value) {
            var originalQty = parseInt(value.original_qty);
            var styleQty = parseInt(value.style_qty);
            if (originalQty != styleQty) {

                if (originalQty > styleQty) {
                    var _value = originalQty - styleQty;
                    Qty = Qty - _value;
                } else {
                    var _value = styleQty - originalQty;
                    Qty = Qty + _value;
                }
            }

        })

        if (Qty > totalQty) {
            alertBox('poAlertMsg', 'red', 'Cannot add/update product , Total Quantity limit reached. Please increase total quantity in Purchase Order to proceed.');
            $('.loading').hide();
            return;
        }


    }



    $.ajax({

        url: location.origin + '/styles/create',

        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {


        if (data == "0") {
            alertBox('poAlertMsg', 'red', 'Unable to create/update style.');
            $('.loading').hide();
            return;
        }
        if (data == "-4") {
            alertBox('poAlertMsg', 'red', 'SKU already exists, please try to bump it by 1.');
            $('.loading').hide();
            return;
        }
        if (data == "-3") {
            alertBox('poAlertMsg', 'red', 'Cannot reduce quantity of sku.');
            $('.loading').hide();
            return;
        }
        else
            if (data != "0") {
                $('#StyleSubType').multiselect("clearSelection");
                $(".style_doc_section").empty();
                $(".vendorSkuBox").remove();
                console.log("New style Added");

                var UploadedFile = $('#newAddStyleForm #StyleImage')[0].files[0];
                if (UploadedFile) {


                    var formData = new FormData();

                    formData.append('product_id', data);
                    formData.append('StyleImage', UploadedFile);

                    $.ajax({
                        url: location.origin + "/styles/newdoc",
                        type: 'POST',
                        data: formData,

                        processData: false,
                        contentType: false,
                    }).always(function (data) {
                        console.log(data);
                    });

                }
                if (IsUpdate) {
                    alertBox('poAlertMsg', 'green', 'Style Updated successfully');
                } else {
                    alertBox('poAlertMsg', 'green', 'New style inserted successfully');
                }


                if (action == 'refreshValue') {
                    $('#newAddStyleForm #po_id').val(CurrentPOID);
                    $('#newAddStyleForm #vendor_id').val(CurrentVendorId);
                    SelectedProductID = null;
                    $("#modaladdstylec input,#modaladdstylec textarea, #modaladdstylec select").val("").removeClass('errorFeild');
                } else {
                    SelectedProductID = null;
                    $("#collapse_" + CurrentPOID).html("");
                    $("#collapse_" + CurrentPOID).hide();
                    $('#modaladdstylec').modal('hide')
                    $('a[data-poid=' + CurrentPOID + ']').trigger('click');
                }

                if (IsUpdate == false) {
                    var thenum = new_sku.match(/[a-z]+|[^a-z]+/gi);
                    var finalSkuFamily = thenum[0] + (parseInt(thenum[1]) + 1);

                    $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("skufamily", new_sku);
                    $('.poSkuFamily').text(finalSkuFamily);
                    new_sku = finalSkuFamily;

                }

                var calulationValuesJson = $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("calculationvalues");
                if (calulationValuesJson == "" || calulationValuesJson == null) {

                    var Qty = 0;
                    var style_sku = jsonData["vendor_style_sku"];
                    $.each(style_sku, function (index, value) {
                        var styleQty = parseInt(value.style_qty);
                        Qty = Qty + styleQty;


                    })
                    var calulationValuesJson = [{ "value": 1, "calculation_id": 6 }, { "value": Qty, "calculation_id": 5 }];
                    $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("calculationvalues", calulationValuesJson);
                } else {

                    var TotalQty = calulationValuesJson.filter(function (el) {
                        return el.calculation_id == 5;
                    });
                    var TotalStyleCount = calulationValuesJson.filter(function (el) {
                        return el.calculation_id == 6;
                    });

                    var Qty = TotalQty[0].value;
                    var style_sku = jsonData["vendor_style_sku"];
                    $.each(style_sku, function (index, value) {
                        var originalQty = parseInt(value.original_qty);
                        var styleQty = parseInt(value.style_qty);
                        if (originalQty != styleQty) {

                            if (originalQty > styleQty) {
                                var _value = originalQty - styleQty;
                                Qty = Qty - _value;
                            } else {
                                var _value = styleQty - originalQty;
                                Qty = Qty + _value;
                            }
                        }

                    })
                    TotalQty[0].value = Qty;
                    TotalStyleCount[0].value += 1;
                    var finalJson = [TotalQty[0], TotalStyleCount[0]];
                    $('button[data-poid="' + CurrentPOID + '"][id=AddItemButton][class="btn btn-light btn-sm"]').data("calculationvalues", finalJson);


                }


            }
        $('.loading').hide();
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
var dropdownlist;
$(document).ready(function () {
    // $('.loading').hide();
    var max_fields = 10; //maximum input boxes allowed
    var wrapper = $("#po_input_fields_wrap"); //Fields wrapper
    var add_button = $(".add_field_button"); //Add button ID
    $(".vendorSkuBox").remove();
    var x = 1; //initlal text box count
    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();
        if (x < max_fields) { //max input box allowed
            x++; //text box increment
            var lastsku = ""
            var options = '';
            if (window.sku_sizes.length) {
                for (i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (i == 0)
                        selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
            }
            $(wrapper).append('<div class="pb-2  vendorSkuBox form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> <div class="form-group col-md-3"><select type="text" class="form-control d-inline required" name="styleSize" id="styleSize">' + options + '</select></div> <div class="form-group col-md-3"><input style="text-transform: uppercase;" type="text" maxlength="7" value="' + lastsku + '" class="form-control d-inline  required" name="styleSku" id="styleSku" placeholder="SKU" /></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="styleSkuQty" min="1" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.
        }

    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        if ($('#po_input_fields_wrap .vendorSkuBox').length==1) {
            alertBox('poAlertMsg', 'red', 'Atleast one item is required.');
        } else {
            e.preventDefault(); $(this).parent('div').remove(); x--;
        }
      
    })


    $('#StyleSubType').multiselect({
        nonSelectedText: 'Select sub type',
        enableFiltering: true,
        templates: {
            li: '<li><a href="javascript:void(0);"><label class="pl-2"></label></a></li>',
            filter: '<li class="multiselect-item filter"><div class="input-group m-0 mb-1"><input class="form-control multiselect-search" type="text"></div></li>',
            filterClearBtn: '<div class="input-group-append"><button class="btn btn btn-primary multiselect-clear-filter" type="button"><i class="fa fa-times"></i></button></div>'
        },
        selectedClass: 'bg-light',
        onInitialized: function (select, container) {
            // hide checkboxes
            container.find('input[type=checkbox]').addClass('d-none');
        },
        onChange: function (option, checked) {
            var actionType = checked ? "insert" : "delete";
            productSubCategoriesAction = productSubCategoriesAction.filter(function (category) {
                return category.product_category_id != $(option).val();
            });


            var categoryAction = { category_id: $(option).val(), action: actionType };
            productSubCategoriesAction.push(categoryAction)

            if (productSubCategoriesAction.length > 0) {

                $('#StyleSubType').removeClass('errorFeild');
                $('#StyleSubType').parents('.form-group').find('span.errorMsg').remove()
            }

        }
    });

    $("#tb_StyleNameSuggest").autocomplete({
        source: function (request, response) {

            if (request.term.length > 3) {
                $.ajax({
                    url: "/product/autosuggest/" + CurrentVendorId + "/" + request.term,
                    dataType: 'json',
                    type: 'GET',
                    contentType: 'application/json',
                    processData: false,
                }).always(function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        response(data);
                        $('#tb_StyleNameSuggest').removeClass("errorFeild");
                        $('.errorMsg').remove();
                    } else {
                        $('#tb_StyleNameSuggest').removeClass("errorFeild");
                        $('.errorMsg').remove();
                        $('#tb_StyleNameSuggest').addClass('errorFeild');
                        $('#tb_StyleNameSuggest').parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">Record Not Found</span>')
                        $('.ui-autocomplete').empty().css("border", "0");
                    }

                });
            } else {
                $('#tb_StyleNameSuggest').removeClass("errorFeild");
                $('.errorMsg').remove();
            }
            
            if (request.term.length == 0) {
                $('#tb_StyleNameSuggest').removeClass("errorFeild");
                $('.errorMsg').remove();
                $('#tb_StyleNameSuggest').val(""); // display the selected text
                $('#tb_StyleNameSuggest').attr("data-val", "");
                $('button[data-poid="' + selectedPurchaseOrderID + '"]').trigger("click");
            }
        },
        select: function (event, ui) {
            // Set selection
            SelectedProductID = ui.item.value;
            GetProductDetails(CurrentVendorId, ui.item.value, CurrentPOID);
            $('#tb_StyleNameSuggest').val(ui.item.label); // display the selected text
            $('#tb_StyleNameSuggest').attr("data-val", ui.item.value);
            // $('#selectuser_id').val(ui.item.value); // save selected id to input
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $("#tb_StyleNameSuggest").val(ui.item.label);
        }
    });

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
Action: Get Data of items by vendor id and show in dropdown and fields
Input: int purchase order id, int vendor id
Output: string of vendor products
*/

$(document).on('click', "#AddItemButton", function () {

    $('#modaladdstylec #ExistingProductSelect').removeAttr('disabled');
    $('#modaladdstylec #StyleType').removeAttr('disabled');
    $('#modaladdstylec input').val("");
    $('#modaladdstylec #StyleSubType option').each(function () {
        if (this.innerText != "Choose..." && this.innerText != "...") {
            this.remove();
        }
    });
    $('#StyleSubType').multiselect("clearSelection");
    $('.errorMsg').remove();
    $("#modaladdstylec input,#modaladdstylec textarea, #modaladdstylec select").val("").removeClass('errorFeild');
    $(".vendorSkuBox").remove();
    $(".vendorSkuBox_disabled").remove();


    var vendor_type = $(this).data("vendorstyle");
    $.when(GetCategories(), GetSkuSizes()).done(function (p1,p2)
    {
        if (vendor_type != null && vendor_type != "" && vendor_type != 3) {
            $('#modaladdstylec #StyleType').val(parseInt(vendor_type)).change();
            $('#modaladdstylec #StyleType').attr('disabled', '');

        }
    })

    var CalculatedValuesArray = $(this).data("calculationvalues");


    if (CalculatedValuesArray != "" && CalculatedValuesArray != null) {

        var PoTotalQty = $(this).data("total_quantity");
        var PoTotalStyleCount = $(this).data("total_styles");
        var TotalQty = CalculatedValuesArray.filter(function (el) {
            return el.calculation_id == 5;
        });
        var TotalStyleCount = CalculatedValuesArray.filter(function (el) {
            return el.calculation_id == 6;
        });

        if (PoTotalStyleCount <= TotalStyleCount[0].value) {
            alertBox('poAlertMsg', 'red', ' Total Style Count reached, Please increase total count in Purchase Order to proceed.');
            return;
        }
        if (PoTotalQty <= TotalQty[0].value) {
            alertBox('poAlertMsg', 'red', ' Total Quantity reached, Please increase total quantity in Purchase Order to proceed.');
            return;
        }
    }

   
    CurrentPOID = $(this).data("poid");
    CurrentVendorId = $(this).data("vendorid");

    var CurrentProductId = $(this).data("proid");
    var productImage = $(this).data("product_vendor_image");
    $('.poNumber').text($(this).data("ponumber"))
  
  

    $('.poVendor').text($(this).data("vendorcode"))

    if (CurrentProductId == null) {
        if ($(this).data("skufamily").length == 0) {
            new_sku = $(this).data("vendorcode") + 100;
        } else {
            new_sku = $(this).data("skufamily");
            var thenum = new_sku.match(/[a-z]+|[^a-z]+/gi);
            var finalSkuFamily = thenum[0] + (parseInt(thenum[1]) + 1);
            new_sku = finalSkuFamily;
        }
    }


    $('.poSkuFamily').text(new_sku);

    if (CurrentProductId == null) {
        SelectedProductID = null;

        $('#div_styleSuggest').show();
        $('#AddNewStyleButton').show();
        $('#DeletefromPOButton').hide();

    } else {
        $('#div_styleSuggest').hide();
        $('#AddNewStyleButton').hide();
        $('#DeletefromPOButton').show();
    }


    //  alert("Please wait for the data to load");
 
    $('#newAddStyleForm #po_id').val(CurrentPOID);
    $('#newAddStyleForm #vendor_id').val(CurrentVendorId);

    if (CurrentProductId) {
        GetProductDetails(CurrentVendorId, CurrentProductId, CurrentPOID);
    } else {
        $('#modaladdstylec').modal('show');
    }
});


function GetProductDetails(vendor_id, product_id, po_id) {
    $('.loading').show();
    SelectedProductID = product_id;
    var CurrentProductId = product_id;

    $.ajax({
        url: '/vendor/products/' + vendor_id + "/" + CurrentProductId + "/" + po_id,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        data_vendor = data.vendor;
        var LineItems = data.productLineItemList;
        // ProductDetails;
        data = data.vendorProducts;


        var last_sku_family = "";

        $('#po_id').val(CurrentPOID);
        $('#vendor_id').val(CurrentVendorId);
        $(".vendorSkuBox_disabled").remove();
        $(".vendorSkuBox").remove();



        if (data.length) {
            for (i = 0; i < data.length; i++) {

                $('#modaladdstylec #product_title').val(data[i].product_name);
                $('#modaladdstylec #product_unit_cost').val(data[i].product_cost);
                $('#modaladdstylec #product_color').val(data[i].vendor_color_name);
                $('#modaladdstylec #product_retail').val(data[i].product_retail);
                $('#modaladdstylec #product_color_code').val(data[i].vendor_color_code);
                $('#modaladdstylec #product_title_no').val(data[i].vendor_product_code);
                $('#modaladdstylec #StyleType').val($('#modaladdstylec #StyleType option[value=' + data[i].product_type_id + ']').val()).change();

                $('#modaladdstylec #StyleType').attr('disabled', '');
                $(".style_doc_section").empty();

                var productImage = data[i].product_vendor_image;
                if (productImage != null) {

                    $(".style_doc_section").append("<img src=" + window.location.origin + '/uploads/' + productImage + " width='50' />  <br>");
                    $(".style_doc_section").removeClass('d-none');

                } else {
                    $(".style_doc_section").addClass('d-none');
                }
                last_sku_family = data[i].sku_family;

                CreateProductLineItems(LineItems, JSON.parse(data[i].skulist))

                var selectProductCategories = JSON.parse(data[i].productCategories);

                if (selectProductCategories != null) {
                    var categoryIds = [];
                    for (var i = 0; i < selectProductCategories.length; i++) {
                        var Category = selectProductCategories[i];
                        categoryIds.push(Category.category_id);
                    }
                    $('#StyleSubType').multiselect('select', categoryIds);
                }

            }

            new_sku = last_sku_family

        } else {
            if (data_vendor[0].latest_sku) {
                var vendorCode = data_vendor[0].vendor_code;
                var sku_number = parseInt(data_vendor[0].latest_sku);
                new_sku = vendorCode + sku_number;
            } else {
                var vendorCode = data_vendor[0].vendor_code;
                var sku_number = 100;
                new_sku = vendorCode + sku_number;
            }

        }

        $('.poSkuFamily').text(new_sku);
        $('.poVendor').text(data_vendor[0].vendor_code)
        $('.loading').hide();
        $('#modaladdstylec').modal({ backdrop: 'static', keyboard: false });
        console.log(data);

    });
}

function CreateProductLineItems(data, skulist) {

    IsLineItemExists = false;
    var wrapper = $("#po_input_fields_wrap"); //Fields wrapper
    $(".vendorSkuBox").remove();
    $(".vendorSkuBox_disabled").remove();
    $(".vendorSkuArea").show();
    var options = '';
    var Temp_Sku = '';

    if (data == null || data.length == 0) {
        $('#AddNewStyleButton').show();
        if (window.sku_sizes) {
            for (var y = 0; y < skulist.length; y++) {
                if (skulist[y].sku == null || skulist[y].sku == '') {
                    continue;
                }
                options = '';
                for (var i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (sku_sizes[i].attribute_id == skulist[y].attribute_id)
                        selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
                $(wrapper).append('<div class="pb-2 vendorSkuBox form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="csize[' + x + ']" value="' + skulist[y].vendor_size + '"  placeholder="Vendor Size" id="styleVendorSize" disabled  /></div><div class="form-group col-md-3"><select class="form-control d-inline" name="styleSize" id="styleSize" disabled >' + options + '</select></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " id="styleSku" name="styleSku"  placeholder="Size" value="' + skulist[y].sku + '" style="text-transform: uppercase;"   disabled /></div> <div class="form-group col-md-3"> <input type="text" class="form-control d-inline " min="1"  placeholder="Qty"  id="styleSkuQty"   />  '); // add input boxes.

                Temp_Sku = skulist[y].sku.split('-')[0];
            }
        }

    } else {
        IsLineItemExists = true;
        $('#AddNewStyleButton').hide();
        var LineItemsskuList = [];
        for (var x = 0; x < data.length; x++) {

            options = '';
            if (window.sku_sizes) {
                for (var i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (sku_sizes[i].attribute_id == data[x].attribute_id)
                        selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
            }
            Temp_Sku = data[x].sku.split('-')[0];
            var skunum = data[x].sku.split('-')[1];
            LineItemsskuList.push({ sku: data[x].sku, attribute_id: data[x].attribute_id, });
            $(wrapper).append('<div class="pb-2 vendorSkuBox form-row" data-skunum="' + skunum + '"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline" name="csize[' + x + ']" value = "' + data[x].vendor_size + '" placeholder="Vendor Size" id="styleVendorSize" disabled  /></div><div class="form-group col-md-3"><select class="form-control d-inline"  name="styleSize" id="styleSize" value = ""  disabled>' + options + '</select></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="size[' + x + ']" placeholder="Size" id="styleSku" name="styleSku" value="' + data[x].sku + '"  style="text-transform: uppercase;"  disabled /></div> <div class="form-group col-md-3"> <input type="text" class="form-control d-inline required " name="cqty[' + x + ']" placeholder="Qty" data-originalQuantity="' + data[x].line_item_ordered_quantity + '" min="1" id="styleSkuQty" value="' + data[x].line_item_ordered_quantity + '"  />  '); // add input boxes.


        }

        if (window.sku_sizes) {
            for (var y = 0; y < skulist.length; y++) {
                if ($("input[value='" + skulist[y].sku + "']").val() != null) {
                    continue;
                }

                if (skulist[y].sku == null || skulist[y].sku == '') {
                    continue;
                }
                options = '';
                for (var i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (sku_sizes[i].attribute_id == skulist[y].attribute_id)
                        selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
                var skunum = skulist[y].sku.split('-')[1];
                $(wrapper).append('<div class="pb-2 vendorSkuBox form-row" data-skunum="' + skunum + '"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline required " name="csize[' + x + ']" value="' + skulist[y].vendor_size + '"  placeholder="Vendor Size" id="styleVendorSize" disabled  /></div><div class="form-group col-md-3"><select class="form-control d-inline" name="styleSize" id="styleSize"  disabled>' + options + '</select></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline "  placeholder="Size" value="' + skulist[y].sku + '" style="text-transform: uppercase;" disabled name="styleSku" id="styleSku" /></div> <div class="form-group col-md-3"> <input type="text" class="form-control d-inline"  placeholder="Qty" min="1"  id="styleSkuQty"   />  '); // add input boxes.

                Temp_Sku = skulist[y].sku.split('-')[0];
            }
        }


    }
    Original_sku = new_sku;
    new_sku = Temp_Sku;
    $('.poSkuFamily').text(new_sku);
    $('.poAlertMsg').html('')

}

function GetCategories() {
    var SetCategory = $.Deferred();
    if (data_Categories == null || data_Categories == 0) {
        $.ajax({
            url: '/categoryoption/GetCategories',
            type: 'GET',
            contentType: 'application/json',
            processData: true,

        }).always(function (data) {
            data_Categories = data;
            $('#modaladdstylec #StyleType option').remove();
            $('#modaladdstylec #StyleType').append("<option id='-1' value=''>Choose...</option>");

            var mainCategories = data_Categories.filter(function (category) {
                return category.category_type == 0;
            });

            if (mainCategories.length) {
                for (var i = 0; i < mainCategories.length; i++) {
                    $('#modaladdstylec #StyleType').append("<option value='" + mainCategories[i].category_id + "'>" + mainCategories[i].category_name + "</option >")
                }
            }
            SetCategory.resolve();

        });
    } else {
        SetCategory.resolve();
    }
    return SetCategory.promise();

}

function GetSkuSizes() {
    var SetSizes = $.Deferred();
    if (window.sku_sizes == null || window.sku_sizes == 0) {
       $.ajax({
            url: '/attributes/getskusizes',
            type: 'GET',
            contentType: 'application/json',
            processData: true,

        }).always(function (data) {
       
            window.sku_sizes = data;
            SetSizes.resolve();
        });
    } else {
        SetSizes.resolve();
    }
    return SetSizes.promise();

}

function AppendSkuTextBoxes(Qtyboxes, styletype) {


    $("#po_input_fields_wrap").html('')
    var btnHtml = "";
    if (styletype == 2) {
        btnHtml = '<button type="button" class="btn btn-primary form-control " name="btstyleVendorSize" id="btstyleVendorSize" > Add Size </button>';
    }

    if ($('.vendorSkuArea #styleVendorSize').length == 0) {


        for (x = 1; x < Qtyboxes; x++) {

            var options = '';
            if (window.sku_sizes) {
                for (i = 0; i < sku_sizes.length; i++) {
                    var selected = "";
                    if (i == x - 1)
                        selected = "selected";
                    options += " <option value='" + sku_sizes[i].attribute_id + "'  " + selected + ">" + sku_sizes[i].attribute_display_name + "</option>";
                }
            }

            $("#po_input_fields_wrap").append('<div class="pb-2 vendorSkuBox form-row"> <div class="form-group col-md-3"> ' + btnHtml + ' <input type="text" class="form-control required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> <div class="form-group col-md-3"><select class="form-control d-inline  required" name="styleSize" id="styleSize">' + options + '</select></div> <div class="form-group col-md-3"><input type="text" maxlength="7" style="text-transform: uppercase;" class="form-control d-inline  required" name="styleSku" id="styleSku" placeholder="SKU" value = "" /></div> <div class="form-group col-md-3"><input type="number" class="form-control d-inline  required"  min="1" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.

        }

        if (styletype == 2) {

            CreateSKU($('#styleVendorSize')[0], true);
            $('#po_input_fields_wrap .vendorSkuBox #styleVendorSize').each(function () {
                $(this).hide();

            });
            $('#po_input_fields_wrap .vendorSkuBox #btstyleVendorSize').click(function () {
                $(this).next().show();
                $(this).hide();
                $('.add_field_button').show();
            });
        }


    }

}

$(document).on('change', "#StyleType", function (event) {


    var selectedStyleType = $(this).val()
    var skutextboxesCount = 0;
    if (selectedStyleType == '2') {
        skutextboxesCount = 2
        $('.vendorSkuArea').show();
        $('.add_field_button').hide();
    } else if (selectedStyleType == '1') {
        $('.vendorSkuArea').show();
        skutextboxesCount = window.sku_sizes.length + 1
        $('.add_field_button').show();
    } else {
        $('.vendorSkuArea').hide();
    }


    AppendSkuTextBoxes(skutextboxesCount, selectedStyleType)



    $('#StyleSubType option').remove();
    $('#StyleSubType').multiselect('rebuild');

    if (selectedStyleType != 0) {

        var subCategories = data_Categories.filter(function (category) {
            return category.category_parent_id == selectedStyleType;
        });

        if (subCategories.length) {
            for (var i = 0; i < subCategories.length; i++) {
                $('#StyleSubType').append("<option value='" + subCategories[i].category_id + "'>" + subCategories[i].category_name + "</option >")
            }

            $('#StyleSubType').multiselect('rebuild');

        }
    }





});

$(document).on('focusout', '#po_input_fields_wrap .vendorSkuBox #styleVendorSize', function (event) {

    var IsAccesory = $('#StyleType').val() == 2 ? true : false;

    CreateSKU(this, IsAccesory);


});

function CreateSKU(element, Isaccessory) {
    var newSkuNum = 0;
    if ($(element).parent().parent().find('#styleSku').val() == '') {

        newSkuNum = $('#po_input_fields_wrap .vendorSkuBox #styleVendorSize').filter(function () {
            return this.value != "";
        }).length;

    } else {
        var skunum = $($(element).parent().parent()[0].previousSibling).data('skunum');
        if (skunum == null) {
            newSkuNum = 0;
        } else {
            newSkuNum = parseInt(skunum) + 1;
        }
    }

    if (Isaccessory == false) {
        var styleSkuAttr = $($(element).parent().parent().find('#styleSku')).attr('disabled');

        if (typeof styleSkuAttr !== typeof undefined && styleSkuAttr !== false) {
            return;
        }
    }

    if ($(element).val() == "" && Isaccessory == false) {

        $(element).parent().parent().find('#styleSku').val('');

    } else {
        if (newSkuNum == 0) {
            newSkuNum += 1;
        }
        var newSkuToAssign = new_sku
        if (Isaccessory) {

        } else {
            newSkuToAssign += '-' + newSkuNum;
        } 123


        $(element).parent().parent().find('#styleSku').val(newSkuToAssign);
        $(element).parent().parent().find('#styleSku').removeClass('errorFeild');
        $(element).parent().parent().find('.errorMsg').remove();

        $(element).parent().parent().data('skunum', newSkuNum);
        if (isSkuDuplicate(newSkuToAssign)) {
            var newSkuToAssign = new_sku + '-' + (newSkuNum + 1);
            $(element).parent().parent().find('#styleSku').val(newSkuToAssign);
        }

    }
}

function isSkuDuplicate(newSKU) {
    var _isDuplicatecheck = $('#po_input_fields_wrap .vendorSkuBox #styleSku').filter(function () {
        return this.value == newSKU;
    }).length;

    if (_isDuplicatecheck == 1) {
        return false;
    } else {
        return true;
    }

}

function isTitleDuplicate(newTitle) {

    if (SelectedProductID != null) {
        return false;
    }

    var _isDuplicateCheckTitle = $('#ExistingProductSelect option').filter(function () {
        return $(this).val() == newTitle
    }).length;

    if (_isDuplicateCheckTitle) {

        return true;
    } else {
        return false
    }
}

$(document).on('focusout', '#po_input_fields_wrap .vendorSkuBox #styleSku', function (event) {

    var _TempSKU = $(this).val();
    if (_TempSKU != '') {

        if (isSkuDuplicate(_TempSKU)) {

            $(this).addClass('errorFeild');
            $(this).parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">Duplicate SKU entered.</span>')
        } else {
            var newSkuNum = _TempSKU.split('-')[1];
            $(this).parent().parent().data('skunum', newSkuNum);
            $(this).removeClass('errorFeild');
            $(this).parents('.form-group').find('.errorMsg').remove();

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

$(document).on('keydown', "#styleSkuQty", function (e) {
    return onlyNumbers(e)
});

//Edit Style for Single PO Using Drop down


$(document).on('change', '#modaleditstylec #ExistingProductSelect', function () {



    var SelectedProduct = $(this.options[this.selectedIndex]);
    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');
    SelectedProductID = SelectedProduct.data("product_id");
    productImage = SelectedProduct.data("product_vendor_image");
    SelectedProductTytle = $(this.options[this.selectedIndex]).val();
    $('#modaleditstylec #product_title').val($(this.options[this.selectedIndex]).attr('data-name'));
    $('#modaleditstylec #product_unit_cost').val(SelectedProduct.data('product_unit_cost'));
    $('#modaleditstylec #product_color').val(SelectedProduct.data('product_color'));
    $('#modaleditstylec #product_retail').val(SelectedProduct.data('product_retail'));

    var SeletedPOID = SelectedProduct.data("po_id");
    $('#modaleditstylec StyleType option').removeAttr('selected');
    if (SelectedProduct.data('product_type') == 1) {
        $('#modaleditstylec #StyleType').val($('#modaleditstylec #StyleType option[value=1]').val()).change()
    }
    else {
        $('#modaleditstylec #StyleType').val($('#modaleditstylec #StyleType option[value=2]').val()).change()
    }
    $(".style_doc_section").empty();
    if (productImage != null) {


        $(".style_doc_section").append("<a onclick='return false' class='documentsLink' data-proid=" + SelectedProductID + " data-val=" + productImage + ">" + productImage + " <span class='podeleteImage'>×</span></a> <br>");


        $(".style_doc_section").removeClass('d-none');

    } else {
        $(".style_doc_section").addClass('d-none');
    }
    $.ajax({
        url: '/purchaseorders/lineitems/' + SelectedProductID + '/' + SeletedPOID,

        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        //data = ;
        //console.log(data);


        var wrapper = $("#modaleditstylec #po_input_fields_wrap"); //Fields wrapper
        $(".vendorSkuBox").remove();
        $(".vendorSkuArea").show();

        $(".vendorSkuBox_disabled").remove();
        var sku_sizes = ["XS", "S", "M", "L"];
        for (x = 0; x < data.length; x++) {

            $(wrapper).append('<div class="pb-2 vendorSkuBox_disabled form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="csize[' + x + ']" placeholder="Vendor Size"   disabled /></div><div class="form-group col-md-3"><input type="text" class="form-control d-inline" name="csku[' + x + ']" placeholder="SKU" value = "' + sku_sizes[x] + '"  disabled /></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline " name="size[' + x + ']" placeholder="Size" value="' + data[x].sku + '" style="text-transform: uppercase;"  disabled /></div> <div class="form-group col-md-3"> <input type="text" class="form-control d-inline " name="cqty[' + x + ']" placeholder="Qty" value="' + data[x].line_item_ordered_quantity + '"  disabled />  '); // add input boxes.


        }

        $('.poAlertMsg').html('')

    });

});


$(document).on('click', "#EditItemButton", function () {

    var CurrentPOID = $(this).data("poid");
    $('.errorMsg').remove();
    $("#modaleditstylec input,textarea,select").val("").removeClass('errorFeild');
    var CurrentVendorId = $(this).data("vendorid");
    var CurrentProductId = $(this).data("proid");
    var productImage = $(this).data("product_vendor_image");
    $('.poNumber').text($(this).data("ponumber"))
    $('#modaleditstylec input').val("");
    $('#modaleditstylec #StyleSubType option').each(function () {
        if (this.innerText != "Choose..." && this.innerText != "...") {
            this.remove();
        }
    });

    $('#modaleditstylec').modal('show');
    //  alert("Please wait for the data to load");

    $.ajax({

        url: '/vendor/products/' + CurrentVendorId,

        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        data = data.vendorProducts;
        $('#modaleditstylec #ExistingProductSelect option').remove();
        $('#modaleditstylec #ExistingProductSelect').append("<option id='-1' value=''>Choose...</option>");
        var last_sku_family = "";

        $('#po_id').val(CurrentPOID);
        $('#vendor_id').val(CurrentVendorId);
        $(".vendorSkuBox_disabled").remove();
        $(".vendorSkuBox").remove();


        for (i = 0; i < data.length; i++) {

            $('#modaleditstylec #ExistingProductSelect').append("<option value='" + data[i].product_id + "' data-product_type='" + data[i].product_type_id + "' data-product_vendor_image='" + data[i].product_vendor_image + "' data-product_color='" + data[i].vendor_color_name + "' data-product_unit_cost='" + data[i].product_cost + "' data-product_retail='" + data[i].product_retail + "' data-Product_id='" + data[i].product_id + "'  data-skufamily='" + data[i].sku_family + "'  data-po_id='" + CurrentPOID + "' data-name='" + data[i].product_name + "' >" + data[i].product_name + "</option>");
            last_sku_family = data[i].sku_family;
        }
        $('#modaleditstylec #ExistingProductSelect').val(CurrentProductId).trigger('change');
        $('#modaleditstylec #ExistingProductSelect').prop('disabled', true);

        var vendorCode = last_sku_family.substring(0, 2);
        var sku_number = parseInt(last_sku_family.substr(2)) + 1;
        var new_sku = vendorCode + sku_number;
        var wrapper = $("#po_input_fields_wrap"); //Fields wrapper

        var sku_sizes = ["", "XS", "S", "M", "L"];
        for (x = 1; x < 5; x++) {
            $(wrapper).append('<div class="pb-2  vendorSkuBox form-row"> <div class="form-group col-md-3"><input type="text" class="form-control d-inline required" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline  required" name="styleSize" id="styleSize" placeholder="Size" value = "' + sku_sizes[x] + '" /></div> <div class="form-group col-md-3"><input type="text" maxlength="7" style="text-transform: uppercase;" class="form-control d-inline  required" name="styleSku" id="styleSku" placeholder="SKU" value = "' + new_sku + '-' + x + '" /></div> <div class="form-group col-md-3"><input type="text" class="form-control d-inline  required" name="styleSkuQty" id="styleSkuQty"  min="1" placeholder="Qty" /></div> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.

        }



        console.log(data);

    });
});

$(document).on('change', "#modaleditstylec input,#modaleditstylec select", function () {
    $(this).removeClass('errorFeild');
    $(this).parents('.form-group').find('.errorMsg').remove();
});

