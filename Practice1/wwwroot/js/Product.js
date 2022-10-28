var dTable;
$(document).ready(function () {
    debugger;
    dTable=$('#myTable').DataTable({
        "ajax": {
            "url":"/Admin/Product/AllProducts"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "imageUrl" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Admin/Product/CreateUpdate?id=${data}" class="btn btn-warning">Edit</a><a onClick=RemoveProduct("/Admin/Product/Delete/${data}") class="btn btn-danger">Delete</a>`;
                }
            }
        ]
    });
});

function RemoveProduct(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        Swal.fire(
                            'Deleted!',
                            data.Error,
                            'success'
                        )
                        dTable.ajax.reload();
                    }
                    else {
                        Swal.fire(
                            'Failed!',
                            data.Error,
                            'warning'
                        )
                    }
                }
            })
            //Swal.fire(
            //    'Deleted!',
            //    'Your file has been deleted.',
            //    'success'
            //)
        }
    })
}
