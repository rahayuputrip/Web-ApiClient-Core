var table = null;
var arrDepart = [];

$(document).ready(function () {
  //  debugger;
    table = $("#employee").DataTable({
        "processing": true,
        "responsive": true,
        "pagination": true,
        "stateSave": true,
        "ajax": {
            url: "/employee/LoadEmp",
            type: "GET",
            dataType: "json",
            dataSrc: "",
        },
        "columns": [
            {
                "data": "id",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                    //return meta.row + 1;
                }
            },
            { "data": "userName" },
            { "data": "address" },
            { "data": "phone" },
            {
                "data": "createDate",
                'render': function (jsonDate) {
                    var date = new Date(jsonDate);
                    return moment(date).format('DD MMMM YYYY, hh:mm');
                }
            },
 
            {
                "sortable": false,
                "render": function (data, type, row, meta) {
                    $('[data-toggle="tooltip"]').tooltip();
                    return '<button class="btn btn-outline-danger btn-circle" data-placement="right" data-toggle="tooltip" data-animation="false" title="Delete" onclick="return Delete(' + meta.row + ')" ><i class="fa fa-lg fa-times"></i></button>'
                }
            }
        ],
        "dom": "Bfrtip",
        "buttons": [
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel"></i> Excel',
                className: 'btn btn-success',
                title: 'Employee Table',
                search: 'applied',
                order: 'applied',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
            {
                extend: 'csv',
                text: '<i class="fas fa-file-csv"></i> CSV',
                className: 'btn btn-info',
                title: 'Employee Table',
                search: 'applied',
                order: 'applied',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
            {
                extend: 'pdf',
                text: '<i class="fas fa-file-pdf"></i> PDF',
                className: 'btn btn-danger',
                title: 'Employee Table',
                search: 'applied',
                order: 'applied',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
            {
                extend: 'print',
                text: '<i class="fas fa-print"></i> Print',
                className: 'btn btn-primary',
                title: 'Employee Table',
                search: 'applied',
                order: 'applied',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
        ],
    });
});

function GetById(number) {
    debugger;
    //console.log(table.row(number).data());
    var id = table.row(number).data().id;
    $.ajax({
        url: "/employee/GetById/",
        data: { Id: id }
    }).then((result) => {
        debugger;
        $('#Id').append(result.id);
        $('#Name').append(result.userName);
        $('#Address').append(result.address);
        $('#Phone').append(result.phone);
        $('#CreateDate').append(result.createDate);

        //var date = new Date(result.createData);
        //$('#HireDate').append(moment(date).format('DD MMMM YYYY'));
        //$('#HireTime').append(moment(date).format('LTS'))
        //$('#add').hide();
        $('#update').show();
        $('#myModal').modal('show');
    })
}

//function GetById(id) {
//    debugger;
//    $.ajax({
//        url: "/employee/GetById/",
//        data: { id: id }
//    }).then((result) => {
//        debugger;
//        $('#Id').val(result.id);
//        $('#Name').val(result.userName);
//        $('#Address').val(result.address);
//        $('#Phone').val(result.phone);
//        $('#CreateDate').val(result.createDate);
//        $('#add').hide();
//        $('#update').show();
//        $('#myModal').modal('show');
//    });
//}

function Delete(number) {
    var id = table.row(number).data().id;
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
        if (result.value) {
          //   debugger;
            $.ajax({
                url: "/employee/Delete/",
                data: { id: id }
            }).then((result) => {
            //       debugger;
                if (result.statusCode === 200) {
              //          debugger;
                    Swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Delete Successfully',
                        showConfirmButton: false,
                        timer: 1500,
                    });
                    table.ajax.reload(null, false);
                } else {
                    Swal.fire('Error', 'Failed to Delete', 'error');
                    ClearScreen();
                }
            })
        };
    });
}