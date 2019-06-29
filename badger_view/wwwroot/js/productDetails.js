var color_added = new Array();
var color_removed = new Array();
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
            if (color_removed.find(item => item === color_value )) {
                
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
    console.log(color_removed);
    console.log(color_added);
})