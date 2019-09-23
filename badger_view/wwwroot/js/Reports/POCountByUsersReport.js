var poTable;
$(document).ready(function () {
    InitializaDatepicker('reportrange');
    poTable = LoadPoCountReport();
});

function InitializaDatepicker(id) {
    var start = moment().subtract(29, 'days');
    var end = moment();
    function cb(start, end) {
        $('#' + id + ' span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        $('#' + id + '-value').val(start.format('YYYY-MM-DD') + "," + end.format('YYYY-MM-DD'));
    }
    $('#reportrange').daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        showDropdowns: true
    }, cb);
    cb(start, end);
}

function LoadPoCountReport() {
    return $('#poCountReport').DataTable({
        'processing': true,
        "serverSide": true,
        'searching': false,
        "initComplete": function (settings, json) {
        },
        "ajax": {
            url: "/report/GetPOCountByUser",
            type: 'POST',
            data: function (data) {
                var date = $('#reportrange-value').val().split(',');
                data.from_date = date[0];
                data.to_date = date[1];
            }
        },
        language: {
            "search": "",
            "searchPlaceholder": "Search...",
            'processing': '<div><i class="fa fa-spinner fa-3x fa-spin"></i></div>'
        },
        columns: [
            { "data": "created_at", name: "created_at" },
            { "data": "username", name: "username" },
            { "data": "po_count", name: "po_count", render: getStyles }

        ]
    });
}

function getStyles(cellvalue, options, rowObject) {
    var aref = "PO(";
    var arefEnd = ")";
    var row = '<div >';
    var column = '<div >'
    var html = row;
    var styles = JSON.parse(rowObject.styles);
    $.each(styles, function (index, value) {
        // html += aref + value.vendor_po_number + ") = Styles(" + value.product_count + arefEnd + "</br>";
        html += column + aref + value.vendor_po_number + arefEnd + '</div>' + column + value.product_count + '<div/>';
    });
    html += '</div>';
    var uniqueId = rowObject.username + rowObject.created_at.replace(/-/g, "");
    return '<p>' +
        ' <button class="btn btn-primary" type="button" data-toggle="collapse" data-target="#' + uniqueId + '" aria-expanded="false" aria-controls="collapseExample">' +
        cellvalue +
        ' </button>' +
        '</p>' +
        '<div class="collapse" id="' + uniqueId + '">' +
        '<div class="card card-body">' +
        html +
        ' </div>' +
        '</div>';
}

function Search() {
    poTable.draw();
}