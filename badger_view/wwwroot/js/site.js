// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initiate data table

$(document).ready(function () {
    $('#openpo').DataTable({ "aaSorting": [] });
    $('.datatable_js').DataTable({
        "columnDefs": [
            { "orderable": false, "targets": [0, 1, 7] },
            // { "orderable": true, "targets": [1, 2, 3] }
        ]
    });
});

// Add another field

$(document).ready(function () {
    var max_fields = 10; //maximum input boxes allowed
    var wrapper = $(".input_fields_wrap"); //Fields wrapper
    var add_button = $(".add_field_button"); //Add button ID

    var x = 1; //initlal text box count
    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();
        if (x < max_fields) { //max input box allowed
            x++; //text box increment
            $(wrapper).append('<div class="pb-2"> <input type="text" class="form-control d-inline w-25" name="csize[' + x + ']" placeholder="Size" /> <input type="text" class="form-control d-inline w-25" name="vendorcsize[' + x + ']" placeholder="Vendor Size" /> <input type="text" class="form-control d-inline w-25" name="csku[' + x + ']" placeholder="SKU" /> <input type="text" class="form-control d-inline w-25" name="cqty[' + x + ']" placeholder="Qty" /> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.
        }
    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault(); $(this).parent('div').remove(); x--;
    })
});

// Accordian PO Mgmt

$(document).ready(function () {
    // Add minus icon for collapse element which is open by default
    $(".collapse.show").each(function () {
        $(this).prev(".card-header").find(".fa").addClass("fa-minus").removeClass("fa-plus");
    });

    // Toggle plus minus icon on show hide of collapse element
    $(".collapse").on('show.bs.collapse', function () {
        $(this).prev(".card-header").find(".fa").removeClass("fa-plus").addClass("fa-minus");
    }).on('hide.bs.collapse', function () {
        $(this).prev(".card-header").find(".fa").removeClass("fa-minus").addClass("fa-plus");
    });
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

    $('#poreporting').daterangepicker();
});

$('#delivery').datepicker({
    uiLibrary: 'bootstrap4'
});
$('#orderdate').datepicker({
    uiLibrary: 'bootstrap4'
});


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
$('#status').multiselect({
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