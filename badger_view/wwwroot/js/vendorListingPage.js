$(document).on('click', "#NewVendorButton", function () {
    debugger;
    var newVendorForm = $("#newVendorForm input");
    var jsonData = {};
    jsonData["vendor_name"] = $('#vendorName').val();
    jsonData["corp_name"] = $('#vendorCorpName').val();
    jsonData["statement_name"] = $('#vendorStatmentName').val();
    jsonData["vendor_code"] = $('#vendorCode').val();
    jsonData["vendor_street"] = $('#vendorStreetAdress').val();
    jsonData["vendor_suite_number"] =$('#vendorUnitNumber').val();
    jsonData["vendor_city"] = $('#vendorCity').val();
    jsonData["vendor_zip"] = $('#vendorZip').val();
    jsonData["vendor_state"] = $('#vendorState').val();
    jsonData["vendor_notes"] = $('#vendorNotes').val();
    jsonData["vendor_reps"] = [];
    $('.venderRepoBox').each(function (){
        var vendor_rep = {};
        vendor_rep["Rep_first_name"] = $(this).find('#vendorRepName').val();
        vendor_rep["Rep_full_name"] = $(this).find('#vendorFullRepName').val();
        if ($(this).find('#vendorRepIsPrimary').is(':checked')) {
             vendor_rep["main"] = 1;
        } else {
            vendor_rep["main"] = 0;
        }
        vendor_rep["Rep_email"] = $(this).find('#vendorRepEmail').val();
        vendor_rep["Rep_phone1"] = $('#vendorRepPhone11').val() + $('#vendorRepPhone12').val() + $('#vendorRepPhone12').val();
        vendor_rep["Rep_phone2"] = $('#vendorRepPhone14').val() + $('#vendorRepPhone15').val() + $('#vendorRepPhone16').val();
        jsonData["vendor_reps"].push(vendor_rep);
    })
    console.log(jsonData);
    $.ajax({
        
        url: '/vendor/newvendor',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data:  JSON.stringify(jsonData) ,
        processData: false,

    }).always(function (data) {
        console.log(data);
        if (data != "0") {
            console.log("New Vender Added");
            alert("vendor created . uploading files");
            var formData = new FormData();
            formData.append('Vendor_id', data);
            var files = $("#newVendorForm #vendorDocument")[0].files;
            for (var i = 0; i != files.length; i++) {
                   formData.append("vendorDocuments", files[i]);
            }
            $.ajax({
                url: "/vendor/newvendor_doc",
                type: 'POST',
                data: formData,
                dataType: 'json',
                processData: false,
                contentType: false,
            }).always(function (data) {
                console.log(data);
            });
        }
    });
   


});
$(document).on('click', "#EditVendor", function () {
    $("#modalvendor #vendorModalLongTitle").text("Edit Vendor");
    $('#modalvendor input').prop("disabled","true");
    $('#modalvendor').modal('show');
    var id = $(this).data("id");
    $.ajax({

        url: '/vendor/details/'+id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
       

    }).always(function (data) {
        $("#NewVendorButton,#EditVendorButton").attr("id", "EditVendorButton");
        $('#modalvendor input').removeAttr("disabled");
        var vendor = data.vendor;
        var addresses = data.addresses;
        var reps = data.reps;
        $("#modalvendor #vendorModalLongTitle").text("Edit Vendor:" + vendor.vendor_name);
        $('#vendorName').val(vendor.vendor_name);
        $('#vendorCorpName').val(vendor.corp_name);
        $('#vendorStatmentName').val(vendor.statement_name);
        $('#vendorCode').val(vendor.vendor_code);
        // $('#vendorourCustomerNumber').val(vendor.vendor_name);
        if (addresses.length > 0) {
            var add1 = addresses[0]
            $('#vendorStreetAdress').val(add1.vendor_street);
            $('#vendorUnitNumber').val(add1.vendor_suite_number);
            $('#vendorCity').val(add1.vendor_city);
            $('#vendorZip').val(add1.vendor_zip);
            $('#vendorState').val(add1.vendor_state);
        }
        if (reps.length > 0) {
            rep1 = reps[0];
            $('#vendorRepName').val(rep1.first_name);
            $('#vendorRepEmail').val(rep1.email);
            $('#vendorRepPhone11').val(rep1.phone1);
            $('#vendorRepPhone12').val(rep1.phone1);
            $('#vendorRepPhone13').val(rep1.phone1);
        }
        
        
    });

});
$(document).on('click', "#EditVendorButton", function () {
    id = $("#newVendorForm").data("currentID");
    jsonData["vendor_name"] = $(newVendorForm[0]).val();
    jsonData["corp_name"] = $(newVendorForm[1]).val();
    jsonData["statement_name"] = $(newVendorForm[3]).val();
    jsonData["vendor_code"] = $(newVendorForm[4]).val();
    jsonData["vendor_street"] = $(newVendorForm[5]).val();
    jsonData["vendor_suite_number"] = $(newVendorForm[6]).val();
    jsonData["vendor_city"] = $(newVendorForm[7]).val();
    jsonData["vendor_zip"] = $(newVendorForm[8]).val();
    jsonData["vendor_state"] = $(newVendorForm[9]).val();
    jsonData["Rep_first_name"] = $(newVendorForm[10]).val();
    jsonData["Rep_email"] = $(newVendorForm[12]).val();
    jsonData["Rep_phone1"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    jsonData["Rep_phone2"] = $(newVendorForm[13]).val() + $(newVendorForm[14]).val() + $(newVendorForm[15]).val();
    $.ajax({

        url: '/vendor/newvendorddf',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

    }).always(function (data) { console.log(data); });
});
$(document).on('click', "#AddNewVendorButton", function () {
    $("#NewVendorButton,#EditVendorButton").attr("id", "NewVendorButton");
    $("#modalvendor #vendorModalLongTitle").text("Add a New Vendor Profile");
    $("#newVendorForm input").val("");
    $("#newVendorForm").data("currentID","");
});
$(document).on('click', "#AddMoreReps", function () {
    //var html = '<span><input type="text" class="form-control d-inline-block" id="vendorRepName" style="width:90%"> <a href="#" id="removeCurrentRep" class="h4">-</a><br><input type="checkbox" id="vendorRepIsPrimary" /> <small>Primary</small></span>'
    //$('.firstRep').append(html);
                    var html  = '<div class="venderRepoBox"><span id="removeCurrentRep" class="repoCloseBtn" >&times;</span>'+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Rep First Name</label>'+
                                            '<input type="text" class="form-control" id="vendorRepName" style="width:90%"><input type="radio" id="vendorRepIsPrimary" name="vendorRepIsPrimary" /> <small>Primary</small>'+
                                        '</div>'+
                                       '<div class="form-group col-md-6">'+
                                            '<label>Rep Full Name</label>'+
                                            '<span class="firstRep">'+
                                                '<input type="text" class="form-control" id="vendorFullRepName" style="width:90%">'+
                                            '</span>'+
                                        '</div>'+
                                    '</div>'+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Phone Number 1</label>'+
                                            '<div class="row">'+
                                                '<div class="col-md-3 p-0">'+
                                                    '<span class="d-inline">(</span> <input type="tel" class="form-control d-inline w-75" id="vendorRepPhone11"> <span class="d-inline">)</span>'+
                                                '</div>'+
                                                '<div class="col-md-4 p-0">'+
                                                   ' <input type="tel" class="form-control" id="vendorRepPhone12">'+
                                                '</div>'+

                                                '<div class="col-md-5">'+
                                                    '<input type="tel" class="form-control" id="vendorRepPhone13">'+
                                                '</div>'+
                                            '</div>'+
                                        '</div>'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Email</label>'+
                                            '<input type="email" class="form-control" id="vendorRepEmail">'+
                                        '</div>'+
                                    '</div>'+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                           '<label>Phone Number 2</label>'+
                                            '<div class="row">'+
                                                '<div class="col-md-3 p-0">'+
                                                    '<span class="d-inline">(</span> <input type="tel" class="form-control d-inline w-75" id="vendorRepPhone14"> <span class="d-inline">)</span>'+
                                                '</div>'+
                                                '<div class="col-md-4 p-0">'+
                                                    '<input type="tel" class="form-control" id="vendorRepPhone15">'+
                                                '</div>'+
                                                '<div class="col-md-5">'+
                                                    '<input type="tel" class="form-control" id="vendorRepPhone16">'+
                                                '</div>'+
                                            '</div>'+
                                        '</div>'+
                                    '</div>'+
                                '</div>'
    $('.venderRepo').append(html);
});
$(document).on('click', "#removeCurrentRep", function () {
    $(this).parent().remove();
});