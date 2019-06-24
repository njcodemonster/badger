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
    var key= document.all ? key= e.keyCode : key= e.which;
    return ((key > 64 && key < 91) || (key> 96 && key< 123) || key== 8 || key== 32 || (key>= 48 && key<= 57));
}

// check email valid
function isEmail(email) {
  var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  return regex.test(email);
}