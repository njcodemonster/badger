// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initiate data table

$(document).ajaxStart(function () {
    // alert("request started");
    console.log("request started");
});

$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    if (jqxhr.responseJSON)
        alertBox('vendorAlertMsg', 'red', jqxhr.responseJSON.Message);
    else
        alertBox('vendorAlertMsg', 'red', jqxhr.responseText, 50000);
});

//$(document).ajaxSuccess(function () {
//    alertBox('vendorAlertMsg', 'green', 'Vendor inserted successfully');
//})

//window.onerror = function (errMsg, url, line, column, error) {
//    var string = errMsg.toLowerCase();
//    var substring = "script error";
//    if (string.indexOf(substring) > -1) {
//        alert('Script Error: See Browser Console for Detail');
//    } else {
//        var message = [
//            'Message: ' + errMsg,
//            'URL: ' + url,
//            'Line: ' + line,
//            'Column: ' + column,
//            'Error object: ' + JSON.stringify(error)
//        ].join(' - ');
//        this.console.error(message);
//        // alert(message);
//    }

//    return false;
//}

$(document).ready(function () {
    $('#openpo').DataTable({ "aaSorting": [] });
    $('.datatable_js').DataTable({
        "columnDefs": [
            { "orderable": false, "targets": [0, 1, 7] },
            // { "orderable": true, "targets": [1, 2, 3] }
        ]
    });
    general_search();
});

$(document).on('click', "#general_search_btn", function () {
    $("#general_search").autocomplete("search", $("#general_search").val()); 
});


/************** General  Search **********/
function general_search() {
    // General Search
    var general_search = $('#general_search');
    general_search.autocomplete({
        delay: 0,
        source: function (request, response) {
            var jsonData = {};
            jsonData["search"] = request.term;
            console.log(jsonData);
            var newData = [];
            $('.search_result_list').hide();
            if (request.term.length > 3) {
                $.ajax({
                    url: "/search/autosuggest/",
                    dataType: 'json',
                    type: 'post',
                    data: JSON.stringify(jsonData),
                    contentType: 'application/json',
                    processData: false,
                }).always(function (data) {
                    console.log(data);
                    /********** Barcode **********************/
                    if (data.barcodeList.length > 0) {
                        newData = newData.concat(data.barcodeList);
                    }

                    /********** SKU **********************/
                    if (data.skuList.length > 0) {
                        newData = newData.concat(data.skuList);
                    }

                    /********** Vendor **********************/
                    if (data.vendorList.length > 0 ) {
                        newData = newData.concat(data.vendorList);
                    }

                    /********** Product **********************/
                    if (data.productList.length > 0 ) {
                        newData = newData.concat(data.productList);
                    }

                    /********** PO **********************/
                    if (data.purchaseOrdersList.length > 0) {
                        newData = newData.concat(data.purchaseOrdersList);
                    }

                    /********** Style Number **********************/
                    if(data.styleNumberList.length > 0){
                        newData = newData.concat(data.styleNumberList);
                    }

                    response(newData);
                });
            }
        },
        select: function (event, ui) {
            //general_search.val(ui.item.label);
            return false;
        },
        focus: function (event, ui) {
            //general_search.val(ui.item.label);
            return false;
        },
       /* open: function () {
            $("ul.ui-menu").width($(this).innerWidth());
        }*/
    });

    general_search.data("ui-autocomplete")._renderItem = function (ul, item) {
        ul.addClass('search_result_list'); //Ul custom class here
        //ul = this.menu.element;
        //ul.outerWidth(this.element.outerWidth());

        var li = $('<li>');
        var img = $('<img style="width:50px;padding-right:4px;">');
        var image_div = $('<div class="s_img">');
        var title_div = $('<div class="s_title">');

        if (item.type == 'vendor') {
            if (item.image != undefined && item.image != null ) {
                img.attr({
                    src: 'https://fashionpass.s3.us-west-1.amazonaws.com/badger_images/' + item.image,
                    //alt: item.label
                });
            }            
        } else {
            if (item.image != undefined || item.image != null) {
                if (item.image.indexOf("http") != -1) {
                    img.attr({
                        src: item.image,
                        //alt: item.label
                    });
                } else {
                    img.attr({
                        src: 'uploads/' + item.image,
                        //alt: item.label
                    });
                }
            }            
        }

        li.attr('data-value', item.label);

        if (item.type == 'barcode') {
            li.append('<a href="#barcode">');
        }

        if (item.type == 'sku') {
            li.append('<a href="#sku">');
        }

        if (item.type == 'vendor') {
            li.append('<a href="'+window.location.origin+'/Vendor/Single/'+item.value+'">');
        }

        if (item.type == 'product' || item.type == 'stylenumber' ) {
            li.append('<a href="'+window.location.origin+'/Product/EditAttributes/'+item.value+'">');
        }

        if (item.type == 'purchase_orders') {
            li.append('<a href="'+window.location.origin+'/PurchaseOrders/Single/'+item.value+'">');
        }

        if (item.image != null && item.image != undefined) {
            image_div.append(img);
            title_div.append(item.label);
            li.find('a').addClass("li_search").append(image_div).append(title_div);
        } else {
            li.find('a').append(item.label);
        } 
        

        return li.appendTo(ul);
    };

}

// Accordian PO Mgmt

$(document).ready(function () {
    // Add minus icon for collapse element which is open by default
    /*$(".collapse.show").each(function () {
        $(this).prev(".card-header").find(".fa").addClass("fa-minus").removeClass("fa-plus");
    });

    // Toggle plus minus icon on show hide of collapse element
    $(".collapse").on('show.bs.collapse', function () {
        $(this).prev(".card-header").find(".fa").removeClass("fa-plus").addClass("fa-minus");
    }).on('hide.bs.collapse', function () {
        $(this).prev(".card-header").find(".fa").removeClass("fa-minus").addClass("fa-plus");
    });*/
});

// Edit Label and Field

$(document).ready(function () {

    $('.edit').click(function () {
        $(this).hide();
        $(this).prev().hide();
        $(this).next().show();
        $(this).next().select();
    });

   
    $('input.edittitlet[type="text"]').blur(function () {
        if ($.trim(this.value) == '') {
            this.value = (this.defaultValue ? this.defaultValue : '');
        }
        else {
            $(this).prev().prev().html(this.value);
        }

        $(this).hide();
        $(this).prev().show();
        $(this).prev().prev().show();
    });

    $('input.edittitlet[type="text"]').keypress(function (event) {
        if (event.keyCode == '13') {
            if ($.trim(this.value) == '') {
                this.value = (this.defaultValue ? this.defaultValue : '');
            }
            else {
                $(this).prev().prev().html(this.value);
            }

            $(this).hide();
            $(this).prev().show();
            $(this).prev().prev().show();
        }
    });

    // date range picker
/** Include your own page where it is required because this js file load on  every page **/
   // $('#poreporting').daterangepicker();
});


/** Include your own page where it is required because this js file load on  every page **/

/*$('#delivery').datepicker({
    uiLibrary: 'bootstrap4'
});
$('#orderdate').datepicker({
    uiLibrary: 'bootstrap4'
});*/


// dropdpwn with checkbox

$('#vendor').multiselect({
    nonSelectedText: 'Select Vendor',
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
    }
});
$('#size').multiselect({
    nonSelectedText: 'Select Size',
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
    }
});
$('#status,#size').multiselect({
    nonSelectedText: 'Select Status',
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
    }
});