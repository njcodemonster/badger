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
    if (emptyFeildValidation('productDetailPage') == false) {
        return false;
    }
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

function readURLAndUploadImg(event) {
    $('.loaderBox').show() 
    var files = event.target.files; //FileList object
    
    for(var i = 0; i< files.length; i++)
    {
        var file = files[i];
        
        //Only pics
        if(!file.type.match('image'))
          continue;
        
        var picReader = new FileReader();
        
        picReader.addEventListener("load",function(event){
            var picFile = event.target;
            var count = $('.productImageArea .viewImage span').length;
            $('.productImageArea .viewImage').append(' <span id="div'+count+'" ondrop="drop(event)" ondragover="allowDrop(event)"><img src="'+picFile.result+'" id="drag'+count+'" draggable="true" ondragstart="drag(event)" width="130" height="200"></span>')
              if ($('.productImageArea .proBigImage span').length == 0) {
                $('.productImageArea .proBigImage').append('<span id="dopBox" ondrop="drop(event)" ondragover="allowDrop(event)"><img src="'+picFile.result+'" height="300" /></span>')

            }
        
        });
        
         //Read the image
        picReader.readAsDataURL(file);
    } 
    var formData = new FormData();
    for (var i = 0; i != files.length; i++) {
        formData.append("productImages", files[i]);
        formData.append("product_id", $('#product_name').attr('data-id'));
        formData.append("product_title", 'my test');
        formData.append("product_primary", '0');
    }
    $.ajax({
        url: "/product/InsertattributeImages",
        type: 'POST',
        data: formData,
        dataType: 'json',
        processData: false,
        contentType: false,
    }).always(function (data) {
        console.log(data);
        $('.loaderBox').hide() 
    });


}

function allowDrop(ev) {
  ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("imageSrc", ev.target.attributes.src.value);
    ev.dataTransfer.setData("dragid", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("imageSrc");
    var dragId = ev.dataTransfer.getData("dragid");
    //ev.target.appendChild(document.getElementById(data));
    dropValue = ev.target.attributes.src.value;
     $('#'+dragId).attr('src',dropValue)
    ev.target.attributes.src.value = data;
}

$(document).on("click", ".viewImage img", function () {
    var currentSrc = $(this).attr('src')
    var dropSrc = $('#dopBox img').attr('src')
    $(this).attr('src', dropSrc)
    $('#dopBox img').attr('src',currentSrc)

})

$(document).on('keydown', "#product_cost,#product_retail,#product_discount", function (e) {
    return onlyNumbersWithDot(e)
})
$(document).on('change', ".required", function (e) {
    $(this).removeClass('errorFeild');
    $('.errorMsg').remove();
})