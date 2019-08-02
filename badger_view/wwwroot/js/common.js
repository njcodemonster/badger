
/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action: checking is number press
*/
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && charCode != 37 && charCode != 39 && (charCode < 48 || charCode > 57) && (charCode < 96 || charCode > 105)) {
       return false;
    }
    return blockspecialcharacter(evt)
    return true;
}

/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action: allow space and block special characters 
*/
function blockspecialcharacter(e) {
    let key = e.key;
    let keyCharCode = key.charCodeAt(0);

    // space        
    if (keyCharCode == 32) {
        return key;
    }
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

/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action:  check email valid
*/
function isEmail(email) {
  var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  return regex.test(email);
}


/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action: only number and dots allow 
*/
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
    if (charCode == 110)
        return true
    if (charCode > 31 && charCode != 37 && charCode != 39 && (charCode < 48 || charCode > 57) && (charCode < 96 || charCode > 105) || charCode == 16) 
       return false;
    
    return true;
}

/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action:  check email valid
*/
function allLetterAllow(event) {
    var inputValue = event.which; console.log(inputValue);
        // allow letters and whitespaces only.
    if (!(inputValue >= 65 && inputValue <= 120) && (inputValue != 32 && inputValue != 0) && inputValue != 8 && inputValue != 37 && inputValue != 39) { 
          return false
        }
}
/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action:  alert function for any event success or failed. give area action and massage to print
*/
function alertBox(area, action, massage) {
    var color = 'success'
    if (action == 'red')
        color = 'danger'
    var html = '<div style="z-index: 9999;width: 30%;left: 0;position: absolute;right: 0;margin: 0 auto;top: 10%;" class="alert alert-' + color + ' alert-dismissible">' +
        '<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>' +
        massage +
        '</div>';
     $('.' + area).html('');
     $('body').append(html);
    setTimeout(function () {
        $('.alert').remove()
    }, 3000)
}

/*
  Developed By: Sajid Khan
  Date: 7-16-19 
  action:  alert function for any event success or failed. give area action and massage to print for inner body box
*/
function alertInnerBox(area, action, massage) {
    var color = 'success'
    if (action == 'red')
        color = 'danger'
    var html = '<div style="z-index: 9999;width: 30%;left: 0;position: absolute;right: 0;margin: 0 auto;top: 10%;" class="alert alert-' + color + ' alert-dismissible">' +
        '<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>' +
        massage +
        '</div>';
    $('.' + area).html('');
    $('.' + area).append(html);
    setTimeout(function () {
        $('.alert').remove()
    }, 3000)
}

function confirmationAlertBox(heading, description, callback) {
    var html = '<div style="z-index: 9999;width: 30%;left: 0;position: absolute;right: 0;margin: 0 auto;top: 10%;" role="alert" class="alert alert-success confirmationBox">' +
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
/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action:  aler for confirmation  yes or no .give heading description and return callback
*/
function confirmationBox(area,heading,description,callback) {
    var html = '<div style="z-index: 9999;width: 30%;left: 0;position: absolute;right: 0;margin: 0 auto;top: 10%;" role="alert" class="alert alert-success confirmationBox">' +
        '<h4 class="alert-heading">' + heading + '</h4>' +
        '<p>' + description + '</p>' +
        '<hr>' +
        '<p style="text-align:right;" class="mb-0"><button type="button" style="margin-right: 10px;" data-val="yes" class="confirmDialog btn btn-success">Yes</button><button type="button" data-val="no" class="confirmDialog btn btn-success">No</button></p>' +
        '</div>';
    $('#collapseOne'+area).prepend(html);
    $('.confirmDialog').click(function () {
         $('.confirmationBox').remove();
        if ($(this).attr('data-val') == 'yes') {
            return callback('yes');
        } else {
            return callback('no');
        }
      
    })
}

/*
  Developed By: Azeem Hassan
  Date: 7-3-19 
  action:  check any field validtaion give form id and return true/false
*/
function emptyFeildValidation(id){
    $('.errorMsg').remove();
    var notvalid = true;
    var emailvalid = true;
    $('#'+id+' .required').removeClass('errorFeild');
    $('#'+id+' .required:visible').each(function (){
        if($(this).val() == ''){
            notvalid = false;
            $(this).addClass('errorFeild');
            $(this).parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">this field is required</span>')
        }
        if (notvalid && $(this).attr('type') == 'email' && isEmail($(this).val()) == false) {
            $(this).parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">enter valid email</span>')
            emailvalid = false;
        }
    });
    if (emailvalid == false)
        notvalid = false
    return notvalid;
}
$(document).on('click', '.collapsButton', function (e) {
    if ($(this).find('.fa').hasClass('fa-minus')) {
        $(this).find('.fa').addClass('fa-plus').removeClass('fa-minus')
    } else {
        $(this).find('.fa').addClass('fa-minus').removeClass('fa-plus')

    }
})