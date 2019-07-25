﻿$(document).ready(function () {
    $(".current_tags").tagsinput('items');
})
var color_added = new Array();
var color_removed = new Array();
var tag_added = new Array();
var tag_removed = new Array();
function setIfDirty(e) {
   
    if (e.data("realvalue") == e.val()) {
        e.removeClass('dirty');
        return false;
    }
    e.addClass('dirty');
    return true;
}
$(document).on("change blur keyup", ".form-control", function () {
    setIfDirty($(this));

});
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
                tag_added.push(color_value);
            }
        }
        else {
            if (tag_added.find(item => item === color_value)) {
                tag_added = tag_added.filter(item => item !== color_value)
            }
            else {

            }
        }
    }

});

$(document).on("click", "#mainSaveButton", function () {
    datatosend = {};
    datatosend["tag_added"] = tag_added;
    datatosend["tag_removed"] = tag_removed;
    datatosend["color_added"] = color_added;
    datatosend["color_removed"] = color_removed;
    $("input.form-control.dirty").each(function (item) {
        datatosend[$(this).attr("id")] = $(this).val();
    });
    $("select.form-control.dirty").each(function (item) {
        datatosend[$(this).attr("id")] = $(this).val();
    });
    $.ajax({

        url: '/product/UpdateAttributes',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(datatosend),
        processData: false,

    }).always(function (data) { });
    console.log(datatosend);
});
$(document).on("click", ".addMorePoints", function () {
    $(this).removeClass('fa-plus addMorePoints').addClass('fa-minus removeMorePoints');
    var html = '<tr><td><input type="text" id="main_page_bulit3" data-realvalue="None" value="None" class="form-control"></td><td><i class="fa fa-plus addMorePoints" aria-hidden="true"></i></td></tr>'
    $('.productDetails tbody').append(html);
})
$(document).on("click", ".removeMorePoints", function () {
    $(this).parents('tr').remove()
})

function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.productImageArea .viewImage').append('<img id="" src="'+e.target.result+'" width="100" height="150">')
            };

            reader.readAsDataURL(input.files[0]);
        }
    }