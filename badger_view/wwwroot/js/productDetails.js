﻿﻿$(document).ready(function () {
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
    var formData = new FormData();
    for (var i = 0; i != files.length; i++) {
          
        formData.append("productImages", files[i]);
        formData.append("product_id", $('#product_name').attr('data-id'));
        formData.append("product_title", files[i].name.split('.')[0]);
        if ($('#dropBox img'), hasClass('dummyImage')) 
            formData.append("product_primary", '1');
        else
            formData.append("product_primary", '0');
    }
    $.ajax({
        url: "/product/InsertattributeImages",
        type: 'POST',
        data: formData,
        dataType: 'json',
        processData: false,
        contentType: false,
    }).always(function (imageData) {
        console.log(imageData);
        for(var i = 0; i< files.length; i++)
        {
            var file = files[i];
            window.currentFilename = file.name.split('.')[0];
            //Only pics
            if(!file.type.match('image'))
                continue;
        
            var picReader = new FileReader();
            picReader.fileName = file.name;
            if (file.name == imageData[i].product_name) {
                picReader.dataImg_id = imageData[i].image_id
            }
            picReader.addEventListener("load",function(event){
                var picFile = event.target;
                console.log(event.target.fileName);
                var dataImg_id = event.target.dataImg_id;
                
                var count = $('.productImageArea .viewImage span').length;
                $('.productImageArea .viewImage').append(' <span id="div' + count +'" ondrop="drop(event)" ondragover="allowDrop(event)"><img class="productImage" data-filename="'+event.target.fileName+'" src="'+picFile.result+'" data-imageId="'+dataImg_id+'" id="drag'+count+'" draggable="true" ondragstart="drag(event)" width="130" height="200"></span>')
                    if ($('.productImageArea .proBigImage span').length == 0) {
                    $('.productImageArea .proBigImage').append('<span id="dropBox" ondrop="drop(event)" ondragover="allowDrop(event)"><img class="productImage" src="'+picFile.result+'" data-filename="'+event.target.fileName+'" data-imageId="'+dataImg_id+'" height="300" /></span>')

                }
            });
        
                //Read the image
            picReader.readAsDataURL(file);
        } 
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
    dropfilename = $('#' + dragId).attr('data-filename')
    dropimageid = $('#' + dragId).attr('data-imageid')
    if (ev.target.attributes.class.value.indexOf('dummyImage') == -1) {
        $('#' + dragId).attr('src', dropValue).attr('data-filename', ev.target.attributes["data-filename"].value).attr('data-imageid', ev.target.attributes["data-imageid"].value)
    } else {
        $('#' + dragId).parent('span').remove();
    }
    ev.target.attributes.src.value = data;
    ev.target.attributes["data-filename"].value = dropfilename;
    ev.target.attributes["data-imageid"].value = dropimageid;
    if (ev.toElement.parentElement.id == 'dropBox') {
          $('.loaderBox').show() 
        var imageData = [];
        var jsonData = {};
        jsonData["dataImage"] = [];
        jsonData["dataImage"].push({ product_img_id: $('#' + dragId).attr('data-imageid'), is_primary: "0" });
        jsonData["dataImage"].push({ product_img_id: ev.target.attributes["data-imageid"].value, is_primary: "1" });
        $.ajax({
            url: "/product/UpdateProductImagePrimary",
            dataType: 'json',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,
        }).always(function (data) {
            console.log(data);
            $('.loaderBox').hide() 
        })
    }
}

/*$(document).on("click", ".viewImage img", function () {
    var currentSrc = $(this).attr('src')
    var dropSrc = $('#dropBox img').attr('src')
    $(this).attr('src', dropSrc)
    $('#dropBox img').attr('src',currentSrc)

})*/

$(document).on('keydown', "#product_cost,#product_retail,#product_discount", function (e) {
    return onlyNumbersWithDot(e)
})
$(document).on('change', ".required", function (e) {
    $(this).removeClass('errorFeild');
    $('.errorMsg').remove();
})