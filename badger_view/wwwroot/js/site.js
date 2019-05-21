// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initiate data table

$(document).ready(function () {
    $('#openpo').DataTable();
    $('.datatable_js').DataTable({
        "columnDefs": [
            { "orderable": false, "targets": [0,1,7] },
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
            $(wrapper).append('<div class="pb-2"> <input type="text" class="form-control d-inline w-25" name="csize[' + x + ']" placeholder="Size" /> <input type="text" class="form-control d-inline w-25" name="csku[' + x + ']" placeholder="SKU" /> <input type="text" class="form-control d-inline w-25" name="cqty[' + x + ']" placeholder="Qty" /> <a href="#" class="remove_field">Remove</a> </div>'); // add input boxes.
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