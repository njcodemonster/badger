// check is number press
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && charCode != 37 && charCode != 39 && (charCode < 48 || charCode > 57) && (charCode < 96 || charCode > 105)) {
       return false;
    }
    return blockspecialcharacter(evt)
    return true;
}

// block special characters 
function blockspecialcharacter(e) {
    let key = e.key;
            let keyCharCode = key.charCodeAt(0);

            // 0-9
            if(keyCharCode >= 48 && keyCharCode <= 57) {
                return key;
            }
            // 0-9 number pad
            if(keyCharCode >= 96 && keyCharCode <= 105) {
                return key;
            }
            // A-Z
            if(keyCharCode >= 65 && keyCharCode <= 90) {
                return key;
            }
            // a-z
            if(keyCharCode >= 97 && keyCharCode <= 122) {
                return key;
            }

            return false;
}

// check email valid
function isEmail(email) {
  var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  return regex.test(email);
}

// only number and dots allow
function onlyNumbersWithDot(e) {           
    var charCode;
    if (e.keyCode > 0) {
        charCode = e.which || e.keyCode;
    }
    else if (typeof (e.charCode) != "undefined") {
        charCode = e.which || e.keyCode;
    }
    if (charCode == 46)
        return true
    if (charCode == 190)
        return true
    if (charCode > 31 && charCode != 37 && charCode != 39 && (charCode < 48 || charCode > 57) && (charCode < 96 || charCode > 105) || charCode == 16) 
       return false;
    
    return true;
}

function allLetterAllow(event){
  var inputValue = event.which;
        // allow letters and whitespaces only.
        if(!(inputValue >= 65 && inputValue <= 120) && (inputValue != 32 && inputValue != 0) && inputValue != 8) { 
          return false
        }
}

function alertBox(area, action, massage) {
    var color = 'success'
    if (action == 'red')
        color = 'danger'
    var html = '<div style="width: 50%;" class="alert alert-' + color + ' alert-dismissible">' +
        '<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>' +
        massage +
        '</div>';
    $('.' + area).html(html);
    setTimeout(function () {
        $('.alert').remove()
    }, 3000)
}
// global confirmation function
function confirmationBox(heading,description,callback) {
    var html = '<div style="z-index: 9999;width: 30%;left: 0;position: absolute;right: 0;margin: 0 auto;top: 20%;" role="alert" class="alert alert-success confirmationBox">' +
        '<h4 class="alert-heading">' + heading + '</h4>' +
        '<p>' + description + '</p>' +
        '<hr>' +
        '<p style="text-align:right;" class="mb-0"><button type="button" style="margin-right: 10px;" data-val="yes" class="confirmDialog btn btn-success">Yes</button><button type="button" data-val="no" class="confirmDialog btn btn-success">No</button></p>' +
        '</div>';
    $('body').prepend(html);
    $('.confirmDialog').click(function () {
         $('.confirmationBox').remove();
        if ($(this).attr('data-val') == 'yes') {
            return callback('yes');
        } else {
            return callback('no');
        }
      
    })
}

function emptyFeildValidation(id){
    $('.errorMsg').remove();
    var notvalid = true;
    $('#'+id+' input').removeClass('errorFeild');
    $('#'+id+' input').each(function (){
        if($(this).val() == '' && $(this).attr('type') != 'radio' && $(this).attr('type') != 'file'){
            notvalid = false;
            $(this).addClass('errorFeild');
            $(this).parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">this field is required</span>')
        }
        if (notvalid && $(this).attr('type') == 'email' && isEmail($('.email').val()) == false) {
            $(this).parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">enter valid email</span>')
            notvalid = false;
        }
    });
    return notvalid;
}