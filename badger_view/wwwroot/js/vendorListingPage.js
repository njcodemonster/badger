$(document).on('click', "#NewVendorButton", function () {
    if (emptyFeildValidation('newVendorForm') == false) {
        return false;
    }
    $('.vendorAlertMsg').append('<div class="spinner-border text-info"></div>');
    var newVendorForm = $("#newVendorForm input");
    var jsonData = {};
    jsonData["vendor_name"] = $('#vendorName').val();
    jsonData["corp_name"] = $('#vendorCorpName').val();
    jsonData["statement_name"] = $('#vendorStatmentName').val();
    jsonData["vendor_code"] = $('#vendorCode').val();
    jsonData["vendor_street"] = $('#vendorStreetAdress').val();
    jsonData["vendor_suite_number"] = $('#vendorUnitNumber').val();
    jsonData["our_customer_number"] = $('#vendorourCustomerNumber').val();
    jsonData["vendor_description"] = $('#vendorDec').val();
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
            var id = data;

            $('#newPurchaseOrderForm #poVendor').append($("<option></option>").attr("value", data).text($('#newVendorForm #vendorName').val()));
            window.vendor_options = '';
            window.vendor_options = $("#newPurchaseOrderForm #poVendor > option").clone();
            $('#newPurchaseOrderForm #poVendor').empty().append(window.vendor_options);

            console.log("New Vender Added");
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
            $('#vendorListingArea').DataTable().row.add([
                $("#newVendorForm #vendorName").val(), $("#newVendorForm #vendorCode").val(), 2, 0, '<button type="button" id="EditPurhaseOrder" data-id="' + id + '" class="btn btn-light btn-sm">Edit</button>', '<a href="javascript:void(0)" data-toggle="modal" data-id="' + id + '" id="VendorNoteButton" data-target="#modaladdnote"><i class="fa fa-edit h3"></i></a>'
            ]).draw();
            var table = $('#vendorListingArea').DataTable();
            table.page('last').draw('page');
            alertBox('vendorAlertMsg', 'green', 'Vendor inserted successfully');
            setTimeout(function () {
               $('#newVendorModal').modal('hide'); 
            }, 3000)
        } else {
            alertBox('vendorAlertMsg', 'red', 'Vendor is not inserted');
        }
       
    });
});


$(document).on('keydown', "#newVendorForm input", function (e) {
    $(this).removeClass('errorFeild');
   $(this).parents('.form-group').find('.errorMsg').remove();
    if ($(this).attr('data-type') == 'number') {
        return isNumber(e)
    } else {
        if ($(this).attr('type') != 'email') {
            return blockspecialcharacter(e);
        }
    }
});
$(document).on('keyup', "#newVendorForm input.phone", function (e) {
    if($(this).val().length == $(this).attr('maxlength')){
        $(this).parent('div').next().find('input').focus();
    }
})
$(document).on('click', "#EditVendor", function () {
    $("#newVendorForm input,textarea").val("");
    $("#newVendorModal #vendorModalLongTitle").text("Edit Vendor");
    $('#newVendorModal input').prop("disabled","true");
    $('#newVendorModal').modal('show');
    var id = $(this).data("id");

    $.ajax({

        url: '/vendor/details/'+id,
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json',
    }).always(function (data) {
        $("#NewVendorButton,#EditVendorButton").attr("id", "EditVendorButton").text('Update');
        $('#newVendorModal input').removeAttr("disabled");
        var vendorData = data.venderAdressandRep;
        var vendorNoteAndDoc = data.venderDocAndNotes;
        var vendor = vendorData.vendor;
        var addresses = vendorData.Addresses;
        var reps = vendorData.Reps;
        var notes = vendorNoteAndDoc.note;
        var documents = vendorNoteAndDoc.doc;
        $("#newVendorForm").data("currentID",vendor.vendor_id);
        $("#newVendorModal #vendorModalLongTitle").text("Edit Vendor:" + vendor.vendor_name);
        if(notes.length > 0)
        $('#vendorNotes').val(notes[notes.length-1].note).attr('data-value',notes[notes.length-1].note);
        $('#vendorName').val(vendor.vendor_name);
        $('#vendorCorpName').val(vendor.corp_name);
        $('#vendorStatmentName').val(vendor.statement_name);
        $('#vendorDec').val(vendor.vendor_description);
        $('#vendorCode').val(vendor.vendor_code);
        $('#vendorourCustomerNumber').val(vendor.our_customer_number);
        // $('#vendorourCustomerNumber').val(vendor.vendor_name);
        if (addresses.length > 0) {
            var add1 = addresses[0]
            $('#vendorStreetAdress').val(add1.vendor_street);
            $('#vendorUnitNumber').val(add1.vendor_suite_number);
            $('#vendorCity').val(add1.vendor_city);
            $('#vendorZip').val(add1.vendor_zip);
            $('#vendorState').val(add1.vendor_state);
            $('#newVendorForm').attr('data-address-id',add1.vendor_address_id);
        }
        $('.documentsLink').remove();
        if (documents.length > 0) {
            for (var i = 0; i < documents.length; i++) {
                var html = '';
                html += '<a class="documentsLink" data-val="'+documents[i].url+'" href="">'+documents[i].url+'</a><br>';
            }
            $('#vendorDocument').parent('div').append(html)
        }
        if (reps.length > 0) {
            $('.venderRepoBox').remove();
            repsHtml(reps);
        }
        
        
    });

});
$(document).on('click', "#EditVendorButton", function () {
    return emptyFeildValidation('newVendorForm');
    $('.vendorAlertMsg').append('<div class="spinner-border text-info"></div>');
    var jsonData = {};
    var id = $("#newVendorForm").data("currentID");
    jsonData["vendor_name"] = $('#vendorName').val();
    jsonData["corp_name"] = $('#vendorCorpName').val();
    jsonData["statement_name"] = $('#vendorStatmentName').val();
    jsonData["vendor_code"] = $('#vendorCode').val();
    jsonData["vendor_street"] = $('#vendorStreetAdress').val();
    jsonData["vendor_suite_number"] = $('#vendorUnitNumber').val();
    jsonData["vendor_description"] = $('#vendorDec').val();
    jsonData["vendor_city"] = $('#vendorCity').val();
    jsonData["vendor_zip"] = $('#vendorZip').val();
    jsonData["vendor_state"] = $('#vendorState').val();
    jsonData["our_customer_number"] = $('#vendorourCustomerNumber').val();
    jsonData["address_id"] = $('#newVendorForm').attr('data-address-id');
     if($('#vendorNotes').val() != $('#vendorNotes').attr('data-value')) {
         jsonData["vendor_notes"] = $('#vendorNotes').val();
         $('#vendorNotes').attr('data-value',$('#vendorNotes').val())
       } else {
         jsonData["vendor_notes"] = '';

       }

    jsonData["vendor_reps"] = [];
    $('.venderRepoBox').each(function (){
        var vendor_rep = {};
        vendor_rep["Rep_first_name"] = $(this).find('#vendorRepName').val();
        vendor_rep["Rep_full_name"] = $(this).find('#vendorFullRepName').val();
        if ($(this).attr('data-id') != undefined) {
            vendor_rep["repo_id"] = $(this).attr('data-id');
        } else {
            vendor_rep["repo_id"] = '0';
        }

        if ($(this).find('#vendorRepIsPrimary').is(':checked')) {
             vendor_rep["main"] = 1;
        } else {
            vendor_rep["main"] = 0;
        }
        vendor_rep["Rep_email"] = $(this).find('#vendorRepEmail').val();
        vendor_rep["Rep_phone1"] =$(this).find('#vendorRepPhone11').val() + $(this).find('#vendorRepPhone12').val() + $(this).find('#vendorRepPhone12').val();
        vendor_rep["Rep_phone2"] = $(this).find('#vendorRepPhone14').val() + $(this).find('#vendorRepPhone15').val() + $(this).find('#vendorRepPhone16').val();
        jsonData["vendor_reps"].push(vendor_rep);
    })
    $.ajax({

        url: '/vendor/updatevendor/'+id,
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        processData: false,

        }).always(function (data) { 
            console.log(data); 
            if (data != "0") {
                alertBox('vendorAlertMsg', 'green', 'Vendor updated successfully');
                console.log("vendor created . uploading files");
                var formData = new FormData();
                formData.append('Vendor_id', id);
                var files = $("#newVendorForm #vendorDocument")[0].files;

                    for (var i = 0; i != files.length; i++) {
                        if ($('#documentsLink').attr('data-val')) {
                            if($('#documentsLink').attr('data-val').indexOf(files[i].name) == -1)
                                formData.append("vendorDocuments", files[i]);
                        } else {
                            formData.append("vendorDocuments", files[i]);
                        }
                         //formData.append("vendorDocuments", files[i]);
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
                setTimeout(function () {
                    $('#newVendorModal').modal('hide'); 
                }, 3000)
            } else {
                alertBox('vendorAlertMsg', 'red', 'Vendor is not updated');
            }
           
        });
});
$(document).on('click', "#AddNewVendorButton", function () {
    $("#NewVendorButton,#EditVendorButton").attr("id", "NewVendorButton").text('Add');
    $("#newVendorModal #vendorModalLongTitle").text("Add a New Vendor Profile");
    $("#newVendorForm input,textarea").val("").removeClass('errorFeild');
    $('.errorMsg').remove();
    $("#newVendorForm").data("currentID","");
});
$(document).on('click', "#AddMoreReps", function () {
                    var html  = '<div class="venderRepoBox" style="border: 1px solid"><span id="removeCurrentRep" class="repoCloseBtn" >&times;</span>'+
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
                                                    '<span class="d-inline">(</span> <input maxlength="3" data-type="number" type="tel" class="phone form-control d-inline w-75" id="vendorRepPhone11"> <span class="d-inline">)</span>'+
                                                '</div>'+
                                                '<div class="col-md-3 p-0">'+
                                                   ' <input type="tel" class="form-control phone" maxlength="3" data-type="number" id="vendorRepPhone12">'+
                                                '</div>'+

                                                '<div class="col-md-5">'+
                                                    '<input type="tel" class="form-control phone" maxlength="4" data-type="number"  id="vendorRepPhone13">'+
                                                '</div>'+
                                            '</div>'+
                                        '</div>'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Email</label>'+
                                            '<input type="email" class="form-control email" id="vendorRepEmail">'+
                                        '</div>'+
                                    '</div>'+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                           '<label>Phone Number 2</label>'+
                                            '<div class="row">'+
                                                '<div class="col-md-3 p-0">'+
                                                    '<span class="d-inline">(</span> <input type="tel" data-type="number" maxlength="3" class="phone form-control d-inline w-75" id="vendorRepPhone14"> <span class="d-inline">)</span>'+
                                                '</div>'+
                                                '<div class="col-md-3 p-0">'+
                                                    '<input type="tel" class="form-control phone" maxlength="3" data-type="number" id="vendorRepPhone15">'+
                                                '</div>'+
                                                '<div class="col-md-5">'+
                                                    '<input type="tel" class="form-control phone" maxlength="4" data-type="number" id="vendorRepPhone16">'+
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

function repsHtml(data) {
    for (i = 0; i < data.length; i++) {
        var phone1 = data[i].phone1;
        var phone2 = data[i].phone2;
        var  border = 'border: 1px solid';
        var  wwchecked = '';
        var crose = '<span id="removeCurrentRep" class="repoCloseBtn" >&times;</span>';
        if (i == 0) {
            crose = '';
            border = '';
        }
        if (data[i].main) {
            wwchecked = 'checked';
        }
        var html  = '<div class="venderRepoBox" style="'+border+'" data-id="'+data[i].contact_id+'">'+crose+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Rep First Name</label>'+
                                            '<input type="text" value="'+data[i].first_name+'" class="form-control" id="vendorRepName" style="width:90%"><input type="radio" id="vendorRepIsPrimary" '+wwchecked+' name="vendorRepIsPrimary" /> <small>Primary</small>'+
                                        '</div>'+
                                       '<div class="form-group col-md-6">'+
                                            '<label>Rep Full Name</label>'+
                                            '<span class="firstRep">'+
                                                '<input type="text" value="'+data[i].full_name+'" class="form-control" id="vendorFullRepName" style="width:90%">'+
                                            '</span>'+
                                        '</div>'+
                                    '</div>'+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Phone Number 1</label>'+
                                            '<div class="row">'+
                                                '<div class="col-md-3 p-0">'+
                                                    '<span class="d-inline">(</span> <input maxlength="3" value="'+phone1.substring(0,3)+'" data-type="number" type="tel" class="phone form-control d-inline w-75" id="vendorRepPhone11"> <span class="d-inline">)</span>'+
                                                '</div>'+
                                                '<div class="col-md-4 p-0">'+
                                                   ' <input type="tel" class="form-control phone" maxlength="3" value="'+phone1.substring(3, 6)+'" data-type="number" id="vendorRepPhone12">'+
                                                '</div>'+

                                                '<div class="col-md-5">'+
                                                    '<input type="tel" class="form-control phone" maxlength="4" value="'+phone1.substring(6, 10)+'" data-type="number" id="vendorRepPhone13">'+
                                                '</div>'+
                                            '</div>'+
                                        '</div>'+
                                        '<div class="form-group col-md-6">'+
                                            '<label>Email</label>'+
                                            '<input type="email" value="'+data[i].email+'" class="form-control email" id="vendorRepEmail">'+
                                        '</div>'+
                                    '</div>'+
                                    '<div class="form-row">'+
                                        '<div class="form-group col-md-6">'+
                                           '<label>Phone Number 2</label>'+
                                            '<div class="row">'+
                                                '<div class="col-md-3 p-0">'+
                                                    '<span class="d-inline">(</span> <input type="tel" value="'+phone2.substring(0,3)+'" data-type="number" maxlength="3" class="phone form-control d-inline w-75" id="vendorRepPhone14"> <span class="d-inline">)</span>'+
                                                '</div>'+
                                                '<div class="col-md-4 p-0">'+
                                                    '<input type="tel" class="form-control phone" maxlength="3" value="'+phone2.substring(3,6)+'" data-type="number" id="vendorRepPhone15">'+
                                                '</div>'+
                                                '<div class="col-md-5">'+
                                                    '<input type="tel" class="form-control phone" maxlength="4" value="'+phone2.substring(6,10)+'" data-type="number" id="vendorRepPhone16">'+
                                                '</div>'+
                                            '</div>'+
                                        '</div>'+
                                    '</div>'+
                                '</div>'
        $('.venderRepo').append(html); 
    }
}

$(document).on('click', "#VendorNoteButton", function () {
    var id = $(this).attr('data-id');
    $('#modaladdnote').attr('data-id', id);
    if (id != undefined) {
        $.ajax({
            url: '/vendor/getvendornoteanddoc/' + id,
            dataType: 'json',
            type: 'Get',
            contentType: 'application/json',
        }).always(function (data) {
            console.log(data);
            if (data.note && data.note.length > 0) {
                $('#vendorNote').attr('data-value',data.note[data.note.length-1].note).val(data.note[data.note.length-1].note);
            }

        })
    }
})
$(document).on('click', "#addVendorNote", function () {
    if ($('#vendorNote').attr('data-value') != $('#vendorNote').val() && $('#vendorNote').val() != '') {
        var id = $('#modaladdnote').attr('data-id');
        var jsonData = {};
        jsonData["vendor_notes"] = $('#vendorNote').val();
         $.ajax({
            url: '/vendor/insertvendornote/' + id,
            dataType: 'json',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            processData: false,
        }).always(function (data) {
            console.log(data);
           
        })
    }
});