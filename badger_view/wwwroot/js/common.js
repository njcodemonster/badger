// check is number press
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
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
    if (charCode > 31 && (charCode < 48 || charCode > 57) || charCode == 16)
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
    var html =  '<div style="width: 50%;" class="alert alert-'+color+' alert-dismissible">'+
                '<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>'+
                massage+             
        '</div>'
    $('.' + area).html(html);
    setTimeout(function () {
        $('.alert').remove()
    }, 3000)
}