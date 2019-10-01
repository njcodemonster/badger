
$(document).ready(function () {

    $(document).on('click', '#addnewFabric', function (e) {

        var wrapper = $(".UpdateFabricGroup")[0];
        var oldFabric =
            '<div class="row col-md-12 " style="margin-bottom: 10px;"> <div class="form-group col-md-6"><input type="text" id="tb_fabricName" data-attributeid="0" data-valueid=0 class="form-control required"></div>' +
            '<div class="form-inline  input-group col-md-5"><input type="number" min="1" max="100" class="form-control col-md-9 required" id="tb_fabricValue" > <div class="input-group-append"> <div class="input-group-text">%</div></div> </div>' +
            '<div class="form-inline col-md-1"><i class="fa fa-trash danger" style="color:red;" aria-hidden="true" onclick="checkFabrics(true,this)"></i></div> </div>';
        $(wrapper).append(oldFabric)
        $(wrapper).show();
        $('.fabricHeading').show();
        $("#btn_fabric_submit").show();

    });

    $(document).on('click', '#btn_fabric_refresh', function (e) {
        $('#div_fabricSuggest').show();
        $('#div_AddfabricInputGroup').hide();
        $('#btn_fabric_submit').html('Submit');
        $('#btn_fabric_refresh').hide();
        $('#tb_fabricName').val('');
        $('#tbH_AttributeID').val('');
        $('#tb_fabricPercentage').val('');
        $('#tb_fabricSuggest').val('');
        $('.errorMsg').remove();

    });

    $(document).on('click', '#btn_fabric_submit', function (e) {

        if (emptyFeildValidation('Fabric_form') == false) {
            $('.loading').hide();
            return false;
        }
        var attribute_id = 0
        var fabricArray = [];

        $.each($('.UpdateFabricGroup').children(), function (index, value) {

            var Attribute_id = 0;
            var isNewAttribute = 0;
            var fabricName = $(this).find('#tb_fabricName').val();
            var fabricValue = $(this).find('#tb_fabricValue').val();
            var ToDelete = $(value).is(":visible") ? false : true;
            isNewAttribute = $(this).find('#tb_fabricName').data('valueid');
            if ($(this).find('#tb_fabricName').data('attributeid') != null || $(this).find('#tb_fabricName').data('attributeid') != "") {
                Attribute_id = $(this).find('#tb_fabricName').data('attributeid');
            }
            var _fabric = Fabric(fabricName, fabricValue, isNewAttribute, Attribute_id == null ? 0 : Attribute_id, ToDelete)

            fabricArray.push(_fabric)

        })
        addFabrics(fabricArray);


    });

    $("#tb_fabricSuggest").autocomplete({
        source: function (request, response) {

            if (request.term.length > 2) {
                $.ajax({
                    url: "/attributes/getfabrics/" + request.term,
                    dataType: 'json',
                    type: 'GET',
                    contentType: 'application/json',
                    processData: false,
                }).always(function (data) {

                    if (data.length > 0) {
                        response(data);
                        $('#tb_fabricSuggest').removeClass("errorFeild");
                        $('.errorMsg').remove();
                    } else {
                        $('#tb_fabricSuggest').removeClass("errorFeild");
                        $('.errorMsg').remove();
                        $('#tb_fabricSuggest').addClass('errorFeild');
                        $('#tb_fabricSuggest').parents('.form-group').append('<span class="errorMsg" style="color:red;font-size: 11px;">Record Not Found</span>')
                        $('.ui-autocomplete').empty().css("border", "0");
                    }

                });
            } else {
                $('#tb_fabricSuggest').removeClass("errorFeild");
                $('#tb_fabricSuggest').parent().find('.errorMsg').remove();
            }

            if (request.term.length == 0) {
                $('#tb_fabricSuggest').removeClass("errorFeild");
                $('#tb_fabricSuggest').parent().find('.errorMsg').remove();
                $('#tb_fabricSuggest').val(""); // display the selected text
                $('#tb_fabricSuggest').attr("data-val", "");
            }
        },
        select: function (event, ui) {

            if ($('.UpdateFabricGroup #tb_fabricName[value="' + ui.item.label + '"]').length == 0) {

                var wrapper = $(".UpdateFabricGroup")[0];
                var oldFabric =
                    '<div class="row col-md-12 " style="margin-bottom: 10px;"> <div class="form-group col-md-6"><input type="text" id="tb_fabricName" data-valueid=0 disabled data-attributeid=' + ui.item.value + ' value="' + ui.item.label + '"  class="form-control required"></div>' +
                    '<div class="form-inline  input-group col-md-5"><input type="number" min="1" max="100" class="form-control col-md-9 required" id="tb_fabricValue" > <div class="input-group-append"> <div class="input-group-text">%</div></div> </div>' +
                    '<div class="form-inline col-md-1"><i class="fa fa-trash danger" style="color:red;" aria-hidden="true" onclick="checkFabrics(true,this);"></i></div> </div>';
                $(wrapper).append(oldFabric)
                $(wrapper).show();
                $('.fabricHeading').show();
                $("#btn_fabric_submit").show();
            } else {
                alertBox('poAlertMsg', 'red', 'Fabric already exists.');
                return false;
            }
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $("#tb_fabricSuggest").val(ui.item.label);
        }
    });

});

function checkFabrics(_isNew, source) {

    if (_isNew) {
        $(source).parent().parent().remove();
        if ($('.UpdateFabricGroup').children(':visible').length == 0) {
            $('.fabricHeading').hide();
            $("#btn_fabric_submit").hide();
            $('#tb_fabricSuggest').val('');
        }
    } else {
        confirmationAlertInnerBox("Remove Fabric", "Are you sure you want to remove this item?", function (result) {
            if (result == "yes") {
                $(source).parent().parent().hide();
                if ($('.UpdateFabricGroup').children(':visible').length == 0) {
                    $('.fabricHeading').hide();
                    $("#btn_fabric_submit").hide();
                    $('#tb_fabricSuggest').val('');
                }
                $('#btn_fabric_submit').click();
            }

        })
       

    }

}

$(document).on('blur focusout', "#tb_fabricSuggest", function (event) {

    if ($(this).val() == "") {
        $(this).removeClass('errorFeild')
        $('#tb_fabricSuggest').parent().find('.errorMsg').remove();
    }
});

function getFabrics(productId) {
    $.ajax({

        url: location.origin + '/product/getfabric/' + productId,
        type: 'get',
        contentType: 'application/json',
        processData: false,

    }).always(function (data) {
        $(".UpdateFabricGroup")[0].innerHTML = ''
        var wrapper = $(".UpdateFabricGroup")[0];

        if (data.length > 0) {

            $("#btn_fabric_submit").show();
            $('.fabricHeading').show();
            $.each(data, function (index, value) {

                var oldFabric =
                    '<div class="row col-md-12 " style="margin-bottom: 10px;"> <div class="form-group col-md-6"><input type="text" id="tb_fabricName" data-valueid=' + value.value_id + ' data-attributeid=' + value.attribute_id + ' class="form-control required" disabled value="' + value.attribute_Name + '"></div>' +
                    '<div class="form-inline  input-group col-md-5"><input type="number" min="1" max="100" class="form-control col-md-9 required" id="tb_fabricValue" value=' + value.value + '> <div class="input-group-append"> <div class="input-group-text">%</div></div> </div>' +
                    '<div class="form-inline col-md-1"><i class="fa fa-trash danger" style="color:red;" aria-hidden="true" onclick="checkFabrics(false,this);"></i></div> </div>';

                $(wrapper).append(oldFabric)
            });
            $(wrapper).show();


        } else {

            $("#btn_fabric_submit").hide();
            $('.fabricHeading').hide();


        }
        $('#tb_fabricSuggest').val('');
        $('#modalFabric').modal('show');
        $('.loading').hide();

    });
};

function addFabrics(_fabrics) {
    $('.loading').show();
    $.ajax({

        url: location.origin + '/product/addFabric',
        type: 'POST',
        contentType: 'application/json',
        processData: false,
        data: JSON.stringify(_fabrics),

    }).always(function (data) {


        var DeleteList = _fabrics.filter(function (el) {
            return el.toDelete == true;
        });
        var message = "added/updated"
        if (DeleteList.length > 0) {
            message = "removed"
        }
        if (data == "Success") {
            alertBox('poAlertMsg', 'green', 'Fabric ' + message + ' successfully.');
            if (message !="removed") {
                $('#modalFabric').modal('hide');
            }
          
        } else {
            alertBox('poAlertMsg', 'red', 'Could not ' + message + ' Fabric.');
            return;
        }

        $('.loading').hide();

    });
}

function Fabric(name, percentage, _isNew, attribute_id, ToDelete) {
    var Fabric = new Object();
    Fabric.name = name;
    Fabric.percentage = percentage;
    Fabric.value_id = _isNew;
    Fabric.attribute_id = attribute_id;
    Fabric.product_id = $('#tbH_ProductID').val();
    Fabric.toDelete = ToDelete

    return Fabric;
}
