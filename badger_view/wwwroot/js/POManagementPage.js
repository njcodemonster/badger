$(document).on('click', "#AddItemButton", function () {
    var CurrentPOID = $(this).data("poid");
    var CurrentVendorId = $(this).data("vendorid");

    
    

    $('#modaladdstylec input').val("");
    $('#modaladdstylec #StyleSubType option').each(function () {
        if (this.innerText != "Choose..." && this.innerText != "...") {
            this.remove();
        }
    });

    $('#modaladdstylec').modal('show');
  //  alert("Please wait for the data to load");
    CurrentVendorId = 1;
    $.ajax({

        url: '/vendor/products/' + CurrentVendorId,
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
       // var sku_family = data.vendorSkufamily;
        data = data.vendorProducts;
        $('#modaladdstylec #ExistingProductSelect option').remove();
        $('#modaladdstylec #ExistingProductSelect').append("<option id='-1'>Choose...</option>");
        var last_sku_family = "";

        $('#po_id').val(CurrentPOID);
        $('#vendor_id').val(CurrentVendorId);
        $(".vendorSkuBox_disabled").remove();
        $(".vendorSkuBox").remove();


        for (i = 0; i < data.length; i++) {

            $('#modaladdstylec #ExistingProductSelect').append("<option data-product_type='" + data[i].product_type_id + "' data-product_color='" + data[i].vendor_color_name + "' data-product_unit_cost='" + data[i].product_cost + "' data-product_retail='" + data[i].product_retail + "' data-Product_id='" + data[i].product_id + "'  data-skufamily='" + data[i].sku_family + "'  data-po_id='" + CurrentPOID +"'  >" + data[i].product_name + "</option>");
            last_sku_family = data[i].sku_family;
        }
        var vendorCode = last_sku_family.substring(0, 2);
        var sku_number = parseInt(last_sku_family.substr(2)) + 1;
        var new_sku = vendorCode + sku_number;
        var wrapper = $("#po_input_fields_wrap"); //Fields wrapper

        var sku_sizes = ["","XS", "S", "M", "L"];
        for (x = 1; x < 5; x++) {
            $(wrapper).append('<div class="pb-2  vendorSkuBox"> <input type="text" class="form-control d-inline w-25" name="styleVendorSize" id="styleVendorSize" placeholder="Vendor Size" /> <input type="text" class="form-control d-inline w-25" name="styleSize" id="styleSize" placeholder="Size" value = "' + sku_sizes[x] + '" /> <input type="text" class="form-control d-inline w-25" name="styleSku" id="styleSku" placeholder="SKU" value = "' + new_sku + '-' + x +'" /> <input type="text" class="form-control d-inline w-25" name="styleSkuQty" id="styleSkuQty" placeholder="Qty" /> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.

        }
       
        console.log(data);
    });
});
$(document).on('change', '#modaladdstylec #ExistingProductSelect', function () {
    var SelectedProduct = $(this.options[this.selectedIndex]);
    SelectedProductID = SelectedProduct.data("product_id");
    SelectedProductTytle = $(this.options[this.selectedIndex]).val();
    $('#modaladdstylec #product_tytle').val(SelectedProductTytle);
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
        var sku_sizes = [ "XS", "S", "M", "L"];
        for (x = 0; x < data.length; x++) {

            $(wrapper).append('<div class="pb-2 vendorSkuBox_disabled"> <input type="text" class="form-control d-inline w-25" name="csize[' + x + ']" placeholder="Vendor Size"  disabled /><input type="text" class="form-control d-inline w-25" name="csku[' + x + ']" placeholder="SKU" value = "' + sku_sizes[x] + '"  disabled /> <input type="text" class="form-control d-inline w-25" name="size[' + x + ']" placeholder="Size" value="' + data[x].sku+'"  disabled />  <input type="text" class="form-control d-inline w-25" name="cqty[' + x + ']" placeholder="Qty" value="' + data[x].line_item_ordered_quantity +'"  disabled />  '); // add input boxes.

        }


    });




});