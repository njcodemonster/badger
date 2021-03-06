﻿$(document).ready(function () {
    debugger;
    $(".current_tags").tagsinput('items');
    $(".loading").hide();
})
var color_added = new Array();
var color_removed = new Array();
var tag_added = new Array();
var tag_added_inDB = new Array();
var tag_removed = new Array();

$(document).on("change", ".colorItem", function () {

    var color_value = $(this).data("colorvalue");
    if ($(this).data("current") == "1") {
        if (this.checked) {
            if (color_removed.find(item => item === color_value)) {
                color_removed = color_removed.filter(item => item !== color_value)
            }
            else {

            }
        }
        else {
            if (color_removed.find(item => item === color_value)) {

            }
            else {
                color_removed.push(color_value);
            }
        }
    }
    else {
        if (this.checked) {
            if (color_added.find(item => item === color_value)) {

            }
            else {
                color_added.push(color_value);
            }
        }
        else {
            if (color_added.find(item => item === color_value)) {
                color_added = color_added.filter(item => item !== color_value)
            }
            else {

            }
        }
    }

});
$(document).on("change", ".tagsData", function () {

    var color_value = $(this).data("attributeid");
    if ($(this).data("current") == "1") {
        if (this.checked) {
            if (tag_removed.find(item => item === color_value)) {
                tag_removed = tag_removed.filter(item => item !== color_value)
            }
            else {

            }
        }
        else {
            if (tag_removed.find(item => item === color_value)) {

            }
            else {
                tag_removed.push(color_value);
            }
        }
    }
    else {
        if (this.checked) {
            if (tag_added.find(item => item === color_value)) {

            }
            else {
                if (tag_added_inDB.find(item => item === color_value))
                {
                    //if the added item is already in db it wont add to list it will be removed from the removed list
                    tag_removed = tag_removed.filter(item => item !== color_value)
                }
                else {
                    //if tag not in db it will be added to new added list
                    tag_added.push(color_value);
                }
            }
        }
        else
        {//work
            //if item is already added then i have to puch on load the db item that are being selested thi work tomorrow
            if (tag_added.find(item => item === color_value)) {
                tag_added = tag_added.filter(item => item !== color_value)
                
            }
            else {
                if (tag_added_inDB.find(item => item === color_value)) {
                    //Tag will be added to removed only when it is already in db the new item will not be added to the list
                    if (tag_removed.find(item => item === color_value)) {

                    }
                    else {
                        tag_removed.push(color_value);
                    }
                }
                else {

                }
            }
        }
    }

});

$(document).on("click", "#mainSaveButton", function () {
    if (emptyFeildValidation('productDetailPage') == false) {
        return false;
    }
    datatosend = {};
    datatosend["tag_added"] = tag_added;
    datatosend["tag_removed"] = tag_removed;
    datatosend["category_id"] = $('#StyleType option:selected').val();
    //debugger;
    if (datatosend["category_id"] == "0") {

        alertBox('poAlertMsg', 'red', 'Please Select category');
        return false;
    }
    if (tag_added.length == 0 && tag_removed.length == 0) {
        alertBox('poAlertMsg', 'red', 'No changes to save');
        return false;
    }
    $.ajax({

        url: '/categoryoption/updateattributes/',
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(datatosend),
        processData: false,

    }).always(function (data) { });
    if (data = "true") {
       
        GetTags("0");
        
        window.scrollTo(0, 0);
        alertBox('poAlertMsg', 'green', 'New Options upadted successfully');
    }
    console.log(datatosend);
});


/*
Developer: Sajid Khan
Date: 7-7-19
Action: Select dropdown data show by id 
URL:  purchaseorders/lineitems/productid/purchaseorderid
Input: int product id, int purchase order id
Output: get data in fields
*/
$(document).on('change', '#StyleType', function () {
    debugger;
    var SelectedStyleType = $(this.options[this.selectedIndex]).val();
    $('#loading').show();
    GetTags(SelectedStyleType);
    //$('.loading').addClass("d-none");
});


function GetTags(SelectedStyleType) {
    debugger;
    $.ajax({
        url: '/CategoryOption/GetTagsSubCategoryWise/' + SelectedStyleType,
        
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        debugger;
       
        $("#productDetailPage").html(data);
        var arr = JSON.parse( $('#tagsinDB').val());
        tag_added_inDB = new Array();
        for (var i = 0; i < arr.length; i++) {
            
            tag_added_inDB.push(arr[i]);
        }
        tag_added = new Array();
        tag_removed = new Array();
    });
}

/*
Developer: Rizwan Ali
Date: 7-7-19
Action: Get Data of items by vendor id and show in dropdown and fields
Input: int purchase order id, int vendor id
Output: string of vendor products
*/
$(document).on('click', "#AddSubCat", function () {
    debugger;
    

    $('#modalAddSubCategory').modal('show');
    //  alert("Please wait for the data to load");

    $.ajax({
       
        url: '/categoryoption/getParentCategory/',
        
        type: 'GET',
        contentType: 'application/json',
        processData: true,

    }).always(function (data) {
        // var sku_family = data.vendorSkufamily;
        debugger;

        $('#modalAddSubCategory #ParentCategorySelect option').remove();
        $('#modalAddSubCategory #ParentCategorySelect').append("<option id='' value=''>Choose...</option>");
       data= JSON.parse(data);
        data = data.vendorProducts;
        if (data.length) {
            for (i = 0; i < data.length; i++) {

                $('#modalAddSubCategory #ParentCategorySelect').append("<option value='" + data[i].category_id + "'>" + data[i].category_name+"</option>");
              
            }

        } 
    });
});

/*
Developer: Sajid Khan
Date: 7-5-19
Action: Add new style
URL: /styles/create
Input: styles data
Output: string of style
*/
$(document).on('click', ".SaveSubCategoryButton", function () {
    var action = $(this).attr('data-action');
    SaveSubCategory();
});

$("#AddCatForm #SubCat").on("keydown", function (event) {
    debugger;
    if (event.which == 13) {
        SaveSubCategory();
        return false;
    }
});

function SaveSubCategory() {
    var action = 'refreshValue';
    var SubCat = $('#SubCat').val();
    if (!SubCat.replace(/\s/g, '').length) {
        $('#SubCat').val("");
    }
    if (emptyFeildValidation('AddCatForm') == false) {
        return false;
    }

    var jsonData = {};
    $('.poAlertMsg').append('<div class="spinner-border text-info"></div>');

    var ParentCategorySelect = $('#ParentCategorySelect option:selected').val();
    jsonData["subCatTitle"] = SubCat;
    jsonData["ParentCatId"] = ParentCategorySelect;
    if (ParentCategorySelect == "") {

        alertBox('poAlertMsg', 'red', 'Please Select parent category');
        return false;
    }
    debugger;
    $.ajax({

        url: location.origin + '/Category/Create',
        
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) {
        console.log(data);
        if (data != "0") {
            alertBox('poAlertMsg', 'green', 'New Sub Category inserted successfully');
            if (action == 'refreshValue') {
                $("#modalAddSubCategory input,textarea,select").val("").removeClass('errorFeild');
                $('#modalAddSubCategory').modal('hide')
                GetTags(data);
            } else {
                $('#modalAddSubCategory').modal('hide')
            }
        }
        $('.poAlertMsg').html('')
    });
}